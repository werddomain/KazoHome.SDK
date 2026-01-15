# NetDaemon Analysis Report

## Executive Summary

This document analyzes the NetDaemon framework to determine the integration strategy for KazoHome.SDK. After thorough code review, **we recommend extending NetDaemon** rather than building from scratch or forking it.

## 1. Architecture Overview

### 1.1 Core Components

NetDaemon is organized into several key modules:

| Module | Purpose |
|--------|---------|
| `NetDaemon.Client` | WebSocket client for Home Assistant communication |
| `NetDaemon.HassModel` | Entity models, state management, and HA context |
| `NetDaemon.Runtime` | Runtime environment and dependency injection |
| `NetDaemon.AppModel` | Application lifecycle management |
| `NetDaemon.Extensions.*` | Scheduling, MQTT, TTS, Logging extensions |

### 1.2 Technology Stack

- **Target Framework**: .NET 10.0
- **Language Version**: C# 14.0
- **Reactive Extensions**: System.Reactive 6.1.0
- **Hosting**: Microsoft.Extensions.Hosting

## 2. State Bus Implementation

### 2.1 EntityStateCache (`Internal/EntityStateCache.cs`)

The state cache is the central state management component:

```csharp
internal class EntityStateCache(IHomeAssistantRunner hassRunner) : IDisposable
{
    private readonly ConcurrentDictionary<string, Lazy<EntityState?>> _latestStates = new();
    private readonly Subject<HassEvent> _eventSubject = new();
    
    public IObservable<HassEvent> AllEvents => _eventSubject;
}
```

**Key Features:**
- **Lazy Deserialization**: States are lazily deserialized to optimize performance
- **Thread-Safe**: Uses `ConcurrentDictionary` for concurrent access
- **Observable Pattern**: Events are exposed via `IObservable<HassEvent>`

### 2.2 WebSocket Communication (`Internal/HomeAssistantConnection.cs`)

```csharp
internal class HomeAssistantConnection : IHomeAssistantConnection, IHomeAssistantHassMessages
{
    private readonly Subject<HassMessage> _hassMessageSubject = new();
    private readonly IWebSocketClientTransportPipeline _transportPipeline;
    
    public async Task<IObservable<HassEvent>> SubscribeToHomeAssistantEventsAsync(
        string? eventType, CancellationToken cancelToken)
}
```

**Communication Flow:**
1. WebSocket connection established via `IWebSocketClientTransportPipeline`
2. Messages are deserialized and pushed to `Subject<HassMessage>`
3. Event subscriptions filter relevant events by type and ID
4. Commands are sent with incrementing message IDs for correlation

## 3. Event Subscription System

### 3.1 IHaContext Interface

```csharp
public interface IHaContext
{
    IObservable<Event> Events { get; }
    IObservable<StateChange> StateAllChanges();
    EntityState? GetState(string entityId);
    IReadOnlyList<Entity> GetAllEntities();
    void CallService(string domain, string service, ServiceTarget? target, object? data);
    void SendEvent(string eventType, object? data);
}
```

### 3.2 Strongly-Typed Entities

NetDaemon generates strongly-typed entity classes:
- `Entity` base class with generic state types
- `NumericEntity` for sensors with numeric values
- Domain-specific entities (Light, Switch, Climate, etc.)

## 4. Integration Strategy

### 4.1 Recommended Approach: **Extend NetDaemon**

| Option | Pros | Cons |
|--------|------|------|
| **Extend** (✓) | Leverage existing stable code, community support, updates | Need to follow upstream conventions |
| Build from scratch | Full control | Massive effort, bugs, no community |
| Fork | Independence | Maintenance burden, divergence |

### 4.2 KazoHome.SDK Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      KazoHome.SDK                           │
├─────────────────────────────────────────────────────────────┤
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │ SDK.Core        │  │ SDK.Blazor      │  │ SDK.Bridge  │ │
│  │ (Abstractions)  │  │ (UI Components) │  │ (C#↔Python) │ │
│  └────────┬────────┘  └────────┬────────┘  └──────┬──────┘ │
│           │                    │                   │        │
│  ┌────────┴────────────────────┴───────────────────┴──────┐ │
│  │              NetDaemon Core Libraries                  │ │
│  │  (Client, HassModel, Runtime, AppModel)                │ │
│  └────────────────────────────────────────────────────────┘ │
│                              │                              │
└──────────────────────────────┼──────────────────────────────┘
                               ▼
                    Home Assistant WebSocket API
```

### 4.3 Extension Points

KazoHome.SDK will extend NetDaemon through:

1. **Custom IHaContext implementations** for enhanced state synchronization
2. **Bridge services** for Python proxy communication
3. **Blazor components** wrapping entity interactions
4. **Source generators** for automation patterns

## 5. Python Bridge Requirements

Home Assistant's `custom_components` require Python. KazoHome.SDK will provide:

### 5.1 Bridge Architecture

```
┌─────────────────┐     gRPC/UDS     ┌─────────────────┐
│    .NET Core    │◄───────────────►│  Python Proxy   │
│   Application   │                  │  (HA Component) │
└─────────────────┘                  └─────────────────┘
```

### 5.2 Communication Protocol

- **Primary**: gRPC (cross-platform, strongly-typed)
- **Fallback**: Unix Domain Sockets (lower latency)
- **Serialization**: Protocol Buffers

## 6. Key Findings

### 6.1 Reusable Components

| Component | Reuse Strategy |
|-----------|----------------|
| `HomeAssistantConnection` | Direct dependency |
| `EntityStateCache` | Extend for cross-project sync |
| `IHaContext` | Implement for Blazor binding |
| Code Generator | Reference for custom generators |

### 6.2 Gaps to Address

1. **Blazor Integration**: No existing Blazor support
2. **Python Bridge**: No native Python interop
3. **UI Templates**: No dashboard card generation
4. **Cross-Project Messaging**: No built-in pub/sub

## 7. Conclusion

NetDaemon provides a solid, well-architected foundation for Home Assistant .NET development. KazoHome.SDK should:

1. **Reference NetDaemon packages** as NuGet dependencies
2. **Extend core abstractions** with Blazor and Bridge layers
3. **Create new project templates** for the five project types
4. **Implement Python proxy generation** as a build-time task

This approach minimizes development effort while providing maximum flexibility for the SDK's unique requirements.
