// System
using System.IO;

using nb.Game.Utility.Logging;

namespace nb.Game.Utility.Resources
{
    public static class ResourceManager
    {
        // TODO: Implement
        public static Stream GetResourceStream(string Name) {
            Logger.Log(new LogMessage(LogSeverity.Warning, "ResourceManager", "GetResourceStream is not implemented"));
            return null;
        }
    }
}