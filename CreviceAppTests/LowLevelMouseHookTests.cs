using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CreviceApp.Tests
{
    [TestClass()]
    public class LowLevelMouseHookTests
    {
        [TestMethod()]
        public void ActivatedTest()
        {
            var hook = new LowLevelMouseHook((evnt, data) => { return LowLevelMouseHook.Result.Transfer; });
            Assert.IsFalse(hook.Activated());
            hook.SetHook();
            Assert.IsTrue(hook.Activated());
            hook.Unhook();
            Assert.IsFalse(hook.Activated());
        }

        [TestMethod()]
        public void SetHookThrowsInvalidOperationExceptionTest()
        {
            var hook = new LowLevelMouseHook((evnt, data) => { return LowLevelMouseHook.Result.Transfer; });
            hook.SetHook();
            try
            {
                hook.SetHook();
            }
            catch (InvalidOperationException ex)
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
            var hook = new LowLevelMouseHook((evnt, data) => { return LowLevelMouseHook.Result.Transfer; });
            hook.SetHook();
            hook.Unhook();
            try
            {
                hook.Unhook();
            }
            catch (InvalidOperationException ex)
            {
                return;
            }
            Assert.Fail();
        }

        [TestMethod()]
        public void MouseHookProcTest()
        {
            var sender = new SingleInputSender();
            var list = new List<LowLevelMouseHook.Event>();
            var hook = new LowLevelMouseHook((evnt, data) => {
                if (data.fromCreviceApp)
                {
                    list.Add(evnt);
                    return LowLevelMouseHook.Result.Cancel;
                }
                return LowLevelMouseHook.Result.Transfer;
            });
            Assert.AreEqual(list.Count, 0);
            hook.SetHook();
            sender.RightClick();
            hook.Unhook();
            Assert.AreEqual(list.Count, 2);
        }

        [TestMethod()]
        public void DisposeWhenActivatedTest()
        {
            var hook = new LowLevelMouseHook((evnt, data) => { return LowLevelMouseHook.Result.Transfer; });
            hook.SetHook();
            Assert.IsTrue(hook.Activated());
            hook.Dispose();
            Assert.IsFalse(hook.Activated());
        }

        [TestMethod()]
        public void DisposeWhenNotActivatedTest()
        {
            var hook = new LowLevelMouseHook((evnt, data) => { return LowLevelMouseHook.Result.Transfer; });
            Assert.IsFalse(hook.Activated());
            hook.Dispose();
            Assert.IsFalse(hook.Activated());
        }
    }
}