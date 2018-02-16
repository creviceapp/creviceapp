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
            var sender = new SingleInputSender();
            var list = new List<LowLevelKeyboardHook.Event>();
            var hook = new LowLevelKeyboardHook((evnt, data) => {
                list.Add(evnt);
                return LowLevelKeyboardHook.Result.Cancel;
            });
            Assert.AreEqual(list.Count, 0);
            hook.SetHook();
            sender.UnicodeKeyStroke("A");
            hook.Unhook();
            Assert.AreEqual(list.Count, 2);
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