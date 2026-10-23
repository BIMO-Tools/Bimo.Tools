namespace Bimo.Tools.Attributes;

/// <summary>
/// Specifies command options for a BIMO tool. This attribute is used to define
/// a unique identifier (GUID), display name, and settings dialog options for a command method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class BimoAttribute(string guid) : Attribute
{
    /// <summary>
    /// Gets the unique identifier (GUID) for the command.
    /// </summary>
    public string Guid { get; } = guid;

    /// <summary>
    /// Gets the display name for the command.
    /// </summary>
    public string? DisplayName { get; set; }

    public string? IconKey { get; set; }

    /// <summary>
    /// Optional stable catalog ID. Descriptions come from Resources.{id}Catalog
    /// in the source command namespace and assembly; this does not publish a DLL.
    /// </summary>
    public string? CatalogId { get; set; }

    /// <summary>
    /// Gets or sets the type of the custom window to use for the settings dialog.
    /// If not specified, a default window will be used.
    /// </summary>
    public Type? CustomWindowType { get; set; }

    public Type? SettingsType { get; set; }

    /// <summary>
    /// Optional document requirement checked by the host before settings preparation
    /// and command service activation. The default declares no restriction.
    /// </summary>
    public CommandDocumentRequirement RequiredDocument { get; set; }

    /// <summary>
    /// Opt in after auditing for agent trials: the host rolls back model transactions
    /// and suppresses framework prompts. Business settings and reports are optional.
    /// External effects and changes in other documents are not undone.
    /// </summary>
    public bool SupportsAgentDryRun { get; set; }

    /// <summary>Opt in after auditing persistent model effects and autonomous execution.</summary>
    public bool SupportsAgentCommit { get; set; }

    /// <summary>Optional embedded resource base name in this command assembly.</summary>
    public string? CommitWarningResourceName { get; set; }

    /// <summary>Resource key for the warning shown before actual execution, not a trial.</summary>
    public string? CommitWarningResourceKey { get; set; }
}
