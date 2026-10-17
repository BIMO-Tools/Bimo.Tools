using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bimo.Tools.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class DynamoInputAttribute(
    string nodeId,
    DynamoInputKind kind,
    string label,
    string? metadataJson = null
) : Attribute
{
    public string NodeGuid { get; } = nodeId;
    public DynamoInputKind Kind { get; } = kind;
    public string Label { get; } = label;
    public string? MetadataJson { get; } = metadataJson;

    // ленивый кэш распарсенного JSON объекта
    private Dictionary<string, JToken>? _meta;
    private Dictionary<string, JToken> Meta => _meta ??= ParseMeta(MetadataJson);

    public bool TryGetMeta(string key, out JToken value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            value = JValue.CreateNull();
            return false;
        }

        return Meta.TryGetValue(key, out value);
    }

    public string? GetMetaString(string key, string? fallback = null)
    {
        if (!TryGetMeta(key, out var v) || v.Type == JTokenType.Null)
            return fallback;

        // если строка — вернём строку, иначе JSON-строку/примитив
        return v.Type == JTokenType.String ? v.Value<string>() : v.ToString(Formatting.None);
    }

    public double? GetMetaDouble(string key, double? fallback = null)
    {
        if (!TryGetMeta(key, out var v) || v.Type == JTokenType.Null)
            return fallback;

        try
        {
            // Newtonsoft умеет конвертировать и из string, и из number
            return v.Value<double>();
        }
        catch
        {
            return fallback;
        }
    }

    public int? GetMetaInt(string key, int? fallback = null)
    {
        if (!TryGetMeta(key, out var v) || v.Type == JTokenType.Null)
            return fallback;

        try
        {
            return v.Value<int>();
        }
        catch
        {
            return fallback;
        }
    }

    public bool? GetMetaBool(string key, bool? fallback = null)
    {
        if (!TryGetMeta(key, out var v) || v.Type == JTokenType.Null)
            return fallback;

        try
        {
            return v.Value<bool>();
        }
        catch
        {
            return fallback;
        }
    }

    public IReadOnlyList<T> GetMetaArray<T>(
        string key,
        IReadOnlyList<T>? fallback = null,
        JsonSerializerSettings? settings = null
    )
    {
        if (!TryGetMeta(key, out var v) || v.Type != JTokenType.Array)
            return fallback ?? [];

        var arr = (JArray)v;
        if (arr.Count == 0)
            return [];

        // Быстрый путь для простых типов
        bool isSimple =
            typeof(T) == typeof(string)
            || typeof(T).IsPrimitive
            || typeof(T).IsEnum
            || typeof(T) == typeof(decimal)
            || typeof(T) == typeof(DateTime)
            || typeof(T) == typeof(Guid);

        var result = new List<T>(arr.Count);

        foreach (var token in arr)
        {
            if (token is null || token.Type == JTokenType.Null)
                continue;

            try
            {
                if (isSimple)
                {
                    // string/int/bool/etc.
                    var value = token.ToObject<T>();
                    if (value is not null)
                        result.Add(value);
                    continue;
                }

                // Сложные типы: поддержим оба варианта
                // 1) token = " { ... } " (строка с JSON объекта)
                if (token.Type == JTokenType.String)
                {
                    var s = token.Value<string>();
                    if (s is null || string.IsNullOrWhiteSpace(s))
                        continue;

                    var obj = JsonConvert.DeserializeObject<T>(s, settings);
                    if (obj is not null)
                        result.Add(obj);

                    continue;
                }

                // 2) token = { ... } (объект)
                {
                    var obj = token.ToObject<T>();
                    if (obj is not null)
                        result.Add(obj);
                }
            }
            catch
            {
                // лучше логнуть выше по стеку, но атрибут пусть не падает
            }
        }

        return result;
    }

    public T? GetMetaAs<T>(string key, T? fallback = default)
    {
        if (!TryGetMeta(key, out var v) || v.Type == JTokenType.Null)
            return fallback;

        try
        {
            return v.ToObject<T>();
        }
        catch
        {
            return fallback;
        }
    }

    private static Dictionary<string, JToken> ParseMeta(string? json)
    {
        var dict = new Dictionary<string, JToken>(StringComparer.OrdinalIgnoreCase);

        if (json is null || string.IsNullOrWhiteSpace(json))
            return dict;

        JObject obj;
        try
        {
            obj = JObject.Parse(json);
        }
        catch
        {
            return dict;
        }

        foreach (var p in obj.Properties())
        {
            // JToken можно хранить напрямую
            dict[p.Name] = p.Value;
        }

        return dict;
    }
}

public static class DynamoInputMetadataKeys
{
    public const string SelectedIndex = "selectedIndex";
    public const string CustomItems = "customItems"; // [{Name, Item}, ...]

    public const string MinimumValue = "min";
    public const string MaximumValue = "max";
    public const string StepValue = "step";
    public const string NumberType = "numberType";

    public const string SelectedCategory = "selectedCategory";

    public const string FilePathFilter = "filePathFilter";
}

public enum DynamoInputKind
{
    Unknown,

    // Core primitive inputs
    String,
    Boolean,
    Number,
    Integer,
    DateTime,
    Color,
    FilePath,
    DirectoryPath,
    CustomSelection,

    // Revit dropdown selections
    CategorySelection,
    LevelSelection,
    ViewSelection,
    ViewFamilyTypeSelection,
    SheetSelection,

    WallTypeSelection,
    FloorTypeSelection,
    RoofTypeSelection,
    CeilingTypeSelection,

    FamilyTypeSelection, // DSRevitNodesUI.FamilyTypes
    ElementClassSelection, // DSRevitNodesUI.ElementTypes (Element Classes)

    StructuralColumnTypeSelection,
    StructuralFramingTypeSelection,

    // Host selections
    ElementSelection,
    ElementsSelection,
    ElementByCategorySelection,
    ElementsByCategorySelection,

    FaceSelection,
    FacesSelection,

    EdgeSelection,
    EdgesSelection,

    PointOnFaceSelection,
    ReferenceOnElementSelection,
    UVOnFaceSelection,

    DividedSurfaceFamiliesSelection,

    // Other dropdown selections (например AdvanceSteel / сторонние)
    DropdownSelection,

    // Append new kinds to preserve existing serialized numeric values.
    FilledRegionTypeSelection,
    ScopeBoxSelection,
}
