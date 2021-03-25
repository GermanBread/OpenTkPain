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
            //VertexPointer = Unsafe.SizeOf<Vector2>() * Transform.Vertices.Length /* 3 because our vectors count as a single object */ * index;
            
            // Get the embedded resources using reflection magic, would be great if it worked
            /*var _assembly = Assembly.Load("nb.Resources");
            
            string _vertexShader = _assembly.GetManifestResourceNames().Single(x => x.EndsWith("default.vert"));
            string _fragmentShader = _assembly.GetManifestResourceNames().Single(x => x.EndsWith("default.frag"));*/
            
            Stream _vertexResourceStream = new StreamReader("default.vert").BaseStream; /*_assembly.GetManifestResourceStream(_vertexShader);*/
            Stream _fragmentResourceStream = new StreamReader("default.frag").BaseStream; /*_assembly.GetManifestResourceStream(_fragmentShader);*/
            
            Shader = new Shader(_vertexResourceStream, _fragmentResourceStream); /* basic Shader */
            
            _vertexResourceStream.Dispose();
            _fragmentResourceStream.Dispose();

            VertexHandle = GL.GenVertexArray();
            ElementBufferHandle = GL.GenBuffer();
            VertexBufferHandle = GL.GenBuffer();

            GL.BindVertexArray(VertexHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, VertexBufferHandle); // I was missing a VBO here... how did I miss it?
            
            Shader.Use();
            
            GL.BufferData(BufferTarget.ArrayBuffer, Transform.Vertices.Length /* Vec2[] */ * Unsafe.SizeOf<Vector2>(), Transform.Coordinates, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Transform.Indices.Length * sizeof(uint), Transform.Indices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(VertexPointer, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vector2>(), 0); // Like @Reimnop (GitHub) told me: The pointer must be initialized at the end
            GL.EnableVertexAttribArray(VertexPointer);

            Shader.Use();
        }
        /// <summary>
        /// Get the scene this object is located in
        /// </summary>
        public Scene Scene { get => SceneManager.GetSceneOfObject(this); }
        /// <summary>
        /// Alias to GameGlobals.window
        /// </summary>
        protected GameWindow gameWindow { get => GameGlobals.window; }
        /// <summary>
        /// Draws the object
        /// </summary>
        public void Draw() {
            // Note: Pass the position data to the vertex Shader
            Shader.Use();

            GL.BufferData(BufferTarget.ArrayBuffer, Transform.Vertices.Length /* Vec2[] */ * Unsafe.SizeOf<Vector2>(), Transform.Coordinates, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Transform.Indices.Length * sizeof(uint), Transform.Indices, BufferUsageHint.StaticDraw);

            GL.DrawElements(PrimitiveType.Triangles, Transform.Indices.Length, DrawElementsType.UnsignedInt, 0);
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
                GL.DeleteVertexArray(VertexHandle);
                GL.DeleteBuffer(VertexBufferHandle);
                GL.DeleteBuffer(ElementBufferHandle);
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
        public Transform Transform = new Transform();
        /// <summary>
        /// Alias to Transform.Skew
        /// </summary>
        public Vector2 Skew { get => Transform.Skew; set => Transform.Skew = value; }
        /// <summary>
        /// Alias to Transform.Size
        /// </summary>
        public Vector2 Size { get => Transform.Size; set => Transform.Size = value; }
        /// <summary>
        /// Alias to Transform.Position
        /// </summary>
        public Vector2 Position { get => Transform.Position; set => Transform.Position = value; }
        /// <summary>
        /// Alias to Transform.Rotation
        /// </summary>
        public float Rotation { get => Transform.Rotation; set => Transform.Rotation = value; }
        /// <summary>
        /// Draw order, 0 = first
        /// </summary>
        public uint Layer = 0;
        public Shader Shader;
        public int VertexHandle;
        public int VertexBufferHandle;
        public int ElementBufferHandle;
        public int VertexPointer;
        private int index;
    }
}