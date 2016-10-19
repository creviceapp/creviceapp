using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core
{
    public interface IWhenEvaluatable
    {
        bool EvaluateUserWhenFunc(UserActionExecutionContext ctx, Dictionary<DSL.Def.WhenFunc, bool> cache);
    }

    public interface IBeforeExecutable
    {
        void ExecuteUserBeforeFunc(UserActionExecutionContext ctx);
    }

    public interface IDoExecutable
    {
        void ExecuteUserDoFunc(UserActionExecutionContext ctx);
    }

    public interface IAfterExecutable
    {
        void ExecuteUserAfterFunc(UserActionExecutionContext ctx);
    }

    public class GestureDefinition 
        : IWhenEvaluatable
    {
        public readonly DSL.Def.WhenFunc whenFunc;

        public GestureDefinition(
            DSL.Def.WhenFunc whenFunc
            )
        {
            this.whenFunc = whenFunc;
        }

        virtual public bool IsComplete
        {
            get { return false; }
        }
        
        public bool EvaluateUserWhenFunc(UserActionExecutionContext ctx, Dictionary<DSL.Def.WhenFunc, bool> cache)
        {
            if (!cache.Keys.Contains(whenFunc))
            {
                cache[whenFunc] = EvaluateSafely(ctx, whenFunc);
            }
            return cache[whenFunc];
        }

        protected internal static void ExecuteSafely(UserActionExecutionContext ctx, DSL.Def.BeforeFunc func)
        {
            try
            {
                func(ctx);
            }
            catch (Exception ex)
            {
                Debug.Print(
                    "An exception was thrown when executing a BeforeFunc of a gesture. " +
                    "This error may automatically be recovered.\n{0} :\n{1}",
                    ex.GetType().Name,
                    ex.StackTrace);
            }
        }

        protected internal static void ExecuteSafely(UserActionExecutionContext ctx, DSL.Def.DoFunc func)
        {
            try
            {
                func(ctx);
            }
            catch (Exception ex)
            {
                Debug.Print(
                    "An exception was thrown when executing a DoFunc of a gesture. " +
                    "This error may automatically be recovered.\n{0} :\n{1}",
                    ex.GetType().Name,
                    ex.StackTrace);
            }
        }
        
        protected internal static void ExecuteSafely(UserActionExecutionContext ctx, DSL.Def.AfterFunc func)
        {
            try
            {
                func(ctx);
            }
            catch (Exception ex)
            {
                Debug.Print(
                    "An exception was thrown when executing a AfterFunc of a gesture. " +
                    "This error may automatically be recovered.\n{0} :\n{1}",
                    ex.GetType().Name,
                    ex.StackTrace);
            }
        }

        protected internal static bool EvaluateSafely(UserActionExecutionContext ctx, DSL.Def.WhenFunc func)
        {
            try
            {
                return func(ctx);
            }
            catch (Exception ex)
            {
                Debug.Print(
                    "An exception was thrown when executing a WhenFunc of a gesture. " +
                    "This error may automatically be recovered.\n{0} :\n{1}",
                    ex.GetType().Name,
                    ex.StackTrace);
            }
            return false;
        }
    }
    
    public class IfButtonGestureDefinition 
        : GestureDefinition, IBeforeExecutable, IDoExecutable, IAfterExecutable
    {
        public readonly DSL.Def.AcceptableInIfButtonClause ifButton;
        public readonly DSL.Def.BeforeFunc beforeFunc;
        public readonly DSL.Def.DoFunc doFunc;
        public readonly DSL.Def.AfterFunc afterFunc;

        public IfButtonGestureDefinition(
            DSL.Def.WhenFunc whenFunc,
            DSL.Def.AcceptableInIfButtonClause ifButton,
            DSL.Def.BeforeFunc beforeFunc,
            DSL.Def.DoFunc doFunc,
            DSL.Def.AfterFunc afterFunc
            ) : base(whenFunc)
        {
            this.ifButton = ifButton;
            this.beforeFunc = beforeFunc;
            this.doFunc = doFunc;
            this.afterFunc = afterFunc;
        }

        override public bool IsComplete
        {
            get
            {
                return whenFunc != null &&
                       ifButton != null &&
                       (beforeFunc != null || doFunc != null || afterFunc != null);
            }
        }

        public void ExecuteUserBeforeFunc(UserActionExecutionContext ctx)
        {
            if (beforeFunc != null)
            {
                ExecuteSafely(ctx, beforeFunc);
            }
        }

        public void ExecuteUserDoFunc(UserActionExecutionContext ctx)
        {
            if (doFunc != null)
            {
                ExecuteSafely(ctx, doFunc);
            }
        }
        
        public void ExecuteUserAfterFunc(UserActionExecutionContext ctx)
        {
            if (afterFunc != null)
            {
                ExecuteSafely(ctx, afterFunc);
            }
        }
    }

    public class OnButtonGestureDefinition 
        : GestureDefinition
    {
        public readonly DSL.Def.AcceptableInOnClause onButton;

        public OnButtonGestureDefinition(
            DSL.Def.WhenFunc whenFunc,
            DSL.Def.AcceptableInOnClause onButton
            ) : base(whenFunc)
        {
            this.onButton = onButton;
        }
    }

    public class OnButtonWithIfButtonGestureDefinition 
        : OnButtonGestureDefinition, IBeforeExecutable, IDoExecutable, IAfterExecutable
    {
        public readonly DSL.Def.AcceptableInIfButtonClause ifButton;
        public readonly DSL.Def.BeforeFunc beforeFunc;
        public readonly DSL.Def.DoFunc doFunc;
        public readonly DSL.Def.AfterFunc afterFunc;

        public OnButtonWithIfButtonGestureDefinition(
            DSL.Def.WhenFunc whenFunc,
            DSL.Def.AcceptableInOnClause onButton,
            DSL.Def.AcceptableInIfButtonClause ifButton,
            DSL.Def.BeforeFunc beforeFunc,
            DSL.Def.DoFunc doFunc,
            DSL.Def.AfterFunc afterFunc
            ) : base(whenFunc, onButton)
        {
            this.ifButton = ifButton;
            this.beforeFunc = beforeFunc;
            this.doFunc = doFunc;
            this.afterFunc = afterFunc;
        }

        override public bool IsComplete
        {
            get
            {
                return whenFunc != null &&
                       onButton != null &&
                       ifButton != null &&
                       (beforeFunc != null || doFunc != null || afterFunc != null);
            }
        }

        public void ExecuteUserBeforeFunc(UserActionExecutionContext ctx)
        {
            if (beforeFunc != null)
            {
                ExecuteSafely(ctx, beforeFunc);
            }
        }

        public void ExecuteUserDoFunc(UserActionExecutionContext ctx)
        {
            if (doFunc != null)
            {
                ExecuteSafely(ctx, doFunc);
            }
        }

        public void ExecuteUserAfterFunc(UserActionExecutionContext ctx)
        {
            if (afterFunc != null)
            {
                ExecuteSafely(ctx, afterFunc);
            }
        }
    }

    public class OnButtonWithIfStrokeGestureDefinition : OnButtonGestureDefinition, IDoExecutable
    {
        public readonly Def.Stroke stroke;
        public readonly DSL.Def.DoFunc doFunc;

        public OnButtonWithIfStrokeGestureDefinition(
            DSL.Def.WhenFunc whenFunc,
            DSL.Def.AcceptableInOnClause onButton,
            Def.Stroke stroke,
            DSL.Def.DoFunc doFunc
            ) : base(whenFunc, onButton)
        {
            this.stroke = stroke;
            this.doFunc = doFunc;
        }

        override public bool IsComplete
        {
            get
            {
                return whenFunc != null &&
                       onButton != null &&
                       stroke != null &&
                       doFunc != null;
            }
        }

        public void ExecuteUserDoFunc(UserActionExecutionContext ctx)
        {
            ExecuteSafely(ctx, doFunc);
        }
    }
}
