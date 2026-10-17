using System.Reflection;
using Bimo.Tools.Attributes;
using Bimo.Tools.Services;

var command = typeof(ExampleCommand).GetMethod(nameof(ExampleCommand.Execute))!;
var attribute = command.GetCustomAttribute<BimoAttribute>();
Assert(attribute is { Guid: "d4f35d5d-c439-4e49-b9ac-4ab06c091150" }, "BimoAttribute contract");
Assert(attribute!.SettingsType == typeof(ExampleSettings), "SettingsType contract");

var meta = new DynamoInputAttribute("", DynamoInputKind.ScopeBoxSelection, "Scope Box", "{\"min\":2}");
Assert(meta.GetMetaInt("min") == 2, "DynamoInput metadata");
Assert(meta.GetMetaInt("missing", 10) == 10, "DynamoInput metadata fallback");

var manager = new FakeTransactionManager();
var executed = false;
manager.ExecuteAndEnsureCommitted("write", () => executed = true);
Assert(executed && manager.LastOptions?.Rollback == false, "Commit contract");
executed = false;
manager.ExecuteAndEnsureRolledBack("temporary", () => executed = true);
Assert(executed && manager.LastOptions?.Rollback == true, "Rollback contract");
Console.WriteLine("Bimo.Tools smoke checks passed.");

static void Assert(bool condition, string name)
{
    if (!condition) throw new InvalidOperationException($"Failed: {name}");
}

public sealed class ExampleSettings
{
    public string Message { get; set; } = "hello";
}

public static class ExampleCommand
{
    [Bimo("d4f35d5d-c439-4e49-b9ac-4ab06c091150", DisplayName = "Example", SettingsType = typeof(ExampleSettings))]
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
        string transactionLabel, Action action, TransactionExecutionOptions? options = null)
    {
        LastOptions = options ?? new TransactionExecutionOptions();
        action();
        return new TransactionExecutionResult(
            LastOptions.Rollback ? TransactionExecutionStatus.RolledBack : TransactionExecutionStatus.Committed);
    }
}
