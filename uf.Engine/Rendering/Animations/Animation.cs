// System
using System;
using System.Collections.Generic;

// Unsigned Framework
using uf.GameObject;
using uf.Utility.Globals;

// Glide
using Glide;

namespace uf.Rendering.Animations
{
    public class Animation
    {
        public Animation(string name, IEnumerable<Keyframe> keyframes, BaseObject gameObject, bool isLooping) {
            this.keyframes.AddRange(keyframes);
            this.name = name;
            target = gameObject;
            looping = isLooping;
        }

        private readonly List<Keyframe> keyframes = new();
        private DateTime StartTime { get; set; }

        private readonly bool looping;
        private readonly string name;
        private readonly BaseObject target;
        private readonly Tweener tweener = new();
        private int currentKeyframe;
        private GameObjectData data;
        // TODO: Not completely done, there should be an option to ignore values
        // FIXME: Currently sends objects into the aether
        public void Play() {
            StartTime = DateTime.Now;
            target.Position = keyframes[currentKeyframe].Position;
            target.Rotation = keyframes[currentKeyframe].Rotation;
            target.Size = keyframes[currentKeyframe].Size;
            target.Color = keyframes[currentKeyframe].Color;
            target.Anchor = keyframes[currentKeyframe].Anchor;
            UpdateTween();
            EngineGlobals.Window.UpdateFrame += (_) => Update();
        }
        public void Stop() {
            tweener.Cancel();
        }
        private void Update() {
            tweener.Update((float)DateTime.Now.Subtract(StartTime).TotalSeconds);
            data.ApplyTo(target);
            if (currentKeyframe + 1 >= keyframes.Count) return;
            if (DateTime.Now.Subtract(StartTime) < keyframes[currentKeyframe + 1].Timing) return;
            currentKeyframe++;
            UpdateTween();
        }
        private void UpdateTween() {
            if (currentKeyframe >= keyframes.Count - 1)
                return;
            data = new GameObjectData(keyframes[currentKeyframe]);
            var _raw = new GameObjectData(keyframes[currentKeyframe + 1]);

            tweener.Tween(data, new {
                    _raw.SkewX,
                    _raw.SkewY,
                    _raw.SizeX,
                    _raw.SizeY,
                    _raw.ColorR,
                    _raw.ColorG,
                    _raw.ColorB,
                    _raw.ColorA,
                    _raw.AnchorX,
                    _raw.AnchorY,
                    _raw.Rotation,
                    _raw.PositionX,
                    _raw.PositionY
                }, 
                (float)keyframes[currentKeyframe + 1].Timing.Subtract(keyframes[currentKeyframe].Timing).TotalSeconds
            ).Repeat(looping ? -1 : 0);
        }
    }
}