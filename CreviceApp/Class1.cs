using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp
{
    using System.Drawing;
    using Crevice.Core.Context;
    using Crevice.Core.DSL;
    using Crevice.Core.Keys;
    using Crevice.Core.FSM;

    using CreviceApp.WinAPI.Window.Impl;

    public class Events
    {
        private static Events Instance = new Events();

        private readonly LogicalSingleThrowKeys LogicalSingleThrowKeys;
        private readonly PhysicalSingleThrowKeys PhysicalSingleThrowKeys;

        private readonly LogicalDoubleThrowKeys LogicalDoubleThrowKeys;
        private readonly PhysicalDoubleThrowKeys PhysicalDoubleThrowKeys;
        
        private Events()
        {
            LogicalSingleThrowKeys = new LogicalSingleThrowKeys(10);
            PhysicalSingleThrowKeys = new PhysicalSingleThrowKeys(LogicalSingleThrowKeys);
            LogicalDoubleThrowKeys = new LogicalDoubleThrowKeys(300);
            PhysicalDoubleThrowKeys = new PhysicalDoubleThrowKeys(LogicalDoubleThrowKeys);
        }




    }

    public class CustomEvaluationContext : EvaluationContext
    {
        public readonly Point GestureStartPosition;
        public readonly ForegroundWindowInfo ForegroundWindow;
        public readonly PointedWindowInfo PointedWindow;

        public CustomEvaluationContext(Point gestureStartPosition)
        {
            this.GestureStartPosition = gestureStartPosition;
            this.ForegroundWindow = new ForegroundWindowInfo();
            this.PointedWindow = new PointedWindowInfo(gestureStartPosition);
        }
    }

    public class CustomExecutionContext : ExecutionContext
    {
        public readonly Point GestureStartPosition;
        public readonly Point GestureEndPosition;
        public readonly ForegroundWindowInfo ForegroundWindow;
        public readonly PointedWindowInfo PointedWindow;

        public CustomExecutionContext(CustomEvaluationContext evaluationContext, Point gestureEndPosition)
        {
            this.GestureStartPosition = evaluationContext.GestureStartPosition;
            this.GestureEndPosition = gestureEndPosition;
            this.ForegroundWindow = evaluationContext.ForegroundWindow;
            this.PointedWindow = evaluationContext.PointedWindow;
        }
    }

    class CustomContextManager : ContextManager<CustomEvaluationContext, CustomExecutionContext>
    {
        public Point CursorPosition { get; set; }

        public override CustomEvaluationContext CreateEvaluateContext()
            => new CustomEvaluationContext(CursorPosition);

        public override CustomExecutionContext CreateExecutionContext(CustomEvaluationContext evaluationContext)
            => new CustomExecutionContext(evaluationContext, CursorPosition);

        /*
         override

        public virtual bool EvaluateWhenEvaluator(TEvalContext evalContext, WhenElement<TEvalContext, TExecContext> whenElement)
            => whenElement.WhenEvaluator(evalContext);

        public virtual void ExecuteExcutor(TExecContext execContext, ExecuteAction<TExecContext> executeAction)
            => executeAction(execContext);

         */
    }

    public class CustomRootElement : RootElement<CustomEvaluationContext, CustomExecutionContext>
    {

    }

    class CustomGestureMachine : GestureMachine<GestureMachineConfig, CustomContextManager, CustomEvaluationContext, CustomExecutionContext>
    {
        public CustomGestureMachine(CustomRootElement rootElement)
            : base(new GestureMachineConfig(), new CustomContextManager(), rootElement)
        {
            this.GestureCancelled += new GestureCancelledEventHandler((sender, e) => 
            {
                // ExecuteInBackground(ctx, RestorePrimaryButtonClickEvent());
            });

            this.GestureTimeout += new GestureTimeoutEventHandler((sender, e) => 
            {
                // ExecuteInBackground(ctx, RestorePrimaryButtonDownEvent());
            });


        }

        internal Action RestorePrimaryButtonPressEvent()
        {
            return () =>
            {
                if (primaryEvent == Def.Constant.LeftButtonDown)
                {
                    InputSender.LeftDown();
                }
                else if (primaryEvent == Def.Constant.MiddleButtonDown)
                {
                    InputSender.MiddleDown();
                }
                else if (primaryEvent == Def.Constant.RightButtonDown)
                {
                    InputSender.RightDown();
                }
                else if (primaryEvent == Def.Constant.X1ButtonDown)
                {
                    InputSender.X1Down();
                }
                else if (primaryEvent == Def.Constant.X2ButtonDown)
                {
                    InputSender.X2Down();
                }
            };
        }

        internal Action RestorePrimaryButtonClickEvent()
        {
            return () =>
            {
                if (primaryEvent == Def.Constant.LeftButtonDown)
                {
                    InputSender.LeftClick();
                }
                else if (primaryEvent == Def.Constant.MiddleButtonDown)
                {
                    InputSender.MiddleClick();
                }
                else if (primaryEvent == Def.Constant.RightButtonDown)
                {
                    InputSender.RightClick();
                }
                else if (primaryEvent == Def.Constant.X1ButtonDown)
                {
                    InputSender.X1Click();
                }
                else if (primaryEvent == Def.Constant.X2ButtonDown)
                {
                    InputSender.X2Click();
                }
            };
        }
    }
}
