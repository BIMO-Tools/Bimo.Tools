namespace Bimo.Tools.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
public sealed class VisibleWhenAttribute : Attribute
{
    public VisibleWhenAttribute(string propertyName, object? expectedValue)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentException("Property name must not be empty.", nameof(propertyName));

        PropertyName = propertyName;
        ExpectedValue = expectedValue;
    }

    public string PropertyName { get; }
    public object? ExpectedValue { get; }
}
