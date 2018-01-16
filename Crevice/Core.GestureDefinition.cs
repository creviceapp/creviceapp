using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crevice.Core
{
    public interface IWhenEvaluatable
    {
        bool EvaluateWhenFunc<V>(V ctx, Config.Def.WhenClauseBinding whenClauseBinding) 
            where V : UserActionContext.UserActionEvaluationContext;
    }

    public interface IBeforeExecutable
    {
        void ExecuteBeforeFunc<X>(X ctx, Config.Def.BeforeClauseBinding beforeClauseBinding)
            where X : UserActionContext.UserActionExecutionContext;
    }

    public interface IDoExecutable
    {
        void ExecuteDoFunc<X>(X ctx, Config.Def.DoClauseBinding doClauseBinding)
            where X : UserActionContext.UserActionExecutionContext;
    }

    public interface IAfterExecutable
    {
        void ExecuteAfterFunc<X>(X ctx, Config.Def.AfterClauseBinding do)
            where X : UserActionContext.UserActionExecutionContext;
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

        /*
         
        internal static IEnumerable<T> FilterByWhenClause<T>(
            UserActionExecutionContextBase ctx,
            IEnumerable<T> gestureDef) 
            where T : IWhenEvaluatable
        {
            return FilterByWhenClause(ctx, gestureDef, new Dictionary<DSL.Def.WhenFunc, bool>());
        }

        internal static IEnumerable<T> FilterByWhenClause<T>(
            UserActionExecutionContextBase ctx,
            IEnumerable<T> gestureDef,
            Dictionary<DSL.Def.WhenFunc, bool> cache) 
            where T : IWhenEvaluatable
        {
            // This evaluation of functions given as the parameter of `@when` clause can be executed in parallel, 
            // but executing it in sequential order here for simplicity.
            return gestureDef
                .Where(x => x.EvaluateUserWhenFunc(ctx, cache))
                .ToList();
        }
             
             */

        public bool EvaluateWhenFunc<V>(V ctx, Config.Def.WhenClauseBinding whenClauseBinding)
            where V : UserActionContext.UserActionEvaluationContext
        {
            try
            {
                return whenClauseBinding(ctx, whenFunc);
            }
            catch (Exception ex)
            {
                Verbose.Print(
                    "An exception was thrown when executing a WhenFunc of a gesture. " +
                    "This error may automatically be recovered.\n{0} :\n{1}",
                    ex.GetType().Name,
                    ex.StackTrace);
            }
            return false;
        }

        /*
         
             
        internal static IEnumerable<T> FilterByWhenClause<T>(
            UserActionExecutionContextBase ctx,
            IEnumerable<T> gestureDef) 
            where T : IWhenEvaluatable
        {
            return FilterByWhenClause(ctx, gestureDef, new Dictionary<DSL.Def.WhenFunc, bool>());
        }

        internal static IEnumerable<T> FilterByWhenClause<T>(
            UserActionExecutionContextBase ctx,
            IEnumerable<T> gestureDef,
            Dictionary<DSL.Def.WhenFunc, bool> cache) 
            where T : IWhenEvaluatable
        {
            // This evaluation of functions given as the parameter of `@when` clause can be executed in parallel, 
            // but executing it in sequential order here for simplicity.
            return gestureDef
                .Where(x => x.EvaluateUserWhenFunc(ctx, cache))
                .ToList();
        }
             */

        public bool EvaluateWhenFunc<V>(V ctx, Config.Def.WhenClauseBinding binding, Dictionary<GestureDefinition, bool> cache)
            where V : UserActionContext.UserActionEvaluationContext
        {
            if (!cache.Keys.Contains(this))
            {
                cache[this] = EvaluateWhenFunc(ctx, binding);
            }
            return cache[this];
        }

        public static IEnumerable<GestureDefinition> EvaluateAndFilterByWhenFunc<V>(V ctx, Config.Def.WhenClauseBinding binding, IEnumerable<GestureDefinition> gestureDef)
            where V : UserActionContext.UserActionEvaluationContext
        {
            var cache = new Dictionary<GestureDefinition, bool>();
            // This evaluation of functions given as the parameter of `@when` clause can be executed in parallel, 
            // but executing it in sequential order here for simplicity.
            return gestureDef
                .Where(x => x.EvaluateWhenFunc(ctx, binding, cache))
                .ToList();
        }

        /*
        public static IEnumerable<GestureDefinition> EvaluateAndFilterByWhenFunc<V>(V ctx, IEnumerable<GestureDefinition> gestureDef)
            where V : UserActionContext.UserActionEvaluationContext
        {
            var cache = new Dictionary<DSL.Def.WhenFunc, bool>();

            if (!cache.Keys.Contains(whenFunc))
            {
                cache[whenFunc] = EvaluateSafely(ctx, whenFunc);
            }
            return cache[whenFunc];
        }

        private static bool EvaluateSafely<T>(T ctx, DSL.Def.WhenFunc func)
            where T : UserActionContext.UserActionEvaluationContext
        {
            try
            {
                return func(ctx);
            }
            catch (Exception ex)
            {
                Verbose.Print(
                    "An exception was thrown when executing a WhenFunc of a gesture. " +
                    "This error may automatically be recovered.\n{0} :\n{1}",
                    ex.GetType().Name,
                    ex.StackTrace);
            }
            return false;
        }

        protected internal static void ExecuteSafely(UserActionExecutionContextBase ctx, DSL.Def.BeforeFunc func)
        {
            try
            {
                func(ctx);
            }
            catch (Exception ex)
            {
                Verbose.Print(
                    "An exception was thrown when executing a BeforeFunc of a gesture. " +
                    "This error may automatically be recovered.\n{0} :\n{1}",
                    ex.GetType().Name,
                    ex.StackTrace);
            }
        }

        protected internal static void ExecuteSafely(UserActionExecutionContextBase ctx, DSL.Def.DoFunc func)
        {
            try
            {
                func(ctx);
            }
            catch (Exception ex)
            {
                Verbose.Print(
                    "An exception was thrown when executing a DoFunc of a gesture. " +
                    "This error may automatically be recovered.\n{0} :\n{1}",
                    ex.GetType().Name,
                    ex.StackTrace);
            }
        }
        
        protected internal static void ExecuteSafely(UserActionExecutionContextBase ctx, DSL.Def.AfterFunc func)
        {
            try
            {
                func(ctx);
            }
            catch (Exception ex)
            {
                Verbose.Print(
                    "An exception was thrown when executing a AfterFunc of a gesture. " +
                    "This error may automatically be recovered.\n{0} :\n{1}",
                    ex.GetType().Name,
                    ex.StackTrace);
            }
        }

        protected internal static bool EvaluateSafely(UserActionExecutionContextBase ctx, DSL.Def.WhenFunc func)
        {
            try
            {
                return func(ctx);
            }
            catch (Exception ex)
            {
                Verbose.Print(
                    "An exception was thrown when executing a WhenFunc of a gesture. " +
                    "This error may automatically be recovered.\n{0} :\n{1}",
                    ex.GetType().Name,
                    ex.StackTrace);
            }
            return false;
        }
        */
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

        public void ExecuteBeforeFunc<X>(X ctx)
            where X : UserActionContext.UserActionExecutionContext
        {
            if (beforeFunc != null)
            {
                try
                {
                    beforeFunc(ctx);
                }
                catch (Exception ex)
                {
                    Verbose.Print(
                        "An exception was thrown when executing a BeforeFunc of a gesture. " +
                        "This error may automatically be recovered.\n{0} :\n{1}",
                        ex.GetType().Name,
                        ex.StackTrace);
                }
            }
        }

        public void ExecuteUserDoFunc<X>(X ctx)
            where X : UserActionContext.UserActionExecutionContext
        {
            try
            {
                doFunc(ctx);
            }
            catch (Exception ex)
            {
                Verbose.Print(
                    "An exception was thrown when executing a AfterFunc of a gesture. " +
                    "This error may automatically be recovered.\n{0} :\n{1}",
                    ex.GetType().Name,
                    ex.StackTrace);
            }
        }
        
        public void ExecuteUserAfterFunc<X>(X ctx)
            where X : UserActionContext.UserActionExecutionContext
        {
            try
            {
                afterFunc(ctx);
            }
            catch (Exception ex)
            {
                Verbose.Print(
                    "An exception was thrown when executing a AfterFunc of a gesture. " +
                    "This error may automatically be recovered.\n{0} :\n{1}",
                    ex.GetType().Name,
                    ex.StackTrace);
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

        public void ExecuteBeforeFunc<X>(X ctx)
            where X : UserActionContext.UserActionExecutionContext
        {
            if (beforeFunc != null)
            {
                try
                {
                    beforeFunc(ctx);
                }
                catch (Exception ex)
                {
                    Verbose.Print(
                        "An exception was thrown when executing a BeforeFunc of a gesture. " +
                        "This error may automatically be recovered.\n{0} :\n{1}",
                        ex.GetType().Name,
                        ex.StackTrace);
                }
            }
        }

        public void ExecuteUserDoFunc<X>(X ctx)
            where X : UserActionContext.UserActionExecutionContext
        {
            try
            {
                doFunc(ctx);
            }
            catch (Exception ex)
            {
                Verbose.Print(
                    "An exception was thrown when executing a AfterFunc of a gesture. " +
                    "This error may automatically be recovered.\n{0} :\n{1}",
                    ex.GetType().Name,
                    ex.StackTrace);
            }
        }

        public void ExecuteUserAfterFunc<X>(X ctx)
            where X : UserActionContext.UserActionExecutionContext
        {
            try
            {
                afterFunc(ctx);
            }
            catch (Exception ex)
            {
                Verbose.Print(
                    "An exception was thrown when executing a AfterFunc of a gesture. " +
                    "This error may automatically be recovered.\n{0} :\n{1}",
                    ex.GetType().Name,
                    ex.StackTrace);
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

        public void ExecuteUserDoFunc<X>(X ctx)
            where X : UserActionContext.UserActionExecutionContext
        {
            try
            {
                doFunc(ctx);
            }
            catch (Exception ex)
            {
                Verbose.Print(
                    "An exception was thrown when executing a DoFunc of a gesture. " +
                    "This error may automatically be recovered.\n{0} :\n{1}",
                    ex.GetType().Name,
                    ex.StackTrace);
            }
        }
    }
}
