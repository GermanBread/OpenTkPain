using System.IO;
// System

namespace uf.Utility.Resources
{
    public class Resource
    {
        public Resource(string name, string path, StreamReader stream) {
            Name = name;
            Path = path;
            Stream = stream;
        }
        public readonly string Name;
        public readonly string Path;
        public readonly StreamReader Stream;
        /// <summary>
        /// A resource with no data, pass this instead of null
        /// </summary>
        public static Resource Empty { get; } = new(null, null, null);

        // Only gets instantiated once, allows for comparison.
    }
}