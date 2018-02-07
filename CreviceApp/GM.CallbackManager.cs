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

    public class CustomCallbackManager : CallbackManager<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext>
    {
        private readonly TaskFactory SystemKeyRestorationTaskFactory = Task.Factory;

        private readonly SingleInputSender SingleInputSender = new SingleInputSender();

        public override void OnGestureCancelled(
            StateN<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext> stateN)
        {
            Verbose.Print("GestureCancelled");
            var systemKey = stateN.NormalEndTrigger.PhysicalKey as PhysicalSystemKey;
            ExecuteInBackground(SystemKeyRestorationTaskFactory, RestoreKeyPressAndReleaseEvent(systemKey));
            base.OnGestureCancelled(stateN);
        }

        public override void OnGestureTimeout(
            StateN<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext> stateN)
        {
            Verbose.Print("GestureTimeout");
            var systemKey = stateN.NormalEndTrigger.PhysicalKey as PhysicalSystemKey;
            ExecuteInBackground(SystemKeyRestorationTaskFactory, RestoreKeyPressEvent(systemKey));
            base.OnGestureTimeout(stateN);
        }

        public override void OnMachineReset(
            IState state)
        {
            Verbose.Print("MachineReset");
            base.OnMachineReset(state);
        }

        protected internal void ExecuteInBackground(TaskFactory taskFactory, Action action)
            => taskFactory.StartNew(action);

        internal Action RestoreKeyPressEvent(PhysicalSystemKey systemKey)
        {
            return () =>
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
                    SingleInputSender.ExtendedKeyDownWithScanCode((ushort)systemKey.KeyId);
                }
            };
        }

        internal Action RestoreKeyPressAndReleaseEvent(PhysicalSystemKey systemKey)
        {
            return () =>
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
                        .ExtendedKeyDownWithScanCode((ushort)systemKey.KeyId)
                        .ExtendedKeyUpWithScanCode((ushort)systemKey.KeyId)
                        .Send();
                }
            };
        }
    }
}
