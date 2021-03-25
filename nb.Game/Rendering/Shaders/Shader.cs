// System
using System;
using System.IO;

// OpenTK
using OpenTK.Graphics.OpenGL;

using nb.Game.Utility.Logging;

namespace nb.Game.Rendering.Shaders
{
    public class Shader : IDisposable
    {
        public int ShaderHandle;
        private int _vertexShaderHandle;
        private int _fragmentShaderHandle;
        private string _vertexSourceCode;
        private string _fragmentSourceCode;
        public Shader(Stream vertex, Stream fragment) {
            // Read the shader code
            using (StreamReader vertexStreamReader = new StreamReader(vertex))
            {
                _vertexSourceCode = vertexStreamReader.ReadToEnd();
            }
            using (StreamReader fragmentStreamReader = new StreamReader(fragment))
            {
                _fragmentSourceCode = fragmentStreamReader.ReadToEnd();
            }

            // Create our handles
            _vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            _fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            // Bind them to the source code
            GL.ShaderSource(_vertexShaderHandle, _vertexSourceCode);
            GL.ShaderSource(_fragmentShaderHandle, _fragmentSourceCode);

            // And now compile them
            GL.CompileShader(_vertexShaderHandle);
            GL.CompileShader(_fragmentShaderHandle);

            // Get error messages
            string _vertexCompilationResult = GL.GetShaderInfoLog(_vertexShaderHandle);
            string _fragmentCompilationREsult = GL.GetShaderInfoLog(_fragmentShaderHandle);

            // If there are any, log them
            if (!string.IsNullOrEmpty(_vertexCompilationResult))
                Logger.Log(new LogMessage(LogSeverity.Error, "Shader", "Failed to compile vertex shader", new ShaderCompilationException(_vertexCompilationResult)));
            if (!string.IsNullOrEmpty(_fragmentCompilationREsult))
                Logger.Log(new LogMessage(LogSeverity.Error, "Shader", "Failed to compile fragment shader", new ShaderCompilationException(_fragmentCompilationREsult)));

            // Now we're making the shader usable
            ShaderHandle = GL.CreateProgram();
            GL.AttachShader(ShaderHandle, _vertexShaderHandle);
            GL.AttachShader(ShaderHandle, _fragmentShaderHandle);
            GL.LinkProgram(ShaderHandle);

            // Do a little cleanup
            GL.DetachShader(ShaderHandle, _vertexShaderHandle);
            GL.DetachShader(ShaderHandle, _fragmentShaderHandle);
            GL.DeleteShader(_vertexShaderHandle);
            GL.DeleteShader(_fragmentShaderHandle);
        }

        public void Use() {
            GL.UseProgram(ShaderHandle);
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                GL.DeleteProgram(ShaderHandle);

                _disposed = true;
            }
        }

        ~Shader()
        {
            GL.DeleteProgram(ShaderHandle);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}