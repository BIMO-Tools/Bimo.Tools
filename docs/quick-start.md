# Quick Start: create a BIMO C# command

This walkthrough builds the **Hello BIMO** sample against the published [Bimo.Tools 1.3.0 package](https://www.nuget.org/packages/Bimo.Tools/1.3.0). It illustrates settings, an invocation-scoped client service, a structured report, and project-document admission without a Revit API reference. Use **BIMO Toolbar 0.46.0 or a verified compatible host**.

## Requirements

- **Windows** with [BIMO Add-in for Revit](https://bimo.tools), **Toolbar 0.46.0** or a verified compatible host. Earlier 0.45.0 beta hosts do not support these 1.3.0 contracts.
- A suitable installed **Autodesk Revit** version supported by your BIMO add-in.
- The **.NET 8 SDK or newer** for `dotnet build` and internet access for the first NuGet restore. The command targets **.NET Standard 2.0**, so it does not need a separate Revit API reference.
- Open an **active project document** (not a family document). This example declares `RequiredDocument = CommandDocumentRequirement.Project` and will not run without a project. There is no **Use alpha features** setting in Toolbar 0.46.0.

## 1. Get the example

Clone the public repository and open its root in PowerShell:

```powershell
git clone https://github.com/BIMO-Tools/Bimo.Tools.git
cd Bimo.Tools
```

The editable sample is:

- [`examples/HelloBimo/HelloBimoCommand.cs`](../examples/HelloBimo/HelloBimoCommand.cs) — the sole `HelloWorldTools` implementation, moved from BIMO Samples into this open project. `HelloWorld` is the single command and demonstrates the 1.3.0 contracts.
- [`examples/HelloBimo/HelloBimo.csproj`](../examples/HelloBimo/HelloBimo.csproj) — standalone library with the **exact** `Bimo.Tools [1.3.0]` NuGet package reference.

## 2. Build the DLL

From the repository root:

```powershell
dotnet restore examples/HelloBimo/HelloBimo.csproj
dotnet build examples/HelloBimo/HelloBimo.csproj -c Release --no-restore
```

The command library is:

```text
examples/HelloBimo/bin/Release/netstandard2.0/HelloBimo.dll
```

You can use that DLL directly from this location or copy **only the command DLL** to a stable folder. Do **not** register or copy `Bimo.Tools.dll`, `Bimo.Tools.resources.dll`, or unrelated build dependencies; the BIMO host already supplies its own SDK contracts. For additional libraries referenced by your own commands, place those dependencies beside your command DLL as needed.

## 3. Load it into BIMO

1. Open Revit with a compatible BIMO Add-in for Revit installed.
2. Open BIMO **Open Settings** → **General**.
3. Open a Revit **project**. No alpha-features switch is needed in Toolbar 0.46.0.
4. Under **DLL commands** (Russian UI: **DLL-команды**), click **Add / Browse** and select the full path to `HelloBimo.dll`, not the path to `Bimo.Tools.dll`.
5. Save the settings and **restart Revit**. BIMO discovers the configured DLL and generates the command during Revit startup.
6. Find **Hello BIMO** in the BIMO ribbon. Its generated settings contain a **Message** value (default: **Привет из пользовательской команды!**). Run the command; BIMO presents a report containing that message.

User command DLLs execute inside the Revit process with the user's permissions. Load only assemblies from sources you trust.

## 4. Understand the 1.3.0 example

The example combines four authoring contracts:

- **Settings:** `[DisplayName("Message")]` exposes the property in the settings editor. `HelloWorldSettings.Message` is editable in BIMO. The host passes an invocation-specific `ISettings<HelloWorldSettings>` snapshot.
- **Scoped service:** `[BimoService]` registers an internal formatter behind the public `IHelloWorldFormatter` contract, resolved per invocation.
- **Structured report:** `HelloWorldReport` returns the greeting; BIMO presents the result rather than the DLL opening its own dialog.
- **Document requirement:** `RequiredDocument = Project` requires an active Revit project before settings or services are created.

Change the message, rebuild and restart Revit. Keep the existing GUID when updating this command; generate a new GUID for a new command. Public static entrypoints must be non-generic and non-overloaded. Hello BIMO retains `SupportsAgentDryRun = true`. Agent trials still require exact host approval; the command does not opt in to committed agent execution.

## Troubleshooting

- **The command is missing:** confirm **DLL commands** points to the compiled `HelloBimo.dll` rather than an SDK DLL, and restart Revit. Confirm the file still exists at the configured path. Ensure an active project document is open; an older host may not support this SDK.
- **A method is not discovered:** it must be `public static` in a public, non-generic type and carry a valid, unique `[Bimo("guid")]` attribute. Do not overload the method name.
- **Assembly/type errors:** use Toolbar 0.46.0 or a verified host compatible with `Bimo.Tools 1.3.0`; do not bundle another copy of `Bimo.Tools.dll` or its satellites with the command.
- **Edits don't show up:** rebuild in **Release**, verify the DLL path and restart Revit. Generation is cached until its inputs change.
- **Build fails:** check the installed .NET SDK (`dotnet --info`) and NuGet connectivity (`dotnet restore`).

## Validation boundary

The public CI builds this sample using **published Bimo.Tools 1.3.0** and runs the smoke consumer against both the extracted source SDK and anonymous NuGet 1.3.0. Tests verify metadata, settings, the client service declaration and direct report production. They **do not** instantiate the real BIMO generator/DI host or replace a manual installed-Revit acceptance test.

The SDK's source of truth currently remains the private BIMO monorepo; public changes are synchronized through reviewed pull requests. The example never publishes NuGet or changes the SDK runtime.
