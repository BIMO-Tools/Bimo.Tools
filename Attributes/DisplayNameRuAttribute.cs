namespace Bimo.Tools.Attributes;

[AttributeUsage(AttributeTargets.All)]
public sealed class DisplayNameRuAttribute(string displayName) : Attribute
{
    public string DisplayName { get; } = displayName;
}
