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
        public readonly SupportedKeys.LogicalKeyDeclaration Keys = SupportedKeys.Keys;

        public Crevice.Core.Keys.LogicalSingleThrowKey WheelDown => Keys.WheelDown;
        public Crevice.Core.Keys.LogicalSingleThrowKey WheelUp => Keys.WheelUp;
        public Crevice.Core.Keys.LogicalSingleThrowKey WheelLeft => Keys.WheelLeft;
        public Crevice.Core.Keys.LogicalSingleThrowKey WheelRight => Keys.WheelRight;

        public LogicalSystemKey LeftButton => Keys.LeftButton;
        public LogicalSystemKey MiddleButton => Keys.MiddleButton;
        public LogicalSystemKey RightButton => Keys.RightButton;
        public LogicalSystemKey X1Button => Keys.X1Button;
        public LogicalSystemKey X2Button => Keys.X2Button;

        public Crevice.Core.Stroke.StrokeDirection MoveUp => Keys.MoveUp;
        public Crevice.Core.Stroke.StrokeDirection MoveDown => Keys.MoveDown;
        public Crevice.Core.Stroke.StrokeDirection MoveLeft => Keys.MoveLeft;
        public Crevice.Core.Stroke.StrokeDirection MoveRight => Keys.MoveRight;

        public readonly WinAPI.SendInput.SingleInputSender SendInput = new WinAPI.SendInput.SingleInputSender();
        
        public Config.UserConfig Config
        {
          get { return AppConfig.UserConfig; }
        }

        public readonly CustomRootElement Root = new CustomRootElement();

        private readonly CreviceApp.App.AppConfig AppConfig;
        
        public UserScriptExecutionContext(CreviceApp.App.AppConfig AppConfig)
        {
            this.AppConfig = AppConfig;
        }
        
        public Crevice.Core.DSL.WhenElement<CustomEvaluationContext, CustomExecutionContext> 
            When(Crevice.Core.Context.EvaluateAction<CustomEvaluationContext> func)
            => Root.When(func);

        public void Tooltip(string text)
            => Tooltip(text, AppConfig.UserConfig.UI.TooltipPositionBinding(WinAPI.Window.Window.GetPhysicalCursorPos()));

        public void Tooltip(string text, Point point)
            => Tooltip(text, point, AppConfig.UserConfig.UI.TooltipTimeout);

        public void Tooltip(string text, Point point, int duration)
            => AppConfig.MainForm.ShowTooltip(text, point, duration);

        public void Balloon(string text)
            => Balloon(text, AppConfig.UserConfig.UI.BalloonTimeout);
        
        public void Balloon(string text, int timeout)
            => AppConfig.MainForm.ShowBalloon(text, "", ToolTipIcon.None, timeout);

        public void Balloon(string text, string title)
            => Balloon(text, title, ToolTipIcon.None, AppConfig.UserConfig.UI.BalloonTimeout);

        public void Balloon(string text, string title, int timeout)
            => AppConfig.MainForm.ShowBalloon(text, title, ToolTipIcon.None, timeout);

        public void Balloon(string text, string title, ToolTipIcon icon)
            => Balloon(text, title, icon, AppConfig.UserConfig.UI.BalloonTimeout);

        public void Balloon(string text, string title, ToolTipIcon icon, int timeout)
            => AppConfig.MainForm.ShowBalloon(text, title, icon, timeout);
    }
}
