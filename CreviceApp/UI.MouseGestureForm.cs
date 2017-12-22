using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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

        public string UserDirectory
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Application.CompanyName,
                    Application.ProductName);
            }
        }

        public string UserScriptFile
        {
            get
            {
                var scriptPath = Global.CLIOption.Script;
                if (Path.IsPathRooted(scriptPath))
                {
                    return scriptPath;
                }
                var uri = new Uri(new Uri(UserDirectory + "\\"), scriptPath);
                return uri.LocalPath;
            }
        }

        private string GetUserScript()
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

        private UserScriptAssembly.Cache CompileUserScript(UserScriptAssembly usa, string userScript)
        {
            Debug.Print("Compiling UserScript");
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
                globalsType: typeof(Core.UserScriptExecutionContext));

            var compilation = script.GetCompilation();
            Debug.Print("Compile finished");
            var peStream = new MemoryStream();
            var pdbStream = new MemoryStream();
            compilation.Emit(peStream, pdbStream);
            var cache = usa.CreateCache(userScript, peStream.GetBuffer(), pdbStream.GetBuffer());
            Debug.Print("UserScriptAssemblyCache generated");
            return cache;
        }

        private IEnumerable<Core.GestureDefinition> EvaluateUserScript(UserScriptAssembly.Cache cache, Core.UserScriptExecutionContext ctx)
        {
            Debug.Print("Evaluating UserScript");
            var assembly = Assembly.Load(cache.pe, cache.pdb);
            var type = assembly.GetType("Submission#0");
            var factory = type.GetMethod("<Factory>");
            var parameterinfo = factory.GetParameters();

            var parameters = new object[] { new object[] { ctx, null } };
            var result = factory.Invoke(null, parameters);
            Debug.Print("UserScript evaluated");

            return ctx.GetGestureDefinition();
        }

        private UserScriptAssembly.Cache GetUserScriptAssemblyCache(string userScript)
        {
            var cachePath = Path.Combine(UserDirectory, "crevice.userscript.assembly.cache");
            var usa = new UserScriptAssembly();
            if (File.Exists(cachePath))
            {
                Debug.Print("Loading UserScriptAssemblyCache");
                var loadedCache = usa.Load(cachePath);
                Debug.Print("UserScriptAssemblyCache loaded");
                if (usa.IsCompatible(loadedCache, GetUserScript()))
                {
                    return loadedCache;
                }
            }
            var generatedCache = CompileUserScript(usa, userScript);
            Debug.Print("Saving UserScriptAssemblyCache");
            usa.Save(cachePath, generatedCache);
            Debug.Print("UserScriptAssemblyCache saved");
            return generatedCache;
        }

        private IEnumerable<Core.GestureDefinition> GetGestureDef(Core.UserScriptExecutionContext ctx)
        {
            var userScript = GetUserScript();
            var cache = GetUserScriptAssemblyCache(userScript);
            return EvaluateUserScript(cache, ctx);
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
