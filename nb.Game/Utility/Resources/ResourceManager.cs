// System
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using nb.Game.Utility.Logging;

namespace nb.Game.Utility.Resources
{
    public static class ResourceManager
    {
        private static List<Resource> resources = new List<Resource>();
        /// <summary>
        /// Retrieves a resource
        /// </summary>
        public static Resource GetResource(string Name) {
            var _firstResult = resources.FirstOrDefault(x => x.Name == Name);
            if (_firstResult == default(Resource))
                // Resource not present in List? Load it!
                LoadResource(Name, null);
            var _output = resources.First(x => x.Name == Name);
            return _output;
        }
        /// <summary>
        /// Loads a resource
        /// </summary>
        public static void LoadResource(string Name, string Path) {
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Loading resource {Name}, path {Path ?? "null"}"));
            // FIXME: I was not able to properly implement this using reflection ... so this will have to do for now...
            var _files = Directory.GetFiles(Environment.CurrentDirectory);
            // Silently replace the output
            var _selected = Path ?? _files.FirstOrDefault(x => x.Contains(Name));
            // Null-checking
            if (_selected == null || !File.Exists(_selected)) {
                Logger.Log(new LogMessage(LogSeverity.Error, $"Failed to load resource {Name}, path {Path ?? "null"}"));
                return;
            }
            var _resource = new Resource(Name, _selected, new StreamReader(_selected));
            resources.Add(_resource);
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Loaded resource {Name}, path {Path ?? "null"}"));
        }
    }
}