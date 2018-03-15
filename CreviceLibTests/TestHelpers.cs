using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using System.Threading;
    using System.IO;
    using System.Runtime.CompilerServices;
    
    public static class TestHelpers
    {
        public static readonly Mutex TestDirectoryMutex = new Mutex(true);

        private static string GetTemporaryDirectory()
        {
            return Path.Combine(Path.GetTempPath(), "Crevice4Test");
        }

        private static string GetTestDirectory([CallerMemberName] string memberName = "")
        {
            var randomString = Guid.NewGuid().ToString("N");
            var directory = Path.Combine(GetTemporaryDirectory(), randomString, memberName);
            Directory.CreateDirectory(directory);
            return directory;
        }
    }
}
