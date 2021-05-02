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
        private static Dictionary<Resource, (Vector2, Vector2)> coordinates = new() { { Resource.Empty, (Vector2.Zero, new Vector2(5)) } };
        private static Image<Rgba32> atlas = new Image<Rgba32>(5, 5, Color.White);
        public Texture(Resource TextureResource) {
            TextureResource ??= Resource.Empty;

            // Store it so that we can refer to it later
            Resource = TextureResource;
            
            // We don't want the same texture wasting VRAM space
            if (coordinates.ContainsKey(TextureResource))
                return;

            // Generate a texture handle
            if (!initialized) {
                handle = GL.GenTexture();
                initialized = true;
            }

            Logger.Log(new LogMessage(LogSeverity.Verbose, "Adding texture to atlas, this will most likely trigger a timeout message"));

            Logger.Log(new LogMessage(LogSeverity.Debug, $"Loading texture {TextureResource.Name}"));
            Image<Rgba32> _image = Image.Load<Rgba32>(TextureResource.Path);

            Logger.Log(new LogMessage(LogSeverity.Debug, $"Resizing texture to 512x?"));
            // Resize & convert to OGL address space
            //_image.Mutate(x => x.Resize(new Size(512, 0)));
            _image.Mutate(x => x.Flip(FlipMode.Vertical));
            
            // Images are rotated. This means X=Y, Y=X. Here be dragons...
            Vector2i _atlasSize = new();
            atlas.Size().Deconstruct(out _atlasSize.X, out _atlasSize.Y);
            Vector2i _imageSize = new();
            _image.Size().Deconstruct(out _imageSize.X, out _imageSize.Y);

            // Calculate a new atlas size
            var _newSize = new Size(Math.Max(_atlasSize.X, _imageSize.X), _atlasSize.Y + _imageSize.Y);
            
            // Create a new atlas
            var _newAtlas = new Image<Rgba32>(_newSize.Width, _newSize.Height);
            
            // Redraw the atlas.
            // My first implementation was moving the entire atlas down but... why? This invalidates all coordinates. What was I thinking?
            Logger.Log(new LogMessage(LogSeverity.Debug, $"Resizing atlas"));
            _newAtlas.Mutate(x => x
                .DrawImage(atlas, 1)
                .DrawImage(_image, new Point(0, _atlasSize.Y), 1)
            );
            
            // Copy the new atlas to the old one
            // Before we copy, make sure to release the memory used by the other one
            atlas.Dispose();
            atlas = _newAtlas.Clone();
            
            // Free resources
            _newAtlas.Dispose();
            _image.Dispose();

            // Create a byte array for OpenGL
            var _bytes = new List<byte>(4 * atlas.Width * atlas.Height);

            for (int y = 0; y < atlas.Height; y++) {
                var _row = atlas.GetPixelRowSpan(y);

                for (int x = 0; x < atlas.Width; x++) {
                    _bytes.Add(_row[x].R);
                    _bytes.Add(_row[x].G);
                    _bytes.Add(_row[x].B);
                    _bytes.Add(_row[x].A);
                }
            }

            coordinates.Add(TextureResource, (
                new Vector2(0, _atlasSize.Y),
                _imageSize + new Vector2(0, _atlasSize.Y)
            ));

            Logger.Log(new LogMessage(LogSeverity.Debug, $"Created new texture: {_bytes.Count / 4} pixels, dimensions {_image.Size()}, start coordinates at {_atlasSize}; ends at {_atlasSize + _imageSize}"));
           
            // Final step: create the texture
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, atlas.Width, atlas.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, _bytes.ToArray());
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        public void Use(TextureUnit Unit = TextureUnit.Texture1) {
            GL.ActiveTexture(Unit);
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }
        /// <summary>
        /// Retrieve UV coordinates
        /// </summary>
        /// <returns>1: UV start. 2: UV end.</returns>
        public (Vector2, Vector2) GetUV() {
            // Ima stop you right there.
            // Since we don't use the incorrect way of storing coordinates as UV, we need to convert them HERE
            var _data = coordinates[Resource];
            
            // Less ugly way of converting Imagesharp Size to OpenTK Vector2i (i stands for integer)
            Vector2i _atlasSize = new();
            atlas.Size().Deconstruct(out _atlasSize.X, out _atlasSize.Y);
            
            _data.Item1 = Vector2.Divide(_data.Item1, _atlasSize);
            _data.Item2 = Vector2.Divide(_data.Item2, _atlasSize);

            return _data;
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
                coordinates.Remove(Resource);
                disposed = true;
            }
        }
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}