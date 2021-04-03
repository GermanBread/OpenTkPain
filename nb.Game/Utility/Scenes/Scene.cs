// System
using System;
using System.Collections.Generic;

using nb.Game.GameObject;

namespace nb.Game.Utility.Scenes
{
    public class Scene
    {
        public string SceneName;
        public List<BaseObject> GameObjects;
        public bool IsLoaded { get; private set; }
        public void Unload() {
            IsLoaded = false;
        }
        public void Load() {
            IsLoaded = true;
        }
        public Scene(string sceneName, List<BaseObject> gameObjects) {
            this.SceneName = sceneName;
            this.GameObjects = gameObjects;
            this.IsLoaded = false;
        }
    }
}