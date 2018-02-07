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
    using Crevice.UserScript;

    public partial class MainForm : MouseGestureForm
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

        private readonly TooltipNotifier Tooltip;

        private FileSystemWatcher UserScriptWatcher;

        private LauncherForm launcherForm = null;

        public MainForm(GlobalConfig globalConfig) : base(globalConfig)
        {
            Tooltip = new TooltipNotifier(this);
            UserScriptWatcher = new UserScript.DirectoryWatcher(this, globalConfig.UserDirectory, "*.csx");
            InitializeComponent();
        }

        private void ReloadGestureMachine(object source, FileSystemEventArgs e)
        {
            Task.Run(() =>
            {
                ReloadableGestureMachine.HotReload();
            });
        }

        private void SetupUserScriptWatcher()
        {
            UserScriptWatcher.Changed += new FileSystemEventHandler(ReloadGestureMachine);
            UserScriptWatcher.Created += new FileSystemEventHandler(ReloadGestureMachine);
            UserScriptWatcher.Renamed += new RenamedEventHandler(ReloadGestureMachine);
            UserScriptWatcher.EnableRaisingEvents = true;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RegisterNotifyIcon();
            UpdateTasktrayMessage("Configuration not loaded.");
            SetupUserScriptWatcher();
            ReloadableGestureMachine.HotReload();
            try
            {
                EnableHook = true;
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
            if (!GlobalConfig.CLIOption.NoGUI)
            {
                while (true)
                {
                    var stopwatch = Stopwatch.StartNew();
                    NotifyIcon1.Visible = true;
                    stopwatch.Stop();
                    if (stopwatch.ElapsedMilliseconds < 4000)
                    {
                        break;
                    }
                    NotifyIcon1.Visible = false;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            UserScriptWatcher.EnableRaisingEvents = false;
            UserScriptWatcher.Dispose();
            try
            {
                EnableHook = false;
            }
            catch (Win32Exception)
            {
                ShowFatalErrorDialog("UnhookWindowsHookEx(WH_MOUSE_LL) was failed.");
            }
            NotifyIcon1.Visible = false;
            Tooltip.Dispose();
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

        public void UpdateTasktrayMessage(string message)
        {
            var text = string.Format("Crevice {0}\n{1}", Application.ProductVersion, message);
            InvokeProperly(delegate ()
            {
                if (!GlobalConfig.CLIOption.NoGUI)
                {
                    NotifyIcon1.Text = text.Length > 63 ? text.Substring(0, 63) : text;
                }
            });
        }

        public void UpdateTasktrayMessage(string formattedtext, params object[] args)
        {
            UpdateTasktrayMessage(string.Format(formattedtext, args));
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
                Tooltip.Show(text, point, duration);
            });
        }

        public void ShowBalloon(string text, string title, ToolTipIcon icon, int timeout)
        {
            InvokeProperly(delegate ()
            {
                Verbose.Print("ShowBalloon: {0}", text);
                if (!GlobalConfig.CLIOption.NoGUI)
                {
                    NotifyIcon1.BalloonTipText = text;
                    NotifyIcon1.BalloonTipTitle = title;
                    NotifyIcon1.BalloonTipIcon = icon;
                    NotifyIcon1.ShowBalloonTip(timeout);
                }
            });
        }

        public void StartExternalProcess(string fileName)
        {
            InvokeProperly(delegate ()
            {
                if (!GlobalConfig.CLIOption.NoGUI)
                {
                    Process.Start(fileName);
                }
            });
        }
        
        public void StartExternalProcess(string fileName, string arguments)
        {
            InvokeProperly(delegate ()
            {
                if (!GlobalConfig.CLIOption.NoGUI)
                {
                    Process.Start(fileName, arguments);
                }
            });
        }

        private void OpenLauncherForm()
        {
            if (launcherForm == null || launcherForm.IsDisposed)
            {
                launcherForm = new LauncherForm(GlobalConfig);
                launcherForm.Opacity = 0;
                launcherForm.Show();
            }
            else
            {
                launcherForm.Activate();
            }
        }

        private void NotifyIcon1_Click(object sender, EventArgs e)
            => OpenLauncherForm();

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

        private void OpenWithNotepad(string path, string text)
        {
            try
            {
                File.WriteAllText(path, text);
                StartExternalProcess("notepad.exe", path);
            } 
            catch (Exception) { }
        }

        private void NotifyIcon1_BalloonTipClicked(object sender, EventArgs e)
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
