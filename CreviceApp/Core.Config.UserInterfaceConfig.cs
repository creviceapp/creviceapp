using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp.Core.Config
{
    public class UserInterfaceConfig
    {
        public Func<Point, Point> TooltipPositionBinding;
        public int TooltipTimeout = 3000;

        public int BaloonTimeout = 10000;
        
        public UserInterfaceConfig()
        {
            this.TooltipPositionBinding = (point) =>
            {
                var rect = Screen.FromPoint(point).WorkingArea;
                return new Point(rect.X + rect.Width - 10, rect.Y + rect.Height - 10);
            };
        }
    }
}
