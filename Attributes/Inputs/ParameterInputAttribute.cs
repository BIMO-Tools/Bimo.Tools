namespace Bimo.Tools.Attributes.Inputs;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class ParameterInputAttribute(ElementCategory category) : InputAttribute
{
    public ElementCategory Category { get; } = category;
    public ParameterType ParameterType { get; set; }
}

public enum ElementCategory
{
    Rooms,
    Doors,
    CurtainWallPanels
}

public enum ParameterType
{
    String,
    Number,
}
