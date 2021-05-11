// System
using System;
using System.Collections.Generic;

// OpenTK
using OpenTK.Mathematics;

using uf.GameObject;

namespace uf.Utility.Scenes
{
    public class Scene
    {
        public string SceneName;
        public List<BaseObject> GameObjects;
        public bool IsLoaded { get; private set; }
        public Vector2 Position = Vector2.Zero;
        public Vector2 Scale = Vector2.One;
        public float Rotation = 0;
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