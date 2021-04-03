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
using nb.Game.Utility.Resources;
using nb.Game.GameObject.Components;

namespace nb.Game
{
    public class Game : BaseGame
    {
        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        Rectangle first = new Rectangle {
            Size = new Vector2(250),
            Anchor = Anchor.TopRight
        };
        Triangle second = new Triangle {
            Size = new Vector2(150f),
            Anchor = Anchor.BottomRight
        };
        Rectangle third = new Rectangle {
            Position = new Vector2(0, 80f),
            Size = new Vector2(50f),
            Anchor = Anchor.Left
        };
        List<Rectangle> visualisers;
        float counter = 0;
        public void Init() {
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
        }

        public void Update() {
            counter += FrameDelta;
            first.Skew = new Vector2(MathF.Sin(counter * 10) * 20f);
            second.Position = new Vector2(MathF.Sin(counter * 5) * 20f - 20f, 0f);
            second.Rotation += FrameDelta;
            third.Position = new Vector2(0f, MathF.Sin(counter * 3) * 50f + 80f);
            FillColor = Color4.FromHsv(new Vector4(counter / 5f % 1, 1f, 1f, 1f));

            var _clip = AudioManager.GetClip("big chungus");
            var _data = _clip.GetWaveform(visualisers.Count);
            var _fft = _data.Item1;
            for (int i = 0; i < _fft.Length; i++) {
                visualisers[i].Size = new Vector2(visualisers[i].Size.X, 5f + _fft[i] / 5000000);
                visualisers[i].Color = Color4.FromHsv(new Vector4((float)i / _fft.Length % 1, 1, 1, 1));
            }
        }
    }
}