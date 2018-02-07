using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceTests
{
    using Crevice.Config;

    [TestClass()]
    public class CLIOptionTests
    {
        [TestMethod()]
        public void ParseNoGuiTest()
        {
            {
                string[] args = { "-g" };
                var result = CLIOption.Parse(args);
                Assert.IsTrue(result.ParseSuccess);
                Assert.IsTrue(result.NoGUI);
            }
            {
                string[] args = { "--nogui" };
                var result = CLIOption.Parse(args);
                Assert.IsTrue(result.ParseSuccess);
                Assert.IsTrue(result.NoGUI);
            }
        }

        [TestMethod()]
        public void ParseNoCacheTest()
        {
            {
                string[] args = { "-n" };
                var result = CLIOption.Parse(args);
                Assert.IsTrue(result.ParseSuccess);
                Assert.IsTrue(result.NoCache);
            }
            {
                string[] args = { "--nocache" };
                var result = CLIOption.Parse(args);
                Assert.IsTrue(result.ParseSuccess);
                Assert.IsTrue(result.NoCache);
            }
        }
        
        [TestMethod()]
        public void ParsePriorityTest()
        {
            {
                string[] args = { "-p" };
                var result = CLIOption.Parse(args);
                Assert.IsFalse(result.ParseSuccess);
                Assert.AreEqual(result.ProcessPriority, System.Diagnostics.ProcessPriorityClass.High);
            }
            {
                string[] args = { "--priority" };
                var result = CLIOption.Parse(args);
                Assert.IsFalse(result.ParseSuccess);
                Assert.AreEqual(result.ProcessPriority, System.Diagnostics.ProcessPriorityClass.High);
            }
            {
                string[] args = { "--priority", "normal"};
                var result = CLIOption.Parse(args);
                Assert.IsTrue(result.ParseSuccess);
                Assert.AreEqual(result.ProcessPriority, System.Diagnostics.ProcessPriorityClass.Normal);
            }
        }

        [TestMethod()]
        public void ParseVerboseTest()
        {
            {
                string[] args = { "-V" };
                var result = CLIOption.Parse(args);
                Assert.IsTrue(result.ParseSuccess);
                Assert.IsTrue(result.Verbose);
            }
            {
                string[] args = { "--verbose" };
                var result = CLIOption.Parse(args);
                Assert.IsTrue(result.ParseSuccess);
                Assert.IsTrue(result.Verbose);
            }
        }

        [TestMethod()]
        public void ParseVersionTest()
        {
            {
                string[] args = { "-v" };
                var result = CLIOption.Parse(args);
                Assert.IsTrue(result.ParseSuccess);
                Assert.IsTrue(result.Version);
            }
            {
                string[] args = { "--version" };
                var result = CLIOption.Parse(args);
                Assert.IsTrue(result.ParseSuccess);
                Assert.IsTrue(result.Version);
            }
        }

        [TestMethod()]
        public void ParseScriptTest()
        {
            {
                string[] args = { "-s" };
                var result = CLIOption.Parse(args);
                Assert.IsFalse(result.ParseSuccess);
                Assert.AreEqual(result.ScriptFile, "default.csx");
            }
            {
                string[] args = { "--script" };
                var result = CLIOption.Parse(args);
                Assert.IsFalse(result.ParseSuccess);
                Assert.AreEqual(result.ScriptFile, "default.csx");
            }
            {
                string[] args = { "-s", "hoge.csx" };
                var result = CLIOption.Parse(args);
                Assert.IsTrue(result.ParseSuccess);
                Assert.AreEqual(result.ScriptFile, "hoge.csx");
            }
            {
                string[] args = { "--script", "hoge.csx" };
                var result = CLIOption.Parse(args);
                Assert.IsTrue(result.ParseSuccess);
                Assert.AreEqual(result.ScriptFile, "hoge.csx");
            }
        }

        [TestMethod()]
        public void ParseEmptyTest()
        {
            string[] args = { };
            var result = CLIOption.Parse(args);
            Assert.IsTrue(result.ParseSuccess);
            Assert.IsFalse(result.NoGUI);
            Assert.AreEqual(result.ProcessPriority, System.Diagnostics.ProcessPriorityClass.High);
            Assert.AreEqual(result.ScriptFile, "default.csx");
        }

        [TestMethod()]
        public void ShowHelpMessageTest()
        {
            {
                string[] args = { "-h" };
                var result = CLIOption.Parse(args);
                // Help option causes parse failure.
                Assert.IsFalse(result.ParseSuccess);
                Assert.AreEqual(result.HelpMessage,
                    "\r\n" +
                    "Usage:\r\n" +
                    "  CreviceApp.exe [--nogui] [--script path] [--help]\r\n" +
                    "\r\n" +
                    "  -g, --nogui       (Default: False) Disable GUI features. Set to true if you \r\n" +
                    "                    use Crevice as a CUI application.\r\n" +
                    "\r\n" +
                    "  -n, --nocache     (Default: False) Disable user script assembly caching. \r\n" +
                    "                    Strongly recommend this value to false because compiling \r\n" +
                    "                    task consumes CPU resources every startup of application if\r\n" +
                    "                    true.\r\n" +
                    "\r\n" +
                    "  -s, --script      (Default: default.csx) Path to user script file. Use this \r\n" +
                    "                    option if you need to change the default location of user \r\n" +
                    "                    script file. If given value is relative path, Crevice will \r\n" +
                    "                    resolve it to absolute path based on the default directory \r\n" +
                    "                    (%USERPROFILE%\\AppData\\Roaming\\Crevice\\CreviceApp).\r\n" +
                    "\r\n" +
                    "  -p, --priority    (Default: High) Process priority. Acceptable values are the\r\n" +
                    "                    following: AboveNormal, BelowNormal, High, Idle, Normal, \r\n" +
                    "                    RealTime.\r\n" +
                    "\r\n" +
                    "  -V, --verbose     (Default: False) Show details about running application.\r\n" +
                    "\r\n" +
                    "  -v, --version     (Default: False) Display product version.\r\n" +
                    "\r\n" +
                    "  --help            Display this help screen.\r\n" +
                    "\r\n");
            }
            {
                string[] args = { "--help" };
                var result = CLIOption.Parse(args);
                // Help option causes parse failure.
                Assert.IsFalse(result.ParseSuccess);
                Assert.AreEqual(result.HelpMessage,
                    "\r\n" +
                    "Usage:\r\n" +
                    "  CreviceApp.exe [--nogui] [--script path] [--help]\r\n" +
                    "\r\n" +
                    "  -g, --nogui       (Default: False) Disable GUI features. Set to true if you \r\n" +
                    "                    use Crevice as a CUI application.\r\n" +
                    "\r\n" +
                    "  -n, --nocache     (Default: False) Disable user script assembly caching. \r\n" +
                    "                    Strongly recommend this value to false because compiling \r\n" +
                    "                    task consumes CPU resources every startup of application if\r\n" +
                    "                    true.\r\n" +
                    "\r\n" +
                    "  -s, --script      (Default: default.csx) Path to user script file. Use this \r\n" +
                    "                    option if you need to change the default location of user \r\n" +
                    "                    script file. If given value is relative path, Crevice will \r\n" +
                    "                    resolve it to absolute path based on the default directory \r\n" +
                    "                    (%USERPROFILE%\\AppData\\Roaming\\Crevice\\CreviceApp).\r\n" +
                    "\r\n" +
                    "  -p, --priority    (Default: High) Process priority. Acceptable values are the\r\n" +
                    "                    following: AboveNormal, BelowNormal, High, Idle, Normal, \r\n" +
                    "                    RealTime.\r\n" +
                    "\r\n" +
                    "  -V, --verbose     (Default: False) Show details about running application.\r\n" +
                    "\r\n" +
                    "  -v, --version     (Default: False) Display product version.\r\n" +
                    "\r\n" +
                    "  --help            Display this help screen.\r\n" +
                    "\r\n");
            }
        }
    }
}
