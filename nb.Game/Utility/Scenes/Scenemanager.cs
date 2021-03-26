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
        public static void AddToScene(BaseObject gameObject, string sceneName) {
            var _scene = GetScene(sceneName);
            _scene.gameObjects.Add(gameObject);
        }
        public static void AddToScene(BaseObject[] gameObjects, string sceneName) {
            var _scene = GetScene(sceneName);
            _scene.gameObjects.AddRange(gameObjects);
        }

        // Getting the scene a object is located in
        public static Scene GetSceneOfObject(BaseObject gameObject) {
            return Globals.EngineGlobals.Scenes.FirstOrDefault(x => x.gameObjects.Contains(gameObject));
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
            var _scene = Globals.EngineGlobals.Scenes.FirstOrDefault(x => x.sceneName == name);
            // Check if the value is a default value
            if (EqualityComparer<Scene>.Default.Equals(_scene, default(Scene))) {
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