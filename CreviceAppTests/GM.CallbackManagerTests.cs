using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Crevice4Tests
{
    using Crevice.DSL;
    using Crevice.GestureMachine;
    using Crevice.UserScript.Keys;
    using Crevice.WinAPI.SendInput;

    using TStateN = Crevice.Core.FSM.StateN<Crevice.GestureMachine.GestureMachineConfig, Crevice.GestureMachine.ContextManager, Crevice.GestureMachine.EvaluationContext, Crevice.GestureMachine.ExecutionContext>;

    [TestClass()]
    public class CallbackManagerTests
    {
        private const int InputMouse = 0x0;
        private const int InputKeyboard = 0x1;

        private const uint MouseRightDown = 0x0008;
        private const uint MouseRightUp = 0x0010;
        private const uint MouseMiddleDown = 0x0020;
        private const uint MouseMiddleUp = 0x0040;
        private const uint MouseXDown = 0x0080;
        private const uint MouseXUp = 0x0100;

        private const uint KeyboardScanCode = 0x08;

        [TestMethod()]
        public void OnGestureCancelledRestoresMousePressAndReleaseEventsTest()
        {
            var root = new RootElement();
            var config = new GestureMachineConfig();
            var inputSender = new RecordingSingleInputSender();

            using (var callbackManager = CreateCallbackManager(inputSender))
            using (var gm = new GestureMachine(config, callbackManager))
            {
                root.When((ctx) => true)
                    .On(SupportedKeys.Keys.RButton)
                    .On(SupportedKeys.Keys.MButton)
                    .On(SupportedKeys.Keys.XButton1)
                    .Do((ctx) => { });

                gm.Run(root);
                var s0 = gm.CurrentState;
                var s1 = s0.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent).NextState;
                var s2 = s1.Input(SupportedKeys.PhysicalKeys.MButton.PressEvent).NextState;
                var s3 = s2.Input(SupportedKeys.PhysicalKeys.XButton1.PressEvent).NextState;

                inputSender.ExpectCalls(1);
                callbackManager.OnGestureCanceled(gm, s1 as TStateN);
                Assert.AreEqual(true, inputSender.WaitForExpectedCalls());
                AssertMouseCall(inputSender.Calls[0], MouseRightDown, MouseRightUp, 0);

                inputSender.Reset();
                inputSender.ExpectCalls(2);
                callbackManager.OnGestureCanceled(gm, s2 as TStateN);
                Assert.AreEqual(true, inputSender.WaitForExpectedCalls());
                AssertMouseCall(inputSender.Calls[0], MouseRightDown, MouseRightUp, 0);
                AssertMouseCall(inputSender.Calls[1], MouseMiddleDown, MouseMiddleUp, 0);

                inputSender.Reset();
                inputSender.ExpectCalls(3);
                callbackManager.OnGestureCanceled(gm, s3 as TStateN);
                Assert.AreEqual(true, inputSender.WaitForExpectedCalls());
                AssertMouseCall(inputSender.Calls[0], MouseRightDown, MouseRightUp, 0);
                AssertMouseCall(inputSender.Calls[1], MouseMiddleDown, MouseMiddleUp, 0);
                AssertMouseCall(inputSender.Calls[2], MouseXDown, MouseXUp, 1);
            }
        }

        [TestMethod()]
        public void OnGestureTimeoutRestoresKeyboardPressEventsTest()
        {
            var root = new RootElement();
            var config = new GestureMachineConfig();
            var inputSender = new RecordingSingleInputSender();

            using (var callbackManager = CreateCallbackManager(inputSender))
            using (var gm = new GestureMachine(config, callbackManager))
            {
                root.When((ctx) => true)
                    .On(SupportedKeys.Keys.RControlKey)
                    .On(SupportedKeys.Keys.RShiftKey)
                    .On(SupportedKeys.Keys.RMenu)
                    .Do((ctx) => { });

                gm.Run(root);
                var s0 = gm.CurrentState;
                var s1 = s0.Input(SupportedKeys.PhysicalKeys.RControlKey.PressEvent).NextState;
                var s2 = s1.Input(SupportedKeys.PhysicalKeys.RShiftKey.PressEvent).NextState;
                var s3 = s2.Input(SupportedKeys.PhysicalKeys.RMenu.PressEvent).NextState;

                inputSender.ExpectCalls(1);
                callbackManager.OnGestureTimeout(gm, s1 as TStateN);
                Assert.AreEqual(true, inputSender.WaitForExpectedCalls());
                AssertKeyboardCall(inputSender.Calls[0], SupportedKeys.PhysicalKeys.RControlKey.KeyId);
                Assert.IsTrue(callbackManager.TimeoutKeyboardKeys.Contains(SupportedKeys.PhysicalKeys.RControlKey));

                inputSender.Reset();
                inputSender.ExpectCalls(2);
                callbackManager.OnGestureTimeout(gm, s2 as TStateN);
                Assert.AreEqual(true, inputSender.WaitForExpectedCalls());
                AssertKeyboardCall(inputSender.Calls[0], SupportedKeys.PhysicalKeys.RControlKey.KeyId);
                AssertKeyboardCall(inputSender.Calls[1], SupportedKeys.PhysicalKeys.RShiftKey.KeyId);
                Assert.IsTrue(callbackManager.TimeoutKeyboardKeys.Contains(SupportedKeys.PhysicalKeys.RShiftKey));

                inputSender.Reset();
                inputSender.ExpectCalls(3);
                callbackManager.OnGestureTimeout(gm, s3 as TStateN);
                Assert.AreEqual(true, inputSender.WaitForExpectedCalls());
                AssertKeyboardCall(inputSender.Calls[0], SupportedKeys.PhysicalKeys.RControlKey.KeyId);
                AssertKeyboardCall(inputSender.Calls[1], SupportedKeys.PhysicalKeys.RShiftKey.KeyId);
                AssertKeyboardCall(inputSender.Calls[2], SupportedKeys.PhysicalKeys.RMenu.KeyId);
                Assert.IsTrue(callbackManager.TimeoutKeyboardKeys.Contains(SupportedKeys.PhysicalKeys.RMenu));
            }
        }

        private static CallbackManager CreateCallbackManager(RecordingSingleInputSender inputSender)
            => new CallbackManager(
                new CallbackManager.ActionExecutor("CallbackManagerTestsCallbackExecutor", ThreadPriority.Highest, 1),
                new CallbackManager.ActionExecutor("CallbackManagerTestsSystemKeyRestorationExecutor", ThreadPriority.Highest, 1),
                inputSender);

        private static void AssertMouseCall(RecordedInput[] inputs, uint downFlags, uint upFlags, int mouseData)
        {
            Assert.AreEqual(2, inputs.Length);
            Assert.AreEqual(InputMouse, inputs[0].Type);
            Assert.AreEqual(downFlags, inputs[0].Flags);
            Assert.AreEqual(mouseData, inputs[0].MouseData);
            Assert.AreEqual(InputMouse, inputs[1].Type);
            Assert.AreEqual(upFlags, inputs[1].Flags);
            Assert.AreEqual(mouseData, inputs[1].MouseData);
        }

        private static void AssertKeyboardCall(RecordedInput[] inputs, int keyId)
        {
            Assert.AreEqual(1, inputs.Length);
            Assert.AreEqual(InputKeyboard, inputs[0].Type);
            Assert.AreEqual(keyId, inputs[0].VirtualKey);
            Assert.AreEqual(KeyboardScanCode, inputs[0].Flags);
        }

        private sealed class RecordingSingleInputSender : SingleInputSender
        {
            private CountdownEvent countdownEvent;

            public List<RecordedInput[]> Calls { get; } = new List<RecordedInput[]>();

            public void ExpectCalls(int count)
            {
                countdownEvent = new CountdownEvent(count);
            }

            public bool WaitForExpectedCalls()
                => countdownEvent.Wait(10000);

            public void Reset()
            {
                Calls.Clear();
                countdownEvent?.Dispose();
                countdownEvent = null;
            }

            protected override void Send(INPUT[] input)
            {
                Calls.Add(input.Select(Record).ToArray());
                countdownEvent?.Signal();
            }

            private static RecordedInput Record(INPUT input)
            {
                var recorded = new RecordedInput
                {
                    Type = input.type,
                };

                if (input.type == InputMouse)
                {
                    var mouse = input.data.asMouseInput;
                    recorded.Flags = mouse.dwFlags;
                    recorded.MouseData = mouse.mouseData.asXButton.type;
                }
                else if (input.type == InputKeyboard)
                {
                    var keyboard = input.data.asKeyboardInput;
                    recorded.VirtualKey = keyboard.wVk;
                    recorded.Flags = keyboard.dwFlags;
                }

                return recorded;
            }
        }

        private sealed class RecordedInput
        {
            public int Type { get; set; }

            public uint Flags { get; set; }

            public int MouseData { get; set; }

            public int VirtualKey { get; set; }
        }
    }
}
