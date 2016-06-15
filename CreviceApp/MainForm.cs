using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp
{
    using Microsoft.CodeAnalysis.Scripting;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using WinAPI.WindowsHookEx;

    public partial class MainForm : Form
    {
        private static readonly Lazy<MainForm> instance = new Lazy<MainForm>(() => 
        {
            return new MainForm(); }
        );
        public static MainForm Instance
        {
            get
            {
                return instance.Value;
            }
        }

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

        private const string url = "https://github.com/rubyu/CreviceApp";

        private const int WM_DISPLAYCHANGE = 0x007E;

        private readonly UI.TooltipNotifier tooltip;
        private readonly LowLevelMouseHook mouseHook;
        private readonly Core.Config.UserConfig userConfig;
        private readonly Core.FSM.GestureMachine GestureMachine;

        private string GetDefaultUserScript()
        {
            return Encoding.UTF8.GetString(Properties.Resources.DefaultUserScript);
        }

        public async void EvaluateUserScript(Core.UserScriptExecutionContext ctx)
        {
            var script = CSharpScript.Create(
                GetDefaultUserScript(), 
                ScriptOptions.Default
                    .WithImports("System.Diagnostics")
                    .WithReferences(typeof(object).Assembly),
                globalsType: typeof(Core.UserScriptExecutionContext));
            Debug.Print("compiled");
            await script.RunAsync(ctx);
            Debug.Print("evaluated");
        }

        public MainForm()
        {
            this.tooltip = new UI.TooltipNotifier(this);
            this.mouseHook = new LowLevelMouseHook(MouseProc);
            this.userConfig = new Core.Config.UserConfig();
            var ctx = new Core.UserScriptExecutionContext(this.userConfig);
            EvaluateUserScript(ctx);
            this.GestureMachine = new Core.FSM.GestureMachine(ctx.GetGestureDefinition());
            //this.GestureMachine = new Core.FSM.GestureMachine(new User.UserConfig().GetGestureDefinition());
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            switch(m.Msg)
            {
                case WM_DISPLAYCHANGE:
                    GestureMachine.Reset();
                    break;
            }
            base.WndProc(ref m);
        }

        public LowLevelMouseHook.Result MouseProc(LowLevelMouseHook.Event evnt, LowLevelMouseHook.MSLLHOOKSTRUCT data)
        {
            if (data.fromCreviceApp)
            {
                Debug.Print("{0} was ignored because this event has the signature of CreviceApp", 
                    Enum.GetName(typeof(LowLevelMouseHook.Event), 
                    evnt));
                return WindowsHook.Result.Determine;
            }

            var point = new Point(data.pt.x, data.pt.y);

            switch (evnt)
            {
                case LowLevelMouseHook.Event.WM_MOUSEMOVE:
                    return Convert(GestureMachine.Input(Core.Def.Constant.Move, point));
                case LowLevelMouseHook.Event.WM_LBUTTONDOWN:
                    return Convert(GestureMachine.Input(Core.Def.Constant.LeftButtonDown, point));
                case LowLevelMouseHook.Event.WM_LBUTTONUP:
                    return Convert(GestureMachine.Input(Core.Def.Constant.LeftButtonUp, point));
                case LowLevelMouseHook.Event.WM_RBUTTONDOWN:
                    return Convert(GestureMachine.Input(Core.Def.Constant.RightButtonDown, point));
                case LowLevelMouseHook.Event.WM_RBUTTONUP:
                    return Convert(GestureMachine.Input(Core.Def.Constant.RightButtonUp, point));
                case LowLevelMouseHook.Event.WM_MBUTTONDOWN:
                    return Convert(GestureMachine.Input(Core.Def.Constant.MiddleButtonDown, point));
                case LowLevelMouseHook.Event.WM_MBUTTONUP:
                    return Convert(GestureMachine.Input(Core.Def.Constant.MiddleButtonUp, point));
                case LowLevelMouseHook.Event.WM_MOUSEWHEEL:
                    if (data.mouseData.asWheelDelta.delta < 0)
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.WheelDown, point));
                    }
                    else
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.WheelUp, point));
                    }
                case LowLevelMouseHook.Event.WM_XBUTTONDOWN:
                    if (data.mouseData.asXButton.isXButton1)
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.X1ButtonDown, point));
                    }
                    else
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.X2ButtonDown, point));
                    }
                case LowLevelMouseHook.Event.WM_XBUTTONUP:
                    if (data.mouseData.asXButton.isXButton1)
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.X1ButtonUp, point));
                    }
                    else
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.X2ButtonUp, point));
                    }
                case LowLevelMouseHook.Event.WM_MOUSEHWHEEL:
                    if (data.mouseData.asWheelDelta.delta < 0)
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.WheelRight, point));
                    }
                    else
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.WheelLeft, point));
                    }
            }
            return LowLevelMouseHook.Result.Transfer;
        } 

        private LowLevelMouseHook.Result Convert(bool consumed)
        {
            if (consumed)
            {
                return LowLevelMouseHook.Result.Cancel;
            }
            else
            {
                return LowLevelMouseHook.Result.Transfer;
            }
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            label1.Text = "Product Name: " + Application.ProductName;
            label2.Text = "Product Version: " + Application.ProductVersion;
            label3.Text = "Company Name: " + Application.CompanyName;
            label4.Text = ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute), false)).Copyright;
            label5.Text = ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyDescriptionAttribute), false)).Description;

            checkBox1.Checked = AutoRun;

            try
            {
                mouseHook.SetHook();
            }
            catch (Win32Exception)
            {
                MessageBox.Show("SetWindowsHookEX(WH_MOUSE_LL) was failed.",
                    "Fatal error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private bool closeRequest = false;

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!closeRequest)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
                return;
            }
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            try
            {
                mouseHook.Unhook();
            }
            catch(Win32Exception)
            {
                MessageBox.Show("UnhookWindowsHookEx(WH_MOUSE_LL) was failed.",
                    "Fatal error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            tooltip.Dispose();
            GestureMachine.Dispose();
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

        public void ShowTooltip(string text)
        {
            ShowTooltip(text, userConfig.UI.TooltipPositionBinding(Cursor.Position));
        }
        
        public void ShowTooltip(string text, Point point)
        {
            ShowTooltip(text, point, userConfig.UI.TooltipTimeout);
        }

        public void ShowTooltip(string text, Point point, int duration)
        {
            var invoker = (MethodInvoker)delegate()
            {
                tooltip.Show(text, point, duration);
            };
            InvokeProperly(invoker);
        }
        
        public void ShowBaloon(string text)
        {
            ShowBaloon(text, userConfig.UI.BaloonTimeout);
        }

        public void ShowBaloon(string text, int timeout)
        {
            var invoker = (MethodInvoker)delegate()
            {
                notifyIcon1.BalloonTipText = text;
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(url);
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            Opacity = 0;
            WindowState = FormWindowState.Normal;
            var rect = Screen.PrimaryScreen.Bounds;
            Left = (rect.Width - Width) / 2;
            Top = (rect.Height - Height) / 2;
            Opacity = 1;
        }
    }
}
