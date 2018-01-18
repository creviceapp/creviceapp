using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceTests

{
    using Crevice.Core;
    using Crevice.Core.FSM;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            var context = new ActionContextFactory<DefaultActionContext>();

            var config = new DefaultGestureMachineConfig();
            var gestureDef = new List<GestureDefinition>();

            var gestureMachine = new GestureMachine<DefaultGestureMachineConfig, DefaultActionContext>(config, gestureDef);
            var global = gestureMachine.Global;

            var nullGestureMachine0 = NullGestureMachine.Create();
            var nullGestureMachine1 = NullGestureMachine.Create(new DefaultGestureMachineConfig());
            {
                var manager = Crevice.Dev.GestureManager.Create();

                var root = manager.Root;
                var Whenever = root.@when((ctx) => { return true; });
                Whenever.
                @if(Crevice.Dev.Def.Constant.LeftButton).
                @do((ctx) =>
                {
                    //
                });

                Assert.AreEqual(manager.GetDefinitions().Count, 1);
            }


            var managerB = Crevice.Dev.GestureManager.Create<Crevice.Dev.DefaultActionContext>();





            // var root = manager.DSLRoot();
            // var @when = root.@when;








            // var root = GestureDSL.Create<ActionContext>();

        }

    }
}
