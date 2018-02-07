using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;


namespace CreviceApp.Core
{
    public class GestureMachineCluster : IDisposable
    {
        public IReadOnlyList<CustomGestureMachine> GestureMachines;

        public GestureMachineCluster(IReadOnlyList<CustomGestureMachine> gestureMachines)
        {
            GestureMachines = gestureMachines;
        }
        public bool Input(Crevice.Core.Events.IPhysicalEvent physicalEvent, System.Drawing.Point? point)
        {
            foreach (var gm in GestureMachines)
            {
                var eventIsConsumed = gm.Input(physicalEvent, point);
                if (eventIsConsumed == true)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Input(Crevice.Core.Events.IPhysicalEvent physicalEvent)
            => Input(physicalEvent, null);

        public void Reset()
        {
            foreach(var gm in GestureMachines)
            {
                gm.Reset();
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            foreach (var gm in GestureMachines)
            {
                gm.Dispose();
            }
        }

        ~GestureMachineCluster()
        {
            Dispose();
        }
    }

    public class NullGestureMachineCluster : GestureMachineCluster
    {
        public NullGestureMachineCluster()
            : base(new List<CustomGestureMachine>())
        { }
    }

    // GestureMachineFactory
    public class GestureMachineCandidate
    {
        public readonly string UserScriptString;
        public readonly string UserScriptCacheFile;
        public readonly bool RestoreAllowed;
        public readonly string ScriptSourceResolverBaseDirectory;
        public readonly string ScriptMetadataResolverBaseDirectory;
        
        public GestureMachineCandidate(
            string userScriptString,
            string userScriptCacheFile,
            bool allowRestore,
            string scriptSourceResolverBaseDirectory,
            string scriptMetadataResolverBaseDirectory
            )
        {
            this.UserScriptString = userScriptString;
            this.UserScriptCacheFile = userScriptCacheFile;
            this.RestoreAllowed = allowRestore;
            this.ScriptSourceResolverBaseDirectory = scriptSourceResolverBaseDirectory;
            this.ScriptMetadataResolverBaseDirectory = scriptMetadataResolverBaseDirectory;
            // appConfig.GetOrSetDefaultUserScriptFile(Encoding.UTF8.GetString(Properties.Resources.DefaultUserScript))
        }

        private Script _parsedUserScript = null;
        public Script ParsedUserScript
        {
            get
            {
                if (_parsedUserScript == null)
                {
                    _parsedUserScript = UserScript.ParseScript(
                        UserScriptString, 
                        ScriptSourceResolverBaseDirectory, 
                        ScriptMetadataResolverBaseDirectory);
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
                    _errors = UserScript.CompileUserScript(ParsedUserScript);
                }
                return _errors;
            }
            private set
            {
                _errors = value;
            }
        }

        private UserScriptAssembly.Cache _userScriptAssemblyCache = null;
        public UserScriptAssembly.Cache UserScriptAssemblyCache
        {
            get
            {
                if (_userScriptAssemblyCache == null)
                {
                    _userScriptAssemblyCache = UserScript.GenerateUserScriptAssemblyCache(UserScriptString, ParsedUserScript);
                }
                return _userScriptAssemblyCache;
            }
            private set
            {
                _userScriptAssemblyCache = value;
            }
        }

        protected GestureMachineCluster Create(
            UserScriptExecutionContext ctx,
            UserScriptAssembly.Cache userScriptAssembly)
        {
            UserScript.EvaluateUserScriptAssembly(ctx, userScriptAssembly);
            var gestureMachines = 
                from profile in ctx.Profiles
                select new CustomGestureMachine(profile.UserConfig.Core, profile.CallbackManager, profile.RootElement);
            return new GestureMachineCluster(gestureMachines.ToList());
        }

        public GestureMachineCluster CreateNew(UserScriptExecutionContext ctx)
        {
            return Create(ctx, UserScriptAssemblyCache);
        }

        private UserScriptAssembly.Cache _restorationCache = null;
        public UserScriptAssembly.Cache RestorationCache
        {
            get
            {
                if (_restorationCache == null)
                {
                    _restorationCache = UserScript.LoadUserScriptAssemblyCache(UserScriptCacheFile, UserScriptString);
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

        public GestureMachineCluster Restore(UserScriptExecutionContext ctx)
        {
            return Create(ctx, RestorationCache);
        }
    }
}
