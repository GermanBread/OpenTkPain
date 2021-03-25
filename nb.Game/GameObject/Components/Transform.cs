// System
using System;
using System.Linq;
using System.Collections.Generic;

// OpenTK
using OpenTK.Mathematics;

namespace nb.Game.GameObject.Components
{
    public class Transform
    {
        /// <summary>
        /// Returns coordinates to be passed as vertices
        /// </summary>
        public Vector2[] Coordinates { get {
            Vector2[] _calculated = Vertices;
            
            // Skewing
            _calculated = Array.ConvertAll(_calculated, vec => new Vector2((vec.X + Skew.X * vec.Y), (vec.Y + Skew.Y * vec.X)));
            // Rotation
            _calculated = Array.ConvertAll(_calculated, vec => {
                double _len = vec.Length;
                // Calculate the rotation relative north
                Console.WriteLine(vec);
                double _rot = Math.Atan(vec.Y / vec.X);
                Console.WriteLine(_rot);
                return vec;
            });
            // Size
            _calculated = Array.ConvertAll(_calculated, vec => Vector2.Multiply(vec, Size));
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