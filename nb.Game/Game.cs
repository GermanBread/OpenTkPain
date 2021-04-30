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
using nb.Game.Utility.Globals;
using nb.Game.Utility.Resources;
using nb.Game.Rendering.Textures;
using nb.Game.GameObject.Components;

namespace nb.Game
{
    public class Game : BaseGame
    {
        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        Rectangle first;
        Triangle second = new() {
            Size = new Vector2(150),
            Anchor = Anchor.BottomRight
        };
        Rectangle third = new() {
            Position = new Vector2(0, 80),
            Size = new Vector2(50, 25),
            Anchor = Anchor.Left
        };
        Rectangle fourth = new("overlay") {
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
        ComplexShape sixth = new() {
            Vertices = new Vector2[] {
                new Vector2( 0, 1),
                new Vector2( 1, 1),
                new Vector2(-1, 1),
                new Vector2(-1, -1),
                new Vector2( 1, -1),
                new Vector2(-.5f, .5f),
                new Vector2( .5f,-.5f),
                new Vector2( 0, -.5f),
            },
            Size = new Vector2(50),
            Position = new Vector2(75),
            Color = Color4.Cyan,
            Anchor = Anchor.Center
        };
        List<Rectangle> visualisers;
        float counter = 0;
        public void Init() {
            PauseOnLostFocus = !EngineGlobals.CLArgs.Contains("--no-pause");

            // The resource manager allows us to create aliases for files on the user's file system. In the future I plan on enforcing the use of the resource manager.
            ResourceManager.LoadResource("music", "TempleOS Hymn Risen (Remix) - Dave Eddy-IdYMA6hY_74.wav");
            //ResourceManager.LoadResource("music", "stereo test.mp3");
            //ResourceManager.LoadResource("music", "sine wave.wav");
            ResourceManager.LoadResource("tonk", "tonk.png");

            var _texture = new Texture(ResourceManager.GetResource("tonk"));
            new Texture(ResourceManager.GetResource("arch btw.png"));
            Texture.DumpAtlas();

            first = new Rectangle {
                Size = new Vector2(250, 750),
                Anchor = Anchor.TopRight,
                Position = new Vector2(-10),
                Color = Color4.Beige,
                Layer = 1,
                Texture = _texture,
                IsHoverable = true
            };

            new Rectangle() {
                Size = new Vector2(50),
                Position = new Vector2(-10),
                Color = Color4.Red
            };
            new Rectangle() {
                Position = new Vector2(10),
                Size = new Vector2(50),
                Color = Color4.Yellow
            };
            new Rectangle() {
                Position = new Vector2(-10),
                Size = new Vector2(50),
                Color = new Color4(255, 0, 0, 127)
            };
            
            var _clip = AudioManager.CreateClip(ResourceManager.GetResource("music"));
            if (_clip != default(AudioClip))
                _clip.Loop = true;
            _clip?.Play();
            
            visualisers = new List<Rectangle>();
            for (int i = 0; i < Size.X / 2; i++) {
                visualisers.Add(new Rectangle("visualisers") {
                    Position = new Vector2(i, 0),
                    Size = new Vector2(1, 5),
                    Anchor = Anchor.BottomLeft,
                    Layer = 500,
                    IsHoverable = i % 2 == 0
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
            Camera.Position = parallax;
            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Right)) {
                Camera.Rotation += FrameDelta;
                first.Scene.Rotation -= FrameDelta;
            }
            else if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Left)) {
                Camera.Rotation -= FrameDelta;
                first.Scene.Rotation += FrameDelta;
            }
            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Up)) {
                Camera.Zoom += Vector2.One * FrameDelta;
                first.Scene.Scale -= Vector2.One * FrameDelta;
            }
            else if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Down)) {
                Camera.Zoom -= Vector2.One * FrameDelta;
                first.Scene.Scale += Vector2.One * FrameDelta;
            }
                

            fourth.Position = Camera.ScreenToWorldSpace((Vector2i)MousePosition);
            fourth.Color = counter % .5 < .25 ? Color4.White : new Color4(0, 0, 0, 100);
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

            var _clip = AudioManager.GetClip("music");
            var _data = (new float[0], -1);
            if (_clip != null)
                _data = _clip.GetWaveform(visualisers.Count);
            var _fft = _data.Item1;
            for (int i = 0; i < _fft.Length; i++) {
                if (visualisers[i].IsHovered)
                    visualisers[i].Color = Color4.Violet;
                else
                    visualisers[i].Color = new Color4(255, 50, 20, 100);
                visualisers[i].Size = new Vector2(visualisers[i].Size.X, 5f + _fft[i] * 100f);
            }
        }
    }
}