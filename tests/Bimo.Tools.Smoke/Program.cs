using System.ComponentModel;
using System.Reflection;
using Bimo.MyUserCommands;
using Bimo.Tools.Attributes;
using Bimo.Tools.Commands;
using Bimo.Tools.Services;

var command = typeof(ExampleCommand).GetMethod(nameof(ExampleCommand.Execute))!;
var attribute = command.GetCustomAttribute<BimoAttribute>();
Assert(attribute is { Guid: "d4f35d5d-c439-4e49-b9ac-4ab06c091150" }, "BimoAttribute contract");
Assert(attribute!.SettingsType == typeof(ExampleSettings), "SettingsType contract");

var helloMethod = typeof(HelloWorldTools).GetMethod(nameof(HelloWorldTools.HelloWorld))!;
var helloAttribute = helloMethod.GetCustomAttribute<BimoAttribute>();
Assert(helloAttribute?.DisplayName == "Hello BIMO", "Quick Start command display name");
Assert(helloAttribute?.SettingsType == typeof(HelloWorldSettings), "Quick Start settings type");
Assert(
    helloAttribute?.RequiredDocument == CommandDocumentRequirement.Project,
    "Project policy metadata"
);
Assert(
    Guid.Parse(helloAttribute!.Guid) == Guid.Parse("4b2f90e1-62fa-46ff-b20a-dabdb1a7305a"),
    "Quick Start GUID"
);
var service = typeof(HelloWorldTools)
    .Assembly.GetTypes()
    .Single(t =>
        t.GetCustomAttribute<BimoServiceAttribute>()?.ServiceType == typeof(IHelloWorldFormatter)
    );
Assert(service.GetConstructor(Type.EmptyTypes) != null, "Public service constructor");
var formatter = (IHelloWorldFormatter)Activator.CreateInstance(service)!;
Assert(
    typeof(HelloWorldSettings)
        .GetProperty(nameof(HelloWorldSettings.Message))!
        .GetCustomAttribute<DisplayNameAttribute>()
        is not null,
    "Message is exposed in the BIMO settings editor"
);
Assert(helloAttribute.SupportsAgentDryRun, "Existing Hello World agent trial opt-in");
var helloSettings = new HelloWorldSettings { Message = "  Configured greeting  " };
var helloReport = HelloWorldTools.HelloWorld(
    new SampleSettings<HelloWorldSettings>(helloSettings),
    formatter
);
Assert(
    helloReport.Title == "Hello BIMO" && helloReport.Summary == "Configured greeting",
    "Configured greeting report"
);
Assert(
    helloSettings.Message == "  Configured greeting  ",
    "Formatting preserves the settings snapshot"
);

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

attribute.CatalogId = "example";
Assert(attribute.CatalogId == "example", "Static catalog metadata contract");
Assert(
    new BimoServiceAttribute(typeof(IDisposable)).ServiceType == typeof(IDisposable),
    "Client service metadata contract"
);
Assert(new BimoServiceAttribute().ServiceType is null, "Self registration contract");
Assert(
    !typeof(BimoAttribute)
        .Assembly.GetExportedTypes()
        .Any(type => type.Name == "CommandCatalogAttribute"),
    "Catalog wrapper metadata belongs to the host, not the SDK"
);
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
Console.WriteLine("Bimo.Tools 1.3.0 smoke checks passed.");

static void Assert(bool condition, string name)
{
    if (!condition)
        throw new InvalidOperationException($"Failed: {name}");
}

internal sealed class SampleSettings<T>(T value) : ISettings<T>
    where T : class
{
    public T Value { get; } = value;
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
