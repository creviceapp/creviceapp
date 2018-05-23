using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crevice4Tests
{
    using Crevice.Config;
    using Crevice.UserScript.Keys;
    using Crevice.GestureMachine;
    using Crevice.DSL;

    [TestClass()]
    public class GestureMachineTests
    {

        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
        {
            TestHelpers.MouseMutex.WaitOne();
            TestHelpers.KeyboardMutex.WaitOne();
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            TestHelpers.MouseMutex.ReleaseMutex();
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
        public void MouseEventTimeoutTest()
        {
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var L = when.On(SupportedKeys.Keys.LButton);
                    L.Do(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var L = when.On(SupportedKeys.Keys.LButton);
                    L.Press(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var L = when.On(SupportedKeys.Keys.LButton);
                    L.Release(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }

            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var L = when.On(SupportedKeys.Keys.LButton);
                    var R = L.On(SupportedKeys.Keys.RButton);
                    R.Do(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    // This behavior is the only different point to when keyboard event is held pressing across timeout span.
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var L = when.On(SupportedKeys.Keys.LButton);
                    var R = L.On(SupportedKeys.Keys.RButton);
                    R.Do(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }

            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var L = when.On(SupportedKeys.Keys.LButton);
                    var R = L.On(SupportedKeys.Keys.RButton);
                    R.Press(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var L = when.On(SupportedKeys.Keys.LButton);
                    var R = L.On(SupportedKeys.Keys.RButton);
                    R.Press(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }

            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var L = when.On(SupportedKeys.Keys.LButton);
                    var R = L.On(SupportedKeys.Keys.RButton);
                    R.Release(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var L = when.On(SupportedKeys.Keys.LButton);
                    var R = L.On(SupportedKeys.Keys.RButton);
                    R.Release(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }

            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var L = when.OnDecomposed(SupportedKeys.Keys.LButton);
                    L.Press(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var L = when.OnDecomposed(SupportedKeys.Keys.LButton);
                    L.Release(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }

            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var L = when.On(SupportedKeys.Keys.LButton);
                    var R = L.OnDecomposed(SupportedKeys.Keys.RButton);
                    R.Press(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var L = when.On(SupportedKeys.Keys.LButton);
                    var R = L.OnDecomposed(SupportedKeys.Keys.RButton);
                    R.Press(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }

            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var L = when.On(SupportedKeys.Keys.LButton);
                    var R = L.OnDecomposed(SupportedKeys.Keys.RButton);
                    R.Release(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var L = when.On(SupportedKeys.Keys.LButton);
                    var B = L.OnDecomposed(SupportedKeys.Keys.RButton);
                    B.Release(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }
        }

        [TestMethod()]
        public void KeyboardEventTimeoutTest()
        {
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var A = when.On(SupportedKeys.Keys.A);
                    A.Do(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var A = when.On(SupportedKeys.Keys.A);
                    A.Press(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var A = when.On(SupportedKeys.Keys.A);
                    A.Release(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }

            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var A = when.On(SupportedKeys.Keys.A);
                    var B = A.On(SupportedKeys.Keys.B);
                    B.Do(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, false);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, false);
                }
            }
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var A = when.On(SupportedKeys.Keys.A);
                    var B = A.On(SupportedKeys.Keys.B);
                    B.Do(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }

            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var A = when.On(SupportedKeys.Keys.A);
                    var B = A.On(SupportedKeys.Keys.B);
                    B.Press(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, false);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, false);
                }
            }
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var A = when.On(SupportedKeys.Keys.A);
                    var B = A.On(SupportedKeys.Keys.B);
                    B.Press(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }

            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var A = when.On(SupportedKeys.Keys.A);
                    var B = A.On(SupportedKeys.Keys.B);
                    B.Release(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, false);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, false);
                }
            }
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var A = when.On(SupportedKeys.Keys.A);
                    var B = A.On(SupportedKeys.Keys.B);
                    B.Release(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }

            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var A = when.OnDecomposed(SupportedKeys.Keys.A);
                    A.Press(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var A = when.OnDecomposed(SupportedKeys.Keys.A);
                    A.Release(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }

            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var A = when.On(SupportedKeys.Keys.A);
                    var B = A.OnDecomposed(SupportedKeys.Keys.B);
                    B.Press(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, false);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, false);
                }
            }
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var A = when.On(SupportedKeys.Keys.A);
                    var B = A.OnDecomposed(SupportedKeys.Keys.B);
                    B.Press(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }

            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var A = when.On(SupportedKeys.Keys.A);
                    var B = A.OnDecomposed(SupportedKeys.Keys.B);
                    B.Release(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, false);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, false);
                }
            }
            {
                var cm = new CallbackManager();
                var uc = new UserConfig(cm.Callback);
                using (var gm = new GestureMachine(uc.Core, cm))
                {
                    var root = new RootElement();

                    var when = root.When(ctx => true);
                    var A = when.On(SupportedKeys.Keys.A);
                    var B = A.OnDecomposed(SupportedKeys.Keys.B);
                    B.Release(ctx => { });
                    gm.Run(root);

                    bool res;
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);

                    Task.Delay(uc.Core.GestureTimeout + 1000).Wait();

                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.A.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);
                    res = gm.Input(SupportedKeys.PhysicalKeys.B.PressEvent);
                    Assert.AreEqual(res, true);
                }
            }
        }
    }
}
