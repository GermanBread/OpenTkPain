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

namespace nb.Game
{
    public class Game : BaseGame
    {
        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        Rectangle first = new Rectangle {
            Position = new Vector2(1, -.8f),
            Size = new Vector2(2, .2f)
        };
        Triangle second = new Triangle {
            Size = new Vector2(.3f),
        };
        Rectangle third = new Rectangle {
            Position = new Vector2(.8f),
            Size = new Vector2(.2f)
        };
        float counter = 0;
        public void init() => FillColor = Color4.Aqua;
        
        public void load() { }

        public void update() {
            counter += (float)FrameDelta;
            first.Size = new Vector2(1, MathF.Sin(counter) / 20f + .1f);
            first.Position = new Vector2(0, MathF.Sin(counter) / 20f - .9f);
            second.Skew = new Vector2(MathF.Sin(counter) * 2, MathF.Sin(counter * 2f));
            second.Size = new Vector2(MathF.Cos(counter / 3f) / 20f + .2f);
        }
    }
}