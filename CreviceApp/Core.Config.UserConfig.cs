using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp.Core.Config
{
    public class UserConfig
    {
        public readonly GestureConfig Gesture = new GestureConfig();
        public readonly UserInterfaceConfig UI = new UserInterfaceConfig();

        private readonly DSL.Root root = new DSL.Root();

        protected readonly DSL.Def.LeftButton   LeftButton   = DSL.Def.Constant.LeftButton;
        protected readonly DSL.Def.MiddleButton MiddleButton = DSL.Def.Constant.MiddleButton;
        protected readonly DSL.Def.RightButton  RightButton  = DSL.Def.Constant.RightButton;
        protected readonly DSL.Def.WheelDown    WheelDown    = DSL.Def.Constant.WheelDown;
        protected readonly DSL.Def.WheelUp      WheelUp      = DSL.Def.Constant.WheelUp;
        protected readonly DSL.Def.WheelLeft    WheelLeft    = DSL.Def.Constant.WheelLeft;
        protected readonly DSL.Def.WheelRight   WheelRight   = DSL.Def.Constant.WheelRight;
        protected readonly DSL.Def.X1Button     X1Button     = DSL.Def.Constant.X1Button;
        protected readonly DSL.Def.X2Button     X2Button     = DSL.Def.Constant.X2Button;

        protected readonly DSL.Def.MoveUp    MoveUp    = DSL.Def.Constant.MoveUp;
        protected readonly DSL.Def.MoveDown  MoveDown  = DSL.Def.Constant.MoveDown;
        protected readonly DSL.Def.MoveLeft  MoveLeft  = DSL.Def.Constant.MoveLeft;
        protected readonly DSL.Def.MoveRight MoveRight = DSL.Def.Constant.MoveRight;

        protected readonly WinAPI.SendInput.SingleInputSender SendInput = new WinAPI.SendInput.SingleInputSender();

        public IEnumerable<GestureDefinition> GetGestureDefinition()
        {
            return DSLTreeParser.TreeToGestureDefinition(root)
                .Where(x => x.IsComplete)
                .ToList();
        }

        public DSL.WhenElement @when(DSL.Def.WhenFunc func)
        {
            return root.@when(func);
        }

        public DSL.WhenElement @when(Func<bool> func)
        {
            return root.@when(func);
        }
        
        public void Tooltip(string text)
        {
            MainForm.Instance.ShowTooltip(text, UI.TooltipPositionBinding(Cursor.Position), UI.TooltipTimeout);
        }

        public void Baloon(string text)
        {
            MainForm.Instance.ShowBaloon(text, UI.BaloonTimeout);
        }
    }
}
