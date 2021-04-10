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
            string _fragmentCompilationResult = GL.GetShaderInfoLog(fragmentShaderHandle);

            // If there are any, log them
            if (!string.IsNullOrEmpty(_vertexCompilationResult))
                Logger.Log(new LogMessage(LogSeverity.Error, "Failed to compile vertex shader", new ShaderCompilationException(_vertexCompilationResult)));
            if (!string.IsNullOrEmpty(_fragmentCompilationResult))
                Logger.Log(new LogMessage(LogSeverity.Error, "Failed to compile fragment shader", new ShaderCompilationException(_fragmentCompilationResult)));

            // Now we're making the shader usable
            ShaderHandle = GL.CreateProgram();

            // Attach a label to our shader
            string _label = $"Shader {ShaderHandle} (vert: {VertexShader.Name}, frag: {FragmentShader.Name})";
            GL.ObjectLabel(ObjectLabelIdentifier.Program, ShaderHandle, _label.Length, _label);

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
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Using shader {ShaderHandle} (vert: {vertexShaderHandle}, frag: {fragmentShaderHandle})"));
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

        private static Shader baseShader;
        /// <summary>
        /// Returns a basic shader that can be used
        /// </summary>
        public static Shader BaseShader { get {
            // TODO: Apply the same principle from Texture.cs here...
            if (baseShader == null)
                baseShader = new(ResourceManager.GetResource("default vertex shader"), ResourceManager.GetResource("default fragment shader"));
            return baseShader;
        } }
    }
}