// System
using System;
using System.Collections.Generic;

// OpenTK
using OpenTK.Mathematics;

// Unsigned Framework
using uf.Utility.Logging;

namespace uf.GameObject
{
    public class Polygon : BaseObject {
        public Polygon(string Scene, ushort Corners) : base(Scene) {
            if (Corners < 3) {
                Logger.Log(new LogMessage(LogSeverity.Warning, $"I was told to create a polygon with {Corners} corners. What do I do with this?"));
                return;
            }
            
            List<Vector2> _verts = new();
            // FIXME: Half circle
            for (float i = 0; i < 360; i += 360f / Corners) {
                Matrix2.CreateRotation(i, out var _rot);
                _verts.Add(new Vector2(-1, 1) * _rot);
            }
            transform.Vertices = _verts.ToArray();
            transform.UV = _verts.ToArray();
            
            List<Vector3i> _indices = new();
            for (int i = 1; i <= Corners; i++) {
                _indices.Add(new Vector3i(0, i, i + 1));
            }
            transform.Indices = new uint[Corners * 3];
            _indices.ForEach(x => {
                    for (int i = 0; i < 3; i++)
                        transform.Indices[_indices.IndexOf(x) * 3 + i] = (uint)x[i];
                }
            );
        }
    }
}