// System
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

// Imagesharp
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using nb.Game.Utility.Logging;
using nb.Game.Utility.Resources;

namespace nb.Game.Rendering.Textures
{
    public class Texture
    {
        private static bool initialized = false;
        private static int handle = -1;
        public Resource Resource;
        // We're putting those in an atlas, so we need the coordinates too
        // To prevent exceptions, we create a default texture. This texture will also be used to display pure color
        private static Dictionary<Resource, (Vector2, Vector2)> coordinates = new() { { Resource.Empty, (Vector2.Zero, Vector2.One) } };
        private static Image<Rgba32> atlas = new Image<Rgba32>(1, 1, Color.White);
        public Texture(Resource TextureResource) {
            // FIXME: Stub
            initialized = true;
        }
        public void Use() {
            // FIXME: Stub
        }
        /// <summary>
        /// Retrieve UV coordinates
        /// </summary>
        /// <returns>1: UV start. 2: UV end.</returns>
        public (Vector2, Vector2) GetUV() {
            // FIXME: Stub
            return (Vector2.Zero, Vector2.One);
        }

        public static void DumpAtlas() {
            Logger.Log(new LogMessage(LogSeverity.Info, "Saving atlas to disk..."));
            atlas.SaveAsPng("atlas.png");
        }
        
        // Disposing
        private bool disposed = false;
        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                GL.DeleteTexture(handle);
                atlas.Dispose();
                disposed = true;
            }
        }
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}