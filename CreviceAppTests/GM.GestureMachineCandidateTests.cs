using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


namespace Crevice4Tests
{
    using System.Reflection;
    using Crevice.GestureMachine;

    [TestClass()]
    public class GestureMachineCandidateTests
    {
        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
        {
            TestHelpers.TestDirectoryMutex.WaitOne();
            Directory.CreateDirectory(TestHelpers.TemporaryDirectory);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Directory.Delete(TestHelpers.TemporaryDirectory, recursive: true);
            TestHelpers.TestDirectoryMutex.ReleaseMutex();
        }
        
        void Setup(DirectoryInfo src, string dst)
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

        [TestMethod()]
        public void UserDirectoryStructureStringTest()
        {
            var tempDir = TestHelpers.GetTestDirectory();
            var binaryDir = (new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent);
            var userScriptFile = Path.Combine(tempDir, "default.csx");
            var userScriptString = File.ReadAllText(Path.Combine(binaryDir.FullName, "Scripts", "DefaultUserScript.csx"), Encoding.UTF8);

            Setup(binaryDir, tempDir);

            var candidate = new GestureMachineCandidate(tempDir, "", Path.Combine(tempDir, "default.csx.cache"), false);

            var res0 = candidate.UserDirectoryStructureString;
            File.WriteAllText(userScriptFile, "");
            var res1 = candidate.UserDirectoryStructureString;
            Assert.AreEqual(res0 == res1, false);
        }
    }
}
