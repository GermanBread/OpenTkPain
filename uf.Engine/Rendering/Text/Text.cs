// System
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

// Unsigned Framework
using uf.GameObject;
using uf.Utility.Logging;
using uf.Rendering.Textures;

namespace uf.Rendering.Text {
    public class Text : BaseObject {
        public Text(string Scene) : base(Scene) {

        }

        internal override void Draw() {
            if (!IsInitialized) {
                Logger.Log(new LogMessage(LogSeverity.Error, "Not initialized; refusing to render object! Was Init() not called?"));
                return;
            }
            
            preDraw();

            // returned vectors from Texture.GetUV() should not be altered!

            List<Vertex> mesh = new();
            List<uint> meshIndices = new();
            float _step = 0;
            int _wrap = 1; // Overflow wrap to next line (if possible). 1 = not wrapped yet

            for (int i = 0; i < content.Length; i++) {
                if (!texes.ContainsKey(content[i])) {
                    // TODO: Generate texture for glyph
                }
                
                var _uv = texes[content[i]].GetUV();
                var _charSize = new Vector2(_uv.Item2.X - _uv.Item1.X, _uv.Item2.Y - _uv.Item1.Y);

                // Check if we will overflow horizontally
                if (_step + _charSize.X > Size.X) {
                    _step = 0;
                    _wrap++;
                }

                // We are overflowing vertically, stop generating the mesh
                if (FontSize*_wrap > Size.Y)
                    break;

                // Top Left
                mesh.Add(new Vertex {
                    Position = new Vector2(Position.X + _step, Position.Y + FontSize*_wrap - _charSize.Y),
                    UV = _uv.Item1,
                    Color = Color
                });
                meshIndices.Add((uint)i*4);
                
                // Top Right
                mesh.Add(new Vertex {
                    Position = new Vector2(Position.X + _charSize.X + _step, Position.Y + FontSize*_wrap - _charSize.Y),
                    UV = _uv.Item1,
                    Color = Color
                });
                meshIndices.Add((uint)i*4 + 1);
                
                // Bottom Right
                mesh.Add(new Vertex {
                    Position = new Vector2(Position.X + _charSize.X + _step, Position.Y + Size.Y),
                    UV = _uv.Item1,
                    Color = Color
                });
                meshIndices.Add((uint)i*4 + 2);
                
                // Bottom Left
                mesh.Add(new Vertex {
                    Position = new Vector2(Position.X + _step, Position.Y + Size.Y),
                    UV = _uv.Item1,
                    Color = Color
                });
                meshIndices.Add((uint)i*4 + 3);

                // Move forwards...
                _step += _charSize.X;
            }

            var _data = mesh.ToArray();
            transform.Indices = meshIndices.ToArray();
            
            GL.BufferData(BufferTarget.ArrayBuffer, _data.Length * Unsafe.SizeOf<Vertex>(), _data, BufferUsageHint.DynamicDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, transform.Indices.Length * sizeof(uint), transform.Indices, BufferUsageHint.DynamicDraw);

            GL.DrawElements(PrimitiveType.Triangles, transform.Indices.Length, DrawElementsType.UnsignedInt, 0);

            postDraw();
        }

        public string Content { get => content; set {
            // Update vertices here. If I were to pass arrays to the shader I should use fixed-size arrays?
            content = value;
        } }

        /// <summary>
        /// Either the name of a font or the path to a font file
        /// </summary>
        public string Font = "good times rg.ttf";
        public float FontSize = 50;
        public Color4 FontColor = Color4.Black;
        private string content = "Placeholder text";
        private static readonly Dictionary<char, Texture> texes = new();
    }
}