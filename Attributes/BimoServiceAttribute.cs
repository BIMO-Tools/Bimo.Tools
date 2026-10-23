namespace Bimo.Tools.Attributes;

/// <summary>
/// Registers an implementation from a configured command DLL for one invocation.
/// Omit the contract to register the implementation type itself. Implementations
/// may be internal, with public constructors. Registration never activates them.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class BimoServiceAttribute(Type? serviceType = null) : Attribute
{
    /// <summary>The service contract, or null for registration as the implementation type.</summary>
    public Type? ServiceType { get; } = serviceType;
}
