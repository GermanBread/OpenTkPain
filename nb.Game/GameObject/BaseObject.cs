// System
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;

using nb.Game.Rendering;
using nb.Game.Utility.Scenes;
using nb.Game.Utility.Globals;
using nb.Game.Utility.Resources;
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
            index = Scene.GameObjects.IndexOf(this);
            //vertexPointer = Unsafe.SizeOf<Vector2>() * transform.Vertices.Length /* 3 because our vectors count as a single object */ * index;
            vertexPointer = 0;
            
            Shader = new Shader(ResourceManager.GetResource("default.vert"), ResourceManager.GetResource("default.frag")); /* basic Shader */
            
            vertexHandle = GL.GenVertexArray();
            elementBufferHandle = GL.GenBuffer();
            vertexBufferHandle = GL.GenBuffer();

            // Set a default color value
            if (Color == default)
                Color = Color4.White;
            
            Shader.Use();

            GL.BindVertexArray(vertexHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vertexBufferHandle); // I was missing a VBO here... how did I miss it?
            
            var _data = transform.CompileData(Color);
            GL.BufferData(BufferTarget.ArrayBuffer, _data.Length * Unsafe.SizeOf<Vertex>(), _data, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, transform.Indices.Length * sizeof(uint), transform.Indices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(vertexPointer, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 0); // Like @Reimnop (GitHub) told me: The pointer must be initialized at the end
            GL.EnableVertexAttribArray(vertexPointer);
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
            Shader.Use();

            var _data = transform.CompileData(Color);
            GL.BufferData(BufferTarget.ArrayBuffer, _data.Length * Unsafe.SizeOf<Vertex>(), _data, BufferUsageHint.StaticDraw);
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
                SceneManager.RemoveFromScene(this);
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
        /// <summary>
        /// Default color of the object
        /// </summary>
        public Color4 Color;
        private int vertexHandle;
        private int vertexBufferHandle;
        private int elementBufferHandle;
        private int vertexPointer;
        private int index;
    }
}