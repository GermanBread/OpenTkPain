// System
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

// OpenTK
using OpenTK.Mathematics;

using uf.Rendering;
using uf.Utility.Scenes;

namespace uf.GameObject.Components
{
    public class Transform
    {
        /// <summary>
        /// Compiles all data necessary for rendering into a Vertex object
        /// </summary>
        public Vertex[] CompileData(Color4 objectColor, Scene scene) {
            var _coordinates = Vertices;
            Vector2 _aspectRatioFactor = new((float)Camera.Resolution.Y / Camera.Resolution.X, 1);

            // -> Local <-
            Matrix2.CreateRotation(Rotation, out var _localRotation);
            var _localSkew = (Skew / GlobalScale);
            var _localSize = (Size / GlobalScale) * new Vector2(Camera.Resolution.X / 720f);
            var _localPosition = (Position / GlobalScale) + Vector2.Divide(Anchor.Xy, _aspectRatioFactor);
            
            // -> Parent <-
            Matrix2.CreateRotation((ParentObject?.Rotation ?? 0), out var _parentRotation);
            var _parentSize = (ParentObject?.Size / GlobalScale ?? Vector2.One);
            var _parentPosition = (ParentObject?.Position / GlobalScale ?? Vector2.Zero);
            
            // -> Scene <-
            Matrix2.CreateRotation(scene.Rotation, out var _sceneRotation);
            var _sceneSize = scene.Scale;
            var _scenePosition = scene.Position / GlobalScale;

            // -> Global <-
            Matrix2.CreateRotation(-Camera.Rotation, out var _globalRotation);
            var _globalSize = Camera.Zoom;
            var _globalPosition = Camera.Position;

            // -> Local <-
            _coordinates = Array.ConvertAll(_coordinates, vec => vec + vec + _localSkew * vec.Yx);
            _coordinates = Array.ConvertAll(_coordinates, vec => vec * _localRotation);
            _coordinates = Array.ConvertAll(_coordinates, vec => vec * _localSize);
            _coordinates = Array.ConvertAll(_coordinates, vec => vec + _localPosition);
            
            // -> Parent <-
            _coordinates = Array.ConvertAll(_coordinates, vec => vec * _parentRotation);
            // Make this optional?
            //_coordinates = Array.ConvertAll(_coordinates, vec => vec * _parentSize);
            _coordinates = Array.ConvertAll(_coordinates, vec => vec + _parentPosition);

            // -> Scene <-
            _coordinates = Array.ConvertAll(_coordinates, vec => vec * _sceneRotation);
            _coordinates = Array.ConvertAll(_coordinates, vec => vec * _sceneSize);
            _coordinates = Array.ConvertAll(_coordinates, vec => vec + _scenePosition);
            
            // -> Global <-
            _coordinates = Array.ConvertAll(_coordinates, vec => vec * _globalRotation);
            _coordinates = Array.ConvertAll(_coordinates, vec => vec * _globalSize);
            _coordinates = Array.ConvertAll(_coordinates, vec => vec + _globalPosition);
            _coordinates = Array.ConvertAll(_coordinates, vec => vec * _aspectRatioFactor);

            List<Vertex> _output = new(); // New C# 9 syntax, nice!
            for (var i = 0; i < Vertices.Length; i++) {
                _output.Add(new Vertex {
                    Position = _coordinates[i],
                    UV = Uv[i],
                    InnerBounds = Uv[i],
                    Color = objectColor
                });
            }
            return _output.ToArray();
        }
        /// <summary>
        /// Position in 2D space
        /// </summary>
        public Vector2 Position = Vector2.Zero;
        /// <summary>
        /// 2D size
        /// </summary>
        public Vector2 Size = Vector2.One;
        /// <summary>
        /// Skew along the x and y axis
        /// </summary>
        public Vector2 Skew = Vector2.Zero;
        /// <summary>
        /// Rotation around the Z-axis
        /// </summary>
        public float Rotation = 0;
        /// <summary>
        /// The Anchor of the object. Used for positioning
        /// </summary>
        public Anchor Anchor = Anchor.Center;
        public ObservableCollection<BaseObject> ChildObjects = new();
        public BaseObject ParentObject;
        public Vector2[] Uv;
        public Vector2[] Vertices;
        public uint[] Indices;
        /// <summary>
        /// Global scale. Affects everything. Lower values make objects bigger (does not behave like camera zooming!)
        /// </summary>
        // I prefer using { get; } over readonly because it looks nicer in my IDE :)
        private static float GlobalScale => 10;
    }
}