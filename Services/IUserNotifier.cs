namespace Bimo.Tools.Services;

public interface IUserNotifier
{
    void Inform(string? message = null);
    bool Warn(string mainInstruction, string mainContent);
    void SayError(string message);
    void DisplayResult(Output output, string? commandName = null);
}
