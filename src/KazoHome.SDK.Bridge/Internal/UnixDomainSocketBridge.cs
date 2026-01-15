using KazoHome.SDK.Bridge.Protocol;

namespace KazoHome.SDK.Bridge.Internal;

/// <summary>
/// Unix Domain Socket implementation of the Python Bridge
/// </summary>
internal sealed class UnixDomainSocketBridge : IPythonBridge
{
    private readonly ILogger<UnixDomainSocketBridge> _logger;
    private readonly PythonBridgeOptions _options;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<BridgeMessage>> _pendingRequests = new();
    private readonly Subject<BridgeMessage> _eventSubject = new();

    private Socket? _socket;
    private NetworkStream? _stream;
    private CancellationTokenSource? _receiveLoopCts;
    private Task? _receiveLoopTask;
    private volatile bool _isConnected;
    private volatile bool _isDisposed;

    public UnixDomainSocketBridge(
        ILogger<UnixDomainSocketBridge> logger,
        IOptions<PythonBridgeOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public bool IsConnected => _isConnected;

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_isConnected)
        {
            _logger.LogWarning("Bridge is already connected");
            return;
        }

        try
        {
            var endpoint = new UnixDomainSocketEndPoint(_options.SocketPath);
            _socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(_options.ConnectionTimeoutMs);

            await _socket.ConnectAsync(endpoint, timeoutCts.Token);
            _stream = new NetworkStream(_socket, ownsSocket: false);

            _receiveLoopCts = new CancellationTokenSource();
            _receiveLoopTask = Task.Run(() => ReceiveLoopAsync(_receiveLoopCts.Token), _receiveLoopCts.Token);

            _isConnected = true;
            _logger.LogInformation("Connected to Python bridge at {SocketPath}", _options.SocketPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to Python bridge at {SocketPath}", _options.SocketPath);
            await CleanupAsync();
            throw;
        }
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (!_isConnected)
            return;

        _isConnected = false;
        await CleanupAsync();
        _logger.LogInformation("Disconnected from Python bridge");
    }

    public async Task<TResponse> SendRequestAsync<TRequest, TResponse>(
        string method,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (!_isConnected)
            throw new InvalidOperationException("Bridge is not connected");

        var messageId = Guid.NewGuid().ToString("N");
        var message = new BridgeMessage
        {
            Id = messageId,
            Type = BridgeMessageTypes.Request,
            Method = method,
            Payload = JsonSerializer.SerializeToElement(request)
        };

        var tcs = new TaskCompletionSource<BridgeMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
        _pendingRequests[messageId] = tcs;

        try
        {
            await SendMessageAsync(message, cancellationToken);

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(_options.RequestTimeoutMs);

            var response = await tcs.Task.WaitAsync(timeoutCts.Token);

            if (response.Error != null)
            {
                throw new BridgeException(response.Error.Code, response.Error.Message);
            }

            return response.Payload.HasValue
                ? response.Payload.Value.Deserialize<TResponse>()!
                : default!;
        }
        finally
        {
            _pendingRequests.TryRemove(messageId, out _);
        }
    }

    public async Task SendNotificationAsync<TRequest>(
        string method,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (!_isConnected)
            throw new InvalidOperationException("Bridge is not connected");

        var message = new BridgeMessage
        {
            Id = Guid.NewGuid().ToString("N"),
            Type = BridgeMessageTypes.Notification,
            Method = method,
            Payload = JsonSerializer.SerializeToElement(request)
        };

        await SendMessageAsync(message, cancellationToken);
    }

    public IObservable<TEvent> SubscribeToEvents<TEvent>(string eventType)
    {
        return _eventSubject
            .Where(m => m.Type == BridgeMessageTypes.Event && m.Method == eventType)
            .Select(m => m.Payload.HasValue
                ? m.Payload.Value.Deserialize<TEvent>()!
                : default!);
    }

    private async Task SendMessageAsync(BridgeMessage message, CancellationToken cancellationToken)
    {
        if (_stream == null)
            throw new InvalidOperationException("Stream is not available");

        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json + "\n");
        await _stream.WriteAsync(bytes, cancellationToken);
        await _stream.FlushAsync(cancellationToken);
    }

    private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
    {
        var buffer = new byte[4096];
        var messageBuffer = new StringBuilder();

        while (!cancellationToken.IsCancellationRequested && _stream != null)
        {
            try
            {
                var bytesRead = await _stream.ReadAsync(buffer, cancellationToken);
                if (bytesRead == 0)
                {
                    _logger.LogWarning("Bridge connection closed by peer");
                    break;
                }

                messageBuffer.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                // Process complete messages (newline-delimited JSON)
                var content = messageBuffer.ToString();
                var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                if (content.EndsWith('\n'))
                {
                    messageBuffer.Clear();
                    foreach (var line in lines)
                    {
                        ProcessMessage(line);
                    }
                }
                else if (lines.Length > 1)
                {
                    messageBuffer.Clear();
                    messageBuffer.Append(lines[^1]);
                    for (int i = 0; i < lines.Length - 1; i++)
                    {
                        ProcessMessage(lines[i]);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in receive loop");
                break;
            }
        }

        _isConnected = false;
    }

    private void ProcessMessage(string json)
    {
        try
        {
            var message = JsonSerializer.Deserialize<BridgeMessage>(json);
            if (message == null)
                return;

            if (message.Type == BridgeMessageTypes.Response)
            {
                if (_pendingRequests.TryRemove(message.Id, out var tcs))
                {
                    tcs.SetResult(message);
                }
            }
            else if (message.Type == BridgeMessageTypes.Event)
            {
                _eventSubject.OnNext(message);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize bridge message");
        }
    }

    private async Task CleanupAsync()
    {
        if (_receiveLoopCts != null)
        {
            await _receiveLoopCts.CancelAsync();
            _receiveLoopCts.Dispose();
            _receiveLoopCts = null;
        }

        if (_receiveLoopTask != null)
        {
            try
            {
                await _receiveLoopTask.WaitAsync(TimeSpan.FromSeconds(5));
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("Receive loop did not terminate in time");
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
            _receiveLoopTask = null;
        }

        _stream?.Dispose();
        _stream = null;

        _socket?.Dispose();
        _socket = null;

        // Complete all pending requests with cancellation
        foreach (var kvp in _pendingRequests)
        {
            kvp.Value.TrySetCanceled();
        }
        _pendingRequests.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;
        await DisconnectAsync();
        _eventSubject.Dispose();
    }
}

/// <summary>
/// Exception thrown by bridge operations
/// </summary>
public class BridgeException : Exception
{
    public string Code { get; }

    public BridgeException(string code, string message) : base(message)
    {
        Code = code;
    }
}
