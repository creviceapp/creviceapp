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
        private static Microsoft.Win32.RegistryKey AutorunRegistry()
        {
            return Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
        }
        private static bool AutoRun
        {
            get
            {
                var registry = AutorunRegistry();
                try
                {
                    var res = registry.GetValue(Application.ProductName);
                    return res != null &&
                           (string)res == Application.ExecutablePath;
                }
                finally
                {
                    registry.Close();
                }
            }
            set
            {
                if (value)
                {
                    var registry = AutorunRegistry();
                    registry.SetValue(Application.ProductName, Application.ExecutablePath);
                    Debug.Print("Autorun was set to true");
                    registry.Close();
                }
                else
                {
                    var registry = AutorunRegistry();
                    try
                    {
                        registry.DeleteValue(Application.ProductName);
                        Debug.Print("Autorun was set to false");
                    }
                    catch (ArgumentException) { }
                    registry.Close();
                }
            }
        }

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

        public MainForm(AppGlobal Global) : base(Global)
        {
            this.tooltip = new UI.TooltipNotifier(this);
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            checkBox1.Checked = AutoRun;
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
                    RegisterNotifyIcon();
                    ShowBaloon(string.Format("{0} gesture definitions were loaded", GestureMachine.GestureDefinition.Count()),
                        string.Format("{0} was started", Application.ProductName),
                        ToolTipIcon.Info, 10000);
                    notifyIcon1.Text = string.Format("{0}\nGestures: {1}",
                        Application.ProductName,
                        GestureMachine.GestureDefinition.Count());
                }
                catch (Win32Exception)
                {
                    ShowFatalErrorDialog("SetWindowsHookEX(WH_MOUSE_LL) was failed.");
                    Application.Exit();
                }
            }
            catch(CompilationErrorException ex)
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
        
        private bool closeRequest = false;

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!closeRequest)
            {
                e.Cancel = true;
                AutoRun = checkBox1.Checked;
                Hide();
                ShowInTaskbar = false;
                return;
            }
            base.OnClosing(e);
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
            tooltip.Dispose();
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
            var invoker = (MethodInvoker)delegate()
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

        private void button1_Click(object sender, EventArgs e)
        {
            closeRequest = true;
            AutoRun = checkBox1.Checked;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", UserDirectory);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var form = new ProductInfoForm();
            form.Show();
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            Opacity = 0;
            Show();
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            var rect = Screen.PrimaryScreen.Bounds;
            Left = (rect.Width - Width) / 2;
            Top = (rect.Height - Height) / 2;
            Opacity = 1;
            Activate();
            BringToFront();
        }
    }
}
