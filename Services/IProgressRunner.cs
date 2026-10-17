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
    void Run(string operation, int stepCount, Action<int> executeStep);
}
