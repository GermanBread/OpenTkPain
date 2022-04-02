// System
using System;
using System.Linq;
using System.Collections.Generic;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

// Unsigned Framework
using uf.Rendering;
using uf.GameObject;
using uf.Utility.Audio;
using uf.Rendering.Text;
using uf.Utility.Scenes;
using uf.Utility.Logging;
using uf.Utility.Globals;
using uf.Utility.Resources;
using uf.Rendering.Textures;
using uf.Rendering.Animations;
using uf.GameObject.Components;

namespace uf
{
    public class Game : BaseGame {
        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        Rectangle first;
        readonly Triangle second = new() {
            Size = new Vector2(1.5f),
            Anchor = Anchor.BottomRight
        };
        readonly Rectangle third = new() {
            Position = new Vector2(0, 8),
            Size = new Vector2(.5f, .25f),
            Anchor = Anchor.Left
        };
        readonly Rectangle fourth = new("overlay") {
            Size = new Vector2(.25f),
            Color = Color4.Black,
            Layer = 500
        };
        readonly Rectangle fifth = new() {
            Size = new Vector2(.1f),
            Anchor = Anchor.BottomRight,
            Color = Color4.Orange,
            Layer = 499
        };
        List<Rectangle> visualisers;
        float counter = 0;
        public override void Start() {
            PauseOnLostFocus = !EngineGlobals.CLArgs.Contains("--no-pause");
            if (EngineGlobals.CLArgs.Contains("--no-audio"))
                AudioManager.GlobalVolume = 0;
            
            Logger.Log(new LogMessage(LogSeverity.Debug, "Logging is cool"));
            Logger.Log(new LogMessage(LogSeverity.Verbose, "Logging is cool"));
            Logger.Log(new LogMessage(LogSeverity.Info, "Logging is cool"));
            Logger.Log(new LogMessage(LogSeverity.Warning, "Logging is cool"));
            Logger.Log(new LogMessage(LogSeverity.Error, "Logging is cool"));
            Logger.Log(new LogMessage(LogSeverity.Critical, "Logging is cool"));

            // The resource manager allows us to create aliases for files on the user's file system. In the future I plan on enforcing the use of the resource manager.
            ResourceManager.LoadResource("E", "results.mp3");
            ResourceManager.LoadResource("music", "old_results.mp3");
            ResourceManager.LoadResource("music", "temple.mp3");
            ResourceManager.LoadResource("music E", "stereo test.mp3");
            ResourceManager.LoadResource("ignore these errors, they are intentional", "sine wave.wav");
            ResourceManager.LoadFile("tonk", "tonk.png");

            var _texture = new Texture(ResourceManager.GetFile("tonk"));

            first = new Rectangle {
                Size = new Vector2(2.5f, 7.5f),
                Anchor = Anchor.TopRight,
                Position = new Vector2(-1),
                Color = Color4.Beige,
                Layer = 1,
                Texture = _texture,
                IsHoverable = true
            };

            // Implied resource loading
            fourth.Texture = new Texture(ResourceManager.GetFile("arch btw.png"));

            _ = new Rectangle("overlay") {
                Position = new Vector2(2.5f),
                Size = new Vector2(7.5f),
                Color = Color4.OrangeRed,
                Parent = fourth
            };

            fourth.Children[0].Texture = new Texture(ResourceManager.GetFile("nixos.png"));

            var _clip = AudioManager.CreateClip(ResourceManager.GetFile("music"));
            if (_clip != default(AudioClip)) {
                _clip.Loop = true;
                _clip.Volume = .5f;
            }
            //_clip?.Play();
            var _clip2 = AudioManager.CreateClip(ResourceManager.GetFile("E"));
            if (_clip2 != default(AudioClip))
                _clip2.Loop = true;
            _clip2?.Play();

            return;

            Animation _anim = new("testanim", new Keyframe[] {
                new Keyframe {
                    Size = new Vector2(50, 100),
                },
                new Keyframe {
                    Size = new Vector2(100, 50)
                }
            }, first, true);
            //_anim.Play();
            
            visualisers = new List<Rectangle>();
            for (int i = 0; i < Size.X / 2; i++) {
                visualisers.Add(new Rectangle("visualisers") {
                    Position = new Vector2(i, 0),
                    Size = new Vector2(1, 5),
                    Anchor = Anchor.BottomLeft,
                    Layer = 999,
                    IsHoverable = i % 2 == 0
                });
            }
            SceneManager.LoadScene("visualisers");

            CursorVisible = false;

            first.Clicked += (e) => {
                first.Size *= new Vector2(e.MouseButton == 0 ? 1.5f : .5f);
            };

            //_ = new Polygon(null, 4) {
            //    Size = new Vector2(2)
            //};

            // Spam (V)RAM
            /*List<Vector2> _verticeSpam = new();
            int _count = 1000;
            for (int i = 0; i < _count; i++) {
                _verticeSpam.Add(new Vector2(MathF.Sin(i / _count), MathF.Cos(i / _count)));
            }
            for (int i = 0; i < _count; i++)
                _ = new ComplexShape("trolled") {
                    Vertices = _verticeSpam.ToArray(),
                    Size = new Vector2(100),
                    Color = Color4.AliceBlue
                };*/
            
            _ = new Text(null) {
                Size = new Vector2(5, 2),
                Position = new Vector2(-10, -2.5f),
                Layer = 9999,
                Color = Color4.AliceBlue,
                FontColor = Color4.Crimson,
                Texture = _texture,
                Content = "Hello, World!"
            };
        }

