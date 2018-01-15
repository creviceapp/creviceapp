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
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;


namespace CreviceApp
{
    using WinAPI.WindowsHookEx;

    using GetGestureMachineResult = 
        Tuple<Core.FSM.GestureMachine,
            System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic>?,
            Exception>;

    public class ReloadableGestureMachine : IDisposable
    {
        public class NullGestureMachine : Core.FSM.GestureMachine
        {
            public NullGestureMachine() : base(new Core.Config.UserConfig(), new List<Core.GestureDefinition>())
            {

            }
        }

        public class GestureMachineCandidate
        {
            public readonly DateTime Created;
            public readonly bool RestoreFromCache;
            public readonly string UserScriptString;

            private readonly UserScript userScript;
            
            public GestureMachineCandidate(
                UserScript userScript, 
                bool restoreFromCache)
            {
                this.userScript = userScript;
                this.RestoreFromCache = restoreFromCache;
                this.Created = DateTime.Now;
                this.UserScriptString = this.userScript.GetUserScriptString();
            }

            private Script _parsedUserScript = null;
            public Script ParsedUserScript
            {
                get
                {
                    if (_parsedUserScript == null)
                    {
                        _parsedUserScript = userScript.ParseScript(UserScriptString);
                    }
                    return _parsedUserScript;
                }
                private set
                {
                    _parsedUserScript = value;
                }
            }
            
            private System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> _errors;
            public System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Errors
            {
                get
                {
                    if (_errors == null)
                    {
                        _errors = userScript.CompileUserScript(ParsedUserScript);
                    }
                    return _errors;
                }
                private set
                {
                    _errors = value;
                }
            }

            private UserScriptAssembly.Cache _restorationCache = null;
            public UserScriptAssembly.Cache RestorationCache
            {
                get
                {
                    if (_restorationCache == null && RestoreFromCache)
                    {
                        _restorationCache = userScript.LoadUserScriptAssemblyCache(UserScriptString);
                    }
                    return _restorationCache;
                }
                private set
                {
                    _restorationCache = value;
                }
            }
            public bool IsRestorable
            {
                get { return RestorationCache != null; }
            }

            private UserScriptAssembly.Cache _userScriptAssemblyCache = null;
            public UserScriptAssembly.Cache UserScriptAssemblyCache
            {
                get
                {
                    if (_userScriptAssemblyCache == null)
                    {
                        _userScriptAssemblyCache = userScript.GenerateUserScriptAssemblyCache(UserScriptString, ParsedUserScript);
                    }
                    return _userScriptAssemblyCache;
                }
                private set
                {
                    _userScriptAssemblyCache = value;
                }
            }
        }

        private Core.FSM.GestureMachine _instance;
        public Core.FSM.GestureMachine Instance
        {
            get { return _instance; }
            private set
            {
                var old = Instance;
                _instance = value;
                if (old != null)
                {
                    old.Dispose();
                }
            }
        }
        public bool IsActivated()
        {
            return Instance.GetType() != typeof(NullGestureMachine);
        }

        private readonly AppGlobal Global;
        private readonly UserScript userScript;

        public ReloadableGestureMachine(AppGlobal Global, UserScript UserScript)
        {
            this.Global = Global;
            this.userScript = UserScript;
            this.Instance = new NullGestureMachine();
        }

