// System
using IO = System.IO;

namespace nb.Game.Utility.Resources
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
    }
}