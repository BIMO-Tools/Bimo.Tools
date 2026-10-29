using System.ComponentModel;
using Bimo.Tools.Attributes;
using Bimo.Tools.Commands;
using Bimo.Tools.Services;

namespace Bimo.MyUserCommands;

// Standalone public SDK example; no Revit API or private host dependency.
public static class HelloWorldTools
{
    [Bimo(
        "4B2F90E1-62FA-46FF-B20A-DABDB1A7305A",
        DisplayName = "Hello World",
        SettingsType = typeof(HelloWorldSettings),
        RequiredDocument = CommandDocumentRequirement.Project,
        SupportsAgentDryRun = true
    )]
    public static HelloWorldReport HelloWorld(
        ISettings<HelloWorldSettings> settings,
        IHelloWorldFormatter formatter
    ) => new(formatter.Format(settings.Value.Message));
}

public class HelloWorldSettings
{
    // BIMO displays properties with explicit display metadata in the settings editor.
    [DisplayName("Message")]
    public string Message { get; set; } = "Привет из пользовательской команды!";
}

public interface IHelloWorldFormatter
{
    string Format(string message);
}

[BimoService(typeof(IHelloWorldFormatter))]
internal sealed class HelloWorldFormatter : IHelloWorldFormatter
{
    // Standard DI needs a public constructor, even for an internal implementation.
    public HelloWorldFormatter() { }

    public string Format(string message) =>
        string.IsNullOrWhiteSpace(message) ? "Привет из пользовательской команды!" : message.Trim();
}

public sealed class HelloWorldReport(string summary) : CommandReport
{
    public override string Title => "Hello World";
    public override string Summary { get; } = summary;
}
