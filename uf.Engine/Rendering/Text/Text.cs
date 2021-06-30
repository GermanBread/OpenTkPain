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

// ImageSharp
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace uf.Rendering.Text {
    public class Text : BaseObject {
        public Text(string Scene) : base(Scene) {
            // Bind a new shader made for text here
        }
        public void Regenerate(string textString) {
            texture?.Dispose();
            Vector2i _dimensions = new((int)Math.Round(Size.X), (int)Math.Round(Size.Y));
            texture = new(_dimensions.X, _dimensions.Y, Rgba32.ParseHex("#FFFFFF"));

            // Yadadada add the generated texture to an atlas and then bind said atlas
        }
        public string Content { get => content; set {
            Regenerate(value);
            content = value;
        } }
        /// <summary>
        /// Either the name of a font or the path to a font file
        /// </summary>
        public string Font = "Arial";

        private string content = "";
        Image<Rgba32> texture;
    }
}