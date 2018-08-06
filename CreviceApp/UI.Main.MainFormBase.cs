using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crevice.UI
{
    using Crevice.GestureMachine;
    using Crevice.UserScript;

    public class MainFormBase : MouseGestureForm
    {
        // Forcely make this application invisible from task switcher applications.
        const int WS_EX_TOOLWINDOW = 0x00000080;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | WS_EX_TOOLWINDOW;
                return cp;
            }
        }

        public readonly LauncherForm LauncherForm;

        public MainFormBase(LauncherForm launcherForm)
        {
            this.LauncherForm = launcherForm;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            UpdateTasktrayMessage("Configuration not loaded.");
            try
            {
                HookEnabled = true;
            }
            catch (Win32Exception)
            {
                ShowFatalErrorDialog("SetWindowsHookEX(WH_MOUSE_LL) was failed.");
                Application.Exit();
            }
        }
        
        protected override void OnClosed(EventArgs e)
        {
            try
            {
                HookEnabled = false;
            }
            catch (Win32Exception)
            {
                ShowFatalErrorDialog("UnhookWindowsHookEx(WH_MOUSE_LL) was failed.");
            }
            WinAPI.Console.Console.FreeConsole();
            Logging.Verbose.Enabled = false;
            base.OnClosed(e);
        }

        public void ShowTooltip(string text, Point point, int duration)
            => LauncherForm.ShowTooltip(text, point, duration);

        public void UpdateTasktrayMessage(string message) 
            => LauncherForm.UpdateTasktrayMessage(message);

        public void UpdateTasktrayMessage(string formattedtext, params object[] args)
            => UpdateTasktrayMessage(string.Format(formattedtext, args));
        
        public void ShowFatalErrorDialog(string formattedtext, params object[] objects)
            => LauncherForm.ShowFatalErrorDialog(string.Format(formattedtext, objects));
        
        public void ShowBalloon(string text, string title, ToolTipIcon icon, int timeout)
            => LauncherForm.ShowBalloon(text, title, icon, timeout);
        
        public void UpdateTasktrayMessage(IReadOnlyList<GestureMachineProfile> profiles)
        {
            var header = $"Crevice {Application.ProductVersion}";
            var sum = profiles.Sum(p => p.RootElement.GestureCount);
            var gesturesMessage = $"Gestures: {sum}";
            var totalMessage = $"Total: {sum}";
            if (profiles.Count > 1)
            {
                var perProfileMessages = Enumerable.
                    Range(0, profiles.Count).
                    Select(n => profiles.Take(n + 1).
                        Select(p => $"({p.ProfileName}): {p.RootElement.GestureCount}").
                        Aggregate("", (a, b) => a + "\r\n" + b)).
                        Select(s => s.Trim()).
                    Reverse().ToList();

                if ((header + "\r\n" + perProfileMessages.First()).Length < 63)
                {
                    UpdateTasktrayMessage(perProfileMessages.First());
                    return;
                }

                perProfileMessages = perProfileMessages.Skip(1).
                    Where(s => (header + "\r\n" + s + "\r\n...\r\n" + totalMessage).Length < 63).ToList();

                if (perProfileMessages.Any())
                {
                    UpdateTasktrayMessage(perProfileMessages.First() + "\r\n...\r\n" + totalMessage);
                    return;
                }
            }
            UpdateTasktrayMessage(gesturesMessage);
        }

        private string GetActivatedMessage(GestureMachineCluster gmCluster)
            => $"{gmCluster.Profiles.Select(p => p.RootElement.GestureCount).Sum()} Gestures Activated";

        public void ShowInfoBalloon(
            GestureMachineCluster gmCluster)
        {
            LauncherForm.LastErrorMessage = "";
            ShowBalloon(GetActivatedMessage(gmCluster), "", ToolTipIcon.Info, 10000);
        }

        public void ShowWarningBalloon(
            GestureMachineCluster gmCluster,
            Exception runtimeError)
        {
            LauncherForm.LastErrorMessage = runtimeError.ToString();
            var text = "The configuration may be loaded incompletely due to an error on the UserScript evaluation.\r\n" +
                "Click to view the detail.";
            ShowBalloon(text, GetActivatedMessage(gmCluster), ToolTipIcon.Warning, 10000);
        }

        public void ShowErrorBalloon(
            System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> errors)
        {
            var errorMessage = UserScript.GetPrettyErrorMessage(errors);
            LauncherForm.LastErrorMessage = errorMessage;

            var title = "UserScript Compilation Error";
            var text = $"{errors.Count()} error(s) found in the UserScript.\r\nClick to view the detail.";
            ShowBalloon(text, title, ToolTipIcon.Error, 10000);
        }
    }
}
