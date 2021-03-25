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

        Rectangle first = new Rectangle("default") {
            Size = new Vector2(1, .2f),
            Position = new Vector2(0, -.8f)
        };
        Rectangle second = new Rectangle("default") {
            Size = new Vector2(.2f)
        };
        Triangle third = new Triangle("default") {
            Size = new Vector2(.4f),
            Position = new Vector2(.6f)
        };

        public void load() {
        }

        public void update() {
        }
    }
}