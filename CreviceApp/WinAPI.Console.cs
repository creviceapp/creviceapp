using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;


namespace Crevice.WinAPI.Console
{
    public static class Console
    {
        /*
         * AttachConsole is a abit buggy with CSharp GUI applictions. This bug also 
         * affects this application. If there is need to fix this issue, add a prefix 
         * before executable file path like following:
         * 
         * > cmd /c CreviceApp.exe
         *
         * See: Console Output from a WinForms Application : C# 411 
         * http://www.csharp411.com/console-output-from-winforms-application/
         */
        [DllImport("kernel32.dll")]
        public static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        public const UInt32 ATTACH_PARENT_PROCESS = UInt32.MaxValue;
        
        public static bool AttachConsole()
        {
            return AttachConsole(ATTACH_PARENT_PROCESS);
        }
    }
}
