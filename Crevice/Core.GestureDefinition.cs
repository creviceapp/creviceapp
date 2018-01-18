using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crevice.Core
{
    public interface IWhenEvaluatable
    {
        bool EvaluateWhenFunc(ActionContext ctx);
        bool EvaluateWhenFunc(ActionContext ctx, Dictionary<DSL.Def.WhenFunc, bool> cache);
    }

    public interface IBeforeExecutable
    {
        void ExecuteBeforeFunc(ActionContext ctx);
    }

    public interface IDoExecutable
    {
        void ExecuteDoFunc(ActionContext ctx);
    }

    public interface IAfterExecutable
    {
        void ExecuteAfterFunc(ActionContext ctx);
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

        virtual public bool IsComplete //HasAction
        {
            get { return false; }
        }

        public bool EvaluateWhenFunc(ActionContext ctx)
        {
            return whenFunc(ctx);
        }

        public bool EvaluateWhenFunc(ActionContext ctx, Dictionary<DSL.Def.WhenFunc, bool> cache)
        {
            if (!cache.Keys.Contains(whenFunc))
            {
                cache[whenFunc] = EvaluateWhenFunc(ctx);
            }
            return cache[whenFunc];
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

        public void ExecuteBeforeFunc(ActionContext ctx)
        {
            beforeFunc(ctx);
        }

        public void ExecuteDoFunc(ActionContext ctx)
        {
            doFunc(ctx);
        }
        
        public void ExecuteAfterFunc(ActionContext ctx)
        {
            afterFunc(ctx);
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

        public void ExecuteBeforeFunc(ActionContext ctx)
        {
            beforeFunc(ctx);
        }

        public void ExecuteDoFunc(ActionContext ctx)
        {
            doFunc(ctx);
        }

        public void ExecuteAfterFunc(ActionContext ctx)
        {
            afterFunc(ctx);
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

        public void ExecuteDoFunc(ActionContext ctx)
        {
            doFunc(ctx);
        }
    }
}
