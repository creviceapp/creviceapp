using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp.Core
{
    public class GestureMachineExecutionProfile
    {
        public readonly Config.UserConfig UserConfig = new Config.UserConfig();

        public readonly CustomRootElement RootElement = new CustomRootElement();
        
        public readonly string ProfileName;
        
        public GestureMachineExecutionProfile(string profileName)
        {
            ProfileName = profileName;
        }
    }

    public class GestureMachineExecutionProfileManager
    {
        private readonly List<GestureMachineExecutionProfile> profiles = new List<GestureMachineExecutionProfile>();
        public IReadOnlyList<GestureMachineExecutionProfile> Profiles => profiles;

        public void DeclareProfile(string profileName)
        {
            profiles.Add(new GestureMachineExecutionProfile(profileName));
        }
    }

    public class UserScriptExecutionContext
    {
        private readonly GestureMachineExecutionProfileManager ProfileManager = new GestureMachineExecutionProfileManager();

        public GestureMachineExecutionProfile CurrentProfile
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

        public IReadOnlyList<GestureMachineExecutionProfile> Profiles
            => ProfileManager.Profiles;

        public readonly SupportedKeys.LogicalKeyDeclaration Keys = SupportedKeys.Keys;

        public readonly SupportedKeys.PhysicalKeyDeclaration PhysicalKeys = SupportedKeys.PhysicalKeys;

        public readonly WinAPI.SendInput.SingleInputSender SendInput = new WinAPI.SendInput.SingleInputSender();

        private readonly App.AppConfig AppConfig;
        
        public UserScriptExecutionContext(App.AppConfig appConfig)
        {
            AppConfig = appConfig;
        }

        public Config.UserConfig Config
            => CurrentProfile.UserConfig;

        public Crevice.Core.DSL.WhenElement<CustomEvaluationContext, CustomExecutionContext> 
            When(Crevice.Core.Context.EvaluateAction<CustomEvaluationContext> func)
            => CurrentProfile.RootElement.When(func);

        public void Tooltip(string text)
            => Tooltip(text, Config.UI.TooltipPositionBinding(WinAPI.Window.Window.GetPhysicalCursorPos()));

        public void Tooltip(string text, Point point)
            => Tooltip(text, point, Config.UI.TooltipTimeout);

        public void Tooltip(string text, Point point, int duration)
            => AppConfig.MainForm.ShowTooltip(text, point, duration);

        public void Balloon(string text)
            => Balloon(text, Config.UI.BalloonTimeout);
        
        public void Balloon(string text, int timeout)
            => AppConfig.MainForm.ShowBalloon(text, "", ToolTipIcon.None, timeout);

        public void Balloon(string text, string title)
            => Balloon(text, title, ToolTipIcon.None, Config.UI.BalloonTimeout);

        public void Balloon(string text, string title, int timeout)
            => AppConfig.MainForm.ShowBalloon(text, title, ToolTipIcon.None, timeout);

        public void Balloon(string text, string title, ToolTipIcon icon)
            => Balloon(text, title, icon, Config.UI.BalloonTimeout);

        public void Balloon(string text, string title, ToolTipIcon icon, int timeout)
            => AppConfig.MainForm.ShowBalloon(text, title, icon, timeout);
    }
}
