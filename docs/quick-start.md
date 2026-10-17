# Quick Start: create a BIMO C# command

This walkthrough builds the **Hello BIMO** sample from the public [Bimo.Tools SDK](https://www.nuget.org/packages/Bimo.Tools/1.1.0). The command uses `IUserNotifier` to show a message in Revit, without a direct Revit API dependency.

## Requirements

- **Windows** with a compatible installed [BIMO Add-in for Revit](https://bimo.tools). For `Bimo.Tools 1.1.0`, the documented host baseline is **BIMO Toolbar 0.45.0 beta**; other host versions must be verified separately.
- A suitable installed **Autodesk Revit** version supported by your BIMO add-in.
- The **.NET 8 SDK or newer** for `dotnet build` and internet access for the first NuGet restore. The command targets **.NET Standard 2.0**, so it does not need a separate Revit API reference.
- **Use alpha features** enabled in BIMO's **General** settings: DLL-command loading currently requires this flag.

## 1. Get the example

Clone the public repository and open its root in PowerShell:

```powershell
git clone https://github.com/BIMO-Tools/Bimo.Tools.git
cd Bimo.Tools
```

The editable sample is:

- [`examples/HelloBimo/HelloBimoCommand.cs`](../examples/HelloBimo/HelloBimoCommand.cs) — one public static method marked with `[Bimo]`.
- [`examples/HelloBimo/HelloBimo.csproj`](../examples/HelloBimo/HelloBimo.csproj) — standalone library with the **exact** `Bimo.Tools [1.1.0]` NuGet package reference.

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
3. Enable **Use alpha features** (if not already enabled).
4. Under **DLL commands** (Russian UI: **DLL-команды**), click **Add / Browse** and select the full path to `HelloBimo.dll`, not the path to `Bimo.Tools.dll`.
5. Save the settings and **restart Revit**. BIMO discovers the configured DLL and generates the command during Revit startup.
6. Find the **Hello BIMO** command in the BIMO ribbon and run it. The host should display **Hello from BIMO!**.

User command DLLs execute inside the Revit process with the user's permissions. Load only assemblies from sources you trust.

## 4. Adapt the command

Edit [`HelloBimoCommand.cs`](../examples/HelloBimo/HelloBimoCommand.cs), change the display name/message, and rebuild. If you create **another** command, generate a fresh GUID for its `[Bimo]` attribute; keep a given command's GUID stable across releases.

Use a **public**, non-generic static method in a **public** class. Avoid method overloads (the generator requires unique method names). Host services such as `IUserNotifier` are passed as method parameters when the command runs.

Rebuild the library and restart Revit to reload the changed command.

## Troubleshooting

- **The command is missing:** confirm **Use alpha features** is enabled, **DLL commands** points to the compiled `HelloBimo.dll` rather than an SDK DLL, and restart Revit. Confirm the file still exists at the configured path.
- **A method is not discovered:** it must be `public static` in a public, non-generic type and carry a valid, unique `[Bimo("guid")]` attribute. Do not overload the method name.
- **Assembly/type errors:** use a host compatible with the pinned `Bimo.Tools 1.1.0`; do not bundle another copy of `Bimo.Tools.dll` or its satellites with the command.
- **Edits don't show up:** rebuild in **Release**, verify the DLL path and restart Revit. Generation is cached until its inputs change.
- **Build fails:** check the installed .NET SDK (`dotnet --info`) and NuGet connectivity (`dotnet restore`).

## Validation boundary

The public CI compiles the example from the **published** `Bimo.Tools 1.1.0` NuGet package and exercises its attributed method with a fake host notifier. This checks the public SDK contract but **does not replace a manual installed-Revit smoke test**.

The SDK's source of truth currently remains the private BIMO monorepo; public changes are synchronized through reviewed pull requests. The example never publishes NuGet or changes the SDK runtime.
