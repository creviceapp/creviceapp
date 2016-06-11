using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace CreviceApp.WinAPI
{
    public static class Helper
    {
        public static void ThrowLastWin32Error()
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
    
}