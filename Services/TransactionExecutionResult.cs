namespace Bimo.Tools.Services;

/// <summary>
/// Describes the final state reported by a host transaction.
/// </summary>
public sealed class TransactionExecutionResult(TransactionExecutionStatus status)
{
    public TransactionExecutionStatus Status { get; } = status;

    public bool IsCommitted => Status == TransactionExecutionStatus.Committed;
}
