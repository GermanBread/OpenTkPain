// System
using System;
using System.Collections.Generic;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

// New Beginnings
using nb.Game.GameObject;
using nb.Game.Utility.Scenes;
using nb.Game.Utility.Logging;
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
        float counter = 0;
        public void Init() => FillColor = Color4.Aqua;

        public void Update() {
            counter += FrameDelta;
            first.Skew = new Vector2(MathF.Sin(counter * 10) * 50f);
            second.Position = new Vector2(MathF.Sin(counter * 5) * 20f - 20f, 0f);
            second.Rotation += FrameDelta;
            third.Position = new Vector2(0f, MathF.Sin(counter * 3) * 50f + 80f);
            third.Rotation += MathF.Sin(counter) * FrameDelta;
        }
    }
}