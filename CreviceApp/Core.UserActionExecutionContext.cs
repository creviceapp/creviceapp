using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp.Core
{
    public class UserActionExecutionContext
    {
        public readonly WindowInfo Window;

        public UserActionExecutionContext(int x, int y)
        {
            this.Window = new WindowInfo(x, y);
        }
        
        public class WindowInfo
        {
            public readonly WinAPI.Window.ForegroundWindowInfo Foreground;
            public readonly WinAPI.Window.OnCursorWindowInfo OnCursor;

            public WindowInfo(int x, int y)
            {
                this.Foreground = new WinAPI.Window.ForegroundWindowInfo();
                this.OnCursor = new WinAPI.Window.OnCursorWindowInfo(x, y);
            }
        }

        public class CachedWindowInfo : WindowInfo
        {
            public WindowInfo Now
            {
                get
                {
                    var pos = Cursor.Position;
                    return new WindowInfo(pos.X, pos.Y);
                }
            }
            public CachedWindowInfo(int x, int y) : base(x, y) { }
        }
    }
}
