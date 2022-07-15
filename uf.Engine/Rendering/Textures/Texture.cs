// System
using System;
using System.IO;

// OpenTK
using OpenTK.Mathematics;

// Imagesharp
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using uf.Utility.Logging;
using uf.Utility.Globals;
using uf.Utility.Resources;

namespace uf.Rendering.Textures
{
    public class Texture : IDisposable
    {
        public Texture(Resource TextureResource) {
            if (TextureResource == null) {
                Logger.Log(new(LogSeverity.Error, "I don't know what to do with \"null\"!"));
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

            id = ++counter;
            TextureAtlas.AddTexture(this);
        }

        static Texture() {
            ResourceManager.LoadFile("white texture", Path.Combine(EngineGlobals.EngineResourcesPath, "white.png"));
            blank = new(ResourceManager.GetFile("white texture"))
            {
                id = 0
            };
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

        public uint ID { get => id; }
        public static Texture Empty { get => blank; }

        public Resource Resource;
        public Image<Rgba32> TextureImage;

        private uint id;

        /// <summary>
        /// This variable keeps count of how many textures have been created, it's value will be used as the texture's ID
        /// </summary>
        private static uint counter = 0;
        private static readonly Texture blank;
    }
}
