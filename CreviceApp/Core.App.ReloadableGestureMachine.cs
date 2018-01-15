using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CreviceApp.Core.App
{
    using GetGestureMachineResult =
           Tuple<FSM.GestureMachine,
               System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic>?,
               Exception>;

    public class ReloadableGestureMachine : IDisposable
    {
        private FSM.GestureMachine _instance;
        public FSM.GestureMachine Instance
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
            return Instance.GetType() != typeof(FSM.NullGestureMachine);
        }

        private readonly AppGlobal Global;
        private readonly UserScript userScript;

        public ReloadableGestureMachine(AppGlobal Global, UserScript UserScript)
        {
            this.Global = Global;
            this.userScript = UserScript;
            this.Instance = new FSM.NullGestureMachine();
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
                    return new GetGestureMachineResult(candidate.Restore(Global), null, null);
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
                var ctx = new UserScriptExecutionContext(Global);
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
                }
                catch (Exception ex)
                {
                    Verbose.Print("Error ocurred in the UserScript on evaluation phase. {0}", ex.ToString());
                    return new GetGestureMachineResult(candidate.CreateNew(Global, ctx), null, ex);
                }
                Verbose.Print("No error ocurred in the UserScript on evaluation phase.");
                return new GetGestureMachineResult(candidate.CreateNew(Global, ctx), null, null);
            }
        }

        private object lockObject = new object();
        private bool reloadRequest = false;
        private bool reloading = false;

        public void HotReload()
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
}
