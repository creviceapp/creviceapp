using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crevice.Config
{
    public class GestureStrokeOverlayConfig
    {
        public bool Enabled { get; set; } = true;
        public Color NormalLineColor { get; set; } = Color.FromArgb(51, 153, 255);
        public Color NewLineColor { get; set; } = Color.FromArgb(255, 51, 153);
        public Color UndeterminedLineColor { get; set; } = Color.FromArgb(255, 141, 75);
        public float LineWidth { get; set; } = 4.0f;
    }
}
