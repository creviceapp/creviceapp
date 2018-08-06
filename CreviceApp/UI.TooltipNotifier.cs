using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crevice.UI
{
    public class TooltipNotifier : IDisposable
    {
        private const int Absolute = 0x0002;

        private readonly IWin32Window win;
        private readonly MethodInfo SetTool;
        private readonly MethodInfo SetTrackPosition;
        private readonly MethodInfo StartTimer;
        private readonly MethodInfo StopTimer;

        private readonly ToolTip tooltip = new ToolTip();

        public TooltipNotifier(IWin32Window win)
        {
            this.win = win;
            this.SetTool = tooltip.GetType().GetMethod("SetTool", BindingFlags.Instance | BindingFlags.NonPublic);
            this.SetTrackPosition = tooltip.GetType().GetMethod("SetTrackPosition", BindingFlags.Instance | BindingFlags.NonPublic);
            this.StartTimer = tooltip.GetType().GetMethod("StartTimer", BindingFlags.Instance | BindingFlags.NonPublic);
            this.StopTimer = tooltip.GetType().GetMethod("StopTimer", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public void Show(string text, Point point, int duration)
        {
            StopTimer.Invoke(tooltip, new object[] { win });
            SetTrackPosition.Invoke(tooltip, new object[] { point.X, point.Y });
            SetTool.Invoke(tooltip, new object[] { win, text, Absolute, point });
            StartTimer.Invoke(tooltip, new object[] { win, duration });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                tooltip.Dispose();
            }
        }

        ~TooltipNotifier() => Dispose(false);
    }
}
