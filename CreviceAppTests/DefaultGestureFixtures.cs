using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Crevice4Tests
{
    using Crevice.Core.Stroke;
    using Crevice.UserScript.Keys;

    internal enum DefaultGestureKind
    {
        Wheel,
        Stroke,
    }

    internal sealed class DefaultGesture
    {
        public DefaultGestureKind Kind { get; private set; }
        public string Label { get; private set; }
        public string Signature { get; private set; }
        public Crevice.Core.Keys.PhysicalSingleThrowKey WheelKey { get; private set; }

        public static DefaultGesture Wheel(
            string label,
            Crevice.Core.Keys.PhysicalSingleThrowKey wheelKey,
            string signature)
            => new DefaultGesture
            {
                Kind = DefaultGestureKind.Wheel,
                Label = label,
                WheelKey = wheelKey,
                Signature = signature,
            };

        public static DefaultGesture Stroke(string label, string signature)
            => new DefaultGesture
            {
                Kind = DefaultGestureKind.Stroke,
                Label = label,
                Signature = signature,
            };
    }

    internal static class ExpectedDefaultGestures
    {
        public static readonly DefaultGesture WheelUp = DefaultGesture.Wheel(
            "WheelUp",
            SupportedKeys.PhysicalKeys.WheelUp,
            "When[0] > On(Keys.RButton) > On(Keys.WheelUp) > Do[0]");

        public static readonly DefaultGesture WheelDown = DefaultGesture.Wheel(
            "WheelDown",
            SupportedKeys.PhysicalKeys.WheelDown,
            "When[0] > On(Keys.RButton) > On(Keys.WheelDown) > Do[0]");

        public static readonly DefaultGesture StrokeUp = DefaultGesture.Stroke(
            "U",
            "When[0] > On(Keys.RButton) > Stroke(U) > Do[0]");

        public static readonly IReadOnlyList<DefaultGesture> Wheel = new[]
        {
            WheelUp,
            WheelDown,
        };

        public static readonly IReadOnlyList<DefaultGesture> Stroke = new[]
        {
            StrokeUp,
            DefaultGesture.Stroke("D", "When[0] > On(Keys.RButton) > Stroke(D) > Do[0]"),
            DefaultGesture.Stroke("L", "When[0] > On(Keys.RButton) > Stroke(L) > Do[0]"),
            DefaultGesture.Stroke("R", "When[0] > On(Keys.RButton) > Stroke(R) > Do[0]"),
            DefaultGesture.Stroke("UD", "When[0] > On(Keys.RButton) > Stroke(UD) > Do[0]"),
            DefaultGesture.Stroke("DR", "When[0] > On(Keys.RButton) > Stroke(DR) > Do[0]"),
        };

        public static readonly IReadOnlyList<DefaultGesture> All = Wheel.Concat(Stroke).ToList();
    }

    internal sealed class MotionScenario
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public string ExpectedStroke { get; set; }
        public Point Start { get; set; }
        public int Distance { get; set; }
        public int Subdivisions { get; set; }
        public int JitterAmplitude { get; set; }
        public List<Point> Points { get; set; }
    }

    internal static class MotionPatternGenerator
    {
        public static readonly IReadOnlyList<Point> StartCoordinates = new[]
        {
            new Point(400, 400),
            new Point(2600, 540),
            new Point(-800, 360),
        };

        public static readonly IReadOnlyList<int> AboveThresholdDistances = new[]
        {
            32,
            48,
            72,
        };

        public static readonly IReadOnlyList<int> PointDensities = new[]
        {
            2,
            4,
            7,
        };

        public static readonly IReadOnlyList<int> JitterAmplitudes = new[]
        {
            0,
            3,
        };

        public static MotionScenario Generate(DefaultGesture gesture)
            => new MotionScenario
            {
                Id = "baseline-" + gesture.Label,
                Category = "positive-baseline",
                ExpectedStroke = gesture.Label,
                Start = new Point(400, 400),
                Distance = 64,
                Subdivisions = 4,
                JitterAmplitude = 0,
                Points = GeneratePoints(new Point(400, 400), GestureStrokeParser.ParseStrokeSequence(gesture.Label), 64, 4, 0),
            };

        public static MotionScenario GenerateBelowThresholdMovement()
            => new MotionScenario
            {
                Id = "baseline-below-threshold",
                Category = "negative-below-threshold",
                ExpectedStroke = "",
                Start = new Point(400, 400),
                Distance = 10,
                Subdivisions = 3,
                JitterAmplitude = 0,
                Points = new List<Point>
                {
                    new Point(400, 400),
                    new Point(405, 400),
                    new Point(405, 405),
                },
            };

        public static IEnumerable<MotionScenario> GeneratePositiveStrokeScenarios()
        {
            foreach (var gesture in ExpectedDefaultGestures.Stroke)
            {
                foreach (var start in StartCoordinates)
                {
                    foreach (var distance in AboveThresholdDistances)
                    {
                        foreach (var subdivisions in PointDensities)
                        {
                            foreach (var jitterAmplitude in JitterAmplitudes)
                            {
                                yield return CreateScenario(
                                    "positive",
                                    "positive-" + gesture.Label + "-start-" + start.X + "-" + start.Y + "-distance-" + distance + "-density-" + subdivisions + "-jitter-" + jitterAmplitude,
                                    gesture.Label,
                                    start,
                                    distance,
                                    subdivisions,
                                    jitterAmplitude);
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<MotionScenario> GenerateNegativeStrokeScenarios()
        {
            yield return GenerateBelowThresholdMovement();
            yield return CreateScenario("negative-unknown", "negative-unknown-RL", "RL", new Point(400, 400), 64, 4, 0);
            yield return CreateScenario("negative-unknown", "negative-unknown-LR", "LR", new Point(2600, 540), 64, 4, 3);
            yield return CreateScenario("negative-noisy", "negative-noisy-URDL", "URDL", new Point(-800, 360), 48, 6, 4);
        }

        private static MotionScenario CreateScenario(
            string category,
            string id,
            string expectedStroke,
            Point start,
            int distance,
            int subdivisions,
            int jitterAmplitude)
            => new MotionScenario
            {
                Id = id,
                Category = category,
                ExpectedStroke = expectedStroke,
                Start = start,
                Distance = distance,
                Subdivisions = subdivisions,
                JitterAmplitude = jitterAmplitude,
                Points = GeneratePoints(start, GestureStrokeParser.ParseStrokeSequence(expectedStroke), distance, subdivisions, jitterAmplitude),
            };

        private static List<Point> GeneratePoints(
            Point start,
            StrokeSequence strokes,
            int distance,
            int subdivisions,
            int jitterAmplitude)
        {
            var points = new List<Point> { start };
            var current = start;
            foreach (var stroke in strokes)
            {
                var segmentStart = current;
                for (var step = 1; step <= subdivisions; step++)
                {
                    var along = (int)Math.Round(distance * step / (double)subdivisions);
                    var jitter = step == subdivisions ? 0 : ((step % 2 == 0) ? -jitterAmplitude : jitterAmplitude);
                    points.Add(Offset(segmentStart, stroke, along, jitter));
                }
                current = points.Last();
            }
            return points;
        }

        private static Point Offset(Point point, StrokeDirection direction, int distance, int orthogonalJitter)
        {
            switch (direction)
            {
                case StrokeDirection.Up: return new Point(point.X + orthogonalJitter, point.Y - distance);
                case StrokeDirection.Down: return new Point(point.X + orthogonalJitter, point.Y + distance);
                case StrokeDirection.Left: return new Point(point.X - distance, point.Y + orthogonalJitter);
                case StrokeDirection.Right: return new Point(point.X + distance, point.Y + orthogonalJitter);
                default: throw new ArgumentException("Unknown stroke direction: " + direction);
            }
        }
    }

    internal static class GestureStrokeParser
    {
        public static StrokeSequence ParseStrokeSequence(string value)
        {
            return new StrokeSequence(value.Select(ParseStrokeDirection));
        }

        private static StrokeDirection ParseStrokeDirection(char value)
        {
            switch (value)
            {
                case 'U': return StrokeDirection.Up;
                case 'D': return StrokeDirection.Down;
                case 'L': return StrokeDirection.Left;
                case 'R': return StrokeDirection.Right;
                default: throw new ArgumentException("Unknown stroke direction: " + value);
            }
        }
    }
}