        private GetGestureMachineResult GetGestureMachine()
        {
            var restoreFromCache = !IsActivated() || !Global.CLIOption.NoCache;
            var saveCache = !Global.CLIOption.NoCache;
            var candidate = new GestureMachineCandidate(userScript, restoreFromCache);

            Verbose.Print("restoreFromCache: {0}", restoreFromCache);
            Verbose.Print("saveCache: {0}", saveCache);
            Verbose.Print("candidate.IsRestorable: {0}", candidate.IsRestorable);

            if (candidate.IsRestorable)
            {
                try
                {
                    var ctx = new Core.UserScriptExecutionContext(Global);
                    userScript.EvaluateUserScriptAssembly(ctx, candidate.RestorationCache);
                    var gestureDef = ctx.GetGestureDefinition();
                    return new GetGestureMachineResult(new Core.FSM.GestureMachine(Global.UserConfig, gestureDef), null, null);
                }
                catch (Exception ex)
                {
                    Verbose.Print("GestureMachine restoration was failed; fallback to normal compilation. {0}", ex.ToString());
                }
            }

            if (candidate.Errors.Count() > 0)
            {
                Verbose.Print("Error(s) found in the UserScript on compilation phase.");
                return new GetGestureMachineResult(null, candidate.Errors, null);
            }
                
            Verbose.Print("No error found in the UserScript on compilation phase.");
            {
                var ctx = new Core.UserScriptExecutionContext(Global);
                try
                {
                    userScript.EvaluateUserScriptAssembly(ctx, candidate.UserScriptAssemblyCache);
                    if (saveCache)
                    {
                        try
                        {
                            userScript.SaveUserScriptAssemblyCache(candidate.UserScriptAssemblyCache);
                        }
                        catch (Exception ex)
                        {
                            Verbose.Print("SaveUserScriptAssemblyCache was failed. {0}", ex.ToString());
                        }
                    }
                    Verbose.Print("Error ocurred in the UserScript on evaluation phase.");
                    var gestureDef = ctx.GetGestureDefinition();
                    var gestureMachine = new Core.FSM.GestureMachine(Global.UserConfig, gestureDef);
                    return new GetGestureMachineResult(gestureMachine, null, null);
                }
                catch (Exception ex)
                {
                    Verbose.Print("No error ocurred in the UserScript on evaluation phase.");
                    var gestureDef = ctx.GetGestureDefinition();
                    var gestureMachine = new Core.FSM.GestureMachine(Global.UserConfig, gestureDef);
                    return new GetGestureMachineResult(gestureMachine, null, ex);
                }
            }
        }

        private object lockObject = new object();
        private bool reloadRequest = false;
        private bool reloading = false;

        public void RequestReload()
        {
            using (Verbose.PrintElapsed("Request hot-reload GestureMachine"))
            {
                if (reloading && !disposed)
                {
                    Verbose.Print("Hot-reload request was queued.");
                    reloadRequest = true;
                    return;
                }
                lock (lockObject)
                {
                    if (disposed)
                    {
                        return;
                    }
                    reloading = true;
                    while (true)
                    {
                        using (Verbose.PrintElapsed("Hot-reload GestureMachine"))
                        {
                            reloadRequest = false;
                            try
                            {
                                var result = GetGestureMachine();
                                var gestureMachine = result.Item1;
                                var compilationErrors = result.Item2;
                                var runtimeError = result.Item3;

                                var balloonIconTitle = "";
                                var balloonIconMessage = "";
                                var balloonIcon = ToolTipIcon.None;
                                var lastErrorMessage = "";
                                if (gestureMachine == null)
                                {
                                    balloonIconTitle = "UserScript Compilation Error";
                                    balloonIconMessage = string.Format("{0} error(s) found in the UserScript.\r\nClick to view the detail.",
                                        compilationErrors.GetValueOrDefault().Count());
                                    balloonIcon = ToolTipIcon.Error;
                                    lastErrorMessage = userScript.GetPrettyErrorMessage(compilationErrors.GetValueOrDefault());
                                }
                                else
                                {
                                    var gestures = gestureMachine.GestureDefinition.Count();
                                    var activatedMessage = string.Format("{0} Gestures Activated", gestures);

                                    Instance = gestureMachine;
                                    if (runtimeError == null)
                                    {
                                        balloonIcon = ToolTipIcon.Info;
                                        balloonIconMessage = activatedMessage;
                                        lastErrorMessage = "";
                                    }
                                    else
                                    {
                                        balloonIcon = ToolTipIcon.Warning;
                                        balloonIconTitle = activatedMessage;
                                        balloonIconMessage = "The configuration may be incomplete due to the UserScript Evaluation Error.\r\nClick to view the detail.";
                                        lastErrorMessage = runtimeError.ToString();
                                    }
                                    Global.MainForm.UpdateTasktrayMessage("Gestures: {0}", gestures);
                                }
                                Global.MainForm.LastErrorMessage = lastErrorMessage;
                                Global.MainForm.ShowBalloon(balloonIconMessage, balloonIconTitle, balloonIcon, 10000);

                                if (!reloadRequest)
                                {
                                    ReleaseUnusedMemory();
                                }
                            }
                            finally
                            {
                                reloading = false;
                            }
                        }
                        if (!reloadRequest)
                        {
                            break;
                        }
                        Verbose.Print("Hot reload request exists; Retrying...");
                    }
                }
            }
        }
        
