using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp.Core
{
    public class UserScriptExecutionContext
    {
        public readonly DSL.Def.LeftButton   LeftButton   = DSL.Def.Constant.LeftButton;
        public readonly DSL.Def.MiddleButton MiddleButton = DSL.Def.Constant.MiddleButton;
        public readonly DSL.Def.RightButton  RightButton  = DSL.Def.Constant.RightButton;
        public readonly DSL.Def.WheelDown    WheelDown    = DSL.Def.Constant.WheelDown;
        public readonly DSL.Def.WheelUp      WheelUp      = DSL.Def.Constant.WheelUp;
        public readonly DSL.Def.WheelLeft    WheelLeft    = DSL.Def.Constant.WheelLeft;
        public readonly DSL.Def.WheelRight   WheelRight   = DSL.Def.Constant.WheelRight;
        public readonly DSL.Def.X1Button     X1Button     = DSL.Def.Constant.X1Button;
        public readonly DSL.Def.X2Button     X2Button     = DSL.Def.Constant.X2Button;

        public readonly DSL.Def.MoveUp    MoveUp    = DSL.Def.Constant.MoveUp;
        public readonly DSL.Def.MoveDown  MoveDown  = DSL.Def.Constant.MoveDown;
        public readonly DSL.Def.MoveLeft  MoveLeft  = DSL.Def.Constant.MoveLeft;
        public readonly DSL.Def.MoveRight MoveRight = DSL.Def.Constant.MoveRight;

        public readonly WinAPI.SendInput.SingleInputSender SendInput = new WinAPI.SendInput.SingleInputSender();
        
        public Config.UserConfig Config
        {
          get { return Global.UserConfig; }
        }

        private readonly DSL.Root root = new DSL.Root();

        private readonly AppGlobal Global;
        
        public UserScriptExecutionContext(AppGlobal Global)
        {
            this.Global = Global;
        }
        
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

        public void Tooltip(string text)
        {
            Tooltip(text, Global.UserConfig.UI.TooltipPositionBinding(WinAPI.Window.Window.GetPhysicalCursorPos()));
        }

        public void Tooltip(string text, Point point)
        {
            Tooltip(text, point, Global.UserConfig.UI.TooltipTimeout);
        }

        public void Tooltip(string text, Point point, int duration)
        {
            Global.MainForm.ShowTooltip(text, point, duration);
        }

        public void Balloon(string text)
        {
            Balloon(text, Global.UserConfig.UI.BalloonTimeout);
        }

        public void Balloon(string text, int timeout)
        {
            Global.MainForm.ShowBalloon(text, "", ToolTipIcon.None, timeout);
        }

        public void Balloon(string text, string title)
        {
            Balloon(text, title, ToolTipIcon.None, Global.UserConfig.UI.BalloonTimeout);
        }

        public void Balloon(string text, string title, int timeout)
        {
            Global.MainForm.ShowBalloon(text, title, ToolTipIcon.None, timeout);
        }

        public void Balloon(string text, string title, ToolTipIcon icon)
        {
            Balloon(text, title, icon, Global.UserConfig.UI.BalloonTimeout);
        }

        public void Balloon(string text, string title, ToolTipIcon icon, int timeout)
        {
            Global.MainForm.ShowBalloon(text, title, icon, timeout);
        }
    }
}
