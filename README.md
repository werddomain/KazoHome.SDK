# KazoHome.SDK

A comprehensive .NET 10 SDK for developing Home Assistant addons, integrations, automations, and UI components using C# and Blazor.

## 🎯 Overview

KazoHome.SDK enables .NET developers to build Home Assistant solutions using modern C# patterns and tooling. The SDK extends [NetDaemon](https://netdaemon.xyz/) and provides:

- **Addons**: Containerized background services
- **Cards**: Blazor WebAssembly dashboard cards
- **Views**: Blazor Server pages via Ingress
- **Integrations**: Hybrid C#/Python custom_components
- **Automations**: Event-driven automations with Source Generators

## 🚀 Quick Start

### Install Templates

**From NuGet (when published):**
```bash
dotnet new install KazoHome.SDK.Templates
```

**Local installation (for development):**
```bash
# Clone the repository
git clone --recurse-submodules https://github.com/werddomain/ha-solution.git
cd ha-solution

# Install templates from local folder
dotnet new install ./templates
```

### Create a New Project

```bash
# Create an addon
dotnet new kazohome-addon -n MyAddon

# Create a dashboard card
dotnet new kazohome-card -n MyCard

# Create an ingress view
dotnet new kazohome-view -n MyView

# Create a hybrid integration
dotnet new kazohome-integration -n MyIntegration --IntegrationDomain my_integration

# Create automations
dotnet new kazohome-automation -n MyAutomations
```

## 📦 SDK Packages

| Package | Description |
|---------|-------------|
| `KazoHome.SDK.Core` | Core abstractions, entities, events, and persistence |
| `KazoHome.SDK.Bridge` | C# ↔ Python bridge for custom_components |
| `KazoHome.SDK.Blazor` | Blazor components for Home Assistant UI |
| `KazoHome.SDK.Automation` | Source generator attributes for automations |
| `KazoHome.SDK.Templates` | `dotnet new` project templates |

## 🏗️ Architecture

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

## 🐍 Python Bridge

For custom_components that require Python, KazoHome.SDK provides a high-performance bridge:

```csharp
// .NET side
services.AddKazoHomeBridge(options =>
{
    options.SocketPath = "/tmp/kazohome_bridge.sock";
});

// Call Python-side methods
var result = await bridge.SendRequestAsync<MyRequest, MyResponse>("my_method", request);
```

```python
# Python side (auto-generated proxy)
from kazohome_bridge import KazoHomeBridge

bridge = KazoHomeBridge("/tmp/kazohome_bridge.sock")
await bridge.connect()
result = await bridge.send_request("my_method", payload)
```

## 📁 Project Structure

```
KazoHome.sln
├── src/
│   ├── KazoHome.SDK.Core/          # Core abstractions
│   ├── KazoHome.SDK.Bridge/        # Python bridge
│   ├── KazoHome.SDK.Blazor/        # UI components
│   ├── KazoHome.SDK.Automation/    # Source generators
│   └── KazoHome.SDK.Templates/     # dotnet new templates
├── templates/
│   ├── kazohome-addon/             # Worker service template
│   ├── kazohome-card/              # Blazor WASM template
│   ├── kazohome-view/              # Blazor Server template
│   ├── kazohome-integration/       # Hybrid C#/Python template
│   └── kazohome-automation/        # Automation template
├── docs/
│   └── NETDAEMON_ANALYSIS.md       # NetDaemon analysis report
└── netdaemon/                      # NetDaemon submodule
```

## 🔧 Development

### Prerequisites

- .NET 10 SDK
- Visual Studio 2022+ or VS Code
- Docker (for containerized addons)
- Home Assistant (for testing)

### Build

```bash
# Clone with submodules
git clone --recurse-submodules https://github.com/werddomain/ha-solution.git
cd ha-solution

# Build SDK
dotnet build KazoHome.sln

# Pack templates
dotnet pack src/KazoHome.SDK.Templates
```

### Debug Configuration

Each template includes launch settings for:
- **Local Development**: Run with mocked Home Assistant
- **Docker**: Containerized debugging
- **Remote Debug (vsdbg)**: Debug in running HA container

## 📖 Documentation

- [NetDaemon Analysis Report](docs/NETDAEMON_ANALYSIS.md) - Strategy for NetDaemon integration
- [NetDaemon Documentation](https://netdaemon.xyz/) - Upstream documentation

## 📜 License

MIT License - see [LICENSE](LICENSE) for details
