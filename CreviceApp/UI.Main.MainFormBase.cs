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
    using Crevice.Logging;
    using Crevice.Config;
    using Crevice.GestureMachine;
    using Crevice.UserScript;

    public class MainFormBase : MouseGestureForm
    {
        private string _lastErrorMessage = "";
        public string LastErrorMessage
        {
            get { return _lastErrorMessage; }
            set
            {
                Verbose.Print("LastErrorMessage: {0}", value);
                _lastErrorMessage = value;
            }
        }

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

        protected virtual internal GlobalConfig Config { get; }

        private readonly TooltipNotifier _tooltip;

        private LauncherForm _launcherForm = null;

        public MainFormBase() 
            : base()
        {
            _tooltip = new TooltipNotifier(this);
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
            Verbose.Print("CreviceApp was started.");
        }
        
        protected void RegisterNotifyIcon(NotifyIcon notifyIcon)
        {
            if (!Config.CLIOption.NoGUI)
            {
                while (true)
                {
                    var stopwatch = Stopwatch.StartNew();
                    notifyIcon.Visible = true;
                    stopwatch.Stop();
                    if (stopwatch.ElapsedMilliseconds < 4000)
                    {
                        break;
                    }
                    notifyIcon.Visible = false;
                }
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
            base.OnClosed(e);
            _tooltip.Dispose();
            Verbose.Print("CreviceApp was ended.");
            WinAPI.Console.Console.FreeConsole();
        }

        private void InvokeProperly(MethodInvoker invoker)
        {
            if (InvokeRequired)
            {
                Invoke(invoker);
            }
            else
            {
                invoker.Invoke();
            }
        }

        protected void UpdateTasktrayMessage(NotifyIcon notifyIcon, string message)
        {
            var header = string.Format("Crevice {0}", Application.ProductVersion);
            var text = header + "\r\n" + message;
            InvokeProperly(delegate ()
            {
                if (!Config.CLIOption.NoGUI)
                {
                    notifyIcon.Text = text.Length > 63 ? text.Substring(0, 60) + "..." : text;
                }
            });
        }

        public virtual void UpdateTasktrayMessage(string message) { }

        public void UpdateTasktrayMessage(string formattedtext, params object[] args)
            => UpdateTasktrayMessage(string.Format(formattedtext, args));
        
        public void ShowFatalErrorDialog(string text)
        {
            InvokeProperly(delegate ()
            {
                Verbose.Error(text);
                MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        public void ShowFatalErrorDialog(string formattedtext, params object[] objects)
            => ShowFatalErrorDialog(string.Format(formattedtext, objects));

        public void ShowTooltip(string text, Point point, int duration)
        {
            InvokeProperly(delegate ()
            {
                Verbose.Print("ShowTooltip: {0}", text);
                _tooltip.Show(text, point, duration);
            });
        }

        public void ShowBalloon(NotifyIcon notifyIcon, string text, string title, ToolTipIcon icon, int timeout)
        {
            InvokeProperly(delegate ()
            {
                Verbose.Print("ShowBalloon: {0}", text);
                if (!Config.CLIOption.NoGUI)
                {
                    notifyIcon.BalloonTipText = text;
                    notifyIcon.BalloonTipTitle = title;
                    notifyIcon.BalloonTipIcon = icon;
                    notifyIcon.ShowBalloonTip(timeout);
                }
            });
        }

        public virtual void ShowBalloon(string text, string title, ToolTipIcon icon, int timeout) { }

        public void StartExternalProcess(string fileName)
        {
            InvokeProperly(delegate ()
            {
                if (!Config.CLIOption.NoGUI)
                {
                    Process.Start(fileName);
                }
            });
        }
        
        public void StartExternalProcess(string fileName, string arguments)
        {
            InvokeProperly(delegate ()
            {
                if (!Config.CLIOption.NoGUI)
                {
                    try
                    {
                        using (Verbose.PrintElapsed(
                            "StartExternalProcess(filename={0}, arguments={1})", 
                            fileName,
                            arguments))
                        {
                            Process.Start(fileName, arguments);
                        }
                    }
                    catch (Exception ex)
                    {
                        Verbose.Error("An exception was thrown while executing an external process: {0}", ex.ToString());
                    }
                }
            });
        }

        protected void OpenLauncherForm()
        {
            if (_launcherForm == null || _launcherForm.IsDisposed)
            {
                _launcherForm = new LauncherForm(this);
                _launcherForm.Opacity = 0;
                _launcherForm.Show();
            }
            else
            {
                _launcherForm.Activate();
            }
        }

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
            LastErrorMessage = "";
            ShowBalloon(GetActivatedMessage(gmCluster), "", ToolTipIcon.Info, 10000);
        }

        public void ShowWarningBalloon(
            GestureMachineCluster gmCluster,
            Exception runtimeError)
        {
            LastErrorMessage = runtimeError.ToString();
            var text = "The configuration may be loaded incompletely due to an error on the UserScript evaluation.\r\n" +
                "Click to view the detail.";
            ShowBalloon(text, GetActivatedMessage(gmCluster), ToolTipIcon.Warning, 10000);
        }

        public void ShowErrorBalloon(
            System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> errors)
        {
            var errorMessage = UserScript.GetPrettyErrorMessage(errors);
            LastErrorMessage = errorMessage;

            var title = "UserScript Compilation Error";
            var text = $"{errors.Count()} error(s) found in the UserScript.\r\nClick to view the detail.";
            ShowBalloon(text, title, ToolTipIcon.Error, 10000);
        }

        private void OpenWithNotepad(string path, string text)
        {
            try
            {
                File.WriteAllText(path, text);
                StartExternalProcess("notepad.exe", path);
            } 
            catch (Exception ex)
            {
                Verbose.Error("An exception was thrown while writing a file: {0}", ex.ToString());
            }
        }

        public void OpenUserScriptWithNotepad()
            => StartExternalProcess("explorer.exe", "/select, " + Config.UserScriptFile);

        public void OpenLastErrorMessageWithNotepad()
        {
            if (String.IsNullOrEmpty(LastErrorMessage))
            {
                OpenLauncherForm();
            }
            else
            {
                var tempPath = Path.Combine(Path.GetTempPath(), "Crevice4.ErrorInformation.txt");
                OpenWithNotepad(tempPath, LastErrorMessage);
            }
        }
    }
}
