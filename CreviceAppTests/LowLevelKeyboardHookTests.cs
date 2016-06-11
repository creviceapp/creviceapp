using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Tests
{
    using WinAPI.WindowsHook;
    using WinAPI.InputSender;

    [TestClass()]
    public class LowLevelKeyboardHookTests
    {
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
        public void SetHookThrowsInvalidOperationExceptionTest()
        {
            var hook = new LowLevelKeyboardHook((evnt, data) => { return LowLevelKeyboardHook.Result.Transfer; });
            hook.SetHook();
            try
            {
                hook.SetHook();
            }
            catch (InvalidOperationException)
            {
                return;
            }
            finally
            {
                hook.Unhook();
            }
            Assert.Fail();
        }

        [TestMethod()]
        public void UnhookThrowsInvalidOperationExceptionTestTest()
        {
            var hook = new LowLevelKeyboardHook((evnt, data) => { return LowLevelKeyboardHook.Result.Transfer; });
            hook.SetHook();
            hook.Unhook();
            try
            {
                hook.Unhook();
            }
            catch (InvalidOperationException)
            {
                return;
            }
            Assert.Fail();
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