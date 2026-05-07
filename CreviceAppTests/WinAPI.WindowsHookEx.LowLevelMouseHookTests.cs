using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Crevice4Tests
{
    using Crevice.WinAPI.SendInput;
    using Crevice.WinAPI.WindowsHookEx;

    [TestClass()]
    public class LowLevelMouseHookTests
    {
        [TestMethod()]
        public void ActivatedTest()
        {
            using (var hook = new FakeWindowsHook())
            {
                Assert.IsFalse(hook.IsActivated);
                hook.SetHook();
                Assert.IsTrue(hook.IsActivated);
                hook.Unhook();
                Assert.IsFalse(hook.IsActivated);
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetHookThrowsInvalidOperationExceptionTest()
        {
            using (var hook = new FakeWindowsHook())
            {
                hook.SetHook();
                hook.SetHook();
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UnhookThrowsInvalidOperationExceptionTest0Test()
        {
            using (var hook = new FakeWindowsHook())
            {
                hook.SetHook();
                hook.Unhook();
                hook.Unhook();
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UnhookThrowsInvalidOperationExceptionTest1Test()
        {
            using (var hook = new FakeWindowsHook())
            {
                hook.Unhook();
            }
        }

        [TestMethod()]
        public void DisposeWhenActivatedTest()
        {
            var hook = new FakeWindowsHook();
            hook.SetHook();
            Assert.IsTrue(hook.IsActivated);
            hook.Dispose();
            Assert.IsFalse(hook.IsActivated);
            Assert.AreEqual(1, hook.UnhookCallCount);
        }

        [TestMethod()]
        public void DisposeWhenNotActivatedTest()
        {
            var hook = new FakeWindowsHook();
            Assert.IsFalse(hook.IsActivated);
            hook.Dispose();
            Assert.IsFalse(hook.IsActivated);
            Assert.AreEqual(0, hook.UnhookCallCount);
        }

        [TestMethod()]
        public void CallbackParsesMouseHookDataTest()
        {
            LowLevelMouseHook.Event observedEvent = 0;
            LowLevelMouseHook.MSLLHOOKSTRUCT observedData = new LowLevelMouseHook.MSLLHOOKSTRUCT();

            using (var hook = new LowLevelMouseHook((evnt, data) =>
            {
                observedEvent = evnt;
                observedData = data;
                return LowLevelMouseHook.Result.Determine;
            }))
            {
                var data = new LowLevelMouseHook.MSLLHOOKSTRUCT
                {
                    pt = new LowLevelMouseHook.POINT { x = 10, y = 20 },
                    dwExtraInfo = new UIntPtr(LowLevelMouseHook.MOUSEEVENTF_CREVICE_APP),
                };
                data.mouseData.asWheelDelta.delta = 120;

                var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(LowLevelMouseHook.MSLLHOOKSTRUCT)));

                try
                {
                    Marshal.StructureToPtr(data, ptr, false);

                    var result = hook.Callback(
                        WindowsHook.HC_ACTION,
                        new IntPtr((int)LowLevelMouseHook.Event.WM_MOUSEWHEEL),
                        ptr);

                    Assert.AreEqual(IntPtr.Zero, result);
                    Assert.AreEqual(LowLevelMouseHook.Event.WM_MOUSEWHEEL, observedEvent);
                    Assert.AreEqual(10, observedData.pt.x);
                    Assert.AreEqual(20, observedData.pt.y);
                    Assert.AreEqual(120, observedData.mouseData.asWheelDelta.delta);
                    Assert.IsTrue(observedData.FromCreviceApp);
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        [TestMethod()]
        public void CallbackTransfersToNextHookWhenUserCallbackThrowsTest()
        {
            var hook = new ThrowingWindowsHook();

            var result = hook.Callback(
                WindowsHook.HC_ACTION,
                IntPtr.Zero,
                IntPtr.Zero);

            Assert.AreEqual(ThrowingWindowsHook.NextHookResult, result);
            Assert.IsTrue(hook.CallNextHookCalled);
            Assert.IsTrue(hook.CallbackExceptionObserved);
        }

        [TestMethod()]
        [TestCategory("OSIntegration")]
        public void LowLevelMouseHookReceivesSendInputWhenEnabledTest()
        {
            TestHelpers.RequireOSIntegrationTestsEnabled();
            TestHelpers.MouseMutex.WaitOne();

            try
            {
                using (var cde = new CountdownEvent(2))
                using (var hook = new LowLevelMouseHook((evnt, data) =>
                {
                    if (data.FromCreviceApp)
                    {
                        cde.Signal();
                    }
                    return LowLevelMouseHook.Result.Cancel;
                }))
                {
                    hook.SetHook();
                    try
                    {
                        var sender = new SingleInputSender();
                        sender.RightDown();
                        sender.RightUp();
                        Assert.AreEqual(true, cde.Wait(10000));
                    }
                    finally
                    {
                        if (hook.IsActivated)
                        {
                            hook.Unhook();
                        }
                    }
                }
            }
            finally
            {
                TestHelpers.MouseMutex.ReleaseMutex();
            }
        }

        private sealed class FakeWindowsHook : WindowsHook
        {
            private static readonly IntPtr HookHandle = new IntPtr(1);

            public int UnhookCallCount { get; private set; }

            public FakeWindowsHook()
                : base(HookType.WH_MOUSE_LL, (wParam, lParam) => Result.Cancel)
            {
            }

            protected override IntPtr GetModuleHandle()
                => new IntPtr(2);

            protected override IntPtr SetHookCore(IntPtr hInstance)
                => HookHandle;

            protected override bool UnhookCore(IntPtr hook)
            {
                Assert.AreEqual(HookHandle, hook);
                UnhookCallCount += 1;
                return true;
            }
        }

        private sealed class ThrowingWindowsHook : WindowsHook
        {
            public static readonly IntPtr NextHookResult = new IntPtr(42);

            public bool CallNextHookCalled { get; private set; }

            public bool CallbackExceptionObserved { get; private set; }

            public ThrowingWindowsHook()
                : base(HookType.WH_MOUSE_LL, (wParam, lParam) =>
                {
                    throw new InvalidOperationException("test callback failure");
                })
            {
            }

            protected override IntPtr CallNextHook(int nCode, IntPtr wParam, IntPtr lParam)
            {
                CallNextHookCalled = true;
                return NextHookResult;
            }

            protected override void OnCallbackException(Exception exception)
            {
                CallbackExceptionObserved = true;
            }
        }
    }
}
