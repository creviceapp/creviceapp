using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.GestureMachine
{
    using System.Drawing;
    using Crevice.WinAPI.Window.Impl;

    public class EvaluationContext : Core.Context.EvaluationContext
    {
        public readonly Point GestureStartPosition;
        public readonly ForegroundWindowInfo ForegroundWindow;
        public readonly PointedWindowInfo PointedWindow;

        public EvaluationContext(Point gestureStartPosition)
        {
            GestureStartPosition = gestureStartPosition;
            ForegroundWindow = new ForegroundWindowInfo();
            PointedWindow = new PointedWindowInfo(gestureStartPosition);
        }
    }
}
