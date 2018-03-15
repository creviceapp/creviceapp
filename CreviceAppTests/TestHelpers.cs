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
        public static readonly Mutex MouseMutex = new Mutex(true);
        public static readonly Mutex KeyboardMutex = new Mutex(true);
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

        public static void SetupUserDirectory(DirectoryInfo src, string dst)
        {
            var userScriptFile = Path.Combine(dst, "default.csx");
            var userScriptString = File.ReadAllText(Path.Combine(src.FullName, "Scripts", "DefaultUserScript.csx"), Encoding.UTF8);

            Directory.CreateDirectory(Path.Combine(dst, "IDESupport"));

            foreach (var file in src.EnumerateFiles())
            {
                File.Copy(file.FullName, Path.Combine(dst, "IDESupport", file.Name));
            }

            foreach (var dir in src.EnumerateDirectories())
            {
                Directory.CreateDirectory(Path.Combine(dst, "IDESupport", dir.Name));
                foreach (var file in dir.EnumerateFiles())
                {
                    File.Copy(file.FullName, Path.Combine(dst, "IDESupport", dir.Name, file.Name));
                }
            }
        }

    }
}
