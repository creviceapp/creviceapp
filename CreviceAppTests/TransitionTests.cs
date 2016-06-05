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
        public void Gen0Test()
        {
            var gestureDef = new List<GestureDefinition>() {
                new ButtonGestureDefinition(
                    () => { return false; },
                    Config.Def.Constant.RightButton,
                    Config.Def.Constant.WheelUp,
                    () => { }),
                 new ButtonGestureDefinition(
                    () => { return false; },
                    Config.Def.Constant.RightButton,
                    Config.Def.Constant.WheelDown,
                    () => { }),
                 new ButtonGestureDefinition(
                    () => { return false; },
                    Config.Def.Constant.MiddleButton,
                    Config.Def.Constant.WheelLeft,
                    () => { }),
                 new ButtonGestureDefinition(
                    () => { return false; },
                    Config.Def.Constant.MiddleButton,
                    Config.Def.Constant.WheelRight,
                    () => { }),
            };
            var Gen0 = Transition.Gen0(gestureDef)
                .ToDictionary(x => x.Key, x => x.Value.Select(y => y as ButtonGestureDefinition))
                .ToDictionary(x => x.Key, x => x.Value.ToList());

            Assert.AreEqual(Gen0.Keys.Count, 2);
            Assert.IsTrue(Gen0.Keys.Contains(Def.Constant.RightButtonDown));
            Assert.IsTrue(Gen0.Keys.Contains(Def.Constant.MiddleButtonDown));
            {
                var gDef = Gen0[Def.Constant.RightButtonDown];
                Assert.AreEqual(gDef[0].ifButton, Config.Def.Constant.WheelUp);
                Assert.AreEqual(gDef[1].ifButton, Config.Def.Constant.WheelDown);
            }
            {
                var gDef = Gen0[Def.Constant.MiddleButtonDown];
                Assert.AreEqual(gDef[0].ifButton, Config.Def.Constant.WheelLeft);
                Assert.AreEqual(gDef[1].ifButton, Config.Def.Constant.WheelRight);
            }
        }

        [TestMethod()]
        public void Gen1MustAcceptOnlyButtonGestureDefinitionHavingIDoubleActionAsIfButtonTest()
        {
            var gestureDef = new List<GestureDefinition>() {
                new StrokeGestureDefinition(
                    () => { return false; },
                    Config.Def.Constant.RightButton,
                    new Def.Stroke(new List<Def.Move>() { Def.Move.Up }),
                    () => { }),
                 new ButtonGestureDefinition(
                    () => { return false; },
                    Config.Def.Constant.RightButton,
                    Config.Def.Constant.WheelUp,
                    () => { }),
                 new ButtonGestureDefinition(
                    () => { return false; },
                    Config.Def.Constant.RightButton,
                    Config.Def.Constant.LeftButton,
                    () => { }),
            };
            var Gen1 = Transition.Gen1(gestureDef)
                .ToDictionary(x => x.Key, x => x.Value.ToList());

            Assert.AreEqual(Gen1.Keys.Count, 1);
            {
                var gDef = Gen1[Def.Constant.LeftButtonDown];
                Assert.AreEqual(gDef[0].onButton, Config.Def.Constant.RightButton);
            }
        }

        [TestMethod()]
        public void Gen2MustAcceptOnlyButtonGestureDefinitionHavingISingleActionAsIfButtonTest()
        {
            var gestureDef = new List<GestureDefinition>() {
                new StrokeGestureDefinition(
                    () => { return false; },
                    Config.Def.Constant.RightButton,
                    new Def.Stroke(new List<Def.Move>() { Def.Move.Up }),
                    () => { }),
                 new ButtonGestureDefinition(
                    () => { return false; },
                    Config.Def.Constant.RightButton,
                    Config.Def.Constant.WheelUp,
                    () => { }),
                 new ButtonGestureDefinition(
                    () => { return false; },
                    Config.Def.Constant.RightButton,
                    Config.Def.Constant.LeftButton,
                    () => { }),
            };
            var Gen2 = Transition.Gen2(gestureDef)
                .ToDictionary(x => x.Key, x => x.Value.ToList());

            Assert.AreEqual(Gen2.Keys.Count, 1);
            {
                var gDef = Gen2[Def.Constant.WheelUp];
                Assert.AreEqual(gDef[0].onButton, Config.Def.Constant.RightButton);
            }
        }

        [TestMethod()]
        public void Gen3MustAcceptOnlyStrokeGestureDefinitionTest()
        {
            var gestureDef = new List<GestureDefinition>() {
                new StrokeGestureDefinition(
                    () => { return false; },
                    Config.Def.Constant.RightButton,
                    new Def.Stroke(new List<Def.Move>() { Def.Move.Up }),
                    () => { }),
                 new ButtonGestureDefinition(
                    () => { return false; },
                    Config.Def.Constant.RightButton,
                    Config.Def.Constant.WheelUp,
                    () => { }),
                 new ButtonGestureDefinition(
                    () => { return false; },
                    Config.Def.Constant.RightButton,
                    Config.Def.Constant.LeftButton,
                    () => { }),
            };
            var Gen3 = Transition.Gen3(gestureDef)
                .ToDictionary(x => x.Key, x => x.Value.ToList());

            Assert.AreEqual(Gen3.Keys.Count, 1);
            {
                var gDef = Gen3[new Def.Stroke(new List<Def.Move>() { Def.Move.Up })];
                Assert.AreEqual(gDef[0].onButton, Config.Def.Constant.RightButton);
            }
        }
    }
}
