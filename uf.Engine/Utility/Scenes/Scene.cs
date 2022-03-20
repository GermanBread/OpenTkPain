// System

using System.Collections.Generic;
using OpenTK.Mathematics;
using uf.GameObject;
// OpenTK

namespace uf.Utility.Scenes
{
    public class Scene
    {
        public readonly string SceneName;
        public readonly List<BaseObject> GameObjects;
        public bool IsLoaded { get; private set; }
        public Vector2 Position = Vector2.Zero;
        public Vector2 Scale = Vector2.One;
        public float Rotation = 0;
        public Scene(string sceneName, List<BaseObject> gameObjects) {
            SceneName = sceneName;
            GameObjects = gameObjects;
            IsLoaded = false;
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