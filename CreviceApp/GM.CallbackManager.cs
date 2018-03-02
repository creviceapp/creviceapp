using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.GestureMachine
{
    using Crevice.Logging;
    using Crevice.Core.FSM;
    using Crevice.Core.Callback;
    using Crevice.UserScript.Keys;
    using Crevice.WinAPI.SendInput;
    using Crevice.Core.Stroke;

    public class CallbackManager : CallbackManager<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext>
    {
        private readonly TaskFactory SystemKeyRestorationTaskFactory = Task.Factory;

        private readonly SingleInputSender SingleInputSender = new SingleInputSender();

        public override void OnStrokeReset()
        {
            Verbose.Print("Stroke was reset.");
            base.OnStrokeReset();
        }

        public override void OnStrokeUpdate(
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
            base.OnStrokeUpdate(strokes);
        }

        public override void OnStateChange(
            State<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext> lastState, 
            State<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext> currentState)
        {
            Verbose.Print("State was changed; CurrentState={0}", currentState);
            base.OnStateChange(lastState, currentState);
        }

        public override void OnGestureCancel(
            StateN<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext> stateN)
        {
            Verbose.Print("Gesture was cancelled.");
            var systemKeys = stateN.History.Records.Select(r => r.ReleaseEvent.PhysicalKey as PhysicalSystemKey);
            Verbose.Print($"Restoring press and release event: {string.Join(", ", systemKeys)}");
            ExecuteInBackground(SystemKeyRestorationTaskFactory, RestoreKeyPressAndReleaseEvents(systemKeys));
            base.OnGestureCancel(stateN);
        }

        public override void OnGestureTimeout(
            StateN<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext> stateN)
        {
            Verbose.Print("Gesture was timeout.");
            var systemKeys = stateN.History.Records.Select(r => r.ReleaseEvent.PhysicalKey as PhysicalSystemKey);
            Verbose.Print($"Restoring press event: {string.Join(", ", systemKeys)}");
            ExecuteInBackground(SystemKeyRestorationTaskFactory, RestoreKeyPressEvents(systemKeys));
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

        protected internal void ExecuteInBackground(TaskFactory taskFactory, Action action)
            => taskFactory.StartNew(action);

        internal Action RestoreKeyPressEvents(IEnumerable<PhysicalSystemKey> systemKeys)
        {
            return () =>
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
            };
        }

        internal Action RestoreKeyPressAndReleaseEvents(IEnumerable<PhysicalSystemKey> systemKeys)
        {
            return () =>
            {
                foreach(var systemKey in systemKeys)
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
            };
        }
    }
}