        //Vector2 parallax;
        public override void Render() {
            counter += FrameDelta;
            
            //parallax = Vector2.Lerp(parallax, Camera.ScreenToWorldSpace(MousePosition), 2.5f * FrameDelta);
            //parallax = Vector2.Clamp(parallax, new Vector2(-50), new Vector2(50));
            //Camera.Position = parallax;
            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Right)) {
                Camera.Rotation += FrameDelta;
                //first.Scene.Rotation -= FrameDelta;
            }
            else if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Left)) {
                Camera.Rotation -= FrameDelta;
                //first.Scene.Rotation += FrameDelta;
            }
            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Up)) {
                Camera.Zoom += Camera.Zoom * FrameDelta;
                //first.Scene.Scale -= Vector2.One * FrameDelta;
            }
            else if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Down)) {
                Camera.Zoom -= Camera.Zoom * FrameDelta;
                //first.Scene.Scale += Vector2.One * FrameDelta;
            }

            first.Scene.Rotation += FrameDelta;

            fourth.Position = Camera.ScreenToWorldSpace((Vector2i)MousePosition);
            fourth.Color = counter % .5 < .25 ? Color4.White : new Color4(0, 0, 0, 100);
            fourth.Rotation += (MathF.Sin(counter) + 1) * FrameDelta;
            fourth.Size = new Vector2(MathF.Sqrt(Size.X * Size.Y) * .005f);
            
            first.Skew = new Vector2(MathF.Sin(counter * 10) * 2f);
            if (first.IsHovered)
                first.Color = Color4.Red;
            else
                first.Color = Color4.Beige;
            second.Position = new Vector2(MathF.Sin(counter * 5) * 2f - 2f, 0f);
            second.Rotation += FrameDelta;
            third.Position = new Vector2(0f, MathF.Sin(counter * 3) * 5f + 8f);
            second.Color = Color4.FromHsv(new Vector4(counter / 2f % 1, 1f, 1f, 1f));
            FillColor = Color4.FromHsv(new Vector4(counter / 5f % 1, 1f, .5f, 1f));

            return;
            
            var _clip = AudioManager.GetClip("music");
            var _data = (Array.Empty<float>(), -1);
            if (_clip != null)
                _data = _clip.GetWaveform();
            var _fft = _data.Item1;
            for (int i = 0; i < visualisers.Count; i++) {
                if (visualisers[i].IsHovered)
                    visualisers[i].Color = Color4.Violet;
                else
                    visualisers[i].Color = new Color4(255, 50, 20, 100);
                visualisers[i].Size = new Vector2(visualisers[i].Size.X, 5f + _fft[i * (_fft.Length / visualisers.Count)] * 100f);
            }
        }
        public override void Update() {

        }
        public override void Stop() {

        }
    }
}