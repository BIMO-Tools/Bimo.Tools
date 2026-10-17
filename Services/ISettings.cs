namespace Bimo.Tools.Services;

/// <summary>
/// Used to retrieve configured <typeparamref name="TSettings"/> instances.
/// </summary>
/// <typeparam name="TSettings">The type of options being requested.</typeparam>
public interface ISettings<out TSettings>
    where TSettings : class
{
    /// <summary>
    /// The default configured <typeparamref name="TSettings"/> instance
    /// </summary>
    TSettings Value { get; }
}
