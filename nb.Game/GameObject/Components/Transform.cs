// System
using System;
using System.Linq;
using System.Collections.Generic;

// OpenTK
using OpenTK.Mathematics;

using nb.Game.Rendering;
using nb.Game.Utility.Globals;

namespace nb.Game.GameObject.Components
{
    public class Transform
    {
        /// <summary>
        /// Returns coordinates to be passed as vertices
        /// </summary>
        [Obsolete("Use CompileData() instead")]
        public Vector2[] Coordinates { get {
            Vector2[] _calculated = Vertices;
            
            // Skewing; Must be RELATIVE to the center of the object!
            _calculated = Array.ConvertAll(_calculated, vec
             => Vector2.Add(vec, new Vector2(
                    (vec.Y - Position.Y / EngineGlobals.CurrentResolution.X) * Skew.X / Size.X,
                    (vec.X - Position.X / EngineGlobals.CurrentResolution.Y) * Skew.Y / Size.Y
                )
            ));
            // Rotation
            _calculated = Array.ConvertAll(_calculated, vec => {
                Matrix2.CreateRotation(Rotation + MathF.Atan2(vec.X, vec.Y), out var _rotMatrix);
                return _rotMatrix.Column0 * vec.LengthFast;
            });
            // Size
            _calculated = Array.ConvertAll(_calculated, vec
             => Vector2.Divide(
                    vec * Size, 
                    EngineGlobals.CurrentResolution
                )
            );
            // Positioning
            _calculated = Array.ConvertAll(_calculated, vec
             => Vector2.Divide(
                    Position - Size * Anchor.Xy, 
                    EngineGlobals.CurrentResolution
                ) + vec + Anchor.Xy
            );

            return _calculated;
        } }
        public Vertex[] CompileData(Color4 ObjectColor) {
            Vector2[] _coordinates = Vertices;
            
            // Skewing; Must be RELATIVE to the center of the object!
            _coordinates = Array.ConvertAll(_coordinates, vec
             => Vector2.Add(vec, new Vector2(
                    (vec.Y - Position.Y / EngineGlobals.CurrentResolution.X) * Skew.X / Size.X,
                    (vec.X - Position.X / EngineGlobals.CurrentResolution.Y) * Skew.Y / Size.Y
                )
            ));
            // Rotation
            _coordinates = Array.ConvertAll(_coordinates, vec => {
                Matrix2.CreateRotation(Rotation + MathF.Atan2(vec.X, vec.Y), out var _rotMatrix);
                return _rotMatrix.Column0 * vec.LengthFast;
            });
            // Size
            _coordinates = Array.ConvertAll(_coordinates, vec
             => Vector2.Divide(
                    vec * Size, 
                    EngineGlobals.CurrentResolution
                )
            );
            // Positioning
            _coordinates = Array.ConvertAll(_coordinates, vec
             => Vector2.Divide(
                    Position - Size * Anchor.Xy, 
                    EngineGlobals.CurrentResolution
                ) + vec + Anchor.Xy
            );

            List<Vertex> _output = new(); // New C# 9 syntax, nice!
            for (int i = 0; i < Vertices.Length; i++) {
                _output.Add(new Vertex {
                    Coordinate = _coordinates[i],
                    UV = UV[i],
                    Color = ObjectColor
                });
            }
            return _output.ToArray();
        }
        /// <summary>
        /// Position in 2D space, scaled with screen resolution
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// 2D size, scaled with screen resolution
        /// </summary>
        public Vector2 Size;
        /// <summary>
        /// Skew along the x and y axis
        /// </summary>
        public Vector2 Skew;
        /// <summary>
        /// Rotation around the Z-axis
        /// </summary>
        public float Rotation = 0;
        /// <summary>
        /// The Anchor of the object. Used for positioning
        /// </summary>
        public Anchor Anchor = Anchor.Center;
        public Vector2[] UV;
        public Vector2[] Vertices;
        public uint[] Indices;
    }
}