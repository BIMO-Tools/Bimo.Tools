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

## Example

```csharp
using Bimo.Tools.Attributes;
using Bimo.Tools.Services;

public sealed class HelloSettings
{
    public string Message { get; set; } = "Hello from BIMO";
}

public static class HelloCommand
{
    [Bimo("d4f35d5d-c439-4e49-b9ac-4ab06c091150",
        DisplayName = "Hello BIMO", SettingsType = typeof(HelloSettings))]
    public static void Execute(ISettings<HelloSettings> settings)
    {
        System.Console.WriteLine(settings.Value.Message);
    }
}
```

Build the command DLL against the approved NuGet version. **Do not bundle `Bimo.Tools.dll`** or its resource satellites with the user-command archive; the BIMO host provides them.

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
