// System
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// Imagesharp
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

using uf.Utility.Conversion;

namespace uf.Rendering.Textures;

public static class TextureAtlas {
    public static void AddTexture(Texture texture) {
        if (textures.ContainsKey(texture.ID)) return;
        textures.Add(texture.ID, texture);
        createTexture();
    }
    public static void RemoveTexture(Texture texture) {
        if (!textures.ContainsKey(texture.ID)) return;
        textures.Remove(texture.ID);
        createTexture();
    }
    public static (Vector2, Vector2) GetUV(Texture texture) {
        var _cleaned = coordinates[texture.ID];
        var _atlasSize = atlas.Size().ToVector2();
        
        _cleaned.Item1 = Vector2.Divide(_cleaned.Item1, _atlasSize);
        _cleaned.Item2 = Vector2.Divide(_cleaned.Item2, _atlasSize);

        return _cleaned;
    }

    private static void createTexture() {
        if (texturehandle >= 0) GL.DeleteTexture(texturehandle);
        texturehandle = GL.GenTexture();

        atlas?.Dispose();
        atlas = new Image<Rgba32>(1, 1, Color.Transparent);

        coordinates.Clear();

        textures.Values.ToList().ForEach(x => {
            if (coordinates.ContainsKey(x.ID)) return;

            var _canvas = new Image<Rgba32>(Math.Max(atlas.Width, x.TextureImage.Width), atlas.Height + x.TextureImage.Height, Color.Transparent);
            var _transform = new AffineTransformBuilder().AppendTranslation(new PointF(0, atlas.Height));
            var _texture = x.TextureImage.Clone();

            _texture.Mutate(y
                => y.Flip(FlipMode.Vertical).Transform(_transform)
            );
            _canvas.Mutate(y
                => y.DrawImage(atlas, 1).DrawImage(_texture, 1)
            );

            coordinates.Add(x.ID, (new Vector2(0, atlas.Height), new Vector2(_texture.Width, _canvas.Height)));

            _texture.Dispose();
            atlas.Dispose();
            atlas = _canvas.Clone();
            _canvas.Dispose();
        });

        // Create a byte array for OpenGL
        var _pixels = new List<byte>(4 * atlas.Width * atlas.Height);

        for (int y = 0; y < atlas.Height; y++) {
            for (int x = 0; x < atlas.Width; x++) {
                _pixels.Add(atlas[x, y].R);
                _pixels.Add(atlas[x, y].G);
                _pixels.Add(atlas[x, y].B);
                _pixels.Add(atlas[x, y].A);
            }
        }
        
        string _label = "atlas";

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, texturehandle);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, atlas.Width, atlas.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, _pixels.ToArray());
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        GL.ObjectLabel(ObjectLabelIdentifier.Texture, texturehandle, _label.Length, _label);
    }

    public static IReadOnlyList<Texture> Textures => textures.Values.ToList();

    private static Image<Rgba32> atlas;
    private static int texturehandle = -1;
    private static readonly Dictionary<uint, Texture> textures = new();
    private static readonly Dictionary<uint, (Vector2, Vector2)> coordinates = new();
}