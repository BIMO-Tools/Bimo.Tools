namespace Bimo.Tools.Commands;

/// <summary>
/// An expected, user-correctable command input error. The message must be
/// localized and safe to show to a user or an approved agent; do not include
/// local file paths, credentials, stack traces or other sensitive diagnostics.
/// Unlike OperationCanceledException, this is reported as Failed.
/// </summary>
public sealed class CommandInputException : Exception
{
    public CommandInputException(string message)
        : base(message) { }

    public CommandInputException(string message, Exception innerException)
        : base(message, innerException) { }
}
