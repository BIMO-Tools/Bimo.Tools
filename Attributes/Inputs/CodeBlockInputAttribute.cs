using System.Reflection;

namespace Bimo.Tools.Attributes.Inputs;

public class CodeBlockInputAttribute : Attribute
{
    public static bool IsCodeBlock(PropertyInfo? propertyInfo)
    {
        return propertyInfo?.CustomAttributes?.Any(
                ca => ca.AttributeType == typeof(CodeBlockInputAttribute)
            ) == true;
    }
}
