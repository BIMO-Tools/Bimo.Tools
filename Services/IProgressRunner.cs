namespace Bimo.Tools.Services;

/// <summary>
/// Runs synchronous command steps with host-provided progress and cooperative cancellation.
/// </summary>
public interface IProgressRunner
{
    /// <summary>
    /// Executes indexed steps and throws <see cref="OperationCanceledException"/> when the user
    /// cancels the operation.
    /// </summary>
    /// <param name="operation">The name of the operation displayed by the host.</param>
    /// <param name="stepCount">The number of steps to execute.</param>
    /// <param name="executeStep">A callback invoked with the current step index.</param>
    void Run(string operation, int stepCount, Action<int> executeStep);
}
