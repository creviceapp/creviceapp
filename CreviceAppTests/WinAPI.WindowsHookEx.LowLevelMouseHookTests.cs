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
    public class LowLevelMouseHookTests
    {
        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
        {
            TestHelpers.MouseMutex.WaitOne();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TestHelpers.MouseMutex.ReleaseMutex();
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
            var hook = new LowLevelMouseHook((evnt, data) => { return LowLevelMouseHook.Result.Transfer; });
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
            var hook = new LowLevelMouseHook((evnt, data) => { return LowLevelMouseHook.Result.Transfer; });
            hook.SetHook();
            try
            {
                hook.SetHook();
            }
            finally
            {
                hook.Unhook();
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UnhookThrowsInvalidOperationExceptionTest0Test()
        {
            var hook = new LowLevelMouseHook((evnt, data) => { return LowLevelMouseHook.Result.Transfer; });
            hook.SetHook();
            hook.Unhook();
            hook.Unhook();
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UnhookThrowsInvalidOperationExceptionTest1Test()
        {
            var hook = new LowLevelMouseHook((evnt, data) => { return LowLevelMouseHook.Result.Transfer; });
            hook.Unhook();
        }

        [TestMethod()]
        public void LowLevelMouseHookProcTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                var hook = new LowLevelMouseHook((evnt, data) => {
                    if (data.FromCreviceApp)
                    {
                        cde.Signal();
                        return LowLevelMouseHook.Result.Cancel;
                    }
                    return LowLevelMouseHook.Result.Transfer;
                });
                try
                {
                    var sender = new SingleInputSender();
                    hook.SetHook();
                    sender.RightClick();
                    Assert.AreEqual(cde.Wait(10000), true);
                }
                finally
                {
                    hook.Unhook();
                }
            }
        }

        [TestMethod()]
        public void DisposeWhenActivatedTest()
        {
            var hook = new LowLevelMouseHook((evnt, data) => { return LowLevelMouseHook.Result.Transfer; });
            hook.SetHook();
            Assert.IsTrue(hook.IsActivated);
            hook.Dispose();
            Assert.IsFalse(hook.IsActivated);
        }

        [TestMethod()]
        public void DisposeWhenNotActivatedTest()
        {
            var hook = new LowLevelMouseHook((evnt, data) => { return LowLevelMouseHook.Result.Transfer; });
            Assert.IsFalse(hook.IsActivated);
            hook.Dispose();
            Assert.IsFalse(hook.IsActivated);
        }
    }
}