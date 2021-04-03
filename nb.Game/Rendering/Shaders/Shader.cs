// System
using System;
using System.IO;

// OpenTK
using OpenTK.Graphics.OpenGL;

using nb.Game.Utility.Logging;
using nb.Game.Utility.Resources;

namespace nb.Game.Rendering.Shaders
{
    public class Shader : IDisposable
    {
        public int ShaderHandle;
        private int vertexShaderHandle;
        private int fragmentShaderHandle;
        private string vertexSourceCode;
        private string fragmentSourceCode;
        public Shader(Resource VertexShader, Resource FragmentShader) {
            // Read the shader code
            vertexSourceCode = VertexShader.Stream.ReadToEnd();
            fragmentSourceCode = FragmentShader.Stream.ReadToEnd();

            // Create our handles
            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            // Bind them to the source code
            GL.ShaderSource(vertexShaderHandle, vertexSourceCode);
            GL.ShaderSource(fragmentShaderHandle, fragmentSourceCode);

            // And now compile them
            GL.CompileShader(vertexShaderHandle);
            GL.CompileShader(fragmentShaderHandle);

            // Get error messages
            string _vertexCompilationResult = GL.GetShaderInfoLog(vertexShaderHandle);
            string _fragmentCompilationREsult = GL.GetShaderInfoLog(fragmentShaderHandle);

            // If there are any, log them
            if (!string.IsNullOrEmpty(_vertexCompilationResult))
                Logger.Log(new LogMessage(LogSeverity.Error, "Shader", "Failed to compile vertex shader", new ShaderCompilationException(_vertexCompilationResult)));
            if (!string.IsNullOrEmpty(_fragmentCompilationREsult))
                Logger.Log(new LogMessage(LogSeverity.Error, "Shader", "Failed to compile fragment shader", new ShaderCompilationException(_fragmentCompilationREsult)));

            // Now we're making the shader usable
            ShaderHandle = GL.CreateProgram();
            GL.AttachShader(ShaderHandle, vertexShaderHandle);
            GL.AttachShader(ShaderHandle, fragmentShaderHandle);
            GL.LinkProgram(ShaderHandle);

            // Do a little cleanup
            GL.DetachShader(ShaderHandle, vertexShaderHandle);
            GL.DetachShader(ShaderHandle, fragmentShaderHandle);
            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(fragmentShaderHandle);
        }

        public void Use() {
            GL.UseProgram(ShaderHandle);
        }

        // Disposing
        private bool disposed = false;
        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                GL.DeleteProgram(ShaderHandle);

                disposed = true;
            }
        }
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}