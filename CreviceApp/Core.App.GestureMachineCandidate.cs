using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;


namespace CreviceApp.Core.App
{

    public class GestureMachineCandidate
    {
        public readonly DateTime Created;
        public readonly bool RestoreFromCache;
        public readonly string UserScriptString;

        private readonly UserScript userScript;

        public GestureMachineCandidate(
            UserScript userScript,
            bool restoreFromCache)
        {
            this.userScript = userScript;
            this.RestoreFromCache = restoreFromCache;
            this.Created = DateTime.Now;
            this.UserScriptString = this.userScript.GetUserScriptString();
        }

        private Script _parsedUserScript = null;
        public Script ParsedUserScript
        {
            get
            {
                if (_parsedUserScript == null)
                {
                    _parsedUserScript = userScript.ParseScript(UserScriptString);
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
                    _errors = userScript.CompileUserScript(ParsedUserScript);
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
                    _restorationCache = userScript.LoadUserScriptAssemblyCache(UserScriptString);
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
                    _userScriptAssemblyCache = userScript.GenerateUserScriptAssemblyCache(UserScriptString, ParsedUserScript);
                }
                return _userScriptAssemblyCache;
            }
            private set
            {
                _userScriptAssemblyCache = value;
            }
        }

        private FSM.GestureMachine Create(
            AppGlobal Global,
            UserScriptExecutionContext ctx,
            UserScriptAssembly.Cache userScriptAssembly)
        {
            userScript.EvaluateUserScriptAssembly(ctx, userScriptAssembly);
            var gestureDef = ctx.GetGestureDefinition();
            return new FSM.GestureMachine(Global.UserConfig, gestureDef);
        }

        public FSM.GestureMachine Restore(AppGlobal Global, UserScriptExecutionContext ctx)
        {
            return Create(Global, ctx, RestorationCache);
        }

        public FSM.GestureMachine Restore(AppGlobal Global)
        {
            var ctx = new UserScriptExecutionContext(Global);
            return Restore(Global, ctx);
        }

        public FSM.GestureMachine CreateNew(AppGlobal Global, UserScriptExecutionContext ctx)
        {
            return Create(Global, ctx, UserScriptAssemblyCache);
        }

        public FSM.GestureMachine CreateNew(AppGlobal Global)
        {
            var ctx = new UserScriptExecutionContext(Global);
            return CreateNew(Global, ctx);
        }
    }
}
