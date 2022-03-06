// System
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

// Imagesharp
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using uf.Utility.Logging;
using uf.Utility.Resources;

namespace uf.Rendering.Textures
{
    public class Texture : IDisposable
    {
        public Texture(Resource TextureResource) {
            if (TextureResource == null) {
                Logger.Log(new LogMessage(LogSeverity.Error, $"Cannot load texture resource is null, please use Texture.Empty instead"));
                return;
            }
            
            // Store it so that we can refer to it later
            Resource = TextureResource;

            Logger.Log(new LogMessage(LogSeverity.Debug, $"Loading texture {TextureResource.Name}"));
            if (TextureResource.Path == null && TextureResource.Stream == null) {
                Logger.Log(new LogMessage(LogSeverity.Error, $"Cannot load texture resource {TextureResource.Name}, both the underlying stream and file path are null"));
                return;
            }

            if (!File.Exists(TextureResource.Path)) {
                var _tmp = Path.GetTempFileName();
                
                using var _writer = File.Create(_tmp);
                TextureResource.Stream.BaseStream.CopyTo(_writer);
                
                TextureImage = Image.Load<Rgba32>(_tmp);
                
                File.Delete(_tmp);
            } else {
                TextureImage = Image.Load<Rgba32>(Resource.Path);
            }
            
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Created new texture: {TextureImage.Width * TextureImage.Height} pixels, dimensions {TextureImage.Width}*{TextureImage.Height}"));

            TextureAtlas.AddTexture(this);
        }

        static Texture() {
            blank = new(ResourceManager.GetFile("white texture"));
            TextureAtlas.AddTexture(blank);
        }

        /// <summary>
        /// Retrieve UV coordinates
        /// </summary>
        /// <returns>1: UV start. 2: UV end.</returns>
        public (Vector2, Vector2) GetUV() => TextureAtlas.GetUV(this);

        private bool disposed = false;
        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                TextureAtlas.RemoveTexture(this);
                TextureImage?.Dispose();
                disposed = true;
            }
        }
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static Texture Empty { get => blank; }

        public Resource Resource;
        public Image<Rgba32> TextureImage;

        private static readonly Texture blank;
    }
}
