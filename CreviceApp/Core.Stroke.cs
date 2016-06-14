using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.Stroke
{
    public class Stroke
    {
        public readonly Def.Direction Direction;
        internal readonly int strokeDirectionChangeThreshold;
        internal readonly int strokeExtensionThreshold;

        private readonly List<Point> points = new List<Point>();

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
            List<Point> input)
        {
            this.Direction = NextDirection(GetAngle(input.First(), input.Last()));
            this.strokeDirectionChangeThreshold = strokeDirectionChangeThreshold;
            this.strokeExtensionThreshold = strokeExtensionThreshold;
            Absorb(input);
        }

        public virtual Stroke Input(List<Point> input)
        {
            var p0 = input.First();
            var p1 = input.Last();
            var dx = Math.Abs(p0.X - p1.X);
            var dy = Math.Abs(p0.Y - p1.Y);
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

        public void Absorb(List<Point> points)
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

        public static bool CanCreate(int initialStrokeThreshold, Point p0, Point p1)
        {
            var dx = Math.Abs(p0.X - p1.X);
            var dy = Math.Abs(p0.Y - p1.Y);
            if (dx > initialStrokeThreshold || dy > initialStrokeThreshold)
            {
                return true;
            }
            return false;
        }

        private static double GetAngle(Point p0, Point p1)
        {
            return Math.Atan2(p1.Y - p0.Y, p1.X - p0.X) * 180 / Math.PI;
        }
    }
}
