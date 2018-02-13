using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crevice.UserScript
{
    using Crevice.Core.FSM;
    using Crevice.Config;
    using Crevice.GestureMachine;
    using Crevice.UserScript.Keys;

    public class UserScriptExecutionContext
    {
        private readonly GestureMachineProfileManager ProfileManager = new GestureMachineProfileManager();

        public GestureMachineProfile CurrentProfile
        {
            get
            {
                if (!ProfileManager.Profiles.Any())
                {
                    DeclareProfile("Default");
                }
                return ProfileManager.Profiles.Last();
            }
        }

        public void DeclareProfile(string profileName)
            => ProfileManager.DeclareProfile(profileName);

        public IReadOnlyList<GestureMachineProfile> Profiles
            => ProfileManager.Profiles;

        public readonly SupportedKeys.LogicalKeyDeclaration Keys = SupportedKeys.Keys;

        #region Crevice3 compatible variables.
        public Core.Keys.LogicalSingleThrowKey WheelDown => Keys.WheelDown;
        public Core.Keys.LogicalSingleThrowKey WheelUp => Keys.WheelUp;
        public Core.Keys.LogicalSingleThrowKey WheelLeft => Keys.WheelLeft;
        public Core.Keys.LogicalSingleThrowKey WheelRight => Keys.WheelRight;

        public LogicalSystemKey LeftButton => Keys.LButton;
        public LogicalSystemKey MiddleButton => Keys.MButton;
        public LogicalSystemKey RightButton => Keys.RButton;
        public LogicalSystemKey X1Button => Keys.XButton1;
        public LogicalSystemKey X2Button => Keys.XButton2;

        public Core.Stroke.StrokeDirection MoveUp => Keys.MoveUp;
        public Core.Stroke.StrokeDirection MoveDown => Keys.MoveDown;
        public Core.Stroke.StrokeDirection MoveLeft => Keys.MoveLeft;
        public Core.Stroke.StrokeDirection MoveRight => Keys.MoveRight;
        #endregion

        public readonly IReadOnlyList<SupportedKeys.PhysicalKeyDeclaration> PhysicalKeys =
            new List<SupportedKeys.PhysicalKeyDeclaration>() { SupportedKeys.PhysicalKeys };

        public readonly WinAPI.SendInput.SingleInputSender SendInput = new WinAPI.SendInput.SingleInputSender();

        private readonly GlobalConfig GlobalConfig;
        
        public UserScriptExecutionContext(GlobalConfig globalConfig)
        {
            GlobalConfig = globalConfig;
        }

        public IGestureMachine RootGestureMachine 
            => GlobalConfig.MainForm.GestureMachine;

        public UserConfig Config
            => CurrentProfile.UserConfig;

        public Core.DSL.WhenElement<EvaluationContext, ExecutionContext> 
            When(Core.Context.EvaluateAction<EvaluationContext> func)
            => CurrentProfile.RootElement.When(func);

        public void Tooltip(string text)
            => Tooltip(text, Config.UI.TooltipPositionBinding(WinAPI.Window.Window.GetPhysicalCursorPos()));

        public void Tooltip(string text, Point point)
            => Tooltip(text, point, Config.UI.TooltipTimeout);

        public void Tooltip(string text, Point point, int duration)
            => GlobalConfig.MainForm.ShowTooltip(text, point, duration);

        public void Balloon(string text)
            => Balloon(text, Config.UI.BalloonTimeout);
        
        public void Balloon(string text, int timeout)
            => GlobalConfig.MainForm.ShowBalloon(text, "", ToolTipIcon.None, timeout);

        public void Balloon(string text, string title)
            => Balloon(text, title, ToolTipIcon.None, Config.UI.BalloonTimeout);

        public void Balloon(string text, string title, int timeout)
            => GlobalConfig.MainForm.ShowBalloon(text, title, ToolTipIcon.None, timeout);

        public void Balloon(string text, string title, ToolTipIcon icon)
            => Balloon(text, title, icon, Config.UI.BalloonTimeout);

        public void Balloon(string text, string title, ToolTipIcon icon, int timeout)
            => GlobalConfig.MainForm.ShowBalloon(text, title, icon, timeout);
    }
}
