namespace Bimo.Tools.Attributes.Inputs;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public sealed class CategoriesInputAttribute : InputAttribute
{
    public CategoriesInputAttribute() { }

    public bool AllowAllIfEmptyList { get; set; }
}
