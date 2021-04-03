// System
using System;
using System.IO;
using System.Collections.Generic;

// OpenTK
using OpenTK.Graphics.OpenGL4;

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
        public Texture(Resource TextureResource) {
            Image<Rgba32> _image = Image.Load<Rgba32>(TextureResource.Path);
            
            // Convert to OGL address space
            _image.Mutate(x => x.Flip(FlipMode.Vertical));

            // Create a byte array for OpenGL
            var _bytes = new List<byte>(4 * _image.Width * _image.Height);

            for (int y = 0; y < _image.Height; y++) {
                var row = _image.GetPixelRowSpan(y);

                for (int x = 0; x < _image.Width; x++) {
                    _bytes.Add(row[x].R);
                    _bytes.Add(row[x].G);
                    _bytes.Add(row[x].B);
                    _bytes.Add(row[x].A);
                }
            }

            // Create the texture
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _image.Width, _image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, _bytes.ToArray());
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
    }
}