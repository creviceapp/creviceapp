using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp
{
    public partial class Form1 : Form
    {
        readonly WindowsApplication winApp;
        readonly LowLevelMouseHook hook;

        public Form1()
        {
            winApp = new WindowsApplication();
            hook = new LowLevelMouseHook(MouseProc);
            hook.SetHook();

            InitializeComponent();
        }

        public LowLevelMouseHook.Result MouseProc(LowLevelMouseHook.Event evnt, LowLevelMouseHook.MSLLHOOKSTRUCT data)
        {
            var app = winApp.GetOnCursor(data.pt.x, data.pt.y);
            Debug.Print("process path: {0}", app.path);
            Debug.Print("process name: {0}", app.name);

            switch (evnt)
            {
                case LowLevelMouseHook.Event.L_BUTTONDOWN:
                case LowLevelMouseHook.Event.L_BUTTONUP:
                case LowLevelMouseHook.Event.MOUSEMOVE:
                case LowLevelMouseHook.Event.R_BUTTONDOWN:
                case LowLevelMouseHook.Event.R_BUTTONUP:
                    Debug.Print("0x{0:X}: x={1}, y={2}", evnt, data.pt.x, data.pt.y);
                    break;
                case LowLevelMouseHook.Event.V_MOUSEWHEEL:
                case LowLevelMouseHook.Event.H_MOUSEWHEEL:
                    Debug.Print("0x{0:X}: delta={1}", evnt, data.mouseData.higher);
                    break;
                case LowLevelMouseHook.Event.X_BUTTONDOWN:
                case LowLevelMouseHook.Event.X_BUTTONUP:
                    Debug.Print("0x{0:X}: type={1}", evnt, data.mouseData.higher);
                    break;
                default:
                    Debug.Print("0x{0:X}", evnt);
                    break;
            }
            return LowLevelMouseHook.Result.Transfer;
        } 
    }
}
