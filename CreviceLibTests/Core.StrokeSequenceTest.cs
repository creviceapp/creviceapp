using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using Crevice.Core.Stroke;

    [TestClass]
    public class StrokeSequenceTest
    {
        [TestMethod()]
        public void Stroke0Test()
        {
            var s = new StrokeSequence();
            Assert.AreEqual(s.Count, 0);
        }

        [TestMethod()]
        public void Stroke1Test()
        {
            var s = new StrokeSequence(10);
            Assert.AreEqual(s.Count, 0);
            Assert.AreEqual(s.Capacity, 10);
        }

        [TestMethod()]
        public void Stroke2Test()
        {
            var s = new StrokeSequence() { StrokeDirection.Up };
            Assert.AreEqual(s.Count, 1);
            Assert.AreEqual(s[0], StrokeDirection.Up);

            s = new StrokeSequence() { StrokeDirection.Up, StrokeDirection.Down };
            Assert.AreEqual(s.Count, 2);
            Assert.AreEqual(s[0], StrokeDirection.Up);
            Assert.AreEqual(s[1], StrokeDirection.Down);
        }

        [TestMethod()]
        public void Stroke3Test()
        {
            var s = new StrokeSequence(new List<StrokeDirection>() { StrokeDirection.Up });
            Assert.AreEqual(s.Count, 1);
            Assert.AreEqual(s[0], StrokeDirection.Up);

            s = new StrokeSequence(new List<StrokeDirection>() { StrokeDirection.Up, StrokeDirection.Down });
            Assert.AreEqual(s.Count, 2);
            Assert.AreEqual(s[0], StrokeDirection.Up);
            Assert.AreEqual(s[1], StrokeDirection.Down);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Assert.AreNotEqual(new StrokeSequence(), null);

            Assert.AreEqual(new StrokeSequence(), new StrokeSequence());

            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Up }, new StrokeSequence() { StrokeDirection.Up });
            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Down }, new StrokeSequence() { StrokeDirection.Down });
            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Left }, new StrokeSequence() { StrokeDirection.Left });
            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Right }, new StrokeSequence() { StrokeDirection.Right });

            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Up, StrokeDirection.Up }, new StrokeSequence() { StrokeDirection.Up, StrokeDirection.Up });
            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Up, StrokeDirection.Down }, new StrokeSequence() { StrokeDirection.Up, StrokeDirection.Down });
            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Up, StrokeDirection.Left }, new StrokeSequence() { StrokeDirection.Up, StrokeDirection.Left });
            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Up, StrokeDirection.Right }, new StrokeSequence() { StrokeDirection.Up, StrokeDirection.Right });

            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Down, StrokeDirection.Up }, new StrokeSequence() { StrokeDirection.Down, StrokeDirection.Up });
            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Down, StrokeDirection.Down }, new StrokeSequence() { StrokeDirection.Down, StrokeDirection.Down });
            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Down, StrokeDirection.Left }, new StrokeSequence() { StrokeDirection.Down, StrokeDirection.Left });
            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Down, StrokeDirection.Right }, new StrokeSequence() { StrokeDirection.Down, StrokeDirection.Right });

            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Left, StrokeDirection.Up }, new StrokeSequence() { StrokeDirection.Left, StrokeDirection.Up });
            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Left, StrokeDirection.Down }, new StrokeSequence() { StrokeDirection.Left, StrokeDirection.Down });
            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Left, StrokeDirection.Left }, new StrokeSequence() { StrokeDirection.Left, StrokeDirection.Left });
            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Left, StrokeDirection.Right }, new StrokeSequence() { StrokeDirection.Left, StrokeDirection.Right });

            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Right, StrokeDirection.Up }, new StrokeSequence() { StrokeDirection.Right, StrokeDirection.Up });
            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Right, StrokeDirection.Down }, new StrokeSequence() { StrokeDirection.Right, StrokeDirection.Down });
            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Right, StrokeDirection.Left }, new StrokeSequence() { StrokeDirection.Right, StrokeDirection.Left });
            Assert.AreEqual(new StrokeSequence() { StrokeDirection.Right, StrokeDirection.Right }, new StrokeSequence() { StrokeDirection.Right, StrokeDirection.Right });
        }

        [TestMethod()]
        public void EqualsTest1()
        {
            Assert.AreNotEqual(new StrokeSequence(), null);
            Assert.AreNotEqual(new StrokeSequence(), new Object());
        }

        [TestMethod()]
        public void GetHashCodeTest()
        {
            var s = new StrokeSequence();

            Assert.AreEqual(s.GetHashCode(), 0);
            s.Add(StrokeDirection.Up);
            Assert.AreEqual(s.GetHashCode(), 0);
            s.Clear();
            s.Add(StrokeDirection.Down);
            Assert.AreEqual(s.GetHashCode(), 1);
            s.Clear();
            s.Add(StrokeDirection.Left);
            Assert.AreEqual(s.GetHashCode(), 2);
            s.Clear();
            s.Add(StrokeDirection.Right);
            Assert.AreEqual(s.GetHashCode(), 3);

            s.Clear();
            s.Add(StrokeDirection.Up);
            s.Add(StrokeDirection.Up);
            Assert.AreEqual(s.GetHashCode(), 0);
            s.Clear();
            s.Add(StrokeDirection.Up);
            s.Add(StrokeDirection.Down);
            Assert.AreEqual(s.GetHashCode(), 1);
            s.Clear();
            s.Add(StrokeDirection.Up);
            s.Add(StrokeDirection.Left);
            Assert.AreEqual(s.GetHashCode(), 2);
            s.Clear();
            s.Add(StrokeDirection.Up);
            s.Add(StrokeDirection.Right);
            Assert.AreEqual(s.GetHashCode(), 3);

            s.Clear();
            s.Add(StrokeDirection.Down);
            s.Add(StrokeDirection.Up);
            Assert.AreEqual(s.GetHashCode(), 4);
            s.Clear();
            s.Add(StrokeDirection.Down);
            s.Add(StrokeDirection.Down);
            Assert.AreEqual(s.GetHashCode(), 5);
            s.Clear();
            s.Add(StrokeDirection.Down);
            s.Add(StrokeDirection.Left);
            Assert.AreEqual(s.GetHashCode(), 6);
            s.Clear();
            s.Add(StrokeDirection.Down);
            s.Add(StrokeDirection.Right);
            Assert.AreEqual(s.GetHashCode(), 7);

            s.Clear();
            s.Add(StrokeDirection.Left);
            s.Add(StrokeDirection.Up);
            Assert.AreEqual(s.GetHashCode(), 8);
            s.Clear();
            s.Add(StrokeDirection.Left);
            s.Add(StrokeDirection.Down);
            Assert.AreEqual(s.GetHashCode(), 9);
            s.Clear();
            s.Add(StrokeDirection.Left);
            s.Add(StrokeDirection.Left);
            Assert.AreEqual(s.GetHashCode(), 10);
            s.Clear();
            s.Add(StrokeDirection.Left);
            s.Add(StrokeDirection.Right);
            Assert.AreEqual(s.GetHashCode(), 11);

            s.Clear();
            s.Add(StrokeDirection.Right);
            s.Add(StrokeDirection.Up);
            Assert.AreEqual(s.GetHashCode(), 12);
            s.Clear();
            s.Add(StrokeDirection.Right);
            s.Add(StrokeDirection.Down);
            Assert.AreEqual(s.GetHashCode(), 13);
            s.Clear();
            s.Add(StrokeDirection.Right);
            s.Add(StrokeDirection.Left);
            Assert.AreEqual(s.GetHashCode(), 14);
            s.Clear();
            s.Add(StrokeDirection.Right);
            s.Add(StrokeDirection.Right);
            Assert.AreEqual(s.GetHashCode(), 15);
        }
    }
}
