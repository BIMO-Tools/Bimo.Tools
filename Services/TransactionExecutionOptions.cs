namespace Bimo.Tools.Services;

/// <summary>
/// Configures host transaction execution without exposing host API types.
/// </summary>
public sealed class TransactionExecutionOptions
{
    /// <summary>Rolls back temporary changes instead of committing. Defaults to false.</summary>
    public bool Rollback { get; set; }

    /// <summary>Uses modal failure handling for synchronous completion. Defaults to true.</summary>
    public bool ForceModalHandling { get; set; } = true;

    /// <summary>Clears host warnings after rollback. Defaults to false.</summary>
    public bool ClearAfterRollback { get; set; }
}
