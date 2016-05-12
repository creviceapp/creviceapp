using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp
{
    public partial class Form1 : Form
    {
        LowLevelMouseHook hook;

        public Form1()
        {
            hook = new LowLevelMouseHook(MouseProc);
            hook.SetHook();

            InitializeComponent();
        }

        public LowLevelMouseHook.Result MouseProc(LowLevelMouseHook.Event ev, LowLevelMouseHook.MSLLHOOKSTRUCT data)
        {
            switch (ev)
            {
                case LowLevelMouseHook.Event.L_BUTTONDOWN:
                case LowLevelMouseHook.Event.L_BUTTONUP:
                case LowLevelMouseHook.Event.MOUSEMOVE:
                case LowLevelMouseHook.Event.R_BUTTONDOWN:
                case LowLevelMouseHook.Event.R_BUTTONUP:
                    Debug.Print("0x{0:X}: x={1}, y={2}", ev, data.pt.x, data.pt.y);
                    break;
                case LowLevelMouseHook.Event.V_MOUSEWHEEL:
                case LowLevelMouseHook.Event.H_MOUSEWHEEL:
                    Debug.Print("0x{0:X}: delta={1}", ev, data.mouseData.higher);
                    break;
                case LowLevelMouseHook.Event.X_BUTTONDOWN:
                case LowLevelMouseHook.Event.X_BUTTONUP:
                    Debug.Print("0x{0:X}: type={1}", ev, data.mouseData.higher);
                    break;
                default:
                    Debug.Print("0x{0:X}", ev);
                    break;
            }
            return LowLevelMouseHook.Result.Transfer;
        } 
    }
}
