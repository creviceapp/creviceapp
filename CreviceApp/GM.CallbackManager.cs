using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.GestureMachine
{
    using System.Threading;
    using Crevice.Logging;
    using Crevice.Core.FSM;
    using Crevice.Core.Callback;
    using Crevice.UserScript.Keys;
    using Crevice.Threading;
    using Crevice.WinAPI.SendInput;
    using Crevice.Core.Stroke;

    public class CallbackManager : CallbackManager<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext>, IDisposable
    {
        public class ActionExecutor : IDisposable
        {
            private readonly LowLatencyScheduler _scheduler;
            private readonly TaskFactory _taskFactory;

            public ActionExecutor(string name, ThreadPriority threadPriority, int poolSize)
            {
                _scheduler = new LowLatencyScheduler($"{name}Scheduler", threadPriority, poolSize);
                _taskFactory = new TaskFactory(_scheduler);
            }

            public void Execute(Action action) => _taskFactory.StartNew(action);

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _scheduler.Dispose();
                }
            }
        }

        public class CustomCallbackContainer : CallbackContainer
        {
            private readonly ActionExecutor _actionExecutor;

            public CustomCallbackContainer(ActionExecutor actionExecutor)
            {
                _actionExecutor = actionExecutor;
            }

            protected override void Invoke(Action action) => _actionExecutor.Execute(action);
        }

        private static ActionExecutor CallbackActionExecutor => new ActionExecutor("CallbackActionExecutor", ThreadPriority.Highest, Math.Max(2, Environment.ProcessorCount / 2));

        private readonly ActionExecutor _callbackActionExecutor;

        private readonly SingleInputSender SingleInputSender = new SingleInputSender();
        
        public CallbackManager() : this(CallbackActionExecutor) {}
        public CallbackManager(ActionExecutor callbackActionExecutor) : base(new CustomCallbackContainer(callbackActionExecutor))
        {
            _callbackActionExecutor = callbackActionExecutor;
        }

        public override void OnStrokeReset()
        {
            Verbose.Print("Stroke was reset.");
            base.OnStrokeReset();
        }

        public override void OnStrokeUpdated(
            IReadOnlyList<Stroke> strokes)
        {
            var strokeString = strokes
                .Select(s => {
                    if (s.Direction == StrokeDirection.Up) return "↑";
                    if (s.Direction == StrokeDirection.Down) return "↓";
                    if (s.Direction == StrokeDirection.Left) return "←";
                    /*if (s.Direction == StrokeDirection.Right)*/ return "→"; })
                .Aggregate("", (a, b) => a + b);
            var strokePoints =
                strokes
                .Select(s => s.Points.Count.ToString())
                .Aggregate((a, b) => a + ", " + b);
            Verbose.Print("Stroke was updated; Directions={0}, Points={1}", strokeString, strokePoints);
            base.OnStrokeUpdated(strokes);
        }

        public override void OnStateChanged(
            State<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext> lastState, 
            State<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext> currentState)
        {
            Verbose.Print("State was changed; CurrentState={0}", currentState);
            base.OnStateChanged(lastState, currentState);
        }

        public override void OnGestureCancelled(
            StateN<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext> stateN)
        {
            Verbose.Print("Gesture was cancelled.");
            var systemKeys = stateN.History.Records.Select(r => r.ReleaseEvent.PhysicalKey as PhysicalSystemKey);
            Verbose.Print($"Restoring press and release event: {string.Join(", ", systemKeys)}");
            RestoreKeyPressAndReleaseEvents(systemKeys);
            base.OnGestureCancelled(stateN);
        }

        public override void OnGestureTimeout(
            StateN<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext> stateN)
        {
            Verbose.Print("Gesture was timeout.");
            var systemKeys = stateN.History.Records.Select(r => r.ReleaseEvent.PhysicalKey as PhysicalSystemKey);
            Verbose.Print($"Restoring press event: {string.Join(", ", systemKeys)}");
            RestoreKeyPressEvents(systemKeys);
            base.OnGestureTimeout(stateN);
        }

        public override void OnMachineReset(
            State<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext> state)
        {
            Verbose.Print("GestureMachine was reset; LastState={0}", state);
            base.OnMachineReset(state);
        }

        public override void OnMachineStart()
        {
            Verbose.Print("GestureMachine was started.");
            base.OnMachineStart();
        }

        public override void OnMachineStop()
        {
            Verbose.Print("GestureMachine was stopped.");
            base.OnMachineStop();
        }

        internal void RestoreKeyPressEvents(IEnumerable<PhysicalSystemKey> systemKeys)
        {
            foreach (var systemKey in systemKeys)
            {
                if (systemKey == SupportedKeys.PhysicalKeys.None)
                {

                }
                else if (systemKey == SupportedKeys.PhysicalKeys.LButton)
                {
                    SingleInputSender.LeftDown();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.RButton)
                {
                    SingleInputSender.RightDown();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.MButton)
                {
                    SingleInputSender.MiddleDown();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.XButton1)
                {
                    SingleInputSender.X1Down();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.XButton2)
                {
                    SingleInputSender.X2Down();
                }
                else
                {
                    SingleInputSender.ExtendedKeyDownWithScanCode(systemKey.KeyId);
                }
            }
        }

        internal void RestoreKeyPressAndReleaseEvents(IEnumerable<PhysicalSystemKey> systemKeys)
        {
            foreach (var systemKey in systemKeys)
            {
                if (systemKey == SupportedKeys.PhysicalKeys.None)
                {

                }
                else if (systemKey == SupportedKeys.PhysicalKeys.LButton)
                {
                    SingleInputSender.LeftClick();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.RButton)
                {
                    SingleInputSender.RightClick();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.MButton)
                {
                    SingleInputSender.MiddleClick();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.XButton1)
                {
                    SingleInputSender.X1Click();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.XButton2)
                {
                    SingleInputSender.X2Click();
                }
                else
                {
                    SingleInputSender.Multiple()
                        .ExtendedKeyDownWithScanCode(systemKey.KeyId)
                        .ExtendedKeyUpWithScanCode(systemKey.KeyId)
                        .Send();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _callbackActionExecutor.Dispose();
                _systemKeyRestorationActionExecutor.Dispose();
            }
        }
    }
}
