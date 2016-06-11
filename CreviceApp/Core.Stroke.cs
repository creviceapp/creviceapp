using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.Stroke
{
    using WinAPI.WindowsHookEx;

    public class Stroke
    {
        public readonly Def.Direction Direction;
        internal readonly int strokeDirectionChangeThreshold;
        internal readonly int strokeExtensionThreshold;

        private readonly List<LowLevelMouseHook.POINT> points = new List<LowLevelMouseHook.POINT>();

        public Stroke(
            Def.Direction dir,
            int strokeDirectionChangeThreshold,
            int strokeExtensionThreshold)
        {
            this.Direction = dir;
            this.strokeDirectionChangeThreshold = strokeDirectionChangeThreshold;
            this.strokeExtensionThreshold = strokeExtensionThreshold;
        }

        public virtual Stroke Input(List<LowLevelMouseHook.POINT> input)
        {
            var p0 = input.First();
            var p1 = input.Last();
            var dx = Math.Abs(p0.x - p1.x);
            var dy = Math.Abs(p0.y - p1.y);
            var angle = GetAngle(p0, p1);
            if (dx > strokeDirectionChangeThreshold || dy > strokeDirectionChangeThreshold)
            {
                var move = NextDirection(angle);
                if (move == Direction)
                {
                    Absorb(input);
                    return this;
                }
                return Create(move);
            }

            if (dx > strokeExtensionThreshold || dy > strokeExtensionThreshold)
            {
                if (IsExtensionable(angle))
                {
                    Absorb(input);
                }
            }
            return this;
        }

        public void Absorb(List<LowLevelMouseHook.POINT> points)
        {
            this.points.AddRange(points);
            points.Clear();
        }

        private static Def.Direction NextDirection(double angle)
        {
            if (-135 <= angle && angle < -45)
            {
                return Def.Direction.Up;
            }
            else if (-45 <= angle && angle < 45)
            {
                return Def.Direction.Right;
            }
            else if (45 <= angle && angle < 135)
            {
                return Def.Direction.Down;
            }
            else // if (135 <= angle || angle < -135)
            {
                return Def.Direction.Left;
            }
        }

        private bool IsExtensionable(double angle)
        {
            return Direction == NextDirection(angle);
        }

        private Stroke Create(Def.Direction dir)
        {
            return new Stroke(dir, strokeDirectionChangeThreshold, strokeExtensionThreshold);
        }

        public static Stroke Create(
            int initialStrokeThreshold,
            int strokeChangeThreshold,
            int strokeExtensionThreshold,
            List<LowLevelMouseHook.POINT> input)
        {
            var p0 = input.First();
            var p1 = input.Last();
            var dx = Math.Abs(p0.x - p1.x);
            var dy = Math.Abs(p0.y - p1.y);
            var angle = GetAngle(p0, p1);
            if (dx > initialStrokeThreshold || dy > initialStrokeThreshold)
            {
                var s = new Stroke(NextDirection(angle), strokeChangeThreshold, strokeExtensionThreshold);
                s.Input(input);
                return s;
            }
            return null;
        }

        private static double GetAngle(LowLevelMouseHook.POINT p0, LowLevelMouseHook.POINT p1)
        {
            return Math.Atan2(p1.y - p0.y, p1.x - p0.x) * 180 / Math.PI;
        }
    }
}
