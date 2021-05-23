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
        public Vertex[] CompileData(Color4 ObjectColor, Scene Scene) {
            Vector2[] _coordinates = Vertices;

            Matrix2.CreateRotation(Rotation, out var _rotMatrix);
            Matrix2.CreateRotation(ParentObject?.Rotation ?? 0, out var _parentRotMatrix);
            Matrix2.CreateRotation(-Camera.Rotation - Scene.Rotation, out var _posMatrix);
            // Normalization should happen before the processing
            Vector2 _normalisationHelper = new(MathF.Max(Camera.Resolution.X, Camera.Resolution.Y));
            Vector2 _aspectRatioHelper = new(1, (float)Camera.Resolution.X / Camera.Resolution.Y);
            Vector2 _adjustedSkew = Vector2.Divide(Skew, _normalisationHelper);
            Vector2 _adjustedSize = Vector2.Divide(Size, _normalisationHelper * 2);
            Vector2 _adjustedParentSize = Vector2.Divide(ParentObject?.Size ?? _normalisationHelper * 2, _normalisationHelper * 2);
            Vector2 _adjustedPosition = Vector2.Divide(Position + Scene.Position, _normalisationHelper);
            Vector2 _adjustedParentPosition = Vector2.Divide(ParentObject?.Position ?? Vector2.Zero, _normalisationHelper);
            Vector2 _adjustedCameraPosition = Vector2.Divide(Camera.Position, _normalisationHelper);

            // Skewing; Must be RELATIVE to the center of the object!
            _coordinates = Array.ConvertAll(_coordinates, vec
             => {
                    Vector2 _output = Vector2.Add(vec, new Vector2(
                        (vec.Y - Position.Y / _normalisationHelper.X) * Skew.X / Size.X,
                        (vec.X - Position.X / _normalisationHelper.Y) * Skew.Y / Size.Y
                    ));
                    return _output;
                }
            );
            // Rotation
            _coordinates = Array.ConvertAll(_coordinates, vec => vec * _rotMatrix);
            // Size
            _coordinates = Array.ConvertAll(_coordinates, vec
             => {
                    Vector2 _output = vec * _adjustedSize;
                    return _output;
                }
            );
            // Positioning
            _coordinates = Array.ConvertAll(_coordinates, vec
             => {
                    Vector2 _anchorPos = Vector2.Divide(Anchor.Xy, _aspectRatioHelper);
                    Vector2 _output = _adjustedPosition - _adjustedSize * Anchor.Xy + vec + _anchorPos;
                    
                    // Parenting part
                    if (ParentObject != null)
                        _output *= _adjustedParentSize * Size;
                    _output *= _parentRotMatrix;
                    _output += _adjustedParentPosition;
                    
                    // Camera
                    _output *= _posMatrix;
                    _output -= _adjustedCameraPosition;
                    return _output;
                }
            );

            // Finishing pass
            _coordinates = Array.ConvertAll(_coordinates, vec
             => {
                    Vector2 _output = vec * _aspectRatioHelper;
                    _output *= Scene.Scale;
                    _output *= Camera.Zoom;
                    return _output;
                }
            );

            List<Vertex> _output = new(); // New C# 9 syntax, nice!
            for (int i = 0; i < Vertices.Length; i++) {
                _output.Add(new Vertex {
                    Coordinate = _coordinates[i],
                    UV = UV[i],
                    InnerPosition = UV[i],
                    Color = ObjectColor
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
        public Vector2[] UV;
        public Vector2[] Vertices;
        public uint[] Indices;
    }
}