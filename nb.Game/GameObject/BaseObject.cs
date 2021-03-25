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
            index = scene.gameObjects.IndexOf(this);
            vertexPointer = sizeof(float) * transform.vertices.Length * 3 /* 3 because our vectors count as a single object */ * index;
            
            // Get the embedded resources using reflection magic, would be great if it worked
            /*var _assembly = Assembly.Load("nb.Resources");
            
            string _vertexShader = _assembly.GetManifestResourceNames().Single(x => x.EndsWith("default.vert"));
            string _fragmentShader = _assembly.GetManifestResourceNames().Single(x => x.EndsWith("default.frag"));*/
            
            Stream _vertexResourceStream = new StreamReader("default.vert").BaseStream; /*_assembly.GetManifestResourceStream(_vertexShader);*/
            Stream _fragmentResourceStream = new StreamReader("default.frag").BaseStream; /*_assembly.GetManifestResourceStream(_fragmentShader);*/
            
            shader = new Shader(_vertexResourceStream, _fragmentResourceStream); /* basic shader */
            
            _vertexResourceStream.Dispose();
            _fragmentResourceStream.Dispose();

            vertexHandle = GL.GenVertexArray();
            ElementBufferHandle = GL.GenBuffer();
            
            GL.BindVertexArray(vertexHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferHandle);
            
            GL.BufferData(BufferTarget.ArrayBuffer, transform.vertices.Length /* Vec2[] */ * Unsafe.SizeOf<Vector2>(), transform.coordinates, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(vertexPointer, 2, VertexAttribPointerType.Float, false, 2, 0);
            if (transform.indices?.Length > 0)
                GL.BufferData(BufferTarget.ElementArrayBuffer, transform.indices.Length * sizeof(uint), transform.indices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(vertexPointer);
        }
        /// <summary>
        /// Get the scene this object is located in
        /// </summary>
        public Scene scene { get => SceneManager.GetSceneOfObject(this); }
        /// <summary>
        /// Alias to GameGlobals.window
        /// </summary>
        protected GameWindow gameWindow { get => GameGlobals.window; }
        /// <summary>
        /// Generic method to test if the object exists
        /// </summary>
        public void SayHello() {
            Logger.Log(new LogMessage(LogSeverity.Verbose, "BaseObject", "Hello World!"));
        }
        /// <summary>
        /// Draws the object
        /// </summary>
        public void Draw() {
            // Note: Pass the position data to the vertex shader

            shader.Use();

            GL.BindVertexArray(vertexHandle);
            if (transform.indices?.Length > 0)
                GL.DrawElements(PrimitiveType.Triangles, transform.indices.Length, DrawElementsType.UnsignedInt, 0);
            else
                GL.DrawArrays(PrimitiveType.Triangles, 0, transform.vertices.Length);
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
        public Vector2 skew { get => transform.skew; set => transform.skew = value; }
        /// <summary>
        /// Alias to transform.size
        /// </summary>
        public Vector2 size { get => transform.size; set => transform.size = value; }
        /// <summary>
        /// Alias to transform.position
        /// </summary>
        public Vector2 position { get => transform.position; set => transform.position = value; }
        /// <summary>
        /// Alias to transform.rotation
        /// </summary>
        public float rotation { get => transform.rotation; set => transform.rotation = value; }
        /// <summary>
        /// Draw order, 0 = first
        /// </summary>
        public uint layer = 0;
        public Shader shader;
        public int vertexHandle;
        public int ElementBufferHandle;
        public int vertexPointer;
        private int index;
    }
}