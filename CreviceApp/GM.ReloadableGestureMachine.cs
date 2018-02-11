using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Crevice.GestureMachine
{
    using System.Threading;
    using Crevice.Logging;
    using Crevice.Config;
    using Crevice.UserScript;

    using GetGestureMachineResult =
        Tuple<GestureMachineCluster,
              System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic>?,
              Exception>;

    public class ReloadableGestureMachine : IDisposable
    {
        private GestureMachineCluster _instance = new NullGestureMachineCluster();
        public GestureMachineCluster Instance
        {
            get { return _instance; }
            private set
            {
                var old = Instance;
                _instance = value;
                old?.Dispose();
            }
        }

        public bool IsActivated()
            => Instance.GetType() != typeof(NullGestureMachineCluster);

        private readonly GlobalConfig GlobalConfig;

        public ReloadableGestureMachine(GlobalConfig globalConfig)
        {
            GlobalConfig = globalConfig;
        }

        private GetGestureMachineResult GetGestureMachine()
        {
            var restoreFromCache = !IsActivated() || !GlobalConfig.CLIOption.NoCache;
            var saveCache = !GlobalConfig.CLIOption.NoCache;
            var userScriptString = GlobalConfig.GetOrSetDefaultUserScriptFile(Encoding.UTF8.GetString(Properties.Resources.DefaultUserScript));
            var candidate = new GestureMachineCandidate(
                userScriptString, 
                GlobalConfig.UserScriptCacheFile,
                true,
                GlobalConfig.UserDirectory, GlobalConfig.UserDirectory);

            UserScriptExecutionContext createContext() => new UserScriptExecutionContext(GlobalConfig);

            Verbose.Print("restoreFromCache: {0}", restoreFromCache);
            Verbose.Print("saveCache: {0}", saveCache);
            Verbose.Print("candidate.IsRestorable: {0}", candidate.IsRestorable);
            
            if (candidate.IsRestorable)
            {
                try
                {
                    return new GetGestureMachineResult(candidate.Restore(createContext()), null, null);
                }
                catch (Exception ex)
                {
                    Verbose.Error("GestureMachine restoration was failed; fallback to normal compilation. {0}", ex.ToString());
                }
            }

            if (candidate.Errors.Count() > 0)
            {
                Verbose.Print("Error(s) found in the UserScript on compilation phase.");
                return new GetGestureMachineResult(null, candidate.Errors, null);
            }

            Verbose.Print("No error found in the UserScript on compilation phase.");
            {
                try
                {
                    UserScript.EvaluateUserScriptAssembly(createContext(), candidate.UserScriptAssemblyCache);
                    if (saveCache)
                    {
                        try
                        {
                            UserScript.SaveUserScriptAssemblyCache(GlobalConfig.UserScriptCacheFile, candidate.UserScriptAssemblyCache);
                        }
                        catch (Exception ex)
                        {
                            Verbose.Error("SaveUserScriptAssemblyCache was failed. {0}", ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Verbose.Error("Error ocurred in the UserScript on evaluation phase. {0}", ex.ToString());
                    return new GetGestureMachineResult(candidate.CreateNew(createContext()), null, ex);
                }
                Verbose.Print("No error ocurred in the UserScript on evaluation phase.");
                return new GetGestureMachineResult(candidate.CreateNew(createContext()), null, null);
            }
        }

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _reloadRequest = false;
        private bool _loading = false;

        public void HotReload()
        {
            if (_disposed)
            {
                new InvalidOperationException();
            }
            if (_loading && !_disposed)
            {
                Verbose.Print("Hot-reload request was queued.");
                _reloadRequest = true;
                return;
            }
            _semaphore.Wait();
            try
            {
                if (_disposed)
                {
                    return;
                }
                while (true)
                {
                    _loading = true;
                    _reloadRequest = false;
                    using (Verbose.PrintElapsed("Hot-reload GestureMachine"))
                    {
                        var (gmCluster, compilationErrors, runtimeError) = GetGestureMachine();
                        var balloonIconTitle = "";
                        var balloonIconMessage = "";
                        var balloonIcon = ToolTipIcon.None;
                        var lastErrorMessage = "";
                        if (gmCluster == null)
                        {
                            balloonIconTitle = "UserScript Compilation Error";
                            balloonIconMessage = string.Format("{0} error(s) found in the UserScript.\r\nClick to view the detail.",
                                compilationErrors.GetValueOrDefault().Count());
                            balloonIcon = ToolTipIcon.Error;
                            lastErrorMessage = UserScript.GetPrettyErrorMessage(compilationErrors.GetValueOrDefault());
                        }
                        else
                        {
                            gmCluster.Run();

                            var gestures = gmCluster.Profiles.Select(p => p.RootElement.GestureCount).Sum();
                            var activatedMessage = string.Format("{0} Gestures Activated", gestures);

                            Instance = gmCluster;
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
                            GlobalConfig.MainForm.UpdateTasktrayMessage(gmCluster.Profiles);
                        }
                        GlobalConfig.MainForm.LastErrorMessage = lastErrorMessage;
                        GlobalConfig.MainForm.ShowBalloon(balloonIconMessage, balloonIconTitle, balloonIcon, 10000);
                    }
                    _loading = false;
                    if (!_reloadRequest)
                    {
                        break;
                    }
                    Verbose.Print("Hot reload request exists; Retrying...");
                }
            }
            finally
            {
                ReleaseUnusedMemory();
                _semaphore.Release();
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
        
        private bool _disposed = false;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _semaphore.Wait();
            try
            {
                GC.SuppressFinalize(this);
                _reloadRequest = false;
                _disposed = true;
                Instance = null;
            }
            finally
            {
                _semaphore.Release();
                _semaphore.Dispose();
            }
        }

        ~ReloadableGestureMachine()
        {
            Dispose();
        }
    }
}
