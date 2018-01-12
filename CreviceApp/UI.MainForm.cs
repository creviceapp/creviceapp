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

namespace CreviceApp
{
    using Microsoft.CodeAnalysis.Scripting;

    public partial class MainForm : MouseGestureForm
    {
        // Force make this application invisible from task switcher applications.
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

        private readonly UI.TooltipNotifier tooltip;

        private FileSystemWatcher userScriptWatcher;

        private LauncherForm launcherForm = null;

        public MainForm(AppGlobal Global) : base(Global)
        {
            this.tooltip = new UI.TooltipNotifier(this);
            this.userScriptWatcher = UserScript.GetWatcher(this);
            InitializeComponent();
        }

        private void ReloadGestureMachine(object source, FileSystemEventArgs e)
        {
            Task.Run(() =>
            {
                ReloadableGestureMachine.RequestReload();
            });
        }

        private void SetupUserScriptWatcher()
        {
            userScriptWatcher.Changed += new FileSystemEventHandler(ReloadGestureMachine);
            userScriptWatcher.Created += new FileSystemEventHandler(ReloadGestureMachine);
            userScriptWatcher.Renamed += new RenamedEventHandler(ReloadGestureMachine);
            userScriptWatcher.EnableRaisingEvents = true;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RegisterNotifyIcon();
            UpdateTasktrayIconText("Crevice\n Gestures not loaded.");
            SetupUserScriptWatcher();
            ReloadableGestureMachine.RequestReload();
            try
            {
                CaptureMouse = true;
            }
            catch (Win32Exception)
            {
                ShowFatalErrorDialog("SetWindowsHookEX(WH_MOUSE_LL) was failed.");
                Application.Exit();
            }
            Verbose.Print("CreviceApp was started.");
        }
        
        private void RegisterNotifyIcon()
        {
            if (!Global.CLIOption.NoGUI)
            {
                while (true)
                {
                    var stopwatch = Stopwatch.StartNew();
                    notifyIcon1.Visible = true;
                    stopwatch.Stop();
                    if (stopwatch.ElapsedMilliseconds < 4000)
                    {
                        break;
                    }
                    notifyIcon1.Visible = false;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            userScriptWatcher.EnableRaisingEvents = false;
            userScriptWatcher.Dispose();
            try
            {
                CaptureMouse = false;
            }
            catch (Win32Exception)
            {
                ShowFatalErrorDialog("UnhookWindowsHookEx(WH_MOUSE_LL) was failed.");
            }
            notifyIcon1.Visible = false;
            tooltip.Dispose();
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

        public void UpdateTasktrayIconText(string text)
        {
            InvokeProperly(delegate ()
            {
                if (!Global.CLIOption.NoGUI)
                {
                    notifyIcon1.Text = text.Length > 63 ? text.Substring(0, 63) : text;
                }
            });
        }

        public void UpdateTasktrayIconText(string formattedtext, params object[] args)
        {
            UpdateTasktrayIconText(string.Format(formattedtext, args));
        }
        
        public void ShowFatalErrorDialog(string text)
        {
            InvokeProperly(delegate ()
            {
                Verbose.Print("ShowFatalErrorDialog: {0}", text);
                MessageBox.Show(text,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            });
        }

        public void ShowFatalErrorDialog(string formattedtext, params object[] objects)
        {
            ShowFatalErrorDialog(string.Format(formattedtext, objects));
        }

        public void ShowTooltip(string text, Point point, int duration)
        {
            InvokeProperly(delegate ()
            {
                Verbose.Print("ShowTooltip: {0}", text);
                tooltip.Show(text, point, duration);
            });
        }

        public void ShowBalloon(string text, string title, ToolTipIcon icon, int timeout)
        {
            InvokeProperly(delegate ()
            {
                Verbose.Print("ShowBalloon: {0}", text);
                if (!Global.CLIOption.NoGUI)
                {
                    notifyIcon1.BalloonTipText = text;
                    notifyIcon1.BalloonTipTitle = title;
                    notifyIcon1.BalloonTipIcon = icon;
                    notifyIcon1.ShowBalloonTip(timeout);
                }
            });
        }
        
        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            if (launcherForm == null || launcherForm.IsDisposed)
            {
                launcherForm = new LauncherForm(Global);
                launcherForm.Opacity = 0;
                launcherForm.Show();
            }
            else
            {
                launcherForm.Activate();
            }
        }
    }
}
