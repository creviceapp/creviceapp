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
        protected Core.FSM.GestureMachine GestureMachine { get; private set; }

        private readonly LowLevelMouseHook mouseHook;
        protected readonly AppGlobal Global;

        // Designer needs this dummy constuctor.
        public MouseGestureForm() : this(new AppGlobal())
        {

        }

        public MouseGestureForm(AppGlobal Global)
        {
            this.mouseHook = new LowLevelMouseHook(MouseProc);
            this.Global = Global;
        }

        // %USERPROFILE%\\AppData\\Roaming\\Crevice\\CreviceApp
        public string DefaultUserDirectory
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Application.CompanyName,
                    Application.ProductName);
            }
        }

        // Parent directory of UserScriptFile.
        public string UserDirectory
        {
            get
            {
                return Directory.GetParent(UserScriptFile).FullName;
            }
        }
        
        public string UserScriptFile
        {
            get
            {
                var scriptPath = Global.CLIOption.ScriptFile;
                if (Path.IsPathRooted(scriptPath))
                {
                    return scriptPath;
                }
                var uri = new Uri(new Uri(DefaultUserDirectory + "\\"), scriptPath);
                return uri.LocalPath;
            }
        }

        private string GetUserScriptCode()
        {
            var scriptFile = UserScriptFile;
            var dir = Directory.GetParent(scriptFile);
            if (!dir.Exists)
            {
                dir.Create();
            }
            if (!File.Exists(scriptFile))
            {
                File.WriteAllText(scriptFile, Encoding.UTF8.GetString(Properties.Resources.DefaultUserScript), Encoding.UTF8);
            }
            return File.ReadAllText(scriptFile, Encoding.UTF8);
        }

        private string GetUserScriptCacheFile()
        {
            return UserScriptFile + ".cache";
        }

        private Script ParseScript(string userScript)
        {
            var stopwatch = new Stopwatch();
            Verbose.Print("Parsing UserScript...");
            stopwatch.Start();
            var script = CSharpScript.Create(
                userScript,
                ScriptOptions.Default
                    .WithSourceResolver(ScriptSourceResolver.Default.WithBaseDirectory(UserDirectory))
                    .WithMetadataResolver(ScriptMetadataResolver.Default.WithBaseDirectory(UserDirectory))
                    .WithReferences("microlib")                   // microlib.dll
                    .WithReferences("System")                     // System.dll
                    .WithReferences("System.Core")                // System.Core.dll
                    .WithReferences("Microsoft.CSharp")           // Microsoft.CSharp.dll
                    .WithReferences(Assembly.GetEntryAssembly()), // CreviceApp.exe
                                                                  // todo dynamic type
                globalsType: typeof(Core.UserScriptExecutionContext));
            stopwatch.Stop();
            Verbose.Print("UserScript parse finished. ({0})", stopwatch.Elapsed);
            return script;
        }

        private UserScriptAssembly.Cache CompileUserScript(UserScriptAssembly usa, string userScriptCode, Script userScript)
        {
            var stopwatch = new Stopwatch();
            Verbose.Print("Compiling UserScript...");
            stopwatch.Start();
            var compilation = userScript.GetCompilation();
            stopwatch.Stop();
            Verbose.Print("UserScript compilation finished. ({0})", stopwatch.Elapsed);

            var peStream = new MemoryStream();
            var pdbStream = new MemoryStream();
            Verbose.Print("Genarating UserScriptAssembly...");
            stopwatch.Restart();
            compilation.Emit(peStream, pdbStream);
            stopwatch.Stop();
            Verbose.Print("UserScriptAssembly generation finished. ({0})", stopwatch.Elapsed);
            return usa.CreateCache(userScriptCode, peStream.GetBuffer(), pdbStream.GetBuffer());
        }

        private IEnumerable<Core.GestureDefinition> EvaluateUserScript(Script userScript, Core.UserScriptExecutionContext ctx)
        {
            var stopwatch = new Stopwatch();
            Verbose.Print("Compiling UserScript...");
            stopwatch.Start();
            var diagnotstics = userScript.Compile();
            stopwatch.Stop();
            Verbose.Print("UserScript compilation finished. ({ 0})", stopwatch.Elapsed);
            foreach (var dg in diagnotstics.Select((v, i) => new { v, i }))
            {
                Verbose.Print("Diagnotstics[{0}]: {1}", dg.i, dg.v.ToString());
            }
            Verbose.Print("Evaluating UserScript...");
            stopwatch.Restart();
            userScript.RunAsync(ctx).Wait();
            stopwatch.Stop();
            Verbose.Print("UserScript evaluation finished. ({0})", stopwatch.Elapsed);
            return ctx.GetGestureDefinition();
        }

        private IEnumerable<Core.GestureDefinition> EvaluateUserScriptAssembly(UserScriptAssembly.Cache cache, Core.UserScriptExecutionContext ctx)
        {
            var stopwatch = new Stopwatch();
            Verbose.Print("Loading UserScriptAssembly...");
            stopwatch.Start();
            var assembly = Assembly.Load(cache.pe, cache.pdb);
            stopwatch.Stop();
            Verbose.Print("UserScriptAssembly loading finished. ({0})", stopwatch.Elapsed);
            Verbose.Print("Evaluating UserScriptAssembly...");
            stopwatch.Restart();
            var type = assembly.GetType("Submission#0");
            var factory = type.GetMethod("<Factory>");
            var parameters = new object[] { new object[] { ctx, null } };
            var result = factory.Invoke(null, parameters);
            stopwatch.Stop();
            Verbose.Print("UserScriptAssembly evaluation finished. ({0})", stopwatch.Elapsed);
            return ctx.GetGestureDefinition();
        }

        private UserScriptAssembly.Cache GetUserScriptAssemblyCache(string userScriptCode, Script userScript)
        {
            var stopwatch = new Stopwatch();
            var cacheFile = GetUserScriptCacheFile();
            var usa = new UserScriptAssembly();
            if (File.Exists(cacheFile))
            {
                Verbose.Print("Loading UserScriptAssemblyCache...");
                stopwatch.Start();
                var loadedCache = usa.Load(cacheFile);
                stopwatch.Stop();
                Verbose.Print("UserScriptAssemblyCache loading finished. ({0})", stopwatch.Elapsed);
                if (usa.IsCompatible(loadedCache, userScriptCode))
                {
                    return loadedCache;
                }
                Verbose.Print("UserScriptAssemblyCache was discarded because of the signature was not match with this application.");
            }
            var generatedCache = CompileUserScript(usa, userScriptCode, userScript);
            Verbose.Print("Saving UserScriptAssemblyCache...");
            stopwatch.Restart();
            usa.Save(cacheFile, generatedCache);
            stopwatch.Stop();
            Verbose.Print("UserScriptAssemblyCache saving finished. ({0})", stopwatch.Elapsed);
            return generatedCache;
        }

        private IEnumerable<Core.GestureDefinition> GetGestureDef(Core.UserScriptExecutionContext ctx)
        {
            var userScriptCode = GetUserScriptCode();
            var userScript = ParseScript(userScriptCode);
            if (!Global.CLIOption.NoCache)
            {
                try
                {
                    var cache = GetUserScriptAssemblyCache(userScriptCode, userScript);
                    return EvaluateUserScriptAssembly(cache, ctx);
                }
                catch (Exception ex)
                {
                    Verbose.Print("Error occured when UserScript conpilation and save and restoration; fallback to the --nocache mode and continume");
                    Verbose.Print(ex.ToString());
                }
            }
            return EvaluateUserScript(userScript, ctx);
        }

        protected void InitializeGestureMachine()
        {
            var ctx = new Core.UserScriptExecutionContext(Global);
            var gestureDef = GetGestureDef(ctx);
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
                    GestureMachine.Reset();
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
