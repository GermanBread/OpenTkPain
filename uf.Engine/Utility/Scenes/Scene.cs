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
        public Scene(string sceneName, List<BaseObject> gameObjects) {
            this.SceneName = sceneName;
            this.GameObjects = gameObjects;
            this.IsLoaded = false;
        }
        public void Load() {
            if (IsLoaded)
                return;
            GameObjects.ForEach(x => {
                if (!x.IsInitialized)
                    x.Init();
            });
            IsLoaded = true;
        }
        public void Unload() {
            if (!IsLoaded)
                return;
            GameObjects.ForEach(x => {
                if (x.IsInitialized)
                    x.Dispose();
            });
            IsLoaded = false;
        }
    }
}