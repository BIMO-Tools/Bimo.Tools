using Bimo.Tools.Attributes;
using Bimo.Tools.Services;

namespace Bimo.QuickStart
{
    public static class HelloBimoCommand
    {
        // Generate a new GUID for each command you create; keep it stable thereafter.
        [Bimo("e377b294-9ddd-4a55-a90e-b428c118c89e", DisplayName = "Hello BIMO")]
        public static void Execute(IUserNotifier notifier)
        {
            notifier.Inform("Hello from BIMO!");
        }
    }
}
