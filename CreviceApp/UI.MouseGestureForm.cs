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
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using WinAPI.WindowsHookEx;
    
    public class MouseGestureForm : Form
    {
        private const int WM_DISPLAYCHANGE = 0x007E;

        protected Core.FSM.GestureMachine GestureMachine { get; private set; }

        private readonly LowLevelMouseHook mouseHook;
        protected readonly AppGlobal Global;

        // Designer needs this dummy constuctor
        public MouseGestureForm() : this(new AppGlobal())
        {

        }

        public MouseGestureForm(AppGlobal Global)
        {
            this.mouseHook = new LowLevelMouseHook(MouseProc);
            this.Global = Global;
        }

        protected string UserDirectory
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Application.CompanyName,
                    Application.ProductName);
            }
        }

        protected string UserScriptFile
        {
            get
            {
                return Path.Combine(UserDirectory, "default.csx");
            }
        }

        private string GetDefaultUserScript()
        {
            var dir = UserDirectory;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var script = UserScriptFile;
            if (!File.Exists(script))
            {
                File.WriteAllText(script, Encoding.UTF8.GetString(Properties.Resources.DefaultUserScript), Encoding.UTF8);
            }
            return File.ReadAllText(script, Encoding.UTF8);
        }

        private IEnumerable<Core.GestureDefinition> EvaluateUserScriptAsync(Core.UserScriptExecutionContext ctx)
        {
            Debug.Print("Trying to compile and evaluate the user script");
            var script = CSharpScript.Create(
                GetDefaultUserScript(),
                ScriptOptions.Default,
                globalsType: typeof(Core.UserScriptExecutionContext));
            var diagnotstics = script.Compile();
            Debug.Print("compiled");
            foreach (var dg in diagnotstics.Select((v, i) => new { v, i }))
            {
                Debug.Print("[{0}] {1}", dg.i, dg.v.ToString());
            }
            script.RunAsync(ctx).Wait();
            Debug.Print("evaluated");
            return ctx.GetGestureDefinition();
        }

        protected void InitializeGestureMachine()
        {
            var ctx = new Core.UserScriptExecutionContext(Global);
            var gestureDef = EvaluateUserScriptAsync(ctx);
            this.GestureMachine = new Core.FSM.GestureMachine(Global.UserConfig, gestureDef);
        }

        protected void StartCapture()
        {
            if (GestureMachine == null)
            {
                throw new InvalidOperationException();
            }
            mouseHook.SetHook();
        }

        protected void EndCapture()
        {
            mouseHook.Unhook();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            GestureMachine.Dispose();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_DISPLAYCHANGE:
                    GestureMachine.Reset();
                    break;
            }
            base.WndProc(ref m);
        }

        public WindowsHook.Result MouseProc(LowLevelMouseHook.Event evnt, LowLevelMouseHook.MSLLHOOKSTRUCT data)
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
            return WindowsHook.Result.Transfer;
        }

        private WindowsHook.Result Convert(bool consumed)
        {
            if (consumed)
            {
                return WindowsHook.Result.Cancel;
            }
            else
            {
                return WindowsHook.Result.Transfer;
            }
        }
    }
}
