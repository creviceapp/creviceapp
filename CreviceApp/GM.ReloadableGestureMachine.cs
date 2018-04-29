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
        internal GestureMachineCluster _instance = new NullGestureMachineCluster();
        private GestureMachineCluster Instance
        {
            get => _instance;
            set
            {
                var old = Instance;
                _instance = value;
                old?.Stop();
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

        private readonly GlobalConfig _config;
        private readonly UI.MainFormBase _mainForm;

        public ReloadableGestureMachine(GlobalConfig config, UI.MainFormBase mainForm)
        {
            _config = config;
            _mainForm = mainForm;
        }

        public event EventHandler Reloaded;

        protected virtual void OnReloaded(EventArgs args) => Reloaded?.Invoke(this, args);

        private GetGestureMachineResult GetGestureMachine()
        {
            var restoreFromCache = !IsActivated() || !_config.CLIOption.NoCache;
            var saveCache = !_config.CLIOption.NoCache;
            var userScriptString = _config.GetOrSetDefaultUserScriptFile(Encoding.UTF8.GetString(Properties.Resources.DefaultUserScript));
            
            var candidate = new GestureMachineCandidate(
                _config.UserDirectory,
                userScriptString,
                _config.UserScriptCacheFile,
                allowRestore: restoreFromCache);

            Verbose.Print("restoreFromCache: {0}", restoreFromCache);
            Verbose.Print("saveCache: {0}", saveCache);
            Verbose.Print("candidate.IsRestorable: {0}", candidate.IsRestorable);
            
            if (candidate.IsRestorable)
            {
                var ctx = new UserScriptExecutionContext(_config, _mainForm);
                try
                {
                    return new GetGestureMachineResult(candidate.Restore(ctx), null, null);
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
                var ctx = new UserScriptExecutionContext(_config, _mainForm);
                try
                {
                    UserScript.EvaluateUserScriptAssembly(ctx, candidate.UserScriptAssemblyCache);
                    if (saveCache)
                    {
                        try
                        {
                            UserScript.SaveUserScriptAssemblyCache(_config.UserScriptCacheFile, candidate.UserScriptAssemblyCache);
                        }
                        catch (Exception ex)
                        {
                            Verbose.Error("SaveUserScriptAssemblyCache was failed. {0}", ex.ToString());
                        }
                    }
                    Verbose.Print("No error ocurred in the UserScript on evaluation phase.");
                    return new GetGestureMachineResult(candidate.Create(ctx), null, null);
                }
                catch (Exception ex)
                {
                    Verbose.Error("Error ocurred in the UserScript on evaluation phase. {0}", ex.ToString());
                    return new GetGestureMachineResult(candidate.Create(ctx), null, ex);
                }
            }
        }

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _reloadRequest = false;
        private bool _loading = false;

        public void HotReload()
        {
            if (_disposed) return;
            if (_loading && !_disposed)
            {
                Verbose.Print("Hot-reload request was queued.");
                _reloadRequest = true;
                return;
            }
            _semaphore.Wait();
            try
            {
                if (_disposed) return;
                while (true)
                {
                    _loading = true;
                    _reloadRequest = false;
                    using (Verbose.PrintElapsed("Hot-reload GestureMachine"))
                    {
                        try
                        {
                            var (gmCluster, compilationErrors, runtimeError) = GetGestureMachine();
                            if (gmCluster == null)
                            {
                                _mainForm.ShowErrorBalloon(compilationErrors.GetValueOrDefault());
                            }
                            else
                            {
                                gmCluster.Run();
                                Instance = gmCluster;
                                if (runtimeError == null)
                                {
                                    _mainForm.ShowInfoBalloon(gmCluster);
                                }
                                else
                                {
                                    _mainForm.ShowWarningBalloon(gmCluster, runtimeError);
                                }
                                _mainForm.UpdateTasktrayMessage(gmCluster.Profiles);
                            }
                        }
                        catch (System.IO.IOException)
                        {
                            Verbose.Print("User script cannot be read; the file may be in use by another process.");
                            Verbose.Print("Waiting 1 sec ...");
                            Thread.Sleep(1000);
                            continue;
                        }
                    }
                    _loading = false;
                    if (!_reloadRequest)
                    {
                        OnReloaded(new EventArgs());
                        ReleaseUnusedMemory();
                        break;
                    }
                    Verbose.Print("Hot reload request exists; Retrying...");
                }
            }
            finally
            {
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
            if (_disposed) return;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
            if (disposing)
            {
                _semaphore.Wait();
                try
                {
                    Instance = null;
                }
                finally
                {
                    _semaphore.Release();
                    _semaphore.Dispose();
                }
            }
        }

        ~ReloadableGestureMachine() => Dispose(false);
    }
}
