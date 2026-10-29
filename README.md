# Bimo.Tools

**Bimo.Tools** is the open-source .NET SDK contract library for building custom C# commands for [BIMO Add-in for Autodesk Revit](https://bimo.tools).

It provides command attributes, settings metadata, and host-provided service interfaces. The **BIMO Toolbar** discovers and executes the commands inside Revit; this package alone is not a command runner, a Revit API replacement, or the BIMO Toolbar source code.

| | |
| --- | --- |
| **NuGet package** | [Bimo.Tools](https://www.nuget.org/packages/Bimo.Tools) |
| **Target framework** | .NET Standard 2.0 |
| **License** | [MIT](LICENSE) |
| **Host** | A compatible, installed [BIMO Toolbar](https://bimo.tools) |

## Install the SDK

For new commands targeting the **BIMO Toolbar 0.46.0** contract set, use **Bimo.Tools 1.3.0**:

```xml
<PackageReference Include="Bimo.Tools" Version="[1.3.0]" />
```

Restore packages from the standard NuGet feed. You do not need access to BIMO's private application source to author a command.

**Match the SDK to the installed host.** Package version, .NET Standard compatibility, and an unchanged assembly version are not guarantees of runtime compatibility with an older Toolbar.

### SDK and Toolbar compatibility

| SDK version | Toolbar compatibility / scope |
| --- | --- |
| **1.1.0** | Documented baseline: **Toolbar 0.45.0 beta**. Historical baseline for the original Hello BIMO sample. |
| **1.2.0** | Adds command-description and client-service authoring contracts; requires a matching SDK/host combination. Do not assume support from the 0.45.0 host. |
| **1.3.0** | Supported by **Toolbar 0.46.0**. Includes resource-backed command descriptions, invocation-scoped client services, and project-document requirements checked before settings and service activation. |

The [Toolbar 0.46.0 release notes](https://bimo.tools/releases) document the 1.3.0 contracts. Check the release notes and your installed Toolbar version before upgrading command DLLs. The host controls which commands can be invoked by an AI agent; installing the SDK alone does **not** grant agent execution permission.

## Build your first command

Start with the [Hello World walkthrough](docs/quick-start.md) and the [example project](examples/HelloBimo/).

**Hello World with SDK 1.3.0:** The sample and the published-package smoke test both reference **[1.3.0]**. This open project is the sole home of `HelloWorldTools`, moved out of BIMO Samples; the private project no longer compiles these greeting commands. The example covers settings, scoped client services, structured reports and project-document requirements. See the [Quick Start](docs/quick-start.md) for build, loading and test instructions.

In outline:

1. Create a C# class library targeting a framework appropriate for your command and host; the provided sample targets `netstandard2.0`.
2. Reference a **pinned, host-compatible** `Bimo.Tools` NuGet version.
3. Expose a public static command method with a stable unique `[Bimo]` GUID, using supported attributes and host service parameters.
4. Build the command DLL. In Revit, open **BIMO → Open Settings → General → DLL commands** and add the path to **your command DLL**, then save and restart Revit.
5. Launch the generated command from the BIMO ribbon.

Starting with **Toolbar 0.46.0**, configured DLL commands load at Revit startup **without** the former *Use alpha features* switch. Older development hosts have different setup requirements.

**Deployment:** Do not copy `Bimo.Tools.dll` or its satellite resource assemblies next to your command DLL; the installed Toolbar supplies its compatible SDK contracts. Package only your command DLL and other dependencies it actually needs. User DLLs are trusted executable code running in the Revit process, so review sources before configuring them.

## What's in the SDK?

- **Command metadata:** attributes for stable IDs, display names, settings, and supported execution capabilities.
- **Settings and input metadata:** contracts for command configuration and generated editors.
- **Host-provided services:** interfaces for notifications, settings, transactions, and progress, where supported by the installed host.
- **Structured command results:** public report and status contracts for commands that need them.

The SDK does not include the Autodesk Revit API, BIMO host internals, or an MCP server. Using Autodesk API types requires the appropriate separate Revit references and host compatibility.

## Build and validate this repository

```sh
dotnet build Bimo.Tools.csproj --configuration Release
dotnet run --project tests/Bimo.Tools.Smoke/Bimo.Tools.Smoke.csproj --configuration Release -p:UsePublishedPackage=false

dotnet run --project tests/Bimo.Tools.Smoke/Bimo.Tools.Smoke.csproj --configuration Release -p:UsePublishedPackage=true
dotnet pack Bimo.Tools.csproj --configuration Release
```

The example and smoke consumer default to published **NuGet 1.3.0**. Pass
`-p:UsePublishedPackage=false` on the command line to validate the extracted source
SDK: the global property selects project references consistently in both projects.
Explicit `true` selects package references throughout the graph. Run the two
commands above separately to validate both providers.

Public CI tests SDK source plus an anonymous **NuGet 1.3.0** consumer. This does **not** publish NuGet packages or validate execution in an installed Revit host.

### Public source snapshot vs. released package

This repository is a **reviewed public snapshot** extracted from the private BIMO monorepo, which remains the source of truth during the current extraction stage.

- The public `Bimo.Tools.csproj` now specifies **1.3.0**, synchronized from the reviewed private SDK through [public sync PR #5](https://github.com/BIMO-Tools/Bimo.Tools/pull/5).
- The Hello BIMO sample and published-package smoke test are pinned to **1.3.0**.
- The **1.3.0 NuGet/host contract** is documented by the Toolbar 0.46.0 release; the synchronized public sources do not, by themselves, prove that their bytes match the published NuGet package.

The extraction source commit and file hashes are recorded in [`SOURCE.json`](SOURCE.json). This snapshot is not guaranteed to be byte-for-byte equivalent to any published NuGet artifact. Keep the package version derived from the reviewed upstream SDK source; it is not an instruction to republish packages from this public repository.

## Contributing and support

See [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request. Maintainers review changes and carry accepted work into the private upstream until a separate source-of-truth migration is approved. Merging a public PR does not directly update the shipping Toolbar or publish a NuGet package.

**Website:** [bimo.tools](https://bimo.tools) · **Release notes:** [BIMO Toolbar](https://bimo.tools/releases) · **Support:** [support@bimo.tools](mailto:support@bimo.tools)
