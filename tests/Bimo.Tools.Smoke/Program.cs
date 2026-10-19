using System.Reflection;
using Bimo.QuickStart;
using Bimo.Tools.Attributes;
using Bimo.Tools.Services;

var command = typeof(ExampleCommand).GetMethod(nameof(ExampleCommand.Execute))!;
var attribute = command.GetCustomAttribute<BimoAttribute>();
Assert(attribute is { Guid: "d4f35d5d-c439-4e49-b9ac-4ab06c091150" }, "BimoAttribute contract");
Assert(attribute!.SettingsType == typeof(ExampleSettings), "SettingsType contract");

// The public Quick Start example must build and execute against the published SDK.
var helloMethod = typeof(HelloBimoCommand).GetMethod(nameof(HelloBimoCommand.Execute))!;
var helloAttribute = helloMethod.GetCustomAttribute<BimoAttribute>();
Assert(helloAttribute?.DisplayName == "Hello BIMO", "Quick Start command display name");
Assert(helloAttribute?.Guid == "e377b294-9ddd-4a55-a90e-b428c118c89e", "Quick Start GUID");
var notifier = new FakeUserNotifier();
HelloBimoCommand.Execute(notifier);
Assert(notifier.Message == "Hello from BIMO!", "Quick Start host notifier interaction");

var meta = new DynamoInputAttribute(
    "",
    DynamoInputKind.ScopeBoxSelection,
    "Scope Box",
    "{\"min\":2}"
);
Assert(meta.GetMetaInt("min") == 2, "DynamoInput metadata");
Assert(meta.GetMetaInt("missing", 10) == 10, "DynamoInput metadata fallback");

var manager = new FakeTransactionManager();
var executed = false;
manager.ExecuteAndEnsureCommitted("write", () => executed = true);
Assert(executed && manager.LastOptions?.Rollback == false, "Commit contract");
executed = false;
manager.ExecuteAndEnsureRolledBack("temporary", () => executed = true);
Assert(executed && manager.LastOptions?.Rollback == true, "Rollback contract");

#if SOURCE_SDK
// These optional authoring contracts are new source SDK APIs, not part of NuGet 1.1.0.
var selection = new CommandServices();
Bimo.Tools.Commands.ICommandHandler handler = new SelectionReportHandler();
var report = handler.Execute(selection);
Assert(report.Status == Bimo.Tools.Commands.CommandStatus.Succeeded, "Report default status");
Assert(report.Summary == "Selected element", "Handler report content");
Assert(
    report.SelectableElementUniqueIds.SequenceEqual(new[] { "element-1" }),
    "Selection references"
);
Assert(!selection.CanPromptForSelection, "Caller selection capability");
selection.ShowElements(report.SelectableElementUniqueIds);
Assert(selection.Shown.SequenceEqual(report.SelectableElementUniqueIds), "Selection presentation");
foreach (var status in Enum.GetValues<Bimo.Tools.Commands.CommandStatus>())
{
    var outcome = new Bimo.Tools.Commands.CommandStatusReport(status, "Outcome");
    Assert(outcome.Status == status && outcome.Summary == "Outcome", "Status-only report");
}
#endif
Console.WriteLine("Bimo.Tools smoke checks passed.");

static void Assert(bool condition, string name)
{
    if (!condition)
        throw new InvalidOperationException($"Failed: {name}");
}

public sealed class ExampleSettings
{
    public string Message { get; set; } = "hello";
}

public static class ExampleCommand
{
    [Bimo(
        "d4f35d5d-c439-4e49-b9ac-4ab06c091150",
        DisplayName = "Example",
        SettingsType = typeof(ExampleSettings)
    )]
    public static void Execute(ISettings<ExampleSettings> settings) { }
}

internal sealed class FakeTransactionManager : ITransactionManager
{
    public TransactionExecutionOptions? LastOptions { get; private set; }

    [Obsolete]
    public void ExecuteInTransaction(string transactionLabel, Action action, bool rollback)
    {
        action();
    }

    public TransactionExecutionResult ExecuteInTransaction(
        string transactionLabel,
        Action action,
        TransactionExecutionOptions? options = null
    )
    {
        LastOptions = options ?? new TransactionExecutionOptions();
        action();
        return new TransactionExecutionResult(
            LastOptions.Rollback
                ? TransactionExecutionStatus.RolledBack
                : TransactionExecutionStatus.Committed
        );
    }
}

internal sealed class FakeUserNotifier : IUserNotifier
{
    public string? Message { get; private set; }

    public void Inform(string? message = null) => Message = message;

    public bool Warn(string mainInstruction, string mainContent) => true;

    public void SayError(string message) => throw new InvalidOperationException(message);

    public void DisplayResult(Bimo.Tools.Output output, string? commandName = null) =>
        throw new NotSupportedException();
}

#if SOURCE_SDK
internal sealed class CommandServices : IServiceProvider, Bimo.Tools.Commands.ICommandSelection
{
    public bool CanPromptForSelection => false;
    public IReadOnlyList<string> Shown { get; private set; } = Array.Empty<string>();

    public object? GetService(Type type) =>
        type == typeof(Bimo.Tools.Commands.ICommandSelection) ? this : null;

    public IReadOnlyList<string> GetSelectedElementUniqueIds() => new[] { "element-1" };

    public void ShowElements(IReadOnlyList<string> ids) => Shown = ids;
}

internal sealed class SelectionReportHandler : Bimo.Tools.Commands.ICommandHandler
{
    public Bimo.Tools.Commands.CommandReport Execute(IServiceProvider services)
    {
        var selection = (Bimo.Tools.Commands.ICommandSelection)
            services.GetService(typeof(Bimo.Tools.Commands.ICommandSelection))!;
        return new SelectionReport(selection.GetSelectedElementUniqueIds());
    }
}

internal sealed class SelectionReport(IReadOnlyList<string> ids) : Bimo.Tools.Commands.CommandReport
{
    public override string Title => "Selection";
    public override string Summary => "Selected element";
    public override IReadOnlyList<string> SelectableElementUniqueIds => ids;
}
#endif
