using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Crevice4Tests
{
    using Crevice.Core.Context;
    using Crevice.Core.DSL;
    using Crevice.Core.Events;
    using Crevice.UserScript.Keys;
    using AppEvaluationContext = Crevice.GestureMachine.EvaluationContext;
    using AppExecutionContext = Crevice.GestureMachine.ExecutionContext;
    using AppRootElement = Crevice.DSL.RootElement;

    internal sealed class GestureCoverageCase
    {
        public string Signature { get; set; }
    }

    internal static class GestureCoverageHarness
    {
        public static List<GestureCoverageCase> Enumerate(AppRootElement root)
        {
            var cases = new List<GestureCoverageCase>();
            for (var i = 0; i < root.WhenElements.Count; i++)
            {
                EnumerateWhen(root.WhenElements[i], "When[" + i + "]", cases);
            }
            return cases;
        }

        private static void EnumerateWhen(
            IReadOnlyWhenElement<AppEvaluationContext, AppExecutionContext> when,
            string path,
            List<GestureCoverageCase> cases)
        {
            EnumerateSingleThrowElements(when.SingleThrowElements, path, cases);
            EnumerateDoubleThrowElements(when.DoubleThrowElements, path, cases);
            EnumerateDecomposedElements(when.DecomposedElements, path, cases);
        }

        private static void EnumerateSingleThrowElements(
            IReadOnlyList<IReadOnlySingleThrowElement<AppExecutionContext>> elements,
            string path,
            List<GestureCoverageCase> cases)
        {
            foreach (var element in elements.Where(e => e.IsFull))
            {
                var elementPath = path + " > On(" + element.Trigger.LogicalKey + ")";
                AddExecutors(elementPath, "Do", element.DoExecutors, cases);
            }
        }

        private static void EnumerateDoubleThrowElements(
            IReadOnlyList<IReadOnlyDoubleThrowElement<AppExecutionContext>> elements,
            string path,
            List<GestureCoverageCase> cases)
        {
            foreach (var element in elements.Where(e => e.IsFull))
            {
                var elementPath = path + " > On(" + element.Trigger.LogicalKey + ")";
                AddExecutors(elementPath, "Press", element.PressExecutors, cases);
                AddExecutors(elementPath, "Do", element.DoExecutors, cases);
                AddExecutors(elementPath, "Release", element.ReleaseExecutors, cases);
                EnumerateSingleThrowElements(element.SingleThrowElements, elementPath, cases);
                EnumerateDoubleThrowElements(element.DoubleThrowElements, elementPath, cases);
                EnumerateDecomposedElements(element.DecomposedElements, elementPath, cases);
                EnumerateStrokeElements(element.StrokeElements, elementPath, cases);
            }
        }

        private static void EnumerateDecomposedElements(
            IReadOnlyList<IReadOnlyDecomposedElement<AppExecutionContext>> elements,
            string path,
            List<GestureCoverageCase> cases)
        {
            foreach (var element in elements.Where(e => e.IsFull))
            {
                var elementPath = path + " > OnDecomposed(" + element.Trigger.LogicalKey + ")";
                AddExecutors(elementPath, "Press", element.PressExecutors, cases);
                AddExecutors(elementPath, "Release", element.ReleaseExecutors, cases);
            }
        }

        private static void EnumerateStrokeElements(
            IReadOnlyList<IReadOnlyStrokeElement<AppExecutionContext>> elements,
            string path,
            List<GestureCoverageCase> cases)
        {
            foreach (var element in elements.Where(e => e.IsFull))
            {
                var elementPath = path + " > Stroke(" + element.Strokes + ")";
                AddExecutors(elementPath, "Do", element.DoExecutors, cases);
            }
        }

        private static void AddExecutors(
            string path,
            string executorName,
            IReadOnlyList<Executor<AppExecutionContext>> executors,
            List<GestureCoverageCase> cases)
        {
            for (var i = 0; i < executors.Count; i++)
            {
                cases.Add(new GestureCoverageCase { Signature = path + " > " + executorName + "[" + i + "]" });
            }
        }
    }

    internal sealed class ExecutionRecorder
    {
        private readonly List<string> labels = new List<string>();

        public List<string> Labels => labels.ToList();

        public int TotalCount => labels.Count;

        public void Record(string label)
        {
            labels.Add(label);
        }

        public int Count(string label)
        {
            return labels.Count(x => x == label);
        }
    }

    internal sealed class GestureReplayResult
    {
        public bool PressConsumed { get; set; }
        public bool TriggerConsumed { get; set; }
        public bool ReleaseConsumed { get; set; }
        public bool StrokeObservedBeforeRelease { get; set; }
        public string ObservedStroke { get; set; }
    }

    internal static class GestureReplayHarness
    {
        private static readonly System.TimeSpan StrokeProcessingTimeout = System.TimeSpan.FromSeconds(5);

        public static GestureReplayResult ReplayWheelGesture(
            ReplayGestureMachine gestureMachine,
            DefaultGesture gesture)
        {
            var point = new Point(400, 400);
            var result = new GestureReplayResult
            {
                PressConsumed = gestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent, point),
                TriggerConsumed = gestureMachine.Input(gesture.WheelKey.FireEvent, point),
                ReleaseConsumed = gestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.ReleaseEvent, point),
            };

            return result;
        }

        public static GestureReplayResult ReplayStrokeGesture(
            ReplayGestureMachine gestureMachine,
            DefaultGesture gesture)
            => ReplayStrokeScenario(gestureMachine, MotionPatternGenerator.Generate(gesture), waitForStrokeBeforeRelease: true);

        public static GestureReplayResult ReplayBelowThresholdMovement(ReplayGestureMachine gestureMachine)
            => ReplayStrokeScenario(gestureMachine, MotionPatternGenerator.GenerateBelowThresholdMovement(), waitForStrokeBeforeRelease: false);

        public static GestureReplayResult ReplayStrokeScenario(
            ReplayGestureMachine gestureMachine,
            MotionScenario scenario,
            bool waitForStrokeBeforeRelease)
        {
            var result = new GestureReplayResult();
            var nullEvent = new NullEvent();

            result.PressConsumed = gestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent, scenario.Points.First());

            foreach (var point in scenario.Points.Skip(1))
            {
                gestureMachine.Input(nullEvent, point);
            }

            if (waitForStrokeBeforeRelease)
            {
                var expected = GestureStrokeParser.ParseStrokeSequence(scenario.ExpectedStroke);
                result.StrokeObservedBeforeRelease = System.Threading.SpinWait.SpinUntil(
                    () => gestureMachine.StrokeWatcher.GetStrokeSequence().Equals(expected),
                    StrokeProcessingTimeout);
            }
            else
            {
                System.Threading.SpinWait.SpinUntil(
                    () => gestureMachine.StrokeWatcher.GetBufferedPoints().Count >= System.Math.Max(0, scenario.Points.Count - 1),
                    StrokeProcessingTimeout);
                result.StrokeObservedBeforeRelease = false;
            }

            result.ObservedStroke = gestureMachine.StrokeWatcher.GetStrokeSequence().ToString();
            result.ReleaseConsumed = gestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.ReleaseEvent, scenario.Points.Last());
            return result;
        }
    }

    internal static class GestureReplayTestFactory
    {
        public static ReplayGestureMachine CreateReplayGestureMachine(
            ExecutionRecorder recorder,
            IEnumerable<DefaultGesture> gestures,
            bool whenResult = true)
        {
            var root = new RootElement<ReplayEvaluationContext, ReplayExecutionContext>();
            var rightButton = root.When(ctx => whenResult).On(SupportedKeys.Keys.RButton);

            foreach (var gesture in gestures)
            {
                var captured = gesture;
                if (captured.Kind == DefaultGestureKind.Wheel)
                {
                    rightButton.On(captured.WheelKey).Do(ctx => recorder.Record(captured.Label), captured.Label);
                }
                else
                {
                    rightButton.On(GestureStrokeParser.ParseStrokeSequence(captured.Label).ToArray()).Do(ctx => recorder.Record(captured.Label), captured.Label);
                }
            }

            var config = new ReplayGestureMachineConfig
            {
                GestureTimeout = 0,
                StrokeWatchInterval = 0,
            };
            var callbackManager = new ReplayCallbackManager();
            var gestureMachine = new ReplayGestureMachine(config, callbackManager);
            gestureMachine.Run(root);
            return gestureMachine;
        }
    }

    internal sealed class ReplayEvaluationContext : EvaluationContext
    { }

    internal sealed class ReplayExecutionContext : ExecutionContext
    { }

    internal sealed class ReplayContextManager : ContextManager<ReplayEvaluationContext, ReplayExecutionContext>
    {
        public override ReplayEvaluationContext CreateEvaluateContext()
            => new ReplayEvaluationContext();

        public override ReplayExecutionContext CreateExecutionContext(ReplayEvaluationContext evaluationContext)
            => new ReplayExecutionContext();
    }

    internal sealed class ReplayGestureMachineConfig : Crevice.Core.FSM.GestureMachineConfig
    { }

    internal sealed class ReplayCallbackManager : Crevice.Core.Callback.CallbackManager<ReplayGestureMachineConfig, ReplayContextManager, ReplayEvaluationContext, ReplayExecutionContext>
    { }

    internal sealed class ReplayGestureMachine : Crevice.Core.FSM.GestureMachine<ReplayGestureMachineConfig, ReplayContextManager, ReplayEvaluationContext, ReplayExecutionContext>
    {
        public ReplayGestureMachine(ReplayGestureMachineConfig config, ReplayCallbackManager callbackManager)
            : base(config, callbackManager, new ReplayContextManager())
        { }

        protected internal override TaskFactory StrokeWatcherTaskFactory => Task.Factory;
    }
}
