// System
using System.Linq;
using System.Collections.Generic;

using nb.Game.GameObject;
using nb.Game.Utility.Lists;

namespace nb.Game.Utility.Scenes
{
    public static class SceneManager
    {
        // Adding objects to scenes
        public static void AddToScene(BaseObject GameObject, string SceneName) {
            var _scene = GetScene(SceneName);
            _scene.GameObjects.Add(GameObject);
        }
        public static void AddToScene(BaseObject[] GameObjects, string SceneName) {
            var _scene = GetScene(SceneName);
            _scene.GameObjects.AddRange(GameObjects);
        }
        
        // Removing
        public static void RemoveFromScene(BaseObject GameObject) {
            GetSceneOfObject(GameObject).GameObjects.Remove(GameObject);
        }
        public static void RemoveFromScene(BaseObject[] GameObjects) {
            foreach (var gameObject in GameObjects) {
                GetSceneOfObject(gameObject).GameObjects.Remove(gameObject);
            }
        }
        public static void RemoveFromScene(BaseObject GameObject, string SceneName) {
            GetScene(SceneName).GameObjects.Remove(GameObject);
        }
        public static void RemoveFromScene(BaseObject[] GameObjects, string SceneName) {

        }

        // Getting the scene a object is located in
        public static Scene GetSceneOfObject(BaseObject GameObject) {
            return Globals.EngineGlobals.Scenes.FirstOrDefault(x => x.GameObjects.Contains(GameObject));
        }
        
        // Adding scenes
        public static void AddScene(Scene sceneObject) {
            Globals.EngineGlobals.Scenes.Add(sceneObject);
        }

        // Removing scenes
        public static void RemoveScene(string name) {
            Globals.EngineGlobals.Scenes.Remove(GetScene(name));
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
            _scene.Unload();
        }
        public static void LoadScene(string name) {
            var _scene = GetScene(name);
            _scene.Load();
        }
    }
}