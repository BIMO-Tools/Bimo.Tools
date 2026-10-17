using System.Reflection;

namespace Bimo.Tools.Attributes.Inputs;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class FilePathInputAttribute : InputAttribute
{
    public string Filter { get; set; } = "All Files";

    public FilePathInputAttribute() { }

    public static bool IsFilePath(PropertyInfo? propertyInfo) =>
        propertyInfo?.CustomAttributes?.Any(ca => ca.AttributeType == typeof(FilePathInputAttribute))
        == true;
}
