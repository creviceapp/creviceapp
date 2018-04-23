using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


namespace Crevice4Tests
{
    using System.Reflection;
    using Crevice.Core.FSM;
    using Crevice.WinAPI.WindowsHookEx;
    using Crevice.UserScript.Keys;
    using Crevice.GestureMachine;
    using Crevice.DSL;
    using Crevice.UserScript;

    using TStateN = Crevice.Core.FSM.StateN<Crevice.Core.FSM.GestureMachineConfig, Crevice.GestureMachine.ContextManager, Crevice.GestureMachine.EvaluationContext, Crevice.GestureMachine.ExecutionContext>;

    [TestClass()]
    public class CallbackManagerTests
    {
        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
        {
            TestHelpers.KeyboardMutex.WaitOne();
            TestHelpers.MouseMutex.WaitOne();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TestHelpers.KeyboardMutex.ReleaseMutex();
            TestHelpers.MouseMutex.ReleaseMutex();
        }

        [TestMethod()]
        public void OnGestureCancelledTest()
        {
            var root = new RootElement();
            var config = new GestureMachineConfig();
            var callbackManager = new CallbackManager();
            using (var gm = new GestureMachine(config, callbackManager))
            {
                root.When((ctx) => { return true; })
                .On(SupportedKeys.Keys.RControlKey)
                .On(SupportedKeys.Keys.RShiftKey)
                .On(SupportedKeys.Keys.RMenu)
                .Do((ctx) => { });

                gm.Run(root);
                var s0 = gm.CurrentState;
                var s1 = s0.Input(SupportedKeys.PhysicalKeys.RControlKey.PressEvent).NextState;
                Assert.AreEqual(s1.Depth, 1);
                var s2 = s1.Input(SupportedKeys.PhysicalKeys.RShiftKey.PressEvent).NextState;
                Assert.AreEqual(s2.Depth, 2);
                var s3 = s2.Input(SupportedKeys.PhysicalKeys.RMenu.PressEvent).NextState;
                Assert.AreEqual(s3.Depth, 3);

                using (var cde = new CountdownEvent(2))
                {
                    using (var hook = new LowLevelKeyboardHook((evnt, data) =>
                    {
                        cde.Signal();
                        return LowLevelKeyboardHook.Result.Cancel;
                    }))
                    {
                        hook.SetHook();

                        callbackManager.OnGestureCancelled(gm, s1 as TStateN);
                        Assert.AreEqual(cde.Wait(10000), true);

                        cde.Reset();
                        cde.AddCount(2);
                        callbackManager.OnGestureCancelled(gm, s2 as TStateN);
                        Assert.AreEqual(cde.Wait(10000), true);

                        cde.Reset();
                        cde.AddCount(4);
                        callbackManager.OnGestureCancelled(gm, s3 as TStateN);
                        Assert.AreEqual(cde.Wait(10000), true);
                    }
                }
            }
        }

        [TestMethod()]
        public void OnGestureTimeoutTest()
        {
            var root = new RootElement();
            var config = new GestureMachineConfig();
            var callbackManager = new CallbackManager();
            using (var gm = new GestureMachine(config, callbackManager))
            {
                root.When((ctx) => { return true; })
                .On(SupportedKeys.Keys.RControlKey)
                .On(SupportedKeys.Keys.RShiftKey)
                .On(SupportedKeys.Keys.RMenu)
                .Do((ctx) => { });

                gm.Run(root);
                var s0 = gm.CurrentState;
                var s1 = s0.Input(SupportedKeys.PhysicalKeys.RControlKey.PressEvent).NextState;
                Assert.AreEqual(s1.Depth, 1);
                var s2 = s1.Input(SupportedKeys.PhysicalKeys.RShiftKey.PressEvent).NextState;
                Assert.AreEqual(s2.Depth, 2);
                var s3 = s2.Input(SupportedKeys.PhysicalKeys.RMenu.PressEvent).NextState;
                Assert.AreEqual(s3.Depth, 3);

                using (var cde = new CountdownEvent(1))
                {
                    using (var hook = new LowLevelKeyboardHook((evnt, data) =>
                    {
                        cde.Signal();
                        return LowLevelKeyboardHook.Result.Cancel;
                    }))
                    {
                        hook.SetHook();

                        callbackManager.OnGestureTimeout(gm, s1 as TStateN);
                        Assert.AreEqual(cde.Wait(10000), true);

                        cde.Reset();
                        cde.AddCount(1);
                        callbackManager.OnGestureTimeout(gm, s2 as TStateN);
                        Assert.AreEqual(cde.Wait(10000), true);

                        cde.Reset();
                        cde.AddCount(2);
                        callbackManager.OnGestureTimeout(gm, s3 as TStateN);
                        Assert.AreEqual(cde.Wait(10000), true);
                    }
                }
            }
        }
    }
}
