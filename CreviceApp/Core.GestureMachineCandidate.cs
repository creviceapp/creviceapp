using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;


namespace CreviceApp.Core
{
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

        protected CustomGestureMachine Create(
            Config.UserConfig userConfig,
            UserScriptExecutionContext ctx,
            UserScriptAssembly.Cache userScriptAssembly)
        {
            UserScript.EvaluateUserScriptAssembly(ctx, userScriptAssembly);
            var gestureMachine = new CustomGestureMachine(ctx.Root);
            gestureMachine.Config.GestureTimeout = userConfig.Gesture.Timeout;
            gestureMachine.Config.StrokeDirectionChangeThreshold = userConfig.Gesture.StrokeDirectionChangeThreshold;
            gestureMachine.Config.StrokeExtensionThreshold = userConfig.Gesture.StrokeExtensionThreshold;
            gestureMachine.Config.StrokeStartThreshold = userConfig.Gesture.InitialStrokeThreshold;
            gestureMachine.Config.StrokeWatchInterval = userConfig.Gesture.WatchInterval;
            return gestureMachine;
        }

        public CustomGestureMachine CreateNew(Config.UserConfig userConfig, UserScriptExecutionContext ctx)
        {
            return Create(userConfig, ctx, UserScriptAssemblyCache);
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

        public CustomGestureMachine Restore(Config.UserConfig userConfig, UserScriptExecutionContext ctx)
        {
            return Create(userConfig, ctx, RestorationCache);
        }
    }
}
