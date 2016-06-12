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
            int strokeDirectionChangeThreshold,
            int strokeExtensionThreshold,
            Def.Direction dir)
        {
            this.Direction = dir;
            this.strokeDirectionChangeThreshold = strokeDirectionChangeThreshold;
            this.strokeExtensionThreshold = strokeExtensionThreshold;
        }

        public Stroke(
            int strokeDirectionChangeThreshold,
            int strokeExtensionThreshold,
            List<LowLevelMouseHook.POINT> input)
        {
            this.Direction = NextDirection(GetAngle(input.First(), input.Last()));
            this.strokeDirectionChangeThreshold = strokeDirectionChangeThreshold;
            this.strokeExtensionThreshold = strokeExtensionThreshold;
            Absorb(input);
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
                var dir = NextDirection(angle);
                if (IsSameDirection(dir))
                {
                    Absorb(input);
                    return this;
                }
                var stroke = CreateNew(dir);
                stroke.Absorb(input);
                return stroke;
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

        private bool IsSameDirection(Def.Direction dir)
        {
            return dir == Direction;
        }

        private bool IsExtensionable(double angle)
        {
            return Direction == NextDirection(angle);
        }

        private Stroke CreateNew(Def.Direction dir)
        {
            return new Stroke(strokeDirectionChangeThreshold, strokeExtensionThreshold, dir);
        }

        public static bool CanCreate(int initialStrokeThreshold, LowLevelMouseHook.POINT p0, LowLevelMouseHook.POINT p1)
        {
            var dx = Math.Abs(p0.x - p1.x);
            var dy = Math.Abs(p0.y - p1.y);
            if (dx > initialStrokeThreshold || dy > initialStrokeThreshold)
            {
                return true;
            }
            return false;
        }

        private static double GetAngle(LowLevelMouseHook.POINT p0, LowLevelMouseHook.POINT p1)
        {
            return Math.Atan2(p1.y - p0.y, p1.x - p0.x) * 180 / Math.PI;
        }
    }
}
