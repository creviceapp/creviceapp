using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp.Core
{
    using WinAPI.Window.Impl;

    public class UserActionExecutionContext
    {
        public readonly Point GestureStartPoint;
        public readonly ForegroundWindowInfo ForegroundWindow;
        public readonly PointedWindowInfo PointedWindow;

        public UserActionExecutionContext(Point point)
        {
            this.GestureStartPoint = point;
            this.ForegroundWindow = new ForegroundWindowInfo();
            this.PointedWindow = new PointedWindowInfo(point);
        }
    }
}
