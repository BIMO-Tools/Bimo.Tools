namespace Bimo.Tools.Services;

public interface ITransactionManager
{
    /// <summary>
    /// Executes an action in a transaction using the legacy non-modal behavior.
    /// </summary>
    [Obsolete(
        "Use ExecuteInTransaction(string, Action, TransactionExecutionOptions) and inspect the returned status."
    )]
    void ExecuteInTransaction(string transactionLabel, Action action, bool rollback);

    /// <summary>
    /// Executes an action in a transaction and returns the status reported by the host.
    /// </summary>
    /// <param name="transactionLabel">The transaction name shown by the host.</param>
    /// <param name="action">The action to execute inside the transaction.</param>
    /// <param name="options">
    /// Optional settings. Omitted or null options use a new instance with
    /// Rollback=false, ForceModalHandling=true, and ClearAfterRollback=false.
    /// </param>
    /// <remarks>Always inspect the returned status before continuing dependent work.</remarks>
    TransactionExecutionResult ExecuteInTransaction(
        string transactionLabel,
        Action action,
        TransactionExecutionOptions? options = null
    );
}
