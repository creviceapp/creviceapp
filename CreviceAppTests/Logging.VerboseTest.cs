using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice4Tests
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
            TestHelpers.ConsoleMutex.WaitOne();
            _consoleOut = Console.Out;
            _consoleError = Console.Error;
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            TestHelpers.ConsoleMutex.ReleaseMutex();
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

        private void WaitForPrintCompletion()
        {
            while (Verbose.queue.Count > 0)
            {
                Thread.Sleep(1);
            }
            Thread.Sleep(100);
        }

        [TestMethod()]
        public void PrintTest()
        {
            Verbose.Enabled = true;
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                var testMessage = "test message";
                Verbose.Print(testMessage);
                var queue = Verbose.queue;
                WaitForPrintCompletion();
                Assert.AreEqual(testMessage + "\r\n", sw.ToString());
            }
            Verbose.Enabled = false;
        }

        [TestMethod()]
        public void ErrorTest()
        {
            Verbose.Enabled = true;
            using (var sw = new StringWriter())
            {
                Console.SetError(sw);
                var testMessage = "test error message";
                Verbose.Error(testMessage);
                WaitForPrintCompletion();
                Assert.AreEqual("[Error] " + testMessage + "\r\n", sw.ToString());
            }
            Verbose.Enabled = false;
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
            TestHelpers.ConsoleMutex.WaitOne();
            _consoleOut = Console.Out;
            _consoleError = Console.Error;
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            TestHelpers.ConsoleMutex.ReleaseMutex();
            Console.SetOut(_consoleOut);
            Console.SetError(_consoleError);
        }

        private void WaitForPrintCompletion()
        {
            while (Verbose.queue.Count > 0)
            {
                Thread.Sleep(1);
            }
            Thread.Sleep(100);
        }

        [TestMethod()]
        public void ConstructorTest()
        {
            Verbose.Enabled = true;
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                var testMessage = "test message";
                var elapsed = Verbose.PrintElapsed(testMessage);
                WaitForPrintCompletion();
                Assert.AreEqual(elapsed.Message, "[" + testMessage + "]"); 
                elapsed.Dispose();
                WaitForPrintCompletion();
            }
            Verbose.Enabled = false;
        }

        [TestMethod()]
        public void DisposeTest()
        {
            Verbose.Enabled = true;
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                var testMessage = "test message";
                var elapsed = Verbose.PrintElapsed(testMessage);
                elapsed.Dispose();
                WaitForPrintCompletion();
                var expect =
                    "[" + testMessage + "] was started.\r\n" +
                    "[" + testMessage + "] was finished.";
                var result = sw.ToString();
                Assert.AreEqual(result.Substring(0, expect.Length), expect);
            }
            Verbose.Enabled = false;
        }
    }
}
