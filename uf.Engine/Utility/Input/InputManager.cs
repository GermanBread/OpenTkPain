// System
using System;
using System.Linq;
using System.Collections.Generic;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;

using uf.Rendering;
using uf.GameObject;
using uf.Utility.Scenes;
using uf.Utility.Globals;
using uf.Rendering.Shaders;

namespace uf.Utility.Input
{
    public class InputManager
    {
        public static List<string> SceneBlackList = new() { "overlay" };
        public static BaseObject HoveredObject { get; private set; }
        public static void PerformMultipassRender(List<BaseObject> Objects) {
            List<BaseObject> _filtered = Objects.Where(x => !SceneBlackList.Contains(x.Scene.SceneName)).ToList();
            Shader.MultipassShader.Use();
            _filtered.ForEach(x => {
                int _index = _filtered.IndexOf(x) + 1;
                Color4 _color = new(
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
            HoveredObject = null;
            if (_reassembledData > 0) {
                var _candidate = _filtered[_reassembledData - 1];
                if (_candidate.IsHoverable)
                    HoveredObject = _candidate;
            }
        }
        // FIXME: Use events instead and cache those
        public static MouseButton? PressedMouseButton { get {
            MouseButton? _button = null;
            for (int i = 0; i < 8; i++) {
                if (EngineGlobals.Window.MouseState.IsButtonDown((MouseButton)i)) {
                    _button = (MouseButton)i;
                    break;
                }
            }
            return _button;
        } }
        public static Keys? PressedKey { get {
            Keys? _key = null;
            for (int i = 0; i < 349; i++) {
                if (EngineGlobals.Window.KeyboardState.IsKeyDown((Keys)i)) {
                    _key = (Keys)i;
                    break;
                }
            }
            return _key;
        } }
    }
}