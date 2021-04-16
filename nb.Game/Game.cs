// System
using System;
using System.Linq;
using System.Collections.Generic;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

// New Beginnings
using nb.Game.Rendering;
using nb.Game.GameObject;
using nb.Game.Utility.Audio;
using nb.Game.Utility.Scenes;
using nb.Game.Utility.Logging;
using nb.Game.Utility.Resources;
using nb.Game.Rendering.Textures;
using nb.Game.Utility.Attributes;
using nb.Game.GameObject.Components;

namespace nb.Game
{
    public class Game : BaseGame
    {
        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        Rectangle first;
        Triangle second = new() {
            Size = new Vector2(150),
            Anchor = Anchor.BottomRight,
        };
        Rectangle third = new() {
            Position = new Vector2(0, 80),
            Size = new Vector2(50, 25),
            Anchor = Anchor.Left
        };
        Rectangle fourth = new("ui") {
            Size = new Vector2(25),
            Color = Color4.Black,
            Layer = int.MaxValue - 1
        };
        Rectangle fifth = new() {
            Size = new Vector2(10),
            Anchor = Anchor.BottomRight,
            Color = Color4.Orange,
            Layer = int.MaxValue - 2
        };
        List<Rectangle> visualisers;
        float counter = 0;
        [NoTimeout]
        public void Init() {
            // The resource manager allows us to create aliases for files on the user's file system. In the future I plan on enforcing the use of the resource manager.
            ResourceManager.LoadResource("cool intro", "cool intro song.mp3");
            ResourceManager.LoadResource("chungus", "bigbigchungus.jpg");
            ResourceManager.LoadResource("tonk", "tonk.png");

            var _texture = new Texture(ResourceManager.GetResource("chungus"));
            new Texture(ResourceManager.GetResource("tonk"));
            new Texture(ResourceManager.GetResource("eggs.jpg"));
            new Texture(ResourceManager.GetResource("arch btw.png"));

            first = new Rectangle {
                Size = new Vector2(250, 750),
                Anchor = Anchor.TopRight,
                Position = new Vector2(-10),
                Color = Color4.Beige,
                Layer = 1,
                Texture = _texture
            };

            AudioManager.GlobalStreamVolume = .5f;
            
            var _clip = AudioManager.CreateClip(ResourceManager.GetResource("cool intro"));
            _clip.Play();
            _clip.Loop = true;
            visualisers = new List<Rectangle>();
            for (int i = 0; i < Size.X / 5; i++) {
                visualisers.Add(new Rectangle("visualisers") {
                    Position = new Vector2(i * 10, 0),
                    Size = new Vector2(4, 5),
                    Anchor = Anchor.BottomLeft,
                    Color = Color4.Crimson,
                    Layer = 500
                });
            }
            SceneManager.LoadScene("visualisers");

            CursorVisible = false;
        }

        Vector2 parallax;
        public void Update() {
            counter += FrameDelta;
            
            // Nornalize the coordinate to (-1,-1)-( 1, 1)
            var _normalizedMouseCoords = (Vector2.Divide(MousePosition, Size) - new Vector2(.5f)) * new Vector2( 1,-1);
            var _mouseCoords = _normalizedMouseCoords * Size;

            parallax = Vector2.Lerp(parallax, _mouseCoords, 2.5f * FrameDelta);
            //Camera.Position = parallax;
            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Right))
                Camera.Rotation += FrameDelta;
            else if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Left))
                Camera.Rotation -= FrameDelta;

            fourth.Position = Camera.ScreenToWorldSpace((Vector2i)MousePosition);
            fourth.Color = counter % .5 < .25 ? Color4.White : Color4.Black;
            fourth.Rotation += (MathF.Sin(counter) + 1) * FrameDelta;
            fourth.Size = new Vector2(MathF.Sqrt(Size.X * Size.Y) * .05f);
            
            first.Skew = new Vector2(MathF.Sin(counter * 10) * 20f);
            if (first.IsHovered)
                first.Color = Color4.Red;
            else
                first.Color = Color4.Beige;
            second.Position = new Vector2(MathF.Sin(counter * 5) * 20f - 20f, 0f);
            second.Rotation += FrameDelta;
            third.Position = new Vector2(0f, MathF.Sin(counter * 3) * 50f + 80f);
            second.Color = Color4.FromHsv(new Vector4(counter / 2f % 1, 1f, 1f, 1f));
            FillColor = Color4.FromHsv(new Vector4(counter / 5f % 1, 1f, .5f, 1f));

            var _clip = AudioManager.GetClip("cool intro");
            var _data = _clip.GetWaveform(visualisers.Count);
            var _fft = _data.Item1;
            for (int i = 0; i < _fft.Length; i++) {
                if (visualisers[i].IsHovered)
                    visualisers[i].Color = Color4.Violet;
                else
                    visualisers[i].Color = Color4.Crimson;
                visualisers[i].Size = new Vector2(visualisers[i].Size.X, 5f + _fft[i] * 100f);
            }
        }
    }
}