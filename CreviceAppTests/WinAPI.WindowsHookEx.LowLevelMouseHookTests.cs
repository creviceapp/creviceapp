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
        public void SetHookThrowsInvalidOperationExceptionTest()
        {
            var hook = new LowLevelMouseHook((evnt, data) => { return LowLevelMouseHook.Result.Transfer; });
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
            var hook = new LowLevelMouseHook((evnt, data) => { return LowLevelMouseHook.Result.Transfer; });
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
        public void LowLevelMouseHookProcTest()
        {
            var sender = new SingleInputSender();
            var list = new List<LowLevelMouseHook.Event>();
            var hook = new LowLevelMouseHook((evnt, data) => {
                if (data.FromCreviceApp)
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