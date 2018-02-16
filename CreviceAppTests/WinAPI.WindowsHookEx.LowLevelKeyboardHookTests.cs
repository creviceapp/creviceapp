using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crevice4Tests
{
    using Crevice.WinAPI.WindowsHookEx;
    using Crevice.WinAPI.SendInput;

    [TestClass()]
    public class LowLevelKeyboardHookTests
    {
        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
        {
            TestHelpers.KeyboardMutex.WaitOne();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TestHelpers.KeyboardMutex.ReleaseMutex();
        }

        static readonly Mutex mutex = new Mutex(true);

        [TestInitialize()]
        public void TestInitialize()
        {
            mutex.WaitOne();
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            mutex.ReleaseMutex();
        }
        
        [TestMethod()]
        public void ActivatedTest()
        {
            var hook = new LowLevelKeyboardHook((evnt, data) => { return LowLevelKeyboardHook.Result.Transfer; });
            Assert.IsFalse(hook.IsActivated);
            hook.SetHook();
            Assert.IsTrue(hook.IsActivated);
            hook.Unhook();
            Assert.IsFalse(hook.IsActivated);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetHookThrowsInvalidOperationExceptionTest()
        {
            var hook = new LowLevelKeyboardHook((evnt, data) => { return LowLevelKeyboardHook.Result.Transfer; });
            hook.SetHook();
            hook.SetHook();
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UnhookThrowsInvalidOperationExceptionTestTest()
        {
            var hook = new LowLevelKeyboardHook((evnt, data) => { return LowLevelKeyboardHook.Result.Transfer; });
            hook.Unhook();
        }

        [TestMethod()]
        public void LowLevelKeyboardHookTest()
        {
            using (var cde = new CountdownEvent(2))
            {

                var sender = new SingleInputSender();
                var hook = new LowLevelKeyboardHook((evnt, data) => {
                    cde.Signal();
                    return LowLevelKeyboardHook.Result.Cancel;
                });
                hook.SetHook();
                sender.UnicodeKeyStroke("A");
                Assert.AreEqual(cde.Wait(10000), true);
                hook.Unhook();
            }
        }

        [TestMethod()]
        public void DisposeWhenActivatedTest()
        {
            var hook = new LowLevelKeyboardHook((evnt, data) => { return LowLevelKeyboardHook.Result.Transfer; });
            hook.SetHook();
            Assert.IsTrue(hook.IsActivated);
            hook.Dispose();
            Assert.IsFalse(hook.IsActivated);
        }

        [TestMethod()]
        public void DisposeWhenNotActivatedTest()
        {
            var hook = new LowLevelKeyboardHook((evnt, data) => { return LowLevelKeyboardHook.Result.Transfer; });
            Assert.IsFalse(hook.IsActivated);
            hook.Dispose();
            Assert.IsFalse(hook.IsActivated);
        }
    }
}