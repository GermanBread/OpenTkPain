// System
using System;
using System.IO;
using System.Collections.Generic;

// OpenTK
using OpenTK.Graphics.OpenGL;

using uf.Utility.Logging;
using uf.Utility.Resources;

namespace uf.Rendering.Shaders
{
    public class Shader : IDisposable
    {
        public readonly int ShaderHandle;
        private readonly int vertexShaderHandle;
        private readonly int fragmentShaderHandle;
        private readonly string vertexSourceCode;
        private readonly string fragmentSourceCode;
        private readonly Dictionary<(Resource, Resource), Shader> shaders = new();
        public Shader(Resource VertexShader, Resource FragmentShader) {
            if (VertexShader == null || FragmentShader == null) {
                ShaderHandle = BaseShader.ShaderHandle;
                return;
            }

            // Don't waste storage space by creating the same shader over and over
            if (shaders.ContainsKey((VertexShader, FragmentShader)) && !shaders[(VertexShader, FragmentShader)].disposed) {
                ShaderHandle = shaders[(VertexShader, FragmentShader)].ShaderHandle;
                return;
            }

            vertexSourceCode = VertexShader.Stream.ReadToEnd();
            fragmentSourceCode = FragmentShader.Stream.ReadToEnd();

            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShaderHandle, vertexSourceCode);
            GL.ShaderSource(fragmentShaderHandle, fragmentSourceCode);

            GL.CompileShader(vertexShaderHandle);
            GL.CompileShader(fragmentShaderHandle);

            string _vertexCompilationResult = GL.GetShaderInfoLog(vertexShaderHandle);
            string _fragmentCompilationResult = GL.GetShaderInfoLog(fragmentShaderHandle);

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
            //Logger.Log(new LogMessage(LogSeverity.Debug, $"Using shader {ShaderHandle} (vert: {vertexShaderHandle}, frag: {fragmentShaderHandle})"));
            GL.UseProgram(ShaderHandle);
        }

        public void SetInt(string Name, int Value)
        {
            int _location = GL.GetUniformLocation(ShaderHandle, Name);

            GL.Uniform1(_location, Value);
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                GL.DeleteProgram(ShaderHandle);

                disposed = true;
            }
        }
        public void Dispose() {
            if (this == BaseShader || this == MultipassShader) {
                Logger.Log(new LogMessage(LogSeverity.Error, "Something attemped to dispose the base shaders!"));
            }
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static Shader baseShader;
        /// <summary>
        /// Returns a basic shader that can be used
        /// </summary>
        public static Shader BaseShader { get {
            if (baseShader == null)
                baseShader = new(ResourceManager.GetFile("default vertex shader"), ResourceManager.GetFile("default fragment shader"));
            return baseShader;
        } }
        private static Shader multipassShader;
        /// <summary>
        /// Returns a basic shader that can be used for multipass rendering
        /// </summary>
        public static Shader MultipassShader { get {
            if (multipassShader == null)
                multipassShader = new(ResourceManager.GetFile("multipass vertex shader"), ResourceManager.GetFile("multipass fragment shader"));
            return multipassShader;
        } }
    }
}