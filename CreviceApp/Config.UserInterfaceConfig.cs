using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crevice.Config
{
    public class UserInterfaceConfig
    {
        public Func<Point, Point> TooltipPositionBinding { get; set; } = (point) =>
         {
             var rect = Screen.FromPoint(point).WorkingArea;
             return new Point(rect.X + rect.Width - 10, rect.Y + rect.Height - 10);
         };

        public int TooltipTimeout { get; set; } = 3000;

        public int BalloonTimeout { get; set; } = 10000;
    }
}
