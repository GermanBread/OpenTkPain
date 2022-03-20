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
        public Text(string scene) : base(scene) {

        }

        internal override void Draw() {
            if (!IsInitialized) {
                Logger.Log(new LogMessage(LogSeverity.Error, "Not initialized; refusing to render object! Was Init() not called?"));
                return;
            }
            
            PreDraw();

            // returned vectors from Texture.GetUV() should not be altered!

            List<Vertex> mesh = new();
            List<uint> meshIndices = new();
            float step = 0;
            int wrap = 1; // Overflow wrap to next line (if possible). 1 = not wrapped yet

            for (int i = 0; i < Content.Length; i++) {
                if (!texes.ContainsKey(Content[i])) {
                    // TODO: Generate texture
                }
                
                var uv = texes[Content[i]].GetUV();
                var charSize = new Vector2(uv.Item2.X - uv.Item1.X, uv.Item2.Y - uv.Item1.Y);

                // Check if we will overflow horizontally
                if (step + charSize.X > Size.X) {
                    step = 0;
                    wrap++;
                }

                // We are overflowing vertically, stop generating the mesh
                if (FontSize*wrap > Size.Y)
                    break;

                // Top Left
                mesh.Add(new Vertex {
                    Position = new Vector2(Position.X + step, Position.Y + FontSize*wrap - charSize.Y),
                    UV = uv.Item1,
                    Color = Color
                });
                meshIndices.Add((uint)i*4);
                
                // Top Right
                mesh.Add(new Vertex {
                    Position = new Vector2(Position.X + charSize.X + step, Position.Y + FontSize*wrap - charSize.Y),
                    UV = uv.Item1,
                    Color = Color
                });
                meshIndices.Add((uint)i*4 + 1);
                
                // Bottom Right
                mesh.Add(new Vertex {
                    Position = new Vector2(Position.X + charSize.X + step, Position.Y + Size.Y),
                    UV = uv.Item1,
                    Color = Color
                });
                meshIndices.Add((uint)i*4 + 2);
                
                // Bottom Left
                mesh.Add(new Vertex {
                    Position = new Vector2(Position.X + step, Position.Y + Size.Y),
                    UV = uv.Item1,
                    Color = Color
                });
                meshIndices.Add((uint)i*4 + 3);

                // Move forwards...
                step += charSize.X;
            }

            var _data = mesh.ToArray();
            transform.Indices = meshIndices.ToArray();
            
            GL.BufferData(BufferTarget.ArrayBuffer, _data.Length * Unsafe.SizeOf<Vertex>(), _data, BufferUsageHint.DynamicDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, transform.Indices.Length * sizeof(uint), transform.Indices, BufferUsageHint.DynamicDraw);

            GL.DrawElements(PrimitiveType.Triangles, transform.Indices.Length, DrawElementsType.UnsignedInt, 0);

            PostDraw();
        }

        public string Content { get; init; } = "Placeholder text";

        /// <summary>
        /// Either the name of a font or the path to a font file
        /// </summary>
        public string Font = "good times rg.ttf";

        private const float FontSize = 50;
        public Color4 FontColor = Color4.Black;
        private static readonly Dictionary<char, Texture> texes = new();
    }
}