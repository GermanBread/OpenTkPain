// System
using System;
using System.Linq;

// OpenTK
using OpenTK.Mathematics;

using uf.GameObject.Components;

namespace uf.GameObject
{
    /// <summary>
    /// 
    /// </summary>
    public class ComplexShape : BaseObject
    {
        public ComplexShape(string scene = null) : base(scene) { }
        public Vector2[] Vertices { get => transform.Vertices; set {
            Vector2[] _verts = value;
            // If any vector goes out of bounds, normalise within reason
            float _maxLength = Vector2.One.Length;
            if (_verts.Any(x => x.Length > _maxLength)) {
                float _scaleFactor = _verts.Max(x => x.Length) / _maxLength;
                _verts = Array.ConvertAll(_verts, vert => Vector2.Divide(vert, _scaleFactor));
            }
            
            transform.Vertices = _verts;

            transform.UV = Array.ConvertAll(_verts, vert => Vector2.Divide(Vector2.Add(vert, Vector2.One), 2));
            
            transform.Indices = Array.Empty<uint>();
            for (uint i = 0; i < _verts.Length - 2; i++) {
                // TODO: This needs to be worked on a bit more...
                transform.Indices = transform.Indices.Append(i).Append(i + 1).Append(i + 2).ToArray();
            }
        } }
    }
}