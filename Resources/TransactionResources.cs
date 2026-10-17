using System.Globalization;
using System.Resources;

namespace Bimo.Tools.Resources;

internal static class TransactionResources
{
    private static readonly ResourceManager ResourceManager = new(
        "Bimo.Tools.Resources.TransactionResources",
        typeof(TransactionResources).Assembly
    );

    public static string TransactionNotCommittedMessage =>
        GetString(nameof(TransactionNotCommittedMessage));
    public static string TransactionNotRolledBackMessage =>
        GetString(nameof(TransactionNotRolledBackMessage));

    private static string GetString(string name) =>
        ResourceManager.GetString(name, CultureInfo.CurrentUICulture)
        ?? throw new MissingManifestResourceException(
            $"The transaction resource '{name}' was not found."
        );
}
