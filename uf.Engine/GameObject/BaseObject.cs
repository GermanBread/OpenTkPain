// System

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using uf.Rendering;
using uf.Utility.Input;
using uf.Utility.Scenes;
using uf.Utility.Globals;
using uf.Utility.Logging;
using uf.Rendering.Shaders;
using uf.Rendering.Textures;
using uf.GameObject.Components;

namespace uf.GameObject
{
    public class BaseObject
    {
        protected BaseObject(string scene)
        {
            Scene = SceneManager.AddToScene(this, scene ?? "default");
            // Automatically initialise
            if (!Scene.IsLoaded) return;
            Init();
            GameWindow?.InvalidateObjectsCache();
        }
        internal void Init()
        {
            if (IsInitialized) {
                Logger.Log(new LogMessage(LogSeverity.Warning, "Init() was called even though this object was already initialized!"));
                return;
            }

            InitValues();

            CreateObjects();
            InitObjects();

            IsInitialized = true;
        }

        private void InitValues() {
            shader ??= Shader.BaseShader;

            // Set a default color value
            if (Color == default)
                Color = Color4.White;

            // No texture? Create a blank one!
            Texture ??= Texture.Empty;

            GameWindow.MouseDown += (e) => {
                if (IsHovered)
                    Clicked?.Invoke(new ClickedEventArgs(e.Button));
            };
            
            // Dummy event handler that prevents a null-ref
            Clicked += (_) => { };

            Children.CollectionChanged += (_, e) => {
                if (e.Action != System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                    foreach (var child in Children)
                        child.Parent = this;
                else
                    foreach (var formerChild in e.OldItems!)
                        if (formerChild is BaseObject a)
                            a.Parent = null;
            };
        }

        private void CreateObjects() {
            vertexHandle = GL.GenVertexArray();
            elementBufferHandle = GL.GenBuffer();
            vertexBufferHandle = GL.GenBuffer();
        }

        private void InitObjects() {
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
        internal void Update() {
            if (IsHoverable && InputManager.HoveredObject == this)
                IsHovered = true;
            else
                IsHovered = false;
        }
        internal void PreDraw() {
            // Small optimization: Don't perform a draw call if the object is 100% transparent
            if (Color.A == 0)
                return;

            //if (Color.A < 1) {
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.BlendEquation(BlendEquationMode.FuncAdd);
                GL.Enable(EnableCap.Blend);
            //}

            shader.Use();
        }
        internal void PostDraw() {
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
            
            PreDraw();

            var _data = transform.CompileData(Color, Scene);

            for (var i = 0; i < _data.Length; i++) {
                var (_vector2, _item2) = Texture.GetUV();
                _data[i].UV *= _item2 - _vector2;
                _data[i].UV += _vector2;
            }

            GL.BufferData(BufferTarget.ArrayBuffer, _data.Length * Unsafe.SizeOf<Vertex>(), _data, BufferUsageHint.DynamicDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, transform.Indices.Length * sizeof(uint), transform.Indices, BufferUsageHint.DynamicDraw);

            GL.DrawElements(PrimitiveType.Triangles, transform.Indices.Length, DrawElementsType.UnsignedInt, 0);

            PostDraw();
        }
        internal void MultiPassDraw(Color4 @override)
        {
            if (!IsInitialized) {
                Logger.Log(new LogMessage(LogSeverity.Error, "Not initialized; refusing to render object! Was Init() not called?"));
                return;
            }
            
            var _data = transform.CompileData(@override, Scene);

            GL.BufferData(BufferTarget.ArrayBuffer, _data.Length * Unsafe.SizeOf<Vertex>(), _data, BufferUsageHint.DynamicDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, transform.Indices.Length * sizeof(uint), transform.Indices, BufferUsageHint.DynamicDraw);

            GL.DrawElements(PrimitiveType.Triangles, transform.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
        /// <summary>
        /// Frees any resources used by this Object and preps it for reinitialization
        /// </summary>
        public void Dispose()
        {
            // Jokes on you, we're not actually disposing this object
            GL.DeleteVertexArray(vertexHandle);
            GL.DeleteBuffer(vertexBufferHandle);
            GL.DeleteBuffer(elementBufferHandle);
            IsInitialized = false;
        }
        /// <summary>
        /// Get the scene this object is located in
        /// </summary>
        public Scene Scene { get => scene;
            private init {
            GameWindow?.InvalidateObjectsCache();
            scene = value;
        } }
        private readonly Scene scene;
        /// <summary>
        /// Alias to EngineGlobals.Window
        /// </summary>
        private static BaseGame GameWindow => EngineGlobals.Window;

        /// <summary>
        /// Contains all information related to size, position and rotation
        /// </summary>
        protected readonly Transform transform = new();
        /// <summary>
        /// Alias to transform.Skew
        /// </summary>
        public Vector2 Skew {
            set => transform.Skew = value; }
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
                    transform.ParentObject = null;
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
        public int Layer { get => layer; init {
            GameWindow?.InvalidateObjectsCache();
            layer = value;
        } }
        private readonly int layer = int.MinValue;
        /// <summary>
        /// The shader currently in use
        /// </summary>
        private Shader shader;
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
        public bool IsHoverable { get; init; }
        public delegate void OnClicked(ClickedEventArgs e);
        /// <summary>
        /// Fired when this object is clicked
        /// </summary>
        public event OnClicked Clicked;
        public bool IsInitialized { get; private set; }

        private int vertexHandle;
        private int vertexBufferHandle;
        private int elementBufferHandle;
    }
}