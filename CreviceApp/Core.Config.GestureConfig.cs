using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.Config
{
    public class GestureConfig
    {
        public int InitialStrokeThreshold         { get; set; } = 10;
        public int StrokeDirectionChangeThreshold { get; set; } = 20;
        public int StrokeExtensionThreshold       { get; set; } = 10;
        public int WatchInterval                  { get; set; } = 10;
    }
}
