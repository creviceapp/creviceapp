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

        private FileSystemWatcher UserScriptWatcher;

        private LauncherForm launcherForm = null;

        public MainForm(AppGlobal Global) : base(Global)
        {
            this.tooltip = new UI.TooltipNotifier(this);
            this.UserScriptWatcher = UserScript.GetWatcher(this);
            InitializeComponent();
        }

        private async void OnUserScriptUpdate() {
            await Task.Run(() =>
            {
                ReloadGestureMachine();
            });
        }

        private void ReloadGestureMachine()
        {
            try
            {
                ReloadableGestureMachine.RequestReload();
                InvokeProperly(delegate
                {
                    if (!Global.CLIOption.NoGUI)
                    {
                        ShowBalloon(string.Format("{0} gesture definitions were loaded", ReloadableGestureMachine.Instance.GestureDefinition.Count()),
                        "Crevice",
                        ToolTipIcon.Info, 10000);
                        UpdateTasktrayIconText("{0}\nGestures: {1}", "Crevice", ReloadableGestureMachine.Instance.GestureDefinition.Count());
                    }
                });
            }
            catch (Exception ex)
            {
                InvokeProperly(delegate
                {
                    ShowFatalErrorDialog("Encounter an error when loading the user script: {0}", ex.ToString());
                    if (!Global.CLIOption.NoGUI)
                    {
                        UpdateTasktrayIconText("{0}\nEncounter an error when loading the user script.", "Crevice");
                    }
                });
            }
        }

        private void ReloadGestureMachine(object source, FileSystemEventArgs e)
        {
            ReloadGestureMachine();
        }

        private void SetupUserScriptWatcher()
        {
            UserScriptWatcher.Changed += new FileSystemEventHandler(ReloadGestureMachine);
            UserScriptWatcher.Created += new FileSystemEventHandler(ReloadGestureMachine);
            UserScriptWatcher.Renamed += new RenamedEventHandler(ReloadGestureMachine);
            UserScriptWatcher.EnableRaisingEvents = true;
        }

        private void UpdateTasktrayIconText(string formatText, params object[] args)
        {
            var text = string.Format(formatText, args);
            notifyIcon1.Text = text;
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (!Global.CLIOption.NoGUI)
            {
                RegisterNotifyIcon();
            }

            SetupUserScriptWatcher();

            await Task.Run(() =>
            {
                ReloadGestureMachine();
            });

            try
            {
                CaptureMouse = true;
            }
            catch (Win32Exception)
            {
                ShowFatalErrorDialog("SetWindowsHookEX(WH_MOUSE_LL) was failed.");
                Application.Exit();
            }
            Verbose.Print("CreviceApp is started.");
        }
        
        private void RegisterNotifyIcon()
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            UserScriptWatcher.EnableRaisingEvents = false;
            UserScriptWatcher.Dispose();
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
            Verbose.Print("CreviceApp is ended.");
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

        public void ShowFatalErrorDialog(string message)
        {
            Verbose.Print(message);
            MessageBox.Show(message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public void ShowFatalErrorDialog(string template, params object[] objects)
        {
            ShowFatalErrorDialog(string.Format(template, objects));
        }

        public void ShowTooltip(string text, Point point, int duration)
        {
            InvokeProperly(delegate ()
            {
                tooltip.Show(text, point, duration);
            });
        }

        public void ShowBalloon(string text, string title, ToolTipIcon icon, int timeout)
        {
            InvokeProperly(delegate ()
            {
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
