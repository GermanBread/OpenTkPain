using System.Drawing;
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
        public Animation(string Name, Keyframe[] Keyframes, BaseObject GameObject, bool IsLooping) {
            this.Name = Name;
            this.Keyframes.AddRange(Keyframes);
            target = GameObject;
            looping = IsLooping;
        }
        public List<Keyframe> Keyframes = new();
        public readonly string Name;
        public DateTime StartTime { get => startTime; }

        private DateTime startTime;
        private readonly bool looping = false;
        private readonly BaseObject target;
        private readonly Tweener tweener = new();
        private int currentKeyframe = 0;
        private GameObjectdata data;
        // TODO: Not completely done, there should be an option to ignore values
        // FIXME: Currently sends objects into the aether
        public void Play() {
            startTime = DateTime.Now;
            target.Position = Keyframes[currentKeyframe].Position;
            target.Rotation = Keyframes[currentKeyframe].Rotation;
            target.Size = Keyframes[currentKeyframe].Size;
            target.Color = Keyframes[currentKeyframe].Color;
            target.Anchor = Keyframes[currentKeyframe].Anchor;
            updateTween();
            EngineGlobals.Window.UpdateFrame += (_) => update();
        }
        public void Stop() {
            tweener.Cancel();
        }
        private void update() {
            tweener.Update((float)DateTime.Now.Subtract(startTime).TotalSeconds);
            data.ApplyTo(target);
            if (currentKeyframe + 1 < Keyframes.Count)
                if (DateTime.Now.Subtract(startTime) >= Keyframes[currentKeyframe + 1].Timing) {
                    currentKeyframe++;
                    updateTween();
                }
        }
        private void updateTween() {
            if (currentKeyframe >= Keyframes.Count - 1)
                return;
            data = new GameObjectdata(Keyframes[currentKeyframe]);
            var _raw = new GameObjectdata(Keyframes[currentKeyframe + 1]);

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
                (float)Keyframes[currentKeyframe + 1].Timing.Subtract(Keyframes[currentKeyframe].Timing).TotalSeconds
            ).Repeat(looping ? -1 : 0);
        }
    }
}