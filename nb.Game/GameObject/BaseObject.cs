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
using OpenTK.Windowing.GraphicsLibraryFramework;

using nb.Game.Rendering;
using nb.Game.Utility.Input;
using nb.Game.Utility.Scenes;
using nb.Game.Utility.Globals;
using nb.Game.Utility.Logging;
using nb.Game.Utility.Resources;
using nb.Game.Rendering.Shaders;
using nb.Game.Rendering.Textures;
using nb.Game.GameObject.Components;

namespace nb.Game.GameObject
{
    public class BaseObject : IDisposable
    {
        public BaseObject(string scene)
        {
            Scene = SceneManager.AddToScene(this, scene ?? "default");
            // Automatically initialise
            if (Scene.IsLoaded)
                Init();
        }
        public void Init()
        {
            if (isInitialized)
            {
                Logger.Log(new LogMessage(LogSeverity.Warning, "Init() was called even though this object was already initialized!"));
                return;
            }

            if (Layer == int.MinValue)
                Layer = Scene.GameObjects.IndexOf(this);
            if (Shader == null)
                Shader = Shader.BaseShader;

            vertexHandle = GL.GenVertexArray();
            elementBufferHandle = GL.GenBuffer();
            vertexBufferHandle = GL.GenBuffer();

            // Set a default color value
            if (Color == default)
                Color = Color4.White;

            // No texture? Create a blank one!
            if (Texture == null)
                Texture = new Texture(Resource.Empty);

            GL.BindVertexArray(vertexHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vertexBufferHandle); // I was missing a VBO here... how did I miss it?

            /*var _data = transform.CompileData(Color);
            GL.BufferData(BufferTarget.ArrayBuffer, _data.Length * Unsafe.SizeOf<Vertex>(), _data, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, transform.Indices.Length * sizeof(uint), transform.Indices, BufferUsageHint.StaticDraw);*/

            // Like @Reimnop (GitHub) told me: The pointer must be initialized at the end
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 2 * sizeof(float));
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 4 * sizeof(float));

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            // Dummy event handler that prevents a nullref
            Clicked += () => { };

            isInitialized = true;
        }
        /// <summary>
        /// Draws the object
        /// </summary>
        public void Draw()
        {
            if (!isInitialized)
            {
                Logger.Log(new LogMessage(LogSeverity.Error, "Not initialized; refusing to render object! Was Init() not called?"));
                return;
            }

            // Small optimization: Don't perform a draw call if the object is 100% transparent
            if (Color.A == 0)
                return;

            if (Color.A < 1) {
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.BlendEquation(BlendEquationMode.FuncAdd);
                GL.Enable(EnableCap.Blend);
            }

            Texture.Use();
            Shader.Use();

            var _data = transform.CompileData(Color, Scene);

            for (int i = 0; i < _data.Length; i++)
            {
                var _uv = Texture.GetUV();
                _data[i].UV *= _uv.Item2 - _uv.Item1;
                _data[i].UV += _uv.Item1;
            }

            // Let me just hijack the Draw() method to fire an event
            // This is pretty much the same if I were to create a Update() method
            if (IsHoverable && InputManager.HoveredObject == this)
                IsHovered = true;
            else
                IsHovered = false;

            if (IsHovered && (!EngineGlobals.Window.MouseState.WasButtonDown(MouseButton.Left) && EngineGlobals.Window.MouseState.IsButtonDown(MouseButton.Left)))
                Clicked.Invoke();

            GL.BufferData(BufferTarget.ArrayBuffer, _data.Length * Unsafe.SizeOf<Vertex>(), _data, BufferUsageHint.DynamicDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, transform.Indices.Length * sizeof(uint), transform.Indices, BufferUsageHint.DynamicDraw);

            GL.DrawElements(PrimitiveType.Triangles, transform.Indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.Disable(EnableCap.Blend);
        }
        public void MultipassDraw(Color4 Override)
        {
            // Small optimization: Don't perform a draw call if the object is 100% transparent
            if (Color.A == 0)
                return;

            var _data = transform.CompileData(Override, Scene);

            GL.BufferData(BufferTarget.ArrayBuffer, _data.Length * Unsafe.SizeOf<Vertex>(), _data, BufferUsageHint.DynamicDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, transform.Indices.Length * sizeof(uint), transform.Indices, BufferUsageHint.DynamicDraw);

            GL.DrawElements(PrimitiveType.Triangles, transform.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
        /// <summary>
        /// Frees any resources used by this Object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                GL.DeleteVertexArray(vertexHandle);
                GL.DeleteBuffer(vertexBufferHandle);
                GL.DeleteBuffer(elementBufferHandle);
                SceneManager.RemoveFromScene(this);
                disposed = true;
            }
        }
        /// <summary>
        /// Get the scene this object is located in
        /// </summary>
        public Scene Scene;
        /// <summary>
        /// Alias to EngineGlobals.Window
        /// </summary>
        protected GameWindow gameWindow { get => EngineGlobals.Window; }
        /// <summary>
        /// Contains all children of this object
        /// </summary>
        public List<BaseObject> Children = new List<BaseObject>();
        /// <summary>
        /// The current parent of the object
        /// </summary>
        public BaseObject Parent;
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
        /// Draw order, smaller = drawn earlier
        /// </summary>
        public int Layer = int.MinValue;
        /// <summary>
        /// The shader currently in use
        /// </summary>
        public Shader Shader;
        /// <summary>
        /// Default color of the object
        /// </summary>
        public Color4 Color;
        /// <summary>
        /// The texture currently in use
        /// </summary>
        public Texture Texture;
        /// <summary>
        /// If a cursor is hovering above this object
        /// </summary>
        public bool IsHovered { get; private set; }
        public bool IsHoverable { get; set; } = false;
        public delegate void OnClicked();
        /// <summary>
        /// Fired when this object is clicked
        /// </summary>
        public event OnClicked Clicked;
        public bool IsInitialized { get => isInitialized; }
        private bool isInitialized = false;
        private int vertexHandle;
        private int vertexBufferHandle;
        private int elementBufferHandle;
    }
}