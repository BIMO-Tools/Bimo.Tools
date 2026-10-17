# Bimo.Tools

Open-source **BIMO SDK contracts** for authoring custom commands for the [BIMO Add-in for Autodesk Revit](https://bimo.tools).

- **License:** MIT
- **Target:** .NET Standard 2.0
- **Stable NuGet:** [Bimo.Tools](https://www.nuget.org/packages/Bimo.Tools)
- **Host:** An installed, compatible BIMO Toolbar is required to run a command. This SDK does not include or replace the host or Revit API.

## Install

Pin the approved SDK version, for example:

```xml
<PackageReference Include="Bimo.Tools" Version="[1.1.0]" />
```

Bimo.Tools 1.1.0 requires a matching updated BIMO host (baseline: **Toolbar 0.45.0 beta** for the existing Trace Facade consumer). API and assembly versions alone do not guarantee host compatibility.

## Quick Start: your first command

Follow **[Quick Start: build and run Hello BIMO](docs/quick-start.md)** for a working C# project, the exact DLL build command and instructions to load it in Revit.

The example source lives in [`examples/HelloBimo/`](examples/HelloBimo/HelloBimoCommand.cs). It uses the published NuGet package and the host-provided `IUserNotifier` service — no Revit API or private BIMO projects are needed.

**Do not deploy `Bimo.Tools.dll`** or its satellite resource assemblies beside a command DLL; the installed BIMO host supplies them.

## Development

```sh
dotnet build Bimo.Tools.csproj --configuration Release
dotnet run --project tests/Bimo.Tools.Smoke/Bimo.Tools.Smoke.csproj --configuration Release
dotnet pack Bimo.Tools.csproj --configuration Release
```

CI validates the SDK and its smoke consumer but **never publishes to nuget.org**.

## Source, contributions and releases

During the first extraction stage, this repository is a reviewed public snapshot of the SDK from a private BIMO monorepo. The originating commit and SHA-256 file manifest are recorded in `SOURCE.json`. It is **not** automatically equivalent to the byte-for-byte source used to publish any prior NuGet package.

Read [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request. Maintainers review contributions and carry accepted changes into the private upstream until a separate source-of-truth migration is approved. Pull requests to this public snapshot alone do not change the shipping BIMO host.

BIMO Toolbar, its implementation, `Bimo.Shared`, SaaS services and customer commands are not part of this repository.

Project: [bimo.tools](https://bimo.tools) · Support: support@bimo.tools
