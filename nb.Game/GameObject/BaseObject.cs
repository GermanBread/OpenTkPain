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
    public class BaseObject
    {
        public BaseObject(string scene) {
            SceneManager.AddToScene(this, scene ?? "default");
        }
        public void Init() {
            // Initialize the pointer
            index = Scene.gameObjects.IndexOf(this);
            //VertexPointer = sizeof(float) * transform.Vertices.Length * 3 /* 3 because our vectors count as a single object */ * index;
            
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
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, VertexBufferHandle);
            
            GL.BufferData(BufferTarget.ArrayBuffer, transform.Vertices.Length /* Vec2[] */ * Unsafe.SizeOf<Vector2>(), transform.Coordinates, BufferUsageHint.StaticDraw);
            if (transform.Indices?.Length > 0)
                GL.BufferData(BufferTarget.ElementArrayBuffer, transform.Indices.Length * sizeof(uint), transform.Indices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(VertexPointer, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vector2>(), 0);
            GL.EnableVertexAttribArray(VertexPointer);
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

            GL.BindVertexArray(VertexHandle);
            if (transform.Indices?.Length > 0)
                GL.DrawElements(PrimitiveType.Triangles, transform.Indices.Length, DrawElementsType.UnsignedInt, 0);
            else
                GL.DrawArrays(PrimitiveType.Triangles, 0, transform.Vertices.Length);
        }
        /// <summary>
        /// Contains all children of this object
        /// </summary>
        public List<BaseObject> children = new List<BaseObject>();
        /// <summary>
        /// Contains all information related to size, position and rotation
        /// </summary>
        public Transform transform = new Transform();
        /// <summary>
        /// Alias to transform.skew
        /// </summary>
        public Vector2 skew { get => transform.Skew; set => transform.Skew = value; }
        /// <summary>
        /// Alias to transform.size
        /// </summary>
        public Vector2 size { get => transform.Size; set => transform.Size = value; }
        /// <summary>
        /// Alias to transform.position
        /// </summary>
        public Vector2 position { get => transform.Position; set => transform.Position = value; }
        /// <summary>
        /// Alias to transform.rotation
        /// </summary>
        public float rotation { get => transform.Rotation; set => transform.Rotation = value; }
        /// <summary>
        /// Draw order, 0 = first
        /// </summary>
        public uint layer = 0;
        public Shader Shader;
        public int VertexHandle;
        public int VertexBufferHandle;
        public int ElementBufferHandle;
        public int VertexPointer;
        private int index;
    }
}