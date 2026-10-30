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

The latest published package is **Bimo.Tools 1.4.1**. Use it only with an installed BIMO host verified for its contracts, including the 1.4 expected-error handling:

```xml
<PackageReference Include="Bimo.Tools" Version="[1.4.1]" />
```

For the **documented BIMO Toolbar 0.46.0 baseline**, continue using SDK **1.3.0** and the matching [Hello BIMO Quick Start](docs/quick-start.md) until you have verified a newer host. Restore packages from nuget.org without private repository access.

**Match the SDK to the installed host.** Package version, .NET Standard compatibility, and an unchanged assembly version do not guarantee compatibility with an older Toolbar.

### SDK and Toolbar compatibility

| SDK version | Toolbar compatibility / scope |
| --- | --- |
| **1.1.0** | Documented baseline: **Toolbar 0.45.0 beta**. Historical baseline for the original Hello BIMO sample. |
| **1.2.0** | Adds command-description and client-service authoring contracts; requires a matching SDK/host combination. Do not assume support from the 0.45.0 host. |
| **1.3.0** | Supported by **Toolbar 0.46.0**. Includes resource-backed command descriptions, invocation-scoped client services, and project-document requirements checked before settings and service activation. |
| **1.4.0** | Introduces `CommandInputException` for expected, user-correctable errors; requires an updated host that recognizes this exception as `Failed` rather than `Cancelled`. |
| **1.4.1** | Documentation/README refresh of 1.4.0. Requires the same matching 1.4-compatible host; no new SDK API is introduced by this patch release. |

The [BIMO release notes](https://bimo.tools/releases) document installed Toolbar versions. The previously verified 0.46.0/1.3.0 pairing does **not** prove 1.4.x host compatibility. Verify the deployed host before upgrading command DLLs. Installing the SDK alone does **not** grant AI-agent execution permission.

## Developer documentation

| Start with | What you will learn |
| --- | --- |
| **[Hello BIMO Quick Start](docs/quick-start.md)** | Build a verified command DLL, register it in Revit, edit a setting and run it from the Ribbon |
| **[C# command authoring guide](docs/command-authoring.md)** | Create your own command: identity and loading, settings, presets, scoped services, reports, Revit requirements, MCP capabilities and troubleshooting |
| **[Example project](examples/HelloBimo/)** | Inspect the separately pinned, verified SDK 1.3.0 sample project |

### Build your first command

**Hello BIMO uses SDK 1.3.0 intentionally.** The example and published-package smoke consumer remain pinned to **[1.3.0]** and target the documented Toolbar 0.46.0 baseline. They demonstrate settings, scoped client services, reports and project-document requirements. They have **not** been migrated to 1.4.1 or revalidated against a 1.4-compatible installed host. Follow the [Quick Start](docs/quick-start.md) for the existing tested exercise.

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
- **Correctable command errors (1.4+):** `CommandInputException` with a localized, user-safe message, classified as `Failed` by a compatible host without an entrypoint `try/catch`.

The SDK does not include the Autodesk Revit API, BIMO host internals, or an MCP server. Using Autodesk API types requires the appropriate separate Revit references and host compatibility.

## Build and validate this repository

```sh
dotnet build Bimo.Tools.csproj --configuration Release
dotnet run --project tests/Bimo.Tools.Smoke/Bimo.Tools.Smoke.csproj --configuration Release -p:UsePublishedPackage=false

dotnet run --project tests/Bimo.Tools.Smoke/Bimo.Tools.Smoke.csproj --configuration Release -p:UsePublishedPackage=true
dotnet pack Bimo.Tools.csproj --configuration Release -p:GeneratePackageOnBuild=false
```

The example and smoke consumer default to published **NuGet 1.3.0**. Pass
`-p:UsePublishedPackage=false` on the command line to validate the extracted source
SDK: the global property selects project references consistently in both projects.
Explicit `true` selects package references throughout the graph. Run the two
commands above separately to validate both providers.

Public CI tests the current SDK source and the separately pinned **NuGet 1.3.0** Hello BIMO consumer. This is deliberate: it does **not** establish 1.4.1 compatibility with an installed Revit host, publish packages or validate live Revit execution.

### Public source snapshot vs. released package

This repository is a **reviewed public snapshot** extracted from the private BIMO monorepo, which remains the source of truth during the current extraction stage.

- The public `Bimo.Tools.csproj` is synchronized from reviewed private SDK source and targets **1.4.1** in this snapshot.
- The Hello BIMO sample and published-package smoke test remain pinned to **1.3.0** until a matching 1.4.x host is tested.
- **NuGet publication is independent of source synchronization.** The snapshot and its `SOURCE.json` provenance do not prove binary identity with a previously published NuGet package or compatibility with an installed host.

The extraction source commit and file hashes are recorded in [`SOURCE.json`](SOURCE.json). This snapshot is not guaranteed to be byte-for-byte equivalent to any published NuGet artifact. Keep the package version derived from the reviewed upstream SDK source; it is not an instruction to republish packages from this public repository.

## Contributing and support

See [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request. Maintainers review changes and carry accepted work into the private upstream until a separate source-of-truth migration is approved. Merging a public PR does not directly update the shipping Toolbar or publish a NuGet package.

**Website:** [bimo.tools](https://bimo.tools) · **Release notes:** [BIMO Toolbar](https://bimo.tools/releases) · **Support:** [support@bimo.tools](mailto:support@bimo.tools)
