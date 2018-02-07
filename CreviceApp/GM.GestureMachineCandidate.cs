using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;


namespace Crevice.GestureMachine
{
    using Crevice.UserScript;

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
            UserScriptString = userScriptString;
            UserScriptCacheFile = userScriptCacheFile;
            RestoreAllowed = allowRestore;
            ScriptSourceResolverBaseDirectory = scriptSourceResolverBaseDirectory;
            ScriptMetadataResolverBaseDirectory = scriptMetadataResolverBaseDirectory;
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
                select new GestureMachine(profile.UserConfig.Core, profile.CallbackManager, profile.RootElement);
            return new GestureMachineCluster(gestureMachines.ToList());
        }

        public GestureMachineCluster CreateNew(UserScriptExecutionContext ctx)
        {
            return Create(ctx, UserScriptAssemblyCache);
        }

        private bool _restorationFailed = false;
        private UserScriptAssembly.Cache _restorationCache = null;
        public UserScriptAssembly.Cache RestorationCache
        {
            get
            {
                if (!_restorationFailed && _restorationCache == null)
                {
                    _restorationCache = UserScript.LoadUserScriptAssemblyCache(UserScriptCacheFile, UserScriptString);
                    if (_restorationCache == null)
                    {
                        _restorationFailed = true;
                    }
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
