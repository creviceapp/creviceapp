using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.GestureMachine
{
    using System.Drawing;
    using System.Threading;
    using Crevice.WinAPI.Window.Impl;

    public class ExecutionContext : Core.Context.ExecutionContext
    {
        public readonly Point GestureStartPosition;
        public readonly Point GestureEndPosition;
        public readonly ForegroundWindowInfo ForegroundWindow;
        public readonly PointedWindowInfo PointedWindow;

        public ExecutionContext(
            EvaluationContext evaluationContext, 
            Point gestureEndPosition)
        {
            GestureStartPosition = evaluationContext.GestureStartPosition;
            GestureEndPosition = gestureEndPosition;
            ForegroundWindow = evaluationContext.ForegroundWindow;
            PointedWindow = evaluationContext.PointedWindow;
        }
    }
}
