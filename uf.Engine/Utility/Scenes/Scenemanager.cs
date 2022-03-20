// System

using System;
using System.Linq;
using System.Collections.Generic;

using uf.GameObject;
using uf.Utility.Globals;

namespace uf.Utility.Scenes
{
    public static class SceneManager
    {
        // Adding objects to scenes
        public static Scene AddToScene(BaseObject gameObject, string sceneName) {
            var _scene = GetScene(sceneName);
            _scene.GameObjects.Add(gameObject);
            EngineGlobals.Window?.InvalidateObjectsCache();
            return _scene;
        }
        public static Scene AddToScene(IEnumerable<BaseObject> gameObjects, string sceneName) {
            var _scene = GetScene(sceneName);
            _scene.GameObjects.AddRange(gameObjects);
            EngineGlobals.Window?.InvalidateObjectsCache();
            return _scene;
        }
        
        // Removing
        public static Scene RemoveFromScene(BaseObject gameObject) {
            var _scene = GetSceneOfObject(gameObject);
            _scene.GameObjects.Remove(gameObject);
            EngineGlobals.Window?.InvalidateObjectsCache();
            return _scene;
        }
        public static Dictionary<BaseObject, Scene> RemoveFromScene(IEnumerable<BaseObject> gameObjects) {
            Dictionary<BaseObject, Scene> _dict = new();
            foreach (var gameObject in gameObjects) {
                var _scene = GetSceneOfObject(gameObject);
                _scene.GameObjects.Remove(gameObject);
                _dict.Add(gameObject, _scene);
            }
            EngineGlobals.Window?.InvalidateObjectsCache();
            return _dict;
        }
        public static Scene RemoveFromScene(BaseObject gameObject, string sceneName) {
            var _scene = GetScene(sceneName);
            _scene.GameObjects.Remove(gameObject);
            EngineGlobals.Window?.InvalidateObjectsCache();
            return _scene;
        }
        public static Dictionary<BaseObject, Scene> RemoveFromScene(IEnumerable<BaseObject> gameObjects, string sceneName) {
            Dictionary<BaseObject, Scene> _dict = new();
            foreach (var gameObject in gameObjects) {
                var _scene = GetScene(sceneName);
                _scene.GameObjects.Remove(gameObject);
                _dict.Add(gameObject, _scene);
            }
            EngineGlobals.Window?.InvalidateObjectsCache();
            return _dict;
        }

        // Getting the scene a object is located in
        private static Scene GetSceneOfObject(BaseObject gameObject) {
            return EngineGlobals.Scenes.FirstOrDefault(x => x.GameObjects.Contains(gameObject));
        }
        
        // Adding scenes
        private static void AddScene(Scene sceneObject) {
            EngineGlobals.Scenes.Add(sceneObject);
            EngineGlobals.Window?.InvalidateObjectsCache();
        }

        // Removing scenes
        public static void RemoveScene(string name) {
            EngineGlobals.Scenes.Remove(GetScene(name));
            EngineGlobals.Window?.InvalidateObjectsCache();
        }

        // Retrieving Scenes
        private static Scene GetScene(string name) {
            var _scene = EngineGlobals.Scenes.FirstOrDefault(x => x.SceneName == name);
            // Check if the value is a default value
            if (_scene != default(Scene)) return _scene;
            _scene = new Scene(name, new List<BaseObject>());
            // This scene dies not exist in the list yet. So add it now.
            AddScene(_scene);
            return _scene;
        }

        // Actual scene management
        public static void UnloadScene(string name) {
            var _scene = GetScene(name);
            EngineGlobals.Window?.SceneLoadQueue.Add((_scene, SceneAction.Unload));
        }
        public static void LoadScene(string name) {
            var _scene = GetScene(name);
            // Queue the loading / unloading until the next Update tick (will then run on the main thread)
            EngineGlobals.Window?.SceneLoadQueue.Add((_scene, SceneAction.Load));
        }

        internal static void OperateOnScene((Scene, SceneAction) actionTuple)
        {
            var (_scene, _sceneAction) = actionTuple;
            OperateOnScene(_scene, _sceneAction);
        }

        private static void OperateOnScene(Scene sceneObject, SceneAction operation) {
            switch (operation)
            {
                case SceneAction.Load:
                    sceneObject.Load();
                    break;
                case SceneAction.Unload:
                    sceneObject.Unload();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }
            EngineGlobals.Window?.InvalidateObjectsCache();
        }
    }
}