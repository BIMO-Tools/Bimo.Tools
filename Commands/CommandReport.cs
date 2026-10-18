namespace Bimo.Tools.Commands;

public enum CommandStatus
{
    Succeeded,
    Cancelled,
    Failed,
}

/// <summary>A structured operation result. Caller adapters own its presentation.</summary>
public abstract class CommandReport
{
    public virtual CommandStatus Status => CommandStatus.Succeeded;
    public abstract string Title { get; }
    public abstract string Summary { get; }
    public virtual string MainContent => string.Empty;
    public virtual IReadOnlyList<string> Details => Array.Empty<string>();
    public virtual IReadOnlyList<string> SelectableElementUniqueIds => Array.Empty<string>();
    public virtual string? SelectionActionLabel => null;
}

/// <summary>An outcome without operation data or a final presentation.</summary>
public sealed class CommandStatusReport(CommandStatus status, string? summary = null)
    : CommandReport
{
    public override CommandStatus Status { get; } = status;
    public override string Title => string.Empty;
    public override string Summary => summary ?? string.Empty;
}

/// <summary>Common operation boundary for static, native and Dynamo bindings.</summary>
public interface ICommandHandler
{
    CommandReport Execute(IServiceProvider services);
}
