using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core
{
    public interface IDoExecutable
    {
        void Execute(UserActionExecutionContext ctx);
    }

    public interface IWhenEvaluatable
    {
        bool Evaluate(UserActionExecutionContext ctx, Dictionary<DSL.Def.WhenFunc, bool> cache);
    }

    public class GestureDefinition : IWhenEvaluatable
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
        
        public bool Evaluate(UserActionExecutionContext ctx, Dictionary<DSL.Def.WhenFunc, bool> cache)
        {
            if (!cache.Keys.Contains(whenFunc))
            {
                cache[whenFunc] = EvaluateSafely(ctx, whenFunc);
            }
            return cache[whenFunc];
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

    
    public class IfButtonGestureDefinition : GestureDefinition, IDoExecutable
    {
        public readonly DSL.Def.AcceptableInIfButtonClause ifButton;
        public readonly DSL.Def.DoFunc doFunc;
        public IfButtonGestureDefinition(
            DSL.Def.WhenFunc whenFunc,
            DSL.Def.AcceptableInIfButtonClause ifButton,
            DSL.Def.DoFunc doFunc
            ) : base(whenFunc)
        {
            this.ifButton = ifButton;
            this.doFunc = doFunc;
        }
        override public bool IsComplete
        {
            get
            {
                return whenFunc != null &&
                       ifButton != null &&
                       doFunc != null;
            }
        }

        public void Execute(UserActionExecutionContext ctx)
        {
            ExecuteSafely(ctx, doFunc);
        }
    }

    public class OnButtonGestureDefinition : GestureDefinition
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

    public class OnButtonIfButtonGestureDefinition : OnButtonGestureDefinition, IDoExecutable
    {
        public readonly DSL.Def.AcceptableInIfButtonClause ifButton;
        public readonly DSL.Def.DoFunc doFunc;
        public OnButtonIfButtonGestureDefinition(
            DSL.Def.WhenFunc whenFunc,
            DSL.Def.AcceptableInOnClause onButton,
            DSL.Def.AcceptableInIfButtonClause ifButton,
            DSL.Def.DoFunc doFunc
            ) : base(whenFunc, onButton)
        {
            this.ifButton = ifButton;
            this.doFunc = doFunc;
        }
        override public bool IsComplete
        {
            get
            {
                return whenFunc != null &&
                       onButton != null &&
                       ifButton != null &&
                       doFunc != null;
            }
        }

        public void Execute(UserActionExecutionContext ctx)
        {
            ExecuteSafely(ctx, doFunc);
        }
    }

    public class OnButtonIfStrokeGestureDefinition : OnButtonGestureDefinition, IDoExecutable
    {
        public readonly Def.Stroke stroke;
        public readonly DSL.Def.DoFunc doFunc;
        public OnButtonIfStrokeGestureDefinition(
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

        public void Execute(UserActionExecutionContext ctx)
        {
            ExecuteSafely(ctx, doFunc);
        }
    }
}
