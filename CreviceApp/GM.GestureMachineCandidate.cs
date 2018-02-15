using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;


namespace Crevice.GestureMachine
{
    using System.IO;
    using Crevice.UserScript;

    public class GestureMachineCandidate
    {
        public readonly string UserDirectory;
        public readonly string UserScriptString;
        public readonly string UserScriptCacheFile;
        public readonly bool RestoreAllowed;
        
        public GestureMachineCandidate(
            string userDirectory,
            string userScriptString,
            string userScriptCacheFile,
            bool allowRestore
            )
        {
            UserDirectory = userDirectory;
            UserScriptString = userScriptString;
            UserScriptCacheFile = userScriptCacheFile;
            RestoreAllowed = allowRestore;
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
                        UserDirectory,
                        UserDirectory);
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

        internal string UserDirectoryStructureString
            => new DirectoryInfo(UserDirectory)
                .EnumerateFiles("*.csx", SearchOption.AllDirectories)
                .Select(f => $"{f.FullName} {f.LastWriteTime}")
                .Aggregate("", (a, b) => a + "\r\n" + b);

        private UserScriptAssembly.Cache _userScriptAssemblyCache = null;
        public UserScriptAssembly.Cache UserScriptAssemblyCache
        {
            get
            {
                if (_userScriptAssemblyCache == null)
                {
                    _userScriptAssemblyCache = UserScript.GenerateUserScriptAssemblyCache(UserScriptString + "\r\n" + UserDirectoryStructureString, ParsedUserScript);
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
            return new GestureMachineCluster(ctx.Profiles);
        }

        public GestureMachineCluster CreateNew(UserScriptExecutionContext ctx)
            => Create(ctx, UserScriptAssemblyCache);

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
            => RestorationCache != null;

        public GestureMachineCluster Restore(UserScriptExecutionContext ctx)
            => Create(ctx, RestorationCache);
    }
}
