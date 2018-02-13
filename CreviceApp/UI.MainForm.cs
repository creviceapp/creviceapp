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

        private readonly TooltipNotifier _tooltip;

        private FileSystemWatcher _userScriptWatcher;

        private LauncherForm _launcherForm = null;
        
        public MainForm(GlobalConfig globalConfig) : base(globalConfig)
        {
            _tooltip = new TooltipNotifier(this);
            _userScriptWatcher = new UserScript.DirectoryWatcher(this, globalConfig.UserDirectory, "*.csx");
            InitializeComponent();
        }

        private void ReloadGestureMachine(object source, FileSystemEventArgs e)
        {
            Task.Run(() =>
            {
                GestureMachine.HotReload();
            });
        }

        private void SetupUserScriptWatcher()
        {
            _userScriptWatcher.Changed += new FileSystemEventHandler(ReloadGestureMachine);
            _userScriptWatcher.Created += new FileSystemEventHandler(ReloadGestureMachine);
            _userScriptWatcher.Renamed += new RenamedEventHandler(ReloadGestureMachine);
            _userScriptWatcher.EnableRaisingEvents = true;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RegisterNotifyIcon();
            UpdateTasktrayMessage("Configuration not loaded.");
            SetupUserScriptWatcher();
            GestureMachine.HotReload();
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
            _userScriptWatcher.EnableRaisingEvents = false;
            _userScriptWatcher.Dispose();
            try
            {
                EnableHook = false;
            }
            catch (Win32Exception)
            {
                ShowFatalErrorDialog("UnhookWindowsHookEx(WH_MOUSE_LL) was failed.");
            }
            NotifyIcon1.Visible = false;
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

        public void UpdateTasktrayMessage(string message)
        {
            var header = string.Format("Crevice {0}", Application.ProductVersion);
            var text = header + "\r\n" + message;
            InvokeProperly(delegate ()
            {
                if (!GlobalConfig.CLIOption.NoGUI)
                {
                    NotifyIcon1.Text = text.Length > 63 ? text.Substring(0, 60) + "..." : text;
                }
            });
        }
        
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

        private void OpenLauncherForm()
        {
            if (_launcherForm == null || _launcherForm.IsDisposed)
            {
                _launcherForm = new LauncherForm(GlobalConfig);
                _launcherForm.Opacity = 0;
                _launcherForm.Show();
            }
            else
            {
                _launcherForm.Activate();
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
            catch (Exception ex)
            {
                Verbose.Error("An exception was thrown while writing a file: {0}", ex.ToString());
            }
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
