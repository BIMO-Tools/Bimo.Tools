namespace Bimo.Tools.Attributes.Inputs;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public abstract class InputAttribute : Attribute
{
    public InputAttribute() { }
}
