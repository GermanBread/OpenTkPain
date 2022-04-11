// System
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using uf.Rendering;
using uf.Utility.Input;
using uf.Utility.Scenes;
using uf.Utility.Globals;
using uf.Utility.Logging;
using uf.Utility.Resources;
using uf.Rendering.Shaders;
using uf.Rendering.Textures;
using uf.Rendering.Animations;
using uf.GameObject.Components;

namespace uf.GameObject
{
    public abstract class BaseObject
    {
        public BaseObject(string scene)
        {
            Scene = SceneManager.AddToScene(this, scene ?? "default");
            // Automatically initialise
            if (Scene.IsLoaded) {
                Init();
                gameWindow?.InvalidateObjectsCache();
            }
        }
        internal virtual void Init()
        {
            if (isInitialized) {
                Logger.Log(new LogMessage(LogSeverity.Warning, "Init() was called even though this object was already initialized!"));
                return;
            }

            initValues();

            createObjects();
            initObjects();

            isInitialized = true;
        }
        internal protected virtual void initValues() {
            if (Shader == null)
                Shader = Shader.BaseShader;

            // Set a default color value
            if (Color == default)
                Color = Color4.White;

            // No texture? Create a blank one!
            if (Texture == null)
                Texture = Texture.Empty;

            gameWindow.MouseDown += (e) => {
                if (IsHovered)
                    Clicked.Invoke(new ClickedEventArgs(e.Button));
            };
            
            // Dummy event handler that prevents a nullref
            Clicked += (_) => { };

            Children.CollectionChanged += (_, e) => {
                if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                    foreach (var child in Children)
                        child.Parent = this;
                else
                    foreach (var formerChild in e.OldItems)
                        if (formerChild is BaseObject a)
                            a.Parent = null;
            };
        }
        internal protected virtual void createObjects() {
            vertexHandle = GL.GenVertexArray();
            elementBufferHandle = GL.GenBuffer();
            vertexBufferHandle = GL.GenBuffer();
        }
        internal protected virtual void initObjects() {
            GL.BindVertexArray(vertexHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vertexBufferHandle);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 2 * sizeof(float));
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 4 * sizeof(float));
            GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), 6 * sizeof(float));

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);
        }
        /// <summary>
        /// Update the object's state
        /// </summary>
        internal virtual void Update() {
            if (IsHoverable && InputManager.HoveredObject == this)
                IsHovered = true;
            else
                IsHovered = false;
        }
        internal protected virtual void preDraw() {
            // Small optimization: Don't perform a draw call if the object is 100% transparent
            if (Color.A == 0)
                return;

            //if (Color.A < 1) {
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.BlendEquation(BlendEquationMode.FuncAdd);
                GL.Enable(EnableCap.Blend);
            //}

            Shader.Use();
        }
        internal protected virtual void postDraw() {
            GL.Disable(EnableCap.Blend);
        }
        /// <summary>
        /// Draws the object. Should be called once per frame.
        /// </summary>
        internal virtual void Draw()
        {
            if (!IsInitialized) {
                Logger.Log(new LogMessage(LogSeverity.Error, "Not initialized; refusing to render object! Was Init() not called?"));
                return;
            }

            preDraw();

            var _data = transform.CompileData(Color, Scene);

            // UV Vectors need to be scaled down
            for (int i = 0; i < _data.Length; i++) {
                var _uv = Texture.GetUV();
                _data[i].UV *= _uv.Item2 - _uv.Item1;
                _data[i].UV += _uv.Item1;
            }

            GL.BufferData(BufferTarget.ArrayBuffer, _data.Length * Unsafe.SizeOf<Vertex>(), _data, BufferUsageHint.DynamicDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, transform.Indices.Length * sizeof(uint), transform.Indices, BufferUsageHint.DynamicDraw);

            GL.DrawElements(PrimitiveType.Triangles, transform.Indices.Length, DrawElementsType.UnsignedInt, 0);

            postDraw();
        }
        internal virtual void MultipassDraw(Color4 Override)
        {
            if (!IsInitialized) {
                Logger.Log(new LogMessage(LogSeverity.Error, "Not initialized; refusing to render object! Was Init() not called?"));
                return;
            }
            
            var _data = transform.CompileData(Override, Scene);

            GL.BufferData(BufferTarget.ArrayBuffer, _data.Length * Unsafe.SizeOf<Vertex>(), _data, BufferUsageHint.DynamicDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, transform.Indices.Length * sizeof(uint), transform.Indices, BufferUsageHint.DynamicDraw);

            GL.DrawElements(PrimitiveType.Triangles, transform.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
        /// <summary>
        /// Frees any resources used by this Object and preps it for reinitialization
        /// </summary>
        public virtual void Dispose()
        {
            // Jokes on you, we're not actually disposing this object
            GL.DeleteVertexArray(vertexHandle);
            GL.DeleteBuffer(vertexBufferHandle);
            GL.DeleteBuffer(elementBufferHandle);
            isInitialized = false;
        }
        /// <summary>
        /// Get the scene this object is located in
        /// </summary>
        public Scene Scene { get => scene; set {
            gameWindow?.InvalidateObjectsCache();
            scene = value;
        } }
        private Scene scene;
        /// <summary>
        /// Alias to EngineGlobals.Window
        /// </summary>
        protected static BaseGame gameWindow { get => EngineGlobals.Window; }
        /// <summary>
        /// Contains all information related to size, position and rotation
        /// </summary>
        protected Transform transform = new();
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
        /// Contains all children of this object
        /// </summary>
        public ObservableCollection<BaseObject> Children {
            get => transform.ChildObjects;
            set => transform.ChildObjects = value;
        }
        /// <summary>
        /// The current parent of the object
        /// </summary>
        public BaseObject Parent {
            get => transform.ParentObject;
            set {
                // Order of  execution is important here
                if (value == null) {
                    // If we set the parent to null before all of this. It will fail.
                    // And of course, if the parent is already null, don't bother wasting CPU cycles
                    if (transform.ParentObject != null && transform.ParentObject.Children.Contains(this))
                        transform.ParentObject.Children.Remove(this);
                    transform.ParentObject = value;
                } else {
                    // If we set the parent after, the code will fail too.
                    transform.ParentObject = value;
                    if (!transform.ParentObject.Children.Contains(this))
                        transform.ParentObject.Children.Add(this);
                }
            }
        }
        /// <summary>
        /// Draw order, smaller = drawn earlier
        /// </summary>
        public int Layer { get => layer; set {
            gameWindow?.InvalidateObjectsCache();
            layer = value;
        } }
        private int layer = int.MinValue;
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
        public delegate void OnClicked(ClickedEventArgs e);
        /// <summary>
        /// Fired when this object is clicked
        /// </summary>
        public event OnClicked Clicked;
        public bool IsInitialized { get => isInitialized; }
        protected bool isInitialized = false;
        protected int vertexHandle;
        protected int vertexBufferHandle;
        protected int elementBufferHandle;
    }
}