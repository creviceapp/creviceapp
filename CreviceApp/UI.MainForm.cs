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
        private LauncherForm launcherForm = null;

        public MainForm(AppGlobal Global) : base(Global)
        {
            this.tooltip = new UI.TooltipNotifier(this);
            InitializeComponent();
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            try
            {
                await Task.Run(() =>
                {
                    InitializeGestureMachine();
                    ReleaseUnusedMemory();
                });
                try
                {
                    StartCapture();
                    Console.WriteLine("CreviceApp is started.");
                    if (!Global.CLIOption.NoGUI)
                    {
                        RegisterNotifyIcon();
                        ShowBaloon(string.Format("{0} gesture definitions were loaded", GestureMachine.GestureDefinition.Count()),
                            string.Format("{0} was started", Application.ProductName),
                            ToolTipIcon.Info, 10000);
                        notifyIcon1.Text = string.Format("{0}\nGestures: {1}",
                            Application.ProductName,
                            GestureMachine.GestureDefinition.Count());
                    }
                }
                catch (Win32Exception)
                {
                    ShowFatalErrorDialog("SetWindowsHookEX(WH_MOUSE_LL) was failed.");
                    Application.Exit();
                }
            }
            catch (CompilationErrorException ex)
            {
                ShowFatalErrorDialog(ex.ToString());
                Application.Exit();
            }
        }

        private void ReleaseUnusedMemory()
        {
            var totalMemory = GC.GetTotalMemory(false);
            Debug.Print("Releasing unused memory");
            GC.Collect(2);
            Debug.Print("GC.GetTotalMemory: {0} -> {1}", totalMemory, GC.GetTotalMemory(false));
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
            try
            {
                EndCapture();
            }
            catch (Win32Exception)
            {
                ShowFatalErrorDialog("UnhookWindowsHookEx(WH_MOUSE_LL) was failed.");
            }
            notifyIcon1.Visible = false;
            tooltip.Dispose();
            WinAPI.Console.Console.FreeConsole();
            Console.WriteLine("CreviceApp is ended.");

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

        public void ShowFatalErrorDialog(string text)
        {
            MessageBox.Show(text,
                "Fatal error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public void ShowTooltip(string text, Point point, int duration)
        {
            var invoker = (MethodInvoker)delegate ()
            {
                tooltip.Show(text, point, duration);
            };
            InvokeProperly(invoker);
        }

        public void ShowBaloon(string text, string title, ToolTipIcon icon, int timeout)
        {
            var invoker = (MethodInvoker)delegate ()
            {
                notifyIcon1.BalloonTipText = text;
                notifyIcon1.BalloonTipTitle = title;
                notifyIcon1.BalloonTipIcon = icon;
                notifyIcon1.ShowBalloonTip(timeout);
            };
            InvokeProperly(invoker);
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
