// System
using System;
using System.Linq;
using System.Collections.Generic;

// OpenTK
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using uf.GameObject;
using uf.Utility.Globals;
using uf.Rendering.Shaders;

namespace uf.Utility.Input
{
    public static class InputManager
    {
        private static readonly List<string> sceneBlackList = new() { "overlay" };
        public static BaseObject HoveredObject { get; private set; }
        public static void PerformMultipassRender(IEnumerable<BaseObject> objects) {
            var _filtered = objects.Where(x => !sceneBlackList.Contains(x.Scene.SceneName)).ToList();
            Shader.MultipassShader.Use();
            _filtered.ForEach(x => {
                var _index = _filtered.IndexOf(x) + 1;
                Color4 _color = new(
                    (byte)(_index % byte.MaxValue),
                    (byte)((_index / byte.MaxValue) % byte.MaxValue),
                    (byte)((_index / (int)Math.Pow(byte.MaxValue, 2)) % byte.MaxValue),
                    (byte)((_index / (int)Math.Pow(byte.MaxValue, 3)) % byte.MaxValue)
                );
                x.MultiPassDraw(_color);
            });
            
            var (_i, _y) = (Vector2i)EngineGlobals.Window.MousePosition;
            var _data = new byte[4]; // RGBA
            GL.ReadPixels(_i, EngineGlobals.Window.Size.Y - _y, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, _data);
            var _reassembledData = 0;
            _reassembledData += _data[0];
            _reassembledData += _data[1] * byte.MaxValue;
            _reassembledData += _data[2] * (int)Math.Pow(byte.MaxValue, 2);
            _reassembledData += _data[3] * (int)Math.Pow(byte.MaxValue, 3);
            HoveredObject = null;
            if (_reassembledData <= 0) return;
            var _candidate = _filtered[_reassembledData - 1];
            if (_candidate.IsHoverable)
                HoveredObject = _candidate;
        }
        // FIXME: Use events instead and cache those
        public static MouseButton? PressedMouseButton { get {
            MouseButton? _button = null;
            for (var i = 0; i < 8; i++)
            {
                if (!EngineGlobals.Window.MouseState.IsButtonDown((MouseButton) i)) continue;
                _button = (MouseButton)i;
                break;
            }
            return _button;
        } }
        public static Keys? PressedKey { get {
            Keys? _key = null;
            for (var i = 0; i < 349; i++)
            {
                if (!EngineGlobals.Window.KeyboardState.IsKeyDown((Keys) i)) continue;
                _key = (Keys)i;
                break;
            }
            return _key;
        } }
    }
}