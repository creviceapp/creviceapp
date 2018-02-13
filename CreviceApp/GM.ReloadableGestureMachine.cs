using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Crevice.GestureMachine
{
    using System.Threading;
    using Crevice.Core.FSM;
    using Crevice.Logging;
    using Crevice.Config;
    using Crevice.UserScript;

    using GetGestureMachineResult =
        Tuple<GestureMachineCluster,
              System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic>?,
              Exception>;

    public class ReloadableGestureMachine 
        : IGestureMachine, IDisposable
    {
        private GestureMachineCluster _instance = new NullGestureMachineCluster();
        private GestureMachineCluster Instance
        {
            get => _instance;
            set
            {
                var old = Instance;
                _instance = value;
                old?.Dispose();
            }
        }

        public bool IsActivated()
            => Instance.GetType() != typeof(NullGestureMachineCluster);

        public bool Input(Core.Events.IPhysicalEvent physicalEvent, System.Drawing.Point? point)
            => Instance.Input(physicalEvent, point);

        public bool Input(Core.Events.IPhysicalEvent physicalEvent)
            => Instance.Input(physicalEvent);

        public void Reset()
            => Instance.Reset();

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
                        if (gmCluster == null)
                        {
                            ShowErrorBalloon(compilationErrors.GetValueOrDefault());
                        }
                        else
                        {
                            gmCluster.Run();
                            Instance = gmCluster;
                            if (runtimeError == null)
                            {
                                ShowInfoBalloon(gmCluster);
                            }
                            else
                            {
                                ShowWarningBalloon(gmCluster, runtimeError);
                            }
                            UpdateTasktrayMessage(gmCluster.Profiles);
                        }
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

        private void UpdateTasktrayMessage(IReadOnlyList<GestureMachineProfile> profiles)
        {
            var header = $"Crevice {Application.ProductVersion}";
            var sum = profiles.Sum(p => p.RootElement.GestureCount);
            var gesturesMessage = $"Gestures: {sum}";
            var totalMessage = $"Total: {sum}";
            if (profiles.Count > 1)
            {
                var perProfileMessages = Enumerable.
                    Range(0, profiles.Count).
                    Select(n => profiles.Take(n + 1).
                        Select(p => $"({p.ProfileName}): {p.RootElement.GestureCount}").
                        Aggregate("", (a, b) => a + "\r\n" + b)).
                        Select(s => s.Trim()).
                    Reverse().ToList();

                if ((header + "\r\n" + perProfileMessages.First()).Length < 63)
                {
                    GlobalConfig.MainForm.UpdateTasktrayMessage(perProfileMessages.First());
                    return;
                }

                perProfileMessages = perProfileMessages.Skip(1).
                    Where(s => (header + "\r\n" + s + "\r\n...\r\n" + totalMessage).Length < 63).ToList();

                if (perProfileMessages.Any())
                {
                    GlobalConfig.MainForm.UpdateTasktrayMessage(perProfileMessages.First() + "\r\n...\r\n" + totalMessage);
                    return;
                }
            }
            GlobalConfig.MainForm.UpdateTasktrayMessage(gesturesMessage);
        }

        private string GetActivatedMessage(GestureMachineCluster gmCluster) 
            => $"{gmCluster.Profiles.Select(p => p.RootElement.GestureCount).Sum()} Gestures Activated";

        private void ShowInfoBalloon(
            GestureMachineCluster gmCluster)
        {
            GlobalConfig.MainForm.LastErrorMessage = "";
            GlobalConfig.MainForm.ShowBalloon(GetActivatedMessage(gmCluster), "", ToolTipIcon.Info, 10000);
        }

        private void ShowWarningBalloon(
            GestureMachineCluster gmCluster, 
            Exception runtimeError)
        {
            GlobalConfig.MainForm.LastErrorMessage = runtimeError.ToString();
            var text = "The configuration may be loaded incompletely due to an error on the UserScript evaluation.\r\n" +
                "Click to view the detail.";
            GlobalConfig.MainForm.ShowBalloon(text, GetActivatedMessage(gmCluster), ToolTipIcon.Warning, 10000);
        }

        private void ShowErrorBalloon(
            System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> errors)
        {
            var errorMessage = UserScript.GetPrettyErrorMessage(errors);
            GlobalConfig.MainForm.LastErrorMessage = errorMessage;

            var title = "UserScript Compilation Error";
            var text = $"{errors.Count()} error(s) found in the UserScript.\r\nClick to view the detail.";
            GlobalConfig.MainForm.ShowBalloon(text, title, ToolTipIcon.Error, 10000);
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
