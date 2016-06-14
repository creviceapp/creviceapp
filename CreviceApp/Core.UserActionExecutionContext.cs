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
            private readonly Point point;
            public CachedWindowInfo(Point point) : base()
            {
                this.point = point;
            }

            public WinAPI.Window.OnCursorWindowInfo OnCursor
            {
                get
                {
                    return new WinAPI.Window.OnCursorWindowInfo(point);
                }
            }

            public CachedWindowInfo Now
            {
                get
                {
                    var pos = Cursor.Position;
                    return new CachedWindowInfo(point);
                }
            }
        }
    }
}
