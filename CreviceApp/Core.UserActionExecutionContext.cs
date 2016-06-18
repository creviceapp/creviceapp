using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp.Core
{
    public class UserActionExecutionContext
    {
        public readonly CachedWindowInfo Window;

        public UserActionExecutionContext(Point point)
        {
            this.Window = new CachedWindowInfo(point);
        }

        public class CachedWindowInfo : WinAPI.Window.ForegroundWindowInfo
        {
            private readonly WinAPI.Window.OnCursorWindowInfo onCursor;
            public CachedWindowInfo(Point point) : base()
            {
                this.onCursor = new WinAPI.Window.OnCursorWindowInfo(point);
            }

            public WinAPI.Window.OnCursorWindowInfo OnCursor
            {
                get
                {
                    return onCursor;
                }
            }

            public CachedWindowInfo Now()
            {
                return new CachedWindowInfo(Cursor.Position);
            }
        }
    }
}
