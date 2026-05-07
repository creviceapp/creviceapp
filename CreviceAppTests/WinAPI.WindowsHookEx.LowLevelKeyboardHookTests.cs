using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Crevice4Tests
{
    using Crevice.WinAPI.Constants;
    using Crevice.WinAPI.SendInput;
    using Crevice.WinAPI.WindowsHookEx;

    [TestClass()]
    public class LowLevelKeyboardHookTests
    {
        [TestMethod()]
        public void CallbackParsesKeyboardHookDataTest()
        {
            LowLevelKeyboardHook.Event observedEvent = 0;
            LowLevelKeyboardHook.KBDLLHOOKSTRUCT observedData = null;

            using (var hook = new LowLevelKeyboardHook((evnt, data) =>
            {
                observedEvent = evnt;
                observedData = data;
                return LowLevelKeyboardHook.Result.Determine;
            }))
            {
                var data = new LowLevelKeyboardHook.KBDLLHOOKSTRUCT
                {
                    vkCode = 0x41,
                    scanCode = 0x1E,
                    flags = LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED,
                    time = 123,
                    dwExtraInfo = new UIntPtr(LowLevelKeyboardHook.KEYBOARDEVENTF_CREVICE_APP),
                };
                var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(LowLevelKeyboardHook.KBDLLHOOKSTRUCT)));

                try
                {
                    Marshal.StructureToPtr(data, ptr, false);

                    var result = hook.Callback(
                        WindowsHook.HC_ACTION,
                        new IntPtr((int)LowLevelKeyboardHook.Event.WM_KEYDOWN),
                        ptr);

                    Assert.AreEqual(IntPtr.Zero, result);
                    Assert.AreEqual(LowLevelKeyboardHook.Event.WM_KEYDOWN, observedEvent);
                    Assert.AreEqual(0x41, observedData.vkCode);
                    Assert.AreEqual(0x1E, observedData.scanCode);
                    Assert.IsTrue(observedData.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
                    Assert.IsTrue(observedData.FromCreviceApp);
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        [TestMethod()]
        [TestCategory("ManualInputIntegration")]
        public void LowLevelKeyboardHookReceivesSignedNonTextSendInputWhenEnabledTest()
        {
            TestHelpers.RequireManualInputIntegrationTestsEnabled();
            TestHelpers.KeyboardMutex.WaitOne();

            try
            {
                using (var cde = new CountdownEvent(2))
                using (var hook = new LowLevelKeyboardHook((evnt, data) =>
                {
                    if (data.FromCreviceApp)
                    {
                        cde.Signal();
                        return LowLevelKeyboardHook.Result.Cancel;
                    }

                    return LowLevelKeyboardHook.Result.Determine;
                }))
                {
                    hook.SetHook();
                    try
                    {
                        var sender = new SingleInputSender();
                        sender.KeyDown(VirtualKeys.VK_F24);
                        sender.KeyUp(VirtualKeys.VK_F24);
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
                TestHelpers.KeyboardMutex.ReleaseMutex();
            }
        }
    }
}
