# Attestia Desktop

Financial truth infrastructure for Windows. A WinUI 3 desktop application and .NET SDK for blockchain intent verification, attestation, and reconciliation.

## What This Is

Attestia verifies financial intent before it hits the chain. Instead of auditing after the fact, you approve transactions structurally -- with typed intents, cryptographic proofs, and deterministic reconciliation.

This repository contains:

- **Attestia.Core** -- Domain models and shared types for verification workflows
- **Attestia.Client** -- HTTP client SDK mirroring the @attestia/sdk TypeScript API
- **Attestia.Sidecar** -- Node.js process manager for hosting the Attestia backend from .NET
- **Attestia.App** -- WinUI 3 desktop application (the reference UI)

## NuGet Packages

| Package | Description |
|---------|-------------|
| `Attestia.Core` | Domain models, enums, and shared types for intent verification and attestation workflows |
| `Attestia.Client` | HTTP client SDK with typed clients for Intents, Proofs, Reconciliation, Compliance, and Events |
| `Attestia.Sidecar` | Node.js sidecar process management -- spawning, health checks, auto-restart, and graceful shutdown |

### Quick Start

```csharp
// Attestia.Client -- verify an intent
using Attestia.Client;

var client = new AttestiaClient(options);
var result = await client.Intents.VerifyAsync(intent);
```

```csharp
// Attestia.Sidecar -- manage the Node.js backend
using Attestia.Sidecar;

await using var sidecar = new SidecarProcess(options);
await sidecar.StartAsync();
// Sidecar handles health checks, auto-restart, and graceful shutdown
```

## Architecture

```
Attestia.App (WinUI 3)          UI layer -- Windows desktop experience
    |
Attestia.Client                 HTTP SDK -- typed API clients
    |
Attestia.Core                   Domain -- models, enums, shared types
    |
Attestia.Sidecar                Process management -- Node.js backend lifecycle
```

The desktop app composes these layers, but each library stands on its own. `Attestia.Client` works in console apps, ASP.NET services, or any .NET 9+ project.

## Prerequisites

- .NET 9.0 SDK
- Node.js 20+ (for the Attestia backend)
- Windows 10 1809+ (for the desktop app)

## Building

```bash
dotnet restore
dotnet build
dotnet test
```

## Support

- **Questions / help:** [Discussions](https://github.com/mcp-tool-shop-org/Attestia-Desktop/discussions)
- **Bug reports:** [Issues](https://github.com/mcp-tool-shop-org/Attestia-Desktop/issues)

## License

[MIT](LICENSE)
