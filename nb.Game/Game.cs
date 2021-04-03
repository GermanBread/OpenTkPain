// System
using System;
using System.Linq;
using System.Collections.Generic;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

// New Beginnings
using nb.Game.GameObject;
using nb.Game.Utility.Audio;
using nb.Game.Utility.Scenes;
using nb.Game.Utility.Logging;
using nb.Game.Utility.Textures;
using nb.Game.Utility.Resources;
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
            Size = new Vector2(50),
            Anchor = Anchor.Left
        };
        Rectangle fourth = new() {
            Size = new Vector2(25),
            Color = Color4.Black,
            Anchor = Anchor.TopLeft,
            Layer = int.MaxValue - 1
        };
        List<Rectangle> visualisers;
        float counter = 0;
        public void Init() {
            ResourceManager.LoadResource("chungus", "bigbigchungus.jpg");
            first = new Rectangle {
                Size = new Vector2(250),
                Anchor = Anchor.TopRight,
                Position = new Vector2(-10),
                Color = Color4.Beige,
                Layer = 1,
                Texture = new Texture(ResourceManager.GetResource("chungus"))
            };

            AudioManager.GlobalStreamVolume = .5f;
            ResourceManager.LoadResource("big chungus", "chungus.mp3");
            var _clip = AudioManager.CreateClip(ResourceManager.GetResource("big chungus"));
            _clip.Play();
            _clip.Loop = true;
            visualisers = new List<Rectangle>();
            for (int i = 0; i < Size.X / 5; i++) {
                visualisers.Add(new Rectangle("visualisers") {
                    Position = new Vector2(i * 5, 0),
                    Size = new Vector2(4, 5),
                    Anchor = Anchor.BottomLeft
                });
            }
            SceneManager.LoadScene("visualisers");

            CursorVisible = false;
        }

        public void Update() {
            counter += FrameDelta;
            
            var _normalizedMouseCoords = MousePosition * 2;
            fourth.Position = new Vector2(_normalizedMouseCoords.X, -_normalizedMouseCoords.Y);
            fourth.Color = counter % .5 < .25 ? Color4.White : Color4.Black;
            fourth.Rotation += (MathF.Sin(counter) + 1) * FrameDelta;
            fourth.Size = new Vector2(MathF.Sqrt(Size.X * Size.Y) * .05f);
            
            first.Skew = new Vector2(MathF.Sin(counter * 10) * 20f);
            second.Position = new Vector2(MathF.Sin(counter * 5) * 20f - 20f, 0f);
            second.Rotation += FrameDelta;
            third.Position = new Vector2(0f, MathF.Sin(counter * 3) * 50f + 80f);
            second.Color = Color4.FromHsv(new Vector4(counter / 2f % 1, 1f, 1f, 1f));
            FillColor = Color4.FromHsv(new Vector4(counter / 5f % 1, 1f, 1f, 1f));

            var _clip = AudioManager.GetClip("big chungus");
            var _data = _clip.GetWaveform(visualisers.Count);
            var _fft = _data.Item1;
            for (int i = 0; i < _fft.Length; i++) {
                visualisers[i].Size = new Vector2(visualisers[i].Size.X, 5f + _fft[i] / 5000000);
            }
        }
    }
}