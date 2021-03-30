// System
using System;
using System.IO;
using System.Linq;
using System.Resources;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;

using nb.Game.Utility.Scenes;
using nb.Game.Utility.Logging;
using nb.Game.Utility.Globals;
using nb.Game.Rendering.Shaders;
using nb.Game.GameObject.Components;

namespace nb.Game.GameObject
{
    public class BaseObject : IDisposable
    {
        public BaseObject(string scene) {
            SceneManager.AddToScene(this, scene ?? "default");
        }
        public void Init() {
            // Initialize the pointer
            index = Scene.gameObjects.IndexOf(this);
            //vertexPointer = Unsafe.SizeOf<Vector2>() * transform.Vertices.Length /* 3 because our vectors count as a single object */ * index;
            vertexPointer = 0;
            
            // FIXME Get the embedded resources using reflection magic, would be great if it worked
            /*var _assembly = Assembly.Load("nb.Resources");
            
            string _vertexShader = _assembly.GetManifestResourceNames().Single(x => x.EndsWith("default.vert"));
            string _fragmentShader = _assembly.GetManifestResourceNames().Single(x => x.EndsWith("default.frag"));*/
            
            Stream _vertexResourceStream = new StreamReader("default.vert").BaseStream; /*_assembly.GetManifestResourceStream(_vertexShader);*/
            Stream _fragmentResourceStream = new StreamReader("default.frag").BaseStream; /*_assembly.GetManifestResourceStream(_fragmentShader);*/
            
            Shader = new Shader(_vertexResourceStream, _fragmentResourceStream); /* basic Shader */
            
            _vertexResourceStream.Dispose();
            _fragmentResourceStream.Dispose();

            vertexHandle = GL.GenVertexArray();
            elementBufferHandle = GL.GenBuffer();
            vertexBufferHandle = GL.GenBuffer();

            GL.BindVertexArray(vertexHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vertexBufferHandle); // I was missing a VBO here... how did I miss it?
            
            Shader.Use();
            
            GL.BufferData(BufferTarget.ArrayBuffer, transform.Vertices.Length /* Vec2[] */ * Unsafe.SizeOf<Vector2>(), transform.Coordinates, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, transform.Indices.Length * sizeof(uint), transform.Indices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(vertexPointer, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vector2>(), 0); // Like @Reimnop (GitHub) told me: The pointer must be initialized at the end
            GL.EnableVertexAttribArray(vertexPointer);

            Shader.Use();
        }
        /// <summary>
        /// Get the scene this object is located in
        /// </summary>
        public Scene Scene { get => SceneManager.GetSceneOfObject(this); }
        /// <summary>
        /// Alias to EngineGlobals.Window
        /// </summary>
        protected GameWindow gameWindow { get => EngineGlobals.Window; }
        /// <summary>
        /// Draws the object
        /// </summary>
        public void Draw() {
            // Note: Pass the position data to the vertex Shader
            Shader.Use();

            GL.BufferData(BufferTarget.ArrayBuffer, transform.Vertices.Length /* Vec2[] */ * Unsafe.SizeOf<Vector2>(), transform.Coordinates, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, transform.Indices.Length * sizeof(uint), transform.Indices, BufferUsageHint.StaticDraw);

            GL.DrawElements(PrimitiveType.Triangles, transform.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
        /// <summary>
        /// Frees any resources used by this Object
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(true);
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                Shader.Dispose();
                GL.DeleteVertexArray(vertexHandle);
                GL.DeleteBuffer(vertexBufferHandle);
                GL.DeleteBuffer(elementBufferHandle);
                disposed = true;
            }
        }
        /// <summary>
        /// Contains all children of this object
        /// </summary>
        public List<BaseObject> Children = new List<BaseObject>();
        /// <summary>
        /// Contains all information related to size, position and rotation
        /// </summary>
        protected Transform transform = new Transform();
        /// <summary>
        /// Alias to transform.Skew
        /// </summary>
        public Vector2 Skew { get => transform.Skew; set => transform.Skew = value; }
        /// <summary>
        /// Alias to transform.Size
        /// </summary>
        public Vector2 Size { get => transform.Size; set => transform.Size = value; }
        /// <summary>
        /// Alias to transform.Position
        /// </summary>
        public Vector2 Position { get => transform.Position; set => transform.Position = value; }
        /// <summary>
        /// Alias to transform.Rotation
        /// </summary>
        public float Rotation { get => transform.Rotation; set => transform.Rotation = value; }
        /// <summary>
        /// Alias to transform.Anchor
        /// </summary>
        public Anchor Anchor { get => transform.Anchor; set => transform.Anchor = value; }
        /// <summary>
        /// Draw order, 0 = first
        /// </summary>
        public uint Layer = 0;
        /// <summary>
        /// The shader currently in use
        /// </summary>
        public Shader Shader;
        private int vertexHandle;
        private int vertexBufferHandle;
        private int elementBufferHandle;
        private int vertexPointer;
        private int index;
    }
}