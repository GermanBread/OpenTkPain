// System
using System;
using System.Collections.Generic;

// OpenTK
using OpenTK.Graphics.OpenGL;

using uf.Utility.Logging;
using uf.Utility.Resources;

namespace uf.Rendering.Shaders
{
    public sealed class Shader : IDisposable
    {
        private readonly int shaderHandle;
        private static readonly Dictionary<(Resource, Resource), Shader> shaders = new();

        private Shader(Resource vertexShader, Resource fragmentShader) {
            if (vertexShader == null || fragmentShader == null) {
                shaderHandle = BaseShader.shaderHandle;
                return;
            }

            // Don't waste storage space by creating the same shader over and over
            if (shaders.ContainsKey((vertexShader, fragmentShader)) && !shaders[(vertexShader, fragmentShader)].disposed) {
                shaderHandle = shaders[(vertexShader, fragmentShader)].shaderHandle;
                return;
            }

            var _vertexSourceCode = vertexShader.Stream.ReadToEnd();
            var _fragmentSourceCode = fragmentShader.Stream.ReadToEnd();

            var _vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            var _fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(_vertexShaderHandle, _vertexSourceCode);
            GL.ShaderSource(_fragmentShaderHandle, _fragmentSourceCode);

            GL.CompileShader(_vertexShaderHandle);
            GL.CompileShader(_fragmentShaderHandle);

            var _vertexCompilationResult = GL.GetShaderInfoLog(_vertexShaderHandle);
            var _fragmentCompilationResult = GL.GetShaderInfoLog(_fragmentShaderHandle);

            if (!string.IsNullOrEmpty(_vertexCompilationResult))
                Logger.Log(new LogMessage(LogSeverity.Error, "Failed to compile vertex shader", new ShaderCompilationException(_vertexCompilationResult)));
            if (!string.IsNullOrEmpty(_fragmentCompilationResult))
                Logger.Log(new LogMessage(LogSeverity.Error, "Failed to compile fragment shader", new ShaderCompilationException(_fragmentCompilationResult)));

            // Now we're making the shader usable
            shaderHandle = GL.CreateProgram();

            // Attach a label to our shader
            var _label = $"Shader {shaderHandle} (vert: {vertexShader.Name}, frag: {fragmentShader.Name})";
            GL.ObjectLabel(ObjectLabelIdentifier.Program, shaderHandle, _label.Length, _label);

            GL.AttachShader(shaderHandle, _vertexShaderHandle);
            GL.AttachShader(shaderHandle, _fragmentShaderHandle);
            GL.LinkProgram(shaderHandle);

            // Do a little cleanup
            GL.DetachShader(shaderHandle, _vertexShaderHandle);
            GL.DetachShader(shaderHandle, _fragmentShaderHandle);
            GL.DeleteShader(_vertexShaderHandle);
            GL.DeleteShader(_fragmentShaderHandle);
        }

        public void Use() {
            //Logger.Log(new LogMessage(LogSeverity.Debug, $"Using shader {ShaderHandle} (vert: {vertexShaderHandle}, frag: {fragmentShaderHandle})"));
            GL.UseProgram(shaderHandle);
        }

        public void SetInt(string name, int value)
        {
            int _location = GL.GetUniformLocation(shaderHandle, name);

            GL.Uniform1(_location, value);
        }

        private bool disposed;

        private void Dispose(bool disposing)
        {
            if (disposed) return;
            GL.DeleteProgram(shaderHandle);

            disposed = true;
        }
        public void Dispose() {
            if (this == BaseShader || this == MultipassShader) {
                Logger.Log(new LogMessage(LogSeverity.Error, "Something attemped to dispose the base shaders!"));
            }
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static Shader _baseShader;
        /// <summary>
        /// Returns a basic shader that can be used
        /// </summary>
        public static Shader BaseShader =>
            _baseShader ??= new Shader(ResourceManager.GetFile("default vertex shader"),
                ResourceManager.GetFile("default fragment shader"));

        private static Shader _multipassShader;
        /// <summary>
        /// Returns a basic shader that can be used for multipass rendering
        /// </summary>
        public static Shader MultipassShader =>
            _multipassShader ??= new Shader(ResourceManager.GetFile("multipass vertex shader"),
                ResourceManager.GetFile("multipass fragment shader"));
    }
}