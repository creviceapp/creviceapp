using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceAppTests
{
    using System.IO;
    using System.Threading;
    using Crevice.Logging;

    [TestClass()]
    public class LoggingVerboseTests
    {
        private static TextWriter _consoleOut;
        private static TextWriter _consoleError;

        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
        {
            TestVar.ConsoleMutex.WaitOne();
            _consoleOut = Console.Out;
            _consoleError = Console.Error;
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            TestVar.ConsoleMutex.ReleaseMutex();
            Console.SetOut(_consoleOut);
            Console.SetError(_consoleError);
        }

        static readonly Mutex mutex = new Mutex(true);

        [TestInitialize()]
        public void TestInitialize()
        {
            mutex.WaitOne();
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            mutex.ReleaseMutex();
        }

        [TestMethod()]
        public void PrintTest()
        {
            Verbose.Enable();
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                var testMessage = "test message";
                Verbose.Print(testMessage);
                Assert.AreEqual(testMessage + "\r\n", sw.ToString());
            }
        }

        [TestMethod()]
        public void ErrorTest()
        {
            Verbose.Enable();
            using (var sw = new StringWriter())
            {
                Console.SetError(sw);
                var testMessage = "test error message";
                Verbose.Error(testMessage);
                Assert.AreEqual("[Error]" + testMessage + "\r\n", sw.ToString());
            }
        }
    }

    [TestClass()]
    public class LoggingVerboseElapsedTimePrinterTests
    {
        private static TextWriter _consoleOut;
        private static TextWriter _consoleError;

        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
        {
            TestVar.ConsoleMutex.WaitOne();
            _consoleOut = Console.Out;
            _consoleError = Console.Error;
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            TestVar.ConsoleMutex.ReleaseMutex();
            Console.SetOut(_consoleOut);
            Console.SetError(_consoleError);
        }

        [TestMethod()]
        public void ConstructorTest()
        {
            Verbose.Enable();
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                var testMessage = "test message";
                var elapsed = Verbose.PrintElapsed(testMessage);
                Assert.AreEqual(elapsed.Message, "[" + testMessage + "]"); 
                elapsed.Dispose();
            }
        }

        [TestMethod()]
        public void DisposeTest()
        {
            Verbose.Enable();
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                var testMessage = "test message";
                var elapsed = Verbose.PrintElapsed(testMessage);
                elapsed.Dispose();
                var expect =
                    "[" + testMessage + "] was started.\r\n" +
                    "[" + testMessage + "] was finished.";
                var result = sw.ToString();
                Assert.AreEqual(result.Substring(0, expect.Length), expect);
                elapsed.Dispose();
            }
        }
    }
}
