namespace Bimo.Tools.Services;

/// <summary>Executes host transactions and requires the expected completion before returning.</summary>
public static class TransactionManagerExtensions
{
    /// <summary>Executes a write and requires a confirmed commit.</summary>
    /// <remarks>Confirmed rollback throws cancellation with a diagnostic cause. Other noncommit states throw an invalid-operation error.</remarks>
    public static void ExecuteAndEnsureCommitted(
        this ITransactionManager manager,
        string transactionLabel,
        Action action,
        TransactionExecutionOptions? options = null
    )
    {
        if (options?.Rollback == true)
            throw new ArgumentException(
                "A commit operation cannot request rollback.",
                nameof(options)
            );

        manager
            .ExecuteInTransaction(transactionLabel, action, options)
            .EnsureCommitted(transactionLabel);
    }

    /// <summary>Executes temporary changes and requires a confirmed rollback.</summary>
    /// <remarks>Requests rollback and copies failure-handling overrides without modifying caller-owned options.</remarks>
    public static void ExecuteAndEnsureRolledBack(
        this ITransactionManager manager,
        string transactionLabel,
        Action action,
        TransactionExecutionOptions? options = null
    )
    {
        // The operation requests rollback without changing caller-owned options.
        var rollbackOptions = new TransactionExecutionOptions { Rollback = true };
        if (options is not null)
        {
            rollbackOptions.ForceModalHandling = options.ForceModalHandling;
            rollbackOptions.ClearAfterRollback = options.ClearAfterRollback;
        }

        manager
            .ExecuteInTransaction(transactionLabel, action, rollbackOptions)
            .EnsureRolledBack(transactionLabel);
    }
}
