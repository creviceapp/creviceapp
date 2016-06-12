using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    
    public class CallLogger
    {
        private readonly StringBuilder buffer = new StringBuilder();
        public CallLogger(string name)
        {
            Add("Calling a native method: {0}", name);
        }

        public void Add(string str)
        {
            buffer.AppendFormat(str);
            buffer.AppendLine();
        }
        
        public void Add(string str, params object[] objects)
        {
            buffer.AppendFormat(str, objects);
            buffer.AppendLine();
        }

        public void Success()
        {
            Add("Success");
            Debug.Print(buffer.ToString());
        }

        public void Fail()
        {
            Add("Failed");
            Debug.Print(buffer.ToString());
        }

        public void FailWithErrorCode()
        {
            Add("Failed; ErrorCode: {0}", Marshal.GetLastWin32Error());
            Debug.Print(buffer.ToString());
        }
    }
}