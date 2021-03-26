// System
using System;
using System.Linq;
using System.Collections.Generic;

// OpenTK
using OpenTK.Mathematics;

using nb.Game.Utility.Globals;

namespace nb.Game.GameObject.Components
{
    public class Transform
    {
        /// <summary>
        /// Returns coordinates to be passed as vertices
        /// </summary>
        public Vector2[] Coordinates { get {
            Vector2[] _calculated = Vertices;
            
            // Skewing; Must be RELATIVE to the center of the object!
            _calculated = Array.ConvertAll(_calculated, vec => new Vector2((vec.X + Skew.X * (vec.Y - Position.Y)), (vec.Y + Skew.Y * (vec.X - Position.X))));
            // Rotation
            _calculated = Array.ConvertAll(_calculated, vec => {
                double _len = vec.Length;
                // Calculate the rotation relative north
                double _rot = Math.Atan(vec.Y / vec.X);
                return vec;
            });
            // Size
            var _sizeAdjust = new Vector2(EngineGlobals.CurrentResolution.Y / EngineGlobals.CurrentResolution.X, 1);
            _sizeAdjust = new Vector2(1, 1);
            _calculated = Array.ConvertAll(_calculated, vec => Vector2.Multiply(vec, Size * _sizeAdjust));
            // Positioning
            _calculated = Array.ConvertAll(_calculated, vec => Vector2.Add(vec, Position));

            return _calculated;
        } }
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
        public Vector2[] Vertices;
        public uint[] Indices;
    }
}