// System
using System;
using System.Collections.Generic;

using nb.Game.GameObject;

namespace nb.Game.Utility.Scenes
{
    public class Scene
    {
        public string sceneName;
        public List<BaseObject> gameObjects;
        public bool isLoaded { get; private set; }
        public void Unload() {
            isLoaded = false;
        }
        public void Load() {
            isLoaded = true;
        }
        public Scene(string sceneName, List<BaseObject> gameObjects) {
            this.sceneName = sceneName;
            this.gameObjects = gameObjects;
            this.isLoaded = false;
        }
    }
}