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
        public static Resource GetFile(string name) {
            var _firstResult = resources.FirstOrDefault(x => x.Name == name);
            if (_firstResult == default(Resource))
                // Resource not present in List? Load it!
                LoadResource(name, null);
            var _output = resources.FirstOrDefault(x => x.Name == name);
            return _output;
        }
        
        public static void LoadFile(string name, string filePath) {
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Loading file {name}, path {filePath ?? "null"}"));
            
            filePath ??= name;

            if (File.Exists(filePath)) {
                resources.Add(new Resource(name, filePath, new StreamReader(filePath)));
                goto End;
            }

            if (filePath != null)
            {
                var _workdirPath = Path.Combine(Environment.CurrentDirectory, filePath);
                var _bindirPath = Path.Combine(AppContext.BaseDirectory, filePath);
            
                if (File.Exists(_workdirPath)) {
                    resources.Add(new Resource(name, _workdirPath, new StreamReader(_bindirPath)));
                } else if (File.Exists(_bindirPath)) {
                    resources.Add(new Resource(name, _bindirPath, new StreamReader(_bindirPath)));
                } else {
                    Logger.Log(new LogMessage(LogSeverity.Error, $"Failed to load file {name}, path {filePath}"));
                }
            }

            End:
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Loaded file {name}, path {filePath ?? "null"}"));
        }

        public static void LoadResource(string name, string resourcePath) {
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Loading resource {name}, path {resourcePath ?? "null"}"));

            if (!(resourcePath ?? name).EndsWith(".ufr")) {
                Logger.Log(new LogMessage(LogSeverity.Warning,
                    $"Falling back to file loading for file {name}, path {resourcePath ?? "null"}. This file is not a resource file (.ufr)"));
                LoadFile(name, resourcePath);
                return;
            }

            resourcePath ??= name;

            if (File.Exists(resourcePath)) {
                AddResources( resourcePath);
                goto End;
            }

            var _workdirPath = Path.Combine(Environment.CurrentDirectory, resourcePath ?? string.Empty);
            var _bindirPath = Path.Combine(AppContext.BaseDirectory, resourcePath ?? string.Empty);
            
            if (File.Exists(_workdirPath)) {
                AddResources(_workdirPath);
            } else if (File.Exists(_bindirPath)) {
                AddResources(_bindirPath);
            } else {
                Logger.Log(new LogMessage(LogSeverity.Error, $"Failed to load resource {name}, path {resourcePath}"));
            }
            
            End:
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Loaded resource {name}, path {resourcePath}"));

            static void AddResources(string path) {
                using var _zipStream = new StreamReader(path);
                using var _zipArchive = new ZipArchive(_zipStream.BaseStream);
                foreach (var item in _zipArchive.Entries)
                    resources.Add(new Resource(item.Name, path, new StreamReader(item.Open())));
            }
        }
    }
}