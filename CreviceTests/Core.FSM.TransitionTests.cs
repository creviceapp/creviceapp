using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.Core.FSM.Tests
{
    [TestClass()]
    public class TransitionTests
    {
        [TestMethod()]
        public void Gen1Test()
        {
            var gestureDef = new List<GestureDefinition>() {
                new OnButtonWithIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    null, 
                    (ctx) => { }, 
                    null),
                 new OnButtonWithIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelDown,
                    null, 
                    (ctx) => { }, 
                    null),
                 new OnButtonWithIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.MiddleButton,
                    DSL.Def.Constant.WheelLeft,
                    null, 
                    (ctx) => { }, 
                    null),
                 new OnButtonWithIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.MiddleButton,
                    DSL.Def.Constant.WheelRight,
                    null, 
                    (ctx) => { }, 
                    null),
            };
            var Gen1 = Transition.Gen0_1(gestureDef)
                .ToDictionary(x => x.Key, x => x.Value.Select(y => y as OnButtonWithIfButtonGestureDefinition))
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
                new OnButtonWithIfStrokeGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    new Def.Stroke(new List<Def.Direction>() { Def.Direction.Up }),
                    (ctx) => { }),
                 new OnButtonWithIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    null, 
                    (ctx) => { }, 
                    null),
                 new OnButtonWithIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    null, 
                    (ctx) => { }, 
                    null),
            };
            var Gen2 = Transition.Gen1_0(gestureDef)
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
                new OnButtonWithIfStrokeGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    new Def.Stroke(new List<Def.Direction>() { Def.Direction.Up }),
                    (ctx) => { }),
                 new OnButtonWithIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    null, 
                    (ctx) => { },
                    null),
                 new OnButtonWithIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    null, 
                    (ctx) => { }, 
                    null),
            };
            var Gen3 = Transition.Gen1_1(gestureDef)
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
                new OnButtonWithIfStrokeGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    new Def.Stroke(new List<Def.Direction>() { Def.Direction.Up }),
                    (ctx) => { }),
                 new OnButtonWithIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    null, 
                    (ctx) => { }, 
                    null),
                 new OnButtonWithIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    null, 
                    (ctx) => { }, 
                    null),
            };
            var Gen4 = Transition.Gen1_2(gestureDef)
                .ToDictionary(x => x.Key, x => x.Value.ToList());

            Assert.AreEqual(Gen4.Keys.Count, 1);
            {
                var gDef = Gen4[new Def.Stroke(new List<Def.Direction>() { Def.Direction.Up })];
                Assert.AreEqual(gDef[0].onButton, DSL.Def.Constant.RightButton);
            }
        }
    }
}
