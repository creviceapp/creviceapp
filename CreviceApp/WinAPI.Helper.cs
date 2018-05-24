using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace Crevice.WinAPI.Helper
{
    using Crevice.Logging;

    public static class ExceptionThrower
    {
        public static void ThrowLastWin32Error()
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
    
    public class WinAPILogger
    {
        private readonly StringBuilder buffer = new StringBuilder();
        public WinAPILogger(string name)
        {
            Add($"Calling a Win32 native API: {name}");
        }

        public void Add(string str, bool omitNewline = false)
        {
            buffer.AppendFormat(str);
            if (!omitNewline)
            {
                buffer.AppendLine();
            }
        }
        
        public void Success()
        {
            Add("Result: Success", omitNewline: true);
            Verbose.Print(buffer.ToString());
        }

        public void Fail()
        {
            Add("Result: Fail", omitNewline: true);
            Verbose.Print(buffer.ToString());
        }

        public void FailWithErrorCode()
        {
            Add($"Result: Fail (ErrorCode={Marshal.GetLastWin32Error()})", omitNewline: true);
            Verbose.Print(buffer.ToString());
        }
    }
}