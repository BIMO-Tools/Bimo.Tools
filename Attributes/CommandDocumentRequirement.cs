namespace Bimo.Tools.Attributes;

/// <summary>The document required before command settings or services are created.</summary>
public enum CommandDocumentRequirement
{
    /// <summary>No document restriction is declared.</summary>
    None = 0,

    /// <summary>An active project document is required; family documents are rejected.</summary>
    Project = 1,
}
