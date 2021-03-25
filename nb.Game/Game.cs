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

        Rectangle rectangle = new Rectangle("default") {
            rotation = 0,
            size = new Vector2(1.5f)
        };

        public void load() {
            Logger.Log(new LogMessage(LogSeverity.Info, "Game", "Hello world!"));

            var _testObject = SceneManager.GetScene("default").gameObjects[0];
            _testObject.SayHello();

            Logger.Log(new LogMessage(LogSeverity.Verbose, "Game", $"Test object ist located in scene \"{_testObject.scene.sceneName}\""));
        }

        public void update() {
        }
    }
}