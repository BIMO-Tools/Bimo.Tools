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
    /// Gets or sets the type of the custom window to use for the settings dialog.
    /// If not specified, a default window will be used.
    /// </summary>
    public Type? CustomWindowType { get; set; }

    public Type? SettingsType { get; set; }
}
