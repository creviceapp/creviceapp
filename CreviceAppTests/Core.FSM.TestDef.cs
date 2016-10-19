using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp.Core.FSM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM.Tests
{
    public static class TestDef
    {
        public class ConstantSingleton
        {
            private static ConstantSingleton singleton = new ConstantSingleton();

            public readonly List<DSL.Def.Button> Buttons = new List<DSL.Def.Button>()
            {
                DSL.Def.Constant.LeftButton,
                DSL.Def.Constant.MiddleButton,
                DSL.Def.Constant.RightButton,
                DSL.Def.Constant.WheelUp,
                DSL.Def.Constant.WheelDown,
                DSL.Def.Constant.WheelLeft,
                DSL.Def.Constant.WheelRight,
                DSL.Def.Constant.X1Button,
                DSL.Def.Constant.X2Button
            };

            public readonly List<DSL.Def.Button> SingleTriggerButtons = new List<DSL.Def.Button>()
            {
                DSL.Def.Constant.WheelUp,
                DSL.Def.Constant.WheelDown,
                DSL.Def.Constant.WheelLeft,
                DSL.Def.Constant.WheelRight
            };

            public readonly List<DSL.Def.Button> DoubleTriggerButtons = new List<DSL.Def.Button>()
            {
                DSL.Def.Constant.LeftButton,
                DSL.Def.Constant.MiddleButton,
                DSL.Def.Constant.RightButton,
                DSL.Def.Constant.X1Button,
                DSL.Def.Constant.X2Button
            };

            public readonly List<DSL.Def.Move> Moves = new List<DSL.Def.Move>()
            {
                DSL.Def.Constant.MoveUp,
                DSL.Def.Constant.MoveDown,
                DSL.Def.Constant.MoveLeft,
                DSL.Def.Constant.MoveRight
            };

            public static ConstantSingleton GetInstance()
            {
                return singleton;
            }
        }

        public static ConstantSingleton Constant
        {
            get { return ConstantSingleton.GetInstance(); }
        }
    }
}