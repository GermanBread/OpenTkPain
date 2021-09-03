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
        
        public static void LoadFile(string Name, string FilePath) {
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Loading file {Name}, path {FilePath ?? "null"}"));
            
            FilePath ??= Name;

            if (File.Exists(FilePath)) {
                resources.Add(new Resource(Name, FilePath, new StreamReader(FilePath)));
                goto End;
            }

            var _workdirPath = Path.Combine(Environment.CurrentDirectory, FilePath);
            var _bindirPath = Path.Combine(AppContext.BaseDirectory, FilePath);
            
            if (File.Exists(_workdirPath)) {
                resources.Add(new Resource(Name, _workdirPath, new StreamReader(_bindirPath)));
            } else if (File.Exists(_bindirPath)) {
                resources.Add(new Resource(Name, _bindirPath, new StreamReader(_bindirPath)));
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
                addResources( ResourcePath);
                goto End;
            }

            var _workdirPath = Path.Combine(Environment.CurrentDirectory, ResourcePath);
            var _bindirPath = Path.Combine(AppContext.BaseDirectory, ResourcePath);
            
            if (File.Exists(_workdirPath)) {
                var _file = _workdirPath;
                addResources(_file);
            } else if (File.Exists(_bindirPath)) {
                addResources(_bindirPath);
            } else {
                Logger.Log(new LogMessage(LogSeverity.Error, $"Failed to load resource {Name}, path {ResourcePath ?? "null"}"));
            }
            
            End:
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Loaded resource {Name}, path {ResourcePath ?? "null"}"));

            static void addResources(string Path) {
                using var _zipStream = new StreamReader(Path);
                using var _zipArchive = new ZipArchive(_zipStream.BaseStream);
                foreach (var item in _zipArchive.Entries)
                    resources.Add(new Resource(item.Name, Path, new StreamReader(item.Open())));
            }
        }
    }
}