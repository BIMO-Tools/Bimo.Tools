namespace Bimo.Tools.Attributes;

[AttributeUsage(AttributeTargets.All)]
public sealed class DescriptionRuAttribute(string description) : Attribute
{
    public string Description { get; } = description;
}
