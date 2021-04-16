// System
using System;
using System.Linq;
using System.Collections.Generic;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;

using nb.Game.Rendering;
using nb.Game.GameObject;
using nb.Game.Utility.Scenes;
using nb.Game.Utility.Globals;
using nb.Game.Rendering.Shaders;

namespace nb.Game.Utility.Input
{
    public class InputManager
    {
        public static List<string> SceneBlackList = new() { "ui" };
        public static BaseObject HoveredObject;
        public static void PerformMultipassRender() {
            List<Scene> _scenes = EngineGlobals.Scenes.Where(x => !SceneBlackList.Contains(x.SceneName)).ToList();
            List<BaseObject> _objects = new();
            _scenes.ForEach(x
             => _objects.AddRange(x.GameObjects));
            
            _objects.Sort((x1, x2) => x1.Layer.CompareTo(x2.Layer));
            Shader.MultipassShader.Use();
            _objects.ForEach(x => {
                int _index = _objects.IndexOf(x) + 1;
                Color4 _color = new Color4(
                    (byte)(_index % byte.MaxValue),
                    (byte)((_index / byte.MaxValue) % byte.MaxValue),
                    (byte)((_index / (int)Math.Pow(byte.MaxValue, 2)) % byte.MaxValue),
                    (byte)((_index / (int)Math.Pow(byte.MaxValue, 3)) % byte.MaxValue)
                );
                x.MultipassDraw(_color);
            });
            
            Vector2i _mousePos = (Vector2i)EngineGlobals.Window.MousePosition;
            byte[] _data = new byte[4]; // RGBA
            GL.ReadPixels(_mousePos.X, EngineGlobals.Window.Size.Y - _mousePos.Y, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, _data);
            int _reassembledData = 0;
            _reassembledData += _data[0];
            _reassembledData += _data[1] * byte.MaxValue;
            _reassembledData += _data[2] * (int)Math.Pow(byte.MaxValue, 2);
            _reassembledData += _data[3] * (int)Math.Pow(byte.MaxValue, 3);
            if (_reassembledData > 0)
                HoveredObject = _objects[_reassembledData - 1];
            else
                HoveredObject = null;
        }
    }
}