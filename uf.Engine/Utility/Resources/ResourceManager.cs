using System.Net.Sockets;
using System.Runtime.InteropServices;
// System
using System;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Collections.Generic;

using uf.Utility.Logging;

namespace uf.Utility.Resources
{
    public static class ResourceManager
    {
        private static readonly List<Resource> resources = new();
        /// <summary>
        /// Retrieves a resource. If the specified resource does not exist, the parameter will be treated as a file path and subsequently LoadResource will be called.
        /// </summary>
        public static Resource GetFile(string Name) {
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
        public static void LoadFile(string Name, string FilePath) {
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Loading file {Name}, path {FilePath ?? "null"}"));
            
            FilePath ??= Name;

            if (File.Exists(FilePath)) {
                resources.Add(new Resource(Name, FilePath, new StreamReader(FilePath)));
                goto End;
            }

            var _workdirFiles = Directory.GetFiles(Environment.CurrentDirectory);
            var _bindirFiles = Directory.GetFiles(AppContext.BaseDirectory);
            
            // Attempt to obtain the files from where the executable is stored
            if (_bindirFiles.Any(x => x.Contains(FilePath))) {
                var _file = _bindirFiles.First(x => x.Contains(FilePath));
                resources.Add(new Resource(Name, _file, new StreamReader(_file)));
            } else if (_workdirFiles.Any(x => x.Contains(FilePath))) {
                var _file = _workdirFiles.First(x => x.Contains(FilePath));
                resources.Add(new Resource(Name, _file, new StreamReader(_file)));
            } else {
                Logger.Log(new LogMessage(LogSeverity.Error, $"Failed to load file {Name}, path {FilePath ?? "null"}"));
            }
            
            End:
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Loaded file {Name}, path {FilePath ?? "null"}"));
        }

        public static void LoadResource(string Name, string ResourcePath) {
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Loading resource {Name}, path {ResourcePath ?? "null"}"));

            if (!(ResourcePath ?? Name).EndsWith(".ufr")) {
                Logger.Log(new LogMessage(LogSeverity.Warning, $"Falling back to file loading for file {Name}, path {ResourcePath ?? "null"}. This file is not a resource file (.ufr)"));
                LoadFile(Name, ResourcePath);
                return;
            }

            ResourcePath ??= Name;

            if (File.Exists(ResourcePath)) {
                addResources(Name, ResourcePath);
                goto End;
            }

            var _workdirFiles = Directory.GetFiles(Environment.CurrentDirectory);
            var _bindirFiles = Directory.GetFiles(AppContext.BaseDirectory);
            
            // Attempt to obtain the files from where the executable is stored
            if (_bindirFiles.Any(x => x.Contains(ResourcePath))) {
                var _file = _bindirFiles.First(x => x.Contains(ResourcePath));
                addResources(Name, _file);
            } else if (_workdirFiles.Any(x => x.Contains(ResourcePath))) {
                var _file = _workdirFiles.First(x => x.Contains(ResourcePath));
                addResources(Name, _file);
            } else {
                Logger.Log(new LogMessage(LogSeverity.Error, $"Failed to load resource {Name}, path {ResourcePath ?? "null"}"));
            }
            
            End:
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Loaded resource {Name}, path {ResourcePath ?? "null"}"));

            void addResources(string Name, string Path) {
                using var _zipStream = new StreamReader(Path);
                using var _zipArchive = new ZipArchive(_zipStream.BaseStream);
                foreach (var item in _zipArchive.Entries)
                    resources.Add(new Resource(item.Name, Path, new StreamReader(item.Open())));
            }
        }
    }
}