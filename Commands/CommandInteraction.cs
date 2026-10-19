namespace Bimo.Tools.Commands;

/// <summary>Element references resolved by the caller in the invocation's document.</summary>
public interface ICommandSelection
{
    /// <summary>Whether native selection helpers may prompt the user in this invocation.</summary>
    bool CanPromptForSelection { get; }

    IReadOnlyList<string> GetSelectedElementUniqueIds();
    void ShowElements(IReadOnlyList<string> elementUniqueIds);
}
