using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Stroke
{
    using System.Linq;
    using System.Drawing;

    public class Stroke
    {
        public readonly StrokeDirection Direction;
        public readonly int StrokeDirectionChangeThreshold;
        public readonly int StrokeExtensionThreshold;

        private readonly List<Point> points = new List<Point>();
        public IReadOnlyList<Point> Points => points;

        public Stroke(
            int strokeDirectionChangeThreshold,
            int strokeExtensionThreshold,
            StrokeDirection direction)
        {
            Direction = direction;
            StrokeDirectionChangeThreshold = strokeDirectionChangeThreshold;
            StrokeExtensionThreshold = strokeExtensionThreshold;
        }

        public Stroke(
            int strokeDirectionChangeThreshold,
            int strokeExtensionThreshold,
            IReadOnlyList<Point> input)
        {
            Direction = NextDirection(GetAngle(input.First(), input.Last()));
            StrokeDirectionChangeThreshold = strokeDirectionChangeThreshold;
            StrokeExtensionThreshold = strokeExtensionThreshold;
            Absorb(input);
        }

        public virtual Stroke Input(IReadOnlyList<Point> input)
        {
            var p0 = points.Count > 0 ? points.Last() : input.First();
            var p1 = input.Last();
            var dx = Math.Abs(p0.X - p1.X);
            var dy = Math.Abs(p0.Y - p1.Y);
            var angle = GetAngle(p0, p1);
            if (dx > StrokeDirectionChangeThreshold || dy > StrokeDirectionChangeThreshold)
            {
                var dir = NextDirection(angle);
                if (IsSameDirection(dir))
                {
                    Absorb(input);
                    return this;
                }
                var stroke = CreateNew(dir);
                stroke.points.Add(this.Points.Last());
                stroke.Absorb(input);
                return stroke;
            }

            if (dx > StrokeExtensionThreshold || dy > StrokeExtensionThreshold)
            {
                if (IsExtensionable(angle))
                {
                    Absorb(input);
                }
            }
            return this;
        }

        public void Absorb(IReadOnlyList<Point> points)
            => this.points.AddRange(points);

        private static StrokeDirection NextDirection(double angle)
        {
            if (-135 <= angle && angle < -45)
            {
                return StrokeDirection.Up;
            }
            else if (-45 <= angle && angle < 45)
            {
                return StrokeDirection.Right;
            }
            else if (45 <= angle && angle < 135)
            {
                return StrokeDirection.Down;
            }
            else // if (135 <= angle || angle < -135)
            {
                return StrokeDirection.Left;
            }
        }

        private bool IsSameDirection(StrokeDirection dir)
            => dir == Direction;

        private bool IsExtensionable(double angle)
            => Direction == NextDirection(angle);

        private Stroke CreateNew(StrokeDirection dir)
            => new Stroke(StrokeDirectionChangeThreshold, StrokeExtensionThreshold, dir);

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
            => Math.Atan2(p1.Y - p0.Y, p1.X - p0.X) * 180 / Math.PI;

        public Stroke Freeze()
        {
            var stroke = new Stroke(StrokeDirectionChangeThreshold, StrokeExtensionThreshold, Direction);
            stroke.Absorb(points);
            return stroke;
        }
    }
}
