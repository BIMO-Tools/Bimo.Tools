# Author custom BIMO commands with C# (SDK 1.3.0)

This guide is for developers who want to build and maintain their own DLL commands for **BIMO Toolbar** in Autodesk Revit. It covers the public SDK authoring contract, not the private BIMO host implementation.

**Start here if you are new:** [Build and run Hello BIMO](quick-start.md) is the shortest end-to-end walkthrough. Return to this page when adapting it or adding more commands.

## Choose a starting point

| Goal | Read or do |
| --- | --- |
| Run one working example | [Hello BIMO quick start](quick-start.md) |
| Create a different C# command | [Create your own command](#create-your-own-command) below |
| Add editable inputs and presets | [Settings and presets](#settings-and-presets) |
| Reuse logic via dependency injection | [Per-invocation services](#per-invocation-services) |
| Return useful output | [Results and reports](#results-and-reports) |
| Make a command available to an AI agent | [MCP and AI-agent execution](#mcp-and-ai-agent-execution) |
| Diagnose load or build failures | [Troubleshooting](#troubleshooting) |

## Requirements and compatibility

- Install Autodesk Revit and a compatible BIMO Toolbar. For SDK **1.3.0**, the documented baseline is **BIMO Toolbar 0.46.0**.
- Use a .NET SDK capable of building a C# library targeting `netstandard2.0` (the example uses .NET 8 for building).
- Restore the publicly released [Bimo.Tools NuGet package](https://www.nuget.org/packages/Bimo.Tools/1.3.0). Private BIMO repositories are **not** required for authoring.
- Build a **command DLL**, not a new Revit add-in. BIMO loads explicitly configured DLLs and generates Toolbar command adapters on Revit startup.

The public `Bimo.Tools` assembly contains attributes and interfaces, **not** a Revit API runtime or an MCP server. Targeting .NET Standard 2.0 does not guarantee every other dependency you add will work in every installed Revit version. Check your Revit-specific references separately.

## Create your own command

For a minimal command that displays a notification, create a `netstandard2.0` class library and pin the compatible package:

```powershell
dotnet new classlib -n MyBimoCommands -f netstandard2.0
dotnet add MyBimoCommands/MyBimoCommands.csproj package Bimo.Tools --version 1.3.0
```

In the project, use a *unique, stable* identifier for each command:

```csharp
using Bimo.Tools.Attributes;
using Bimo.Tools.Services;

namespace MyBimoCommands;

public static class GreetingCommands
{
    // Replace this example GUID with one generated for your own command.
    [Bimo("f29b90b8-6bb6-40c0-9e79-b7b6cd765e1a", DisplayName = "My Greeting")]
    public static void Execute(IUserNotifier notifier)
    {
        notifier.Inform("Hello from my command!");
    }
}
```

Generate a GUID once for a **new** command and preserve it when changing its name, implementation or DLL version. A rename should not create a second command identity. Pass it directly to `[Bimo]` unless the same command GUID is also consumed elsewhere in the command code; avoid a separate `const string CommandGuid` that is only used by the attribute. The public class and command method must be static-compatible: a public, non-generic class; a public, non-generic static method; **no overloaded command method names**. Parameters must be public host-supported service contracts. A settings parameter must match the declared `SettingsType`.

Build with `dotnet build MyBimoCommands/MyBimoCommands.csproj -c Release`. In Revit, open **BIMO → Open Settings → General → DLL commands**, add the **command DLL** file path, save and **restart Revit**. The generated command should appear on the BIMO ribbon.

Do **not** add `Bimo.Tools.dll` or its satellite resources to the configured DLL list or copy them beside your command DLL. The installed host supplies its own compatible contracts. Include other libraries **your command actually requires** only when compatible with the target Revit host. Load code only from trusted sources: DLL commands execute inside Revit with the current user's permissions.

## Recommended command architecture

Start with the **smallest static entrypoint** that meets the requirement. A
`public static` method marked with `[Bimo]` is the command boundary; the host
generates its Revit adapter. Do not implement another `IExternalCommand`,
command dispatcher or DI container inside the DLL.

**Simple command:** keep a short operation in the static method and request
only the existing host services it actually uses.

```csharp
using Bimo.Tools.Attributes;
using Bimo.Tools.Services;

public static class PingCommands
{
    // Generate a different GUID for every new command.
    [Bimo("dd8ca0af-30e7-42b2-9298-fb888c80d7cb", DisplayName = "Ping")]
    public static void Execute(IUserNotifier notifier) =>
        notifier.Inform("BIMO is ready.");
}
```

**Command with domain services:** when execution has substantial business
logic, multiple stages, or dependencies worth testing independently, keep the
static method as a thin delegate. Register the implementation in the **same
configured DLL** with `[BimoService]`; the host creates it per invocation.
This example shows the structure, not a complete model-processing algorithm.

```csharp
using Bimo.Tools.Attributes;
using Bimo.Tools.Commands;

public static class SelectionCommands
{
    [Bimo(
        "a380ff5f-4e78-45be-9390-363a2d3e33ba",
        DisplayName = "Selection summary",
        RequiredDocument = CommandDocumentRequirement.Project
    )]
    public static SelectionSummaryReport Execute(ISelectionSummaryService service) =>
        service.Execute();
}

public interface ISelectionSummaryService
{
    SelectionSummaryReport Execute();
}

[BimoService(typeof(ISelectionSummaryService))]
internal sealed class SelectionSummaryService : ISelectionSummaryService
{
    private readonly ICommandSelection selection;

    public SelectionSummaryService(ICommandSelection selection) =>
        this.selection = selection;

    public SelectionSummaryReport Execute() =>
        new SelectionSummaryReport(selection.GetSelectedElementUniqueIds().Count);
}

public sealed class SelectionSummaryReport : CommandReport
{
    private readonly int count;

    public SelectionSummaryReport(int count) => this.count = count;

    public override string Title => "Selection summary";
    public override string Summary => $"Selected: {count}";
}
```

The host owns command registration, project-document admission when declared,
settings/preset preparation, invocation-scoped DI and disposal, caller-specific
selection/notifications, common progress and transaction **services**, exception
and status normalization, telemetry, and Ribbon/MCP dispatch and report
presentation. Request those contracts rather than duplicating their
implementations. A `RequiredDocument = Project` declaration replaces manual
project admission in the entrypoint; it does not replace feature-specific
document or element validation.

The command owns its **domain behavior**: supported elements, business
validation, Revit geometry and model changes, tolerances, and any
operation-specific preservation or post-commit verification. The host provides
`ITransactionManager` but does not choose every command's write/verification
boundary. In particular, an MCP trial transaction is **not** automatically a
replacement for a command's Ribbon Dry Run or for a required check *after* an
inner transaction commits but *before* the operation is accepted.

Do not add a service interface for a trivial command, or move domain-specific
guards into the host without a second proven consumer. For a command that needs
settings, inject the existing `ISettings<T>` snapshot; for a modifying command,
use the host's transaction/progress services and test commit, rollback and
cancellation in real Revit. Agent capabilities remain explicit, audited opt-ins,
not an automatic consequence of using a static method. See
[per-invocation services](#per-invocation-services) and
[MCP and AI-agent execution](#mcp-and-ai-agent-execution).

## Settings and presets

For an editable command configuration, declare a public settings type and reference it from `[Bimo]`. The current [Hello BIMO source](../examples/HelloBimo/HelloBimoCommand.cs) demonstrates this pattern:

```csharp
public class MySettings
{
    [System.ComponentModel.DisplayName("Message")]
    public string Message { get; set; } = "Hello!";
}

// On the [Bimo] command method:
// SettingsType = typeof(MySettings)
//
// The command parameter is:
// ISettings<MySettings> settings
```

`ISettings<T>.Value` provides the settings prepared for the **current invocation**. Treat them as an input snapshot; do not assume that changing the object will save a preset. BIMO's settings editor uses the declared type and supported property metadata. Keep public settings types and values compatible with the host.

For commands that declare settings, the Toolbar generates the usual command/preset variants. Users can create, rename and duplicate saved presets in BIMO **Settings** and assign up to three quick-launch variants (`Variant #1`, `#2`, `#3`). Variant launches use their assigned settings; invalid input may reopen the settings editor. Presets belong to the interactive Ribbon workflow, **not** to an agent request: MCP does not implicitly select the currently active preset.

For a command without settings, omit `SettingsType` and its `ISettings<T>` argument. Do not manufacture a business settings class solely to support an agent trial.

## Per-invocation services

Use the public `[BimoService]` attribute when the command DLL needs its own DI-backed service. An implementation can be `internal` but must have a **public constructor**; the interface used by a public entrypoint must be public.

```csharp
public interface IGreetingFormatter
{
    string Format(string message);
}

[BimoService(typeof(IGreetingFormatter))]
internal sealed class GreetingFormatter : IGreetingFormatter
{
    public GreetingFormatter() { }

    public string Format(string message) => message.Trim();
}

// Add IGreetingFormatter formatter to the command method parameters.
```

Register implementations in the same configured source DLL, not in an unrelated dependency. BIMO manages the invocation scope: scoped services are created **after** document policy and settings preparation, and disposed after the operation. Do not cache host/Revit-scoped services or retain them beyond a command execution. Duplicate registrations, invalid contracts and dependency cycles are not supported.

Host-provided interfaces include `ISettings<T>`, `IUserNotifier`, transactions, progress and other services **where available in the installed host**. The public SDK does not supply a standalone service container for executing a Revit command outside BIMO.

## Results and reports

A static command can return `void` for a simple operation, or return a public concrete `CommandReport` subtype to provide structured output. The existing [Hello BIMO report](../examples/HelloBimo/HelloBimoCommand.cs) defines `Title` and `Summary`. The BIMO host owns presentation; your code should not depend on an extra report UI being opened in an AI-agent invocation.

`CommandReport` also supports additional details and selectable element UniqueIds. Only report genuinely persistent elements as selectable results; an element created during a rolled-back trial must **not** be presented as a surviving model change.

For a command that requires an active Revit project, add `RequiredDocument = CommandDocumentRequirement.Project` to `[Bimo]`. This rejects no-document and family-document contexts **before** settings or client service activation. The default is `None`, and SDK 1.3.0 exposes only `None` and `Project`.

## MCP and AI-agent execution

**One command, multiple callers:** BIMO Toolbar and an eligible MCP client use the same host command runtime. The SDK does not create an MCP endpoint and installing your DLL does **not** grant an AI agent permission to run it.

For an audited command, capabilities are declared explicitly on `[Bimo]`:

- `SupportsAgentDryRun = true` makes a trial *eligible* in a compatible host. The host uses a Revit transaction group to roll back trial model edits, but **cannot roll back** external effects or changes in other documents.
- `SupportsAgentCommit = true` separately opts in to persistent execution. Do not enable it simply because the trial path works. Audit model effects, interaction requirements, and cancellation/error behavior first.

**Hello BIMO** demonstrates `SupportsAgentDryRun = true` but does not opt in to committed agent execution. Agent availability still depends on the **installed host**, current Revit session, applicable access checks and explicit **BIMO-side approval for the exact request**. Client-side permission alone is insufficient.

For an agent-driven command, avoid interactive prompts, direct selection picking and modal dialogs. The agent must supply the supported typed inputs and explicit document-bound element references described by the live command catalog. Agent calls do not silently read the active Ribbon selection or a saved preset. For actual setup, use **BIMO → Interact → AI Agent Access** and follow the [BIMO MCP user documentation](https://bimo.tools/docs/library/agent-access). Never publish an MCP credential in source code or documentation.

If you are not certain an operation is safely reversible, **leave the agent capability flags disabled**. Test it through the ordinary Ribbon path first.

## Testing and distributing

From the **public Bimo.Tools checkout**:

```powershell
dotnet build Bimo.Tools.csproj -c Release
dotnet run --project tests/Bimo.Tools.Smoke/Bimo.Tools.Smoke.csproj -c Release -p:UsePublishedPackage=false
dotnet run --project tests/Bimo.Tools.Smoke/Bimo.Tools.Smoke.csproj -c Release -p:UsePublishedPackage=true
```

These verify the exported source API and the **published NuGet 1.3.0** consumer independently. They do not start Revit, generate real host wrappers or verify live model changes. For your own DLL, complete an **installed-Revit acceptance check**: build, load and restart; inspect the Ribbon label; edit settings; launch the normal command; verify its result; validate no-project/family behavior if required; then separately test permitted agent trials and document rollback if relevant.

The public repository is a **reviewed SDK snapshot**. Its `SOURCE.json` records provenance; public CI does not publish a NuGet package. Contributions to SDK contracts are reviewed and carried to the private upstream before a controlled public sync.

## Troubleshooting

| Symptom | What to check |
| --- | --- |
| No command after restart | Confirm BIMO version, configured **command** DLL path, that the DLL still exists, and that Revit was restarted |
| Command not discovered | Public/non-generic static entrypoint, unique GUID, no overloaded name, valid public settings and parameter types |
| Error loading types or services | Match SDK to host; ensure client service contracts are registered only once and constructors are resolvable; check adjacent non-SDK dependencies |
| Settings missing or not editable | Declare `SettingsType`; use supported property metadata; ensure `ISettings<T>` refers to the same type |
| No command in a family/no document | A `Project` document requirement intentionally blocks this context |
| Agent does not list or execute the command | Check live session catalog, explicit capability flags, access checks and BIMO approval; a Ribbon command is not automatically agent-enabled |
| Unexpected duplicate or renamed command | Keep the original GUID for updates and verify you did not leave an old DLL path configured |
| SDK compile succeeds but Revit fails | Source-level smoke does not validate the installed host: check installed Toolbar, Revit version and generator/host diagnostics |

For a first working exercise, return to [Hello BIMO Quick Start](quick-start.md).
