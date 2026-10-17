using Bimo.Tools.Resources;

namespace Bimo.Tools.Services;

/// <summary>Checks host-neutral transaction results before dependent work continues.</summary>
public static class TransactionExecutionResultExtensions
{
    /// <summary>Requires Committed; confirmed rollback cancels with a cause, other states fail diagnostically.</summary>
    public static void EnsureCommitted(
        this TransactionExecutionResult result,
        string transactionLabel
    )
    {
        if (result.IsCommitted)
            return;

        InvalidOperationException cause = CreateCause(
            transactionLabel,
            result.Status,
            TransactionExecutionStatus.Committed
        );

        // Revit has already rolled back and handled the modal failure dialog.
        // Retain the cause so the command boundary logs a controlled failure once.
        if (result.Status == TransactionExecutionStatus.RolledBack)
            throw new OperationCanceledException(
                TransactionResources.TransactionNotCommittedMessage,
                cause
            );

        throw new InvalidOperationException(
            TransactionResources.TransactionNotCommittedMessage,
            cause
        );
    }

    /// <summary>Requires RolledBack; any other status is an invalid-operation error.</summary>
    public static void EnsureRolledBack(
        this TransactionExecutionResult result,
        string transactionLabel
    )
    {
        if (result.Status == TransactionExecutionStatus.RolledBack)
            return;

        // A missing rollback is an invariant violation, not a confirmed cancellation.
        throw new InvalidOperationException(
            TransactionResources.TransactionNotRolledBackMessage,
            CreateCause(transactionLabel, result.Status, TransactionExecutionStatus.RolledBack)
        );
    }

    private static InvalidOperationException CreateCause(
        string transactionLabel,
        TransactionExecutionStatus actualStatus,
        TransactionExecutionStatus expectedStatus
    ) =>
        new(
            $"Transaction '{transactionLabel}' returned {actualStatus}; expected {expectedStatus}."
        );
}
