// System
using System;
using System.Runtime.Serialization;

namespace nb.Game.Rendering.Shaders
{
    [Serializable]
    public class ShaderCompilationException : Exception
    {
        public ShaderCompilationException() { }
        public ShaderCompilationException(string message) : base(message) { }
        public ShaderCompilationException(string message, Exception inner) : base(message, inner) { }
        protected ShaderCompilationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}