namespace Bimo.Tools.Services;

public enum TransactionExecutionStatus
{
    NotStarted,
    Committed,
    RolledBack,
    Pending,
    Failed,
}
