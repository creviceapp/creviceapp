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
        public readonly CachedWindowInfo Window;

        public UserActionExecutionContext(int x, int y)
        {
            this.Window = new CachedWindowInfo(x, y);
        }

        public class CachedWindowInfo : WinAPI.Window.ForegroundWindowInfo
        {
            private readonly int x;
            private readonly int y;
            public CachedWindowInfo(int x, int y) : base()
            {
                this.x = x;
                this.y = y;
            }

            public WinAPI.Window.OnCursorWindowInfo OnCursor
            {
                get
                {
                    return new WinAPI.Window.OnCursorWindowInfo(x, y);
                }
            }

            public CachedWindowInfo Now
            {
                get
                {
                    var pos = Cursor.Position;
                    return new CachedWindowInfo(pos.X, pos.Y);
                }
            }
        }
    }
}
