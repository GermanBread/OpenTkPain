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
        public Vector2[] coordinates { get {
            Vector2[] _calculated = vertices;
            
            // Skewing
            _calculated = Array.ConvertAll(_calculated, vec => new Vector2((vec.X + skew.X * vec.Y), (vec.Y + skew.Y * vec.X)));
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
            _calculated = Array.ConvertAll(_calculated, vec => Vector2.Multiply(vec, size));
            // Positioning
            _calculated = Array.ConvertAll(_calculated, vec => Vector2.Add(vec, position));

            return _calculated;
        } }
        /// <summary>
        /// Position in 2D space, scaled with screen resolution
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// 2D size, scaled with screen resolution
        /// </summary>
        public Vector2 size;
        /// <summary>
        /// Skew along the x and y axis
        /// </summary>
        /// <returns></returns>
        public Vector2 skew;
        /// <summary>
        /// Rotation around the Z-axis
        /// </summary>
        public float rotation = 0;
        public Vector2[] vertices;
        public uint[] indices;
    }
}