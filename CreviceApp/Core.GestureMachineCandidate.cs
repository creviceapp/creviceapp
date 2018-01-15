using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;


namespace CreviceApp.Core
{

    public class GestureMachineCandidate
    {
        public readonly DateTime Created;
        public readonly bool RestoreFromCache;
        public readonly string UserScriptString;

        private readonly CreviceApp.App.AppConfig appConfig;

        public GestureMachineCandidate(
            CreviceApp.App.AppConfig appConfig,
            bool restoreFromCache)
        {
            this.appConfig = appConfig;
            this.RestoreFromCache = restoreFromCache;
            this.Created = DateTime.Now;
            this.UserScriptString = appConfig.GetOrSetDefaultUserScriptFile(Encoding.UTF8.GetString(Properties.Resources.DefaultUserScript));
        }

        private Script _parsedUserScript = null;
        public Script ParsedUserScript
        {
            get
            {
                if (_parsedUserScript == null)
                {
                    _parsedUserScript = UserScript.ParseScript(UserScriptString, appConfig.UserDirectory, appConfig.UserDirectory);
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

        private UserScriptAssembly.Cache _restorationCache = null;
        public UserScriptAssembly.Cache RestorationCache
        {
            get
            {
                if (_restorationCache == null && RestoreFromCache)
                {
                    _restorationCache = UserScript.LoadUserScriptAssemblyCache(appConfig.UserScriptCacheFile, UserScriptString);
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

        private FSM.GestureMachine Create(
            CreviceApp.App.AppConfig AppConfig,
            UserScriptExecutionContext ctx,
            UserScriptAssembly.Cache userScriptAssembly)
        {
            UserScript.EvaluateUserScriptAssembly(ctx, userScriptAssembly);
            var gestureDef = ctx.GetGestureDefinition();
            return new FSM.GestureMachine(AppConfig.UserConfig, gestureDef);
        }

        public FSM.GestureMachine Restore(CreviceApp.App.AppConfig AppConfig, UserScriptExecutionContext ctx)
        {
            return Create(AppConfig, ctx, RestorationCache);
        }

        public FSM.GestureMachine Restore(CreviceApp.App.AppConfig AppConfig)
        {
            var ctx = new UserScriptExecutionContext(AppConfig);
            return Restore(AppConfig, ctx);
        }

        public FSM.GestureMachine CreateNew(CreviceApp.App.AppConfig AppConfig, UserScriptExecutionContext ctx)
        {
            return Create(AppConfig, ctx, UserScriptAssemblyCache);
        }

        public FSM.GestureMachine CreateNew(CreviceApp.App.AppConfig AppConfig)
        {
            var ctx = new UserScriptExecutionContext(AppConfig);
            return CreateNew(AppConfig, ctx);
        }
    }
}
