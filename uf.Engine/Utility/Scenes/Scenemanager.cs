// System
using System.Linq;
using System.Collections.Generic;

using uf.GameObject;
using uf.Utility.Globals;

namespace uf.Utility.Scenes
{
    public static class SceneManager
    {
        // Adding objects to scenes
        public static Scene AddToScene(BaseObject GameObject, string SceneName) {
            var _scene = GetScene(SceneName);
            _scene.GameObjects.Add(GameObject);
            EngineGlobals.Window?.InvalidateObjectsCache();
            return _scene;
        }
        public static Scene AddToScene(BaseObject[] GameObjects, string SceneName) {
            var _scene = GetScene(SceneName);
            _scene.GameObjects.AddRange(GameObjects);
            EngineGlobals.Window?.InvalidateObjectsCache();
            return _scene;
        }
        
        // Removing
        public static Scene RemoveFromScene(BaseObject GameObject) {
            var _scene = GetSceneOfObject(GameObject);
            _scene.GameObjects.Remove(GameObject);
            EngineGlobals.Window?.InvalidateObjectsCache();
            return _scene;
        }
        public static Dictionary<BaseObject, Scene> RemoveFromScene(BaseObject[] GameObjects) {
            Dictionary<BaseObject, Scene> _dict = new();
            foreach (var gameObject in GameObjects) {
                var _scene = GetSceneOfObject(gameObject);
                _scene.GameObjects.Remove(gameObject);
                _dict.Add(gameObject, _scene);
            }
            EngineGlobals.Window?.InvalidateObjectsCache();
            return _dict;
        }
        public static Scene RemoveFromScene(BaseObject GameObject, string SceneName) {
            var _scene = GetScene(SceneName);
            _scene.GameObjects.Remove(GameObject);
            EngineGlobals.Window?.InvalidateObjectsCache();
            return _scene;
        }
        public static Dictionary<BaseObject, Scene> RemoveFromScene(BaseObject[] GameObjects, string SceneName) {
            Dictionary<BaseObject, Scene> _dict = new();
            foreach (var gameObject in GameObjects) {
                var _scene = GetScene(SceneName);
                _scene.GameObjects.Remove(gameObject);
                _dict.Add(gameObject, _scene);
            }
            EngineGlobals.Window?.InvalidateObjectsCache();
            return _dict;
        }

        // Getting the scene a object is located in
        public static Scene GetSceneOfObject(BaseObject GameObject) {
            return Globals.EngineGlobals.Scenes.FirstOrDefault(x => x.GameObjects.Contains(GameObject));
        }
        
        // Adding scenes
        public static void AddScene(Scene sceneObject) {
            Globals.EngineGlobals.Scenes.Add(sceneObject);
            EngineGlobals.Window?.InvalidateObjectsCache();
        }

        // Removing scenes
        public static void RemoveScene(string name) {
            Globals.EngineGlobals.Scenes.Remove(GetScene(name));
            EngineGlobals.Window?.InvalidateObjectsCache();
        }

        // Retrieving Scenes
        public static Scene GetScene(string name) {
            var _scene = Globals.EngineGlobals.Scenes.FirstOrDefault(x => x.SceneName == name);
            // Check if the value is a default value
            if (_scene == default(Scene)) {
                _scene = new Scene(name, new List<BaseObject>());
                // This scene dies not exist in the list yet. So add it now.
                AddScene(_scene);
            }
            return _scene;
        }

        // Actual scene management
        public static void UnloadScene(string name) {
            var _scene = GetScene(name);
            if (!_scene.IsLoaded)
                return;
            _scene.Unload();
            EngineGlobals.Window?.InvalidateObjectsCache();
        }
        public static void LoadScene(string name) {
            var _scene = GetScene(name);
            if (_scene.IsLoaded)
                return;
            //_scene.GameObjects.Sort((val1, val2) => val1.Layer.CompareTo(val2.Layer));
            _scene.GameObjects.ForEach(x => {
                if (!x.IsInitialized)
                    // FIXME: That's great and all, but what do we do if an object gets created after the scene loaded?
                    x.Init();
            });
            _scene.Load();
            EngineGlobals.Window?.InvalidateObjectsCache();
        }
    }
}