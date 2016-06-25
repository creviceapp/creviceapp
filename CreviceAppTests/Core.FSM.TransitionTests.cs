using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp.Core.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM.Tests
{
    [TestClass()]
    public class TransitionTests
    {
        [TestMethod()]
        public void Gen1Test()
        {
            var gestureDef = new List<GestureDefinition>() {
                new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    (ctx) => { }),
                 new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelDown,
                    (ctx) => { }),
                 new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.MiddleButton,
                    DSL.Def.Constant.WheelLeft,
                    (ctx) => { }),
                 new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.MiddleButton,
                    DSL.Def.Constant.WheelRight,
                    (ctx) => { }),
            };
            var Gen1 = Transition.Gen1(gestureDef)
                .ToDictionary(x => x.Key, x => x.Value.Select(y => y as OnButtonIfButtonGestureDefinition))
                .ToDictionary(x => x.Key, x => x.Value.ToList());

            Assert.AreEqual(Gen1.Keys.Count, 2);
            Assert.IsTrue(Gen1.Keys.Contains(Def.Constant.RightButtonDown));
            Assert.IsTrue(Gen1.Keys.Contains(Def.Constant.MiddleButtonDown));
            {
                var gDef = Gen1[Def.Constant.RightButtonDown];
                Assert.AreEqual(gDef[0].ifButton, DSL.Def.Constant.WheelUp);
                Assert.AreEqual(gDef[1].ifButton, DSL.Def.Constant.WheelDown);
            }
            {
                var gDef = Gen1[Def.Constant.MiddleButtonDown];
                Assert.AreEqual(gDef[0].ifButton, DSL.Def.Constant.WheelLeft);
                Assert.AreEqual(gDef[1].ifButton, DSL.Def.Constant.WheelRight);
            }
        }

        [TestMethod()]
        public void Gen2MustAcceptOnlyButtonGestureDefinitionHavingISingleActionAsIfButtonTest()
        {
            var gestureDef = new List<OnButtonGestureDefinition>() {
                new OnButtonIfStrokeGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    new Def.Stroke(new List<Def.Direction>() { Def.Direction.Up }),
                    (ctx) => { }),
                 new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    (ctx) => { }),
                 new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    (ctx) => { }),
            };
            var Gen2 = Transition.Gen2(gestureDef)
                .ToDictionary(x => x.Key, x => x.Value.ToList());

            Assert.AreEqual(Gen2.Keys.Count, 1);
            {
                var gDef = Gen2[Def.Constant.WheelUp];
                Assert.AreEqual(gDef[0].onButton, DSL.Def.Constant.RightButton);
            }
        }

        [TestMethod()]
        public void Gen3MustAcceptOnlyButtonGestureDefinitionHavingIDoubleActionAsIfButtonTest()
        {
            var gestureDef = new List<OnButtonGestureDefinition>() {
                new OnButtonIfStrokeGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    new Def.Stroke(new List<Def.Direction>() { Def.Direction.Up }),
                    (ctx) => { }),
                 new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    (ctx) => { }),
                 new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    (ctx) => { }),
            };
            var Gen3 = Transition.Gen3(gestureDef)
                .ToDictionary(x => x.Key, x => x.Value.ToList());

            Assert.AreEqual(Gen3.Keys.Count, 1);
            {
                var gDef = Gen3[Def.Constant.LeftButtonDown];
                Assert.AreEqual(gDef[0].onButton, DSL.Def.Constant.RightButton);
            }
        }

        [TestMethod()]
        public void Gen4MustAcceptOnlyStrokeGestureDefinitionTest()
        {
            var gestureDef = new List<OnButtonGestureDefinition>() {
                new OnButtonIfStrokeGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    new Def.Stroke(new List<Def.Direction>() { Def.Direction.Up }),
                    (ctx) => { }),
                 new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    (ctx) => { }),
                 new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    (ctx) => { }),
            };
            var Gen4 = Transition.Gen4(gestureDef)
                .ToDictionary(x => x.Key, x => x.Value.ToList());

            Assert.AreEqual(Gen4.Keys.Count, 1);
            {
                var gDef = Gen4[new Def.Stroke(new List<Def.Direction>() { Def.Direction.Up })];
                Assert.AreEqual(gDef[0].onButton, DSL.Def.Constant.RightButton);
            }
        }
    }
}
