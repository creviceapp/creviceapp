using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crevice.UserScript
{
    using Crevice.Config;
    using Crevice.Element;
    using Crevice.GestureMachine;
    using Crevice.UserScript.Keys;

    public class GestureMachineExecutionProfile
    {
        public readonly CallbackManager CallbackManager = new CallbackManager();

        public readonly RootElement RootElement = new RootElement();

        public readonly UserConfig UserConfig;

        public readonly string ProfileName;
        
        public GestureMachineExecutionProfile(string profileName)
        {
            ProfileName = profileName;
            UserConfig = new UserConfig(CallbackManager.Receiver);
        }
    }
    
    public class GestureMachineExecutionProfileManager
{
        private readonly List<GestureMachineExecutionProfile> profiles = new List<GestureMachineExecutionProfile>();
        
        public IReadOnlyList<GestureMachineExecutionProfile> Profiles => profiles;

        public void DeclareProfile(string profileName)
            => profiles.Add(new GestureMachineExecutionProfile(profileName));
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

        private readonly GlobalConfig GlobalConfig;
        
        public UserScriptExecutionContext(GlobalConfig globalConfig)
        {
            GlobalConfig = globalConfig;
        }

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
