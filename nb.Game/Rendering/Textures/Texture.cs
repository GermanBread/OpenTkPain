// System
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

// Imagesharp
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using nb.Game.Utility.Resources;

namespace nb.Game.Utility.Textures
{
    public class Texture
    {
        private int handle;
        public Resource Resource;
        // We're putting those in an atlas, so we need the coordinates too
        // To prevent exceptions, we create a default texture. This texture will also be used to display pure color
        private static Dictionary<Resource, (Vector2, Vector2)> coordinates = new() { { Resource.Empty, (Vector2.Zero, Vector2.One) } };
        private static Image<Rgba32> atlas = new Image<Rgba32>(1, 1, Color.HotPink);
        public Texture(Resource TextureResource) {
            if (TextureResource == null)
                throw new NullReferenceException("Passing NULL as a parameter is not permitted, use Resource.Empty instead");

            Resource = TextureResource;
            
            // We don't want the same texture wasting VRAM space
            if (coordinates.ContainsKey(Resource))
                return;
            
            // Generate a texture handle
            handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, handle);
            
            Image<Rgba32> _image = Image.Load<Rgba32>(TextureResource.Path);

            // Resize & convert to OGL address space
            //_image.Mutate(x => x.Resize(new Size(512, 512)));
            _image.Mutate(x => x.Rotate(RotateMode.Rotate270));
            
            var _atlasSize = new Vector2i(atlas.Size().Width, atlas.Size().Height);
            var _imageSize = new Vector2i(_image.Size().Width, _image.Size().Height);

            // Grow the atlas by the new image size (prettyprinted)
            var _newSize = atlas.Size() + new Size(_imageSize.X, Math.Max(_atlasSize.Y, _imageSize.Y));
            var _newAtlas = new Image<Rgba32>(_newSize.Width, _newSize.Height);
            _newAtlas.Mutate(x => x
                .DrawImage(atlas, new Point(_newSize.Width - _atlasSize.X, _newSize.Height - _atlasSize.Y), 1)
                .DrawImage(_image, 1)
            );
            
            // Copy the new atlas to the old one
            atlas.Dispose();
            atlas = _newAtlas.Clone();
            
            _newAtlas.Dispose();
            _image.Dispose();

            // Create a byte array for OpenGL
            var _bytes = new List<byte>(4 * atlas.Width * atlas.Height);

            for (int y = 0; y < atlas.Height; y++) {
                var row = atlas.GetPixelRowSpan(y);

                for (int x = 0; x < atlas.Width; x++) {
                    _bytes.Add(row[x].R);
                    _bytes.Add(row[x].G);
                    _bytes.Add(row[x].B);
                    _bytes.Add(row[x].A);
                }
            }

            coordinates.Add(TextureResource, (
                Vector2.Divide(_atlasSize, _atlasSize + _imageSize), 
                Vector2.Divide(_imageSize, _atlasSize + _imageSize)
            ));

            // Create the texture
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, atlas.Width, atlas.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, _bytes.ToArray());
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
        /// <summary>
        /// Applies the texture to texture channel 1
        /// </summary>
        /// <returns>1: UV start. 2: UV end.</returns>
        public (Vector2, Vector2) Use() {
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, handle);
            return coordinates[Resource];
        }

        public static void DumpAtlas() {
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