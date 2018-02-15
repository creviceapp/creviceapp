using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice4Tests
{
    using System.Threading;
    using System.IO;
    using System.Runtime.CompilerServices;

    public static class TestHelpers
    {
        public static readonly Mutex ConsoleMutex = new Mutex(true);

        public static readonly Mutex TestDirectoryMutex = new Mutex(true);

        public static string TemporaryDirectory
            => Path.Combine(Path.GetTempPath(), "Crevice4Test");

        public static string GetTestDirectory([CallerMemberName] string memberName = "")
        {
            var randomString = Guid.NewGuid().ToString("N");
            var directory = Path.Combine(TemporaryDirectory, randomString, memberName);
            Directory.CreateDirectory(directory);
            return directory;
        }
    }
}