        private void ReleaseUnusedMemory()
        {
            using (Verbose.PrintElapsed("Release unused memory"))
            {
                var totalMemory = GC.GetTotalMemory(false);
                GC.Collect(2);
                Verbose.Print("GC.GetTotalMemory: {0} -> {1}", totalMemory, GC.GetTotalMemory(false));
            }
        }

        private bool disposed = false;
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            lock (lockObject)
            {
                reloadRequest = false;
                disposed = true;
                Instance = null;
            }
        }

        ~ReloadableGestureMachine()
        {
            Dispose();
        }
    }

    public class MouseGestureForm : Form
    {
        private bool _captureMouse = false;
        protected bool CaptureMouse
        {
            get { return _captureMouse; }
            set
            {
                if (_captureMouse != value)
                {
                    if (value)
                    {
                        mouseHook.SetHook();
                        _captureMouse = true;
                    }
                    else
                    {
                        mouseHook.Unhook();
                        _captureMouse = false;
                    }
                }
            }
        }

        private readonly LowLevelMouseHook mouseHook;
        protected readonly AppGlobal Global;
        public readonly UserScript UserScript;
        protected readonly ReloadableGestureMachine ReloadableGestureMachine;

        // Designer requires this dummy constuctor.
        public MouseGestureForm() : this(new AppGlobal())
        {

        }

        public MouseGestureForm(AppGlobal Global)
        {
            this.mouseHook = new LowLevelMouseHook(MouseProc);
            this.Global = Global;
            this.UserScript = new UserScript(Global);
            this.ReloadableGestureMachine = new ReloadableGestureMachine(Global, UserScript);
        }
        
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ReloadableGestureMachine.Dispose();
        }

        private const int WM_DISPLAYCHANGE = 0x007E;
        private const int WM_POWERBROADCAST = 0x0218;

        private const int PBT_APMQUERYSUSPEND = 0x0000;
        private const int PBT_APMQUERYSTANDBY = 0x0001;
        private const int PBT_APMQUERYSUSPENDFAILED = 0x0002;
        private const int PBT_APMQUERYSTANDBYFAILED = 0x0003;
        private const int PBT_APMSUSPEND = 0x0004;
        private const int PBT_APMSTANDBY = 0x0005;
        private const int PBT_APMRESUMECRITICAL = 0x0006;
        private const int PBT_APMRESUMESUSPEND = 0x0007;
        private const int PBT_APMRESUMESTANDBY = 0x0008;
        private const int PBT_APMBATTERYLOW = 0x0009;
        private const int PBT_APMPOWERSTATUSCHANGE = 0x000A;
        private const int PBT_APMOEMEVENT = 0x000B;
        private const int PBT_APMRESUMEAUTOMATIC = 0x0012;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_DISPLAYCHANGE:
                    Verbose.Print("WndProc: WM_DISPLAYCHANGE");
                    ReloadableGestureMachine.Instance.Reset();
                    Verbose.Print("GestureMachine was reset.");
                    break;

                case WM_POWERBROADCAST:
                    int reason = m.WParam.ToInt32();
                    switch(reason)
                    {
                        case PBT_APMQUERYSUSPEND:
                            Verbose.Print("WndProc: PBT_APMQUERYSUSPEND");
                            break;
                        case PBT_APMQUERYSTANDBY:
                            Verbose.Print("WndProc: PBT_APMQUERYSTANDBY");
                            break;
                        case PBT_APMQUERYSUSPENDFAILED:
                            Verbose.Print("WndProc: PBT_APMQUERYSUSPENDFAILED");
                            break;
                        case PBT_APMQUERYSTANDBYFAILED:
                            Verbose.Print("WndProc: PBT_APMQUERYSTANDBYFAILED");
                            break;
                        case PBT_APMSUSPEND:
                            Verbose.Print("WndProc: PBT_APMSUSPEND");
                            break;
                        case PBT_APMSTANDBY:
                            Verbose.Print("WndProc: PBT_APMSTANDBY");
                            break;
                        case PBT_APMRESUMECRITICAL:
                            Verbose.Print("WndProc: PBT_APMRESUMECRITICAL");
                            break;
                        case PBT_APMRESUMESUSPEND:
                            Verbose.Print("WndProc: PBT_APMRESUMESUSPEND");
                            break;
                        case PBT_APMRESUMESTANDBY:
                            Verbose.Print("WndProc: PBT_APMRESUMESTANDBY");
                            break;
                        case PBT_APMBATTERYLOW:
                            Verbose.Print("WndProc: PBT_APMBATTERYLOW");
                            break;
                        case PBT_APMPOWERSTATUSCHANGE:
                            Verbose.Print("WndProc: PBT_APMPOWERSTATUSCHANGE");
                            break;
                        case PBT_APMOEMEVENT:
                            Verbose.Print("WndProc: PBT_APMOEMEVENT");
                            break;
                        case PBT_APMRESUMEAUTOMATIC:
                            Verbose.Print("WndProc: PBT_APMRESUMEAUTOMATIC");
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        public WindowsHook.Result MouseProc(LowLevelMouseHook.Event evnt, LowLevelMouseHook.MSLLHOOKSTRUCT data)
        {
            Debug.Print("MouseEvent: {0} | {1}",
                    Enum.GetName(typeof(LowLevelMouseHook.Event), evnt),
                    BitConverter.ToString(BitConverter.GetBytes((uint)data.dwExtraInfo))
                    );

            if (data.fromCreviceApp)
            {
                Debug.Print("{0} was passed to the next hook because this event has the signature of CreviceApp",
                    Enum.GetName(typeof(LowLevelMouseHook.Event),
                    evnt));
                return WindowsHook.Result.Transfer;
            }
            else if (data.fromTablet)
            {
                Debug.Print("{0} was passed to the next hook because this event has the signature of Tablet",
                    Enum.GetName(typeof(LowLevelMouseHook.Event),
                    evnt));
                return WindowsHook.Result.Transfer;
            }

            var point = new Point(data.pt.x, data.pt.y);

            switch (evnt)
            {
                case LowLevelMouseHook.Event.WM_MOUSEMOVE:
                    return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.Move, point));
                case LowLevelMouseHook.Event.WM_LBUTTONDOWN:
                    return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.LeftButtonDown, point));
                case LowLevelMouseHook.Event.WM_LBUTTONUP:
                    return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.LeftButtonUp, point));
                case LowLevelMouseHook.Event.WM_RBUTTONDOWN:
                    return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.RightButtonDown, point));
                case LowLevelMouseHook.Event.WM_RBUTTONUP:
                    return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.RightButtonUp, point));
                case LowLevelMouseHook.Event.WM_MBUTTONDOWN:
                    return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.MiddleButtonDown, point));
                case LowLevelMouseHook.Event.WM_MBUTTONUP:
                    return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.MiddleButtonUp, point));
                case LowLevelMouseHook.Event.WM_MOUSEWHEEL:
                    if (data.mouseData.asWheelDelta.delta < 0)
                    {
                        return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.WheelDown, point));
                    }
                    else
                    {
                        return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.WheelUp, point));
                    }
                case LowLevelMouseHook.Event.WM_XBUTTONDOWN:
                    if (data.mouseData.asXButton.isXButton1)
                    {
                        return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.X1ButtonDown, point));
                    }
                    else
                    {
                        return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.X2ButtonDown, point));
                    }
                case LowLevelMouseHook.Event.WM_XBUTTONUP:
                    if (data.mouseData.asXButton.isXButton1)
                    {
                        return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.X1ButtonUp, point));
                    }
                    else
                    {
                        return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.X2ButtonUp, point));
                    }
                case LowLevelMouseHook.Event.WM_MOUSEHWHEEL:
                    if (data.mouseData.asWheelDelta.delta < 0)
                    {
                        return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.WheelRight, point));
                    }
                    else
                    {
                        return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.WheelLeft, point));
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
        
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MouseGestureForm));
            this.SuspendLayout();
            // 
            // MouseGestureForm
            // 
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MouseGestureForm";
            this.ResumeLayout(false);
        }
    }
}
