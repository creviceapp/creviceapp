using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core
{
    public class GestureDefinition
    {
        public readonly DSL.Def.WhenFunc whenFunc;
        public readonly DSL.Def.AcceptableInOnClause onButton;
        public GestureDefinition(
            DSL.Def.WhenFunc whenFunc,
            DSL.Def.AcceptableInOnClause onButton
            )
        {
            this.whenFunc = whenFunc;
            this.onButton = onButton;
        }
        virtual public bool IsComplete
        {
            get { return false; }
        }
    }

    public class ButtonGestureDefinition : GestureDefinition
    {
        public readonly DSL.Def.AcceptableInIfButtonClause ifButton;
        public readonly DSL.Def.DoFunc doFunc;
        public ButtonGestureDefinition(
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
    }

    public class StrokeGestureDefinition : GestureDefinition
    {
        public readonly Def.Stroke stroke;
        public readonly DSL.Def.DoFunc doFunc;
        public StrokeGestureDefinition(
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
    }
}
