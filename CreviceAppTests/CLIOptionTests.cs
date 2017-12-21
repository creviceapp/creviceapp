using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp
{
    [TestClass()]
    public class CLIOptionTests
    {
        [TestMethod()]
        public void ParseNoGuiTest()
        {
            var cliOption = new CLIOption();
            string[] args = { "--nogui" };
            var result = CLIOption.Parse(args);
            Assert.IsTrue(result.parseSuccess);
            Assert.IsTrue(result.NoGUI);
        }

        [TestMethod()]
        public void ParseScriptTest()
        {
            var cliOption = new CLIOption();
            string[] args = { "--script", "hoge.csx" };
            var result = CLIOption.Parse(args);
            Assert.IsTrue(result.parseSuccess);
            Assert.AreEqual(result.Script, "hoge.csx");
        }

        [TestMethod()]
        public void ParseEmptyTest()
        {
            var cliOption = new CLIOption();
            string[] args = { };
            var result = CLIOption.Parse(args);
            Assert.IsTrue(result.parseSuccess);
            Assert.IsFalse(result.NoGUI);
            Assert.AreEqual(result.Script, "default.csx");
        }

        [TestMethod()]
        public void ShowHelpMessageTest()
        {
            var cliOption = new CLIOption();
            string[] args = { "--help" };
            var result = CLIOption.Parse(args);
            // Help option causes parse failure.
            Assert.IsFalse(result.parseSuccess);
            // Geneate correct string from output using following pattern and replace:
            // Pattern: (.*?\\r\\n)
            // Replace: "\1" + \r\n
            Assert.AreEqual(result.helpMessage,
                "\r\n" +
                "Usage:\r\n" +
                "  CreviceApp.exe [--nogui] [--script path] [--help]\r\n" +
                "\r\n" +
                "  --nogui     (Default: False) Whether disable GUI features or not. Set to true\r\n" +
                "              if you use CreviceApp as a CUI application.\r\n" +
                "\r\n" +
                "  --script    (Default: default.csx) The path to the user script file. Use this\r\n" +
                "              option if you need to change the default location of the user \r\n" +
                "              script. If given value is relative path, CreviceApp will resolve \r\n" +
                "              it to absolute path based on the default folder \r\n" +
                "              (%USERPROFILE%\\AppData\\Roaming\\Crevice\\CreviceApp).\r\n" +
                "\r\n" +
                "  --help      Display this help screen.\r\n" +
                "\r\n");
        }
    }
}
