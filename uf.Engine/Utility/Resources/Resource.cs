using System.Globalization;
// System
using IO = System.IO;

namespace uf.Utility.Resources
{
    public class Resource
    {
        public Resource(string Name, string Path, IO.StreamReader Stream) {
            this.Name = Name;
            this.Path = Path;
            this.Stream = Stream;
        }
        public string Name;
        public string Path;
        public IO.StreamReader Stream;
        /// <summary>
        /// A resource with no data, pass this instead of null
        /// </summary>
        public static Resource Empty { get => uniqueEmtpyInstance; }
        // Only gets instantiated once, allows for comparison.
        private static Resource uniqueEmtpyInstance = new(null, null, null);
    }
}