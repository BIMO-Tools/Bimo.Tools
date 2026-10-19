namespace Bimo.Tools.Services;

/// <summary>
/// Caller-contextual notifications. Warn requests confirmation, never merely an
/// informational warning. Suppressed UI must not authorize an agent operation.
/// Structured command reports are presented by the caller after execution.
/// </summary>
public interface IUserNotifier
{
    void Inform(string? message = null);
    bool Warn(string mainInstruction, string mainContent);
    void SayError(string message);
    void DisplayResult(Output output, string? commandName = null);
}
