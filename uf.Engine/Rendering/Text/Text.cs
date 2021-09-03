using System.Drawing;
// System
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

// Unsigned Framework
using uf.GameObject;
using uf.Utility.Logging;
using uf.Rendering.Shaders;
using uf.Utility.Resources;
using uf.GameObject.Components;

// Imagesharp
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using IColor = SixLabors.ImageSharp.Color;
using IPointF = SixLabors.ImageSharp.PointF;
using SixLabors.ImageSharp.Drawing.Processing;

namespace uf.Rendering.Text {
    public class Text : BaseObject {
        public Text(string Scene) : base(Scene) {
            // Copied from Rectangle.cs
            transform.Vertices = new Vector2[] {
                new Vector2(-1, -1),
                new Vector2(-1,  1),
                new Vector2( 1,  1),
                new Vector2( 1, -1)
            };
            transform.UV = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0)
            };
            transform.Indices = new uint[] {
                0, 1, 2,
                0, 2, 3
            };
            
            // Bind a new shader made for text here
            ResourceManager.LoadFile("text vertex shader", "Resources/text.vert");
            ResourceManager.LoadFile("text fragment shader", "Resources/text.frag");
            Shader = new Shader(ResourceManager.GetFile("text vertex shader"), ResourceManager.GetFile("text fragment shader"));
        }
        
        internal override void Draw() {
            base.Draw();
        }
        public override void Dispose() {
            base.Dispose();
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
    }
}