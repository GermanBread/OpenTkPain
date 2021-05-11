using System.Runtime.InteropServices;
// System
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using uf.Utility.Logging;

namespace uf.Utility.Resources
{
    public static class ResourceManager
    {
        private static List<Resource> resources = new List<Resource>();
        /// <summary>
        /// Retrieves a resource. If the specified resource does not exist, the parameter will be treated as a file path and subsequently LoadResource will be called.
        /// </summary>
        public static Resource GetResource(string Name) {
            var _firstResult = resources.FirstOrDefault(x => x.Name == Name);
            if (_firstResult == default(Resource))
                // Resource not present in List? Load it!
                LoadResource(Name, null);
            var _output = resources.FirstOrDefault(x => x.Name == Name);
            return _output;
        }
        /// <summary>
        /// Loads a resource
        /// </summary>
        public static void LoadResource(string Name, string FilePath) {
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Loading resource {Name}, path {FilePath ?? "null"}"));
            // FIXME: I was not able to properly implement this using reflection ... so this will have to do for now...

            // Fallback
            var _files = Directory.GetFiles(Environment.CurrentDirectory);

            // Silently replace the output
            string _file = FilePath ?? _files.FirstOrDefault(x => x.Contains(Name));
            if (string.IsNullOrEmpty(_file)) {
                Logger.Log(new LogMessage(LogSeverity.Error, $"Failed to load resource {Name}, path {FilePath ?? "null"}"));
                return;
            }
            var _selected = Path.GetFullPath(_file);
            
            // Null-checking
            if (!File.Exists(_selected)) {
                // Try again with a different directory
                _files = Directory.GetFiles(AppContext.BaseDirectory);
                _selected = _files.FirstOrDefault(x => x.Contains(Name));
                if (_selected != null)
                    _selected = Path.GetFullPath(_selected);
                
                // We couldn't locate the resource referenced
                if (_selected == null) {
                    Logger.Log(new LogMessage(LogSeverity.Error, $"Failed to load resource {Name}, path {FilePath ?? "null"}"));
                    return;
                }
            }
            
            var _resource = new Resource(Name, _selected, new StreamReader(_selected));
            resources.Add(_resource);
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Loaded resource {Name}, path {FilePath ?? "null"}"));
        }
    }
}