using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.Core.Tests
{
    [TestClass()]
    public class StrokeTests
    {
        [TestMethod()]
        public void Stroke0Test()
        {
            var s = new Def.Stroke();
            Assert.AreEqual(s.Count, 0);
        }

        [TestMethod()]
        public void Stroke1Test()
        {
            var s = new Def.Stroke(10);
            Assert.AreEqual(s.Count, 0);
            Assert.AreEqual(s.Capacity, 10);
        }
        
        [TestMethod()]
        public void Stroke2Test()
        {
            var s = new Def.Stroke() { Def.Direction.Up };
            Assert.AreEqual(s.Count, 1);
            Assert.AreEqual(s[0], Def.Direction.Up);

            s = new Def.Stroke() { Def.Direction.Up, Def.Direction.Down };
            Assert.AreEqual(s.Count, 2);
            Assert.AreEqual(s[0], Def.Direction.Up);
            Assert.AreEqual(s[1], Def.Direction.Down);
        }
        
        [TestMethod()]
        public void Stroke3Test()
        {
            var s = new Def.Stroke(new List<Def.Direction>() { Def.Direction.Up });
            Assert.AreEqual(s.Count, 1);
            Assert.AreEqual(s[0], Def.Direction.Up);

            s = new Def.Stroke(new List<Def.Direction>() { Def.Direction.Up, Def.Direction.Down });
            Assert.AreEqual(s.Count, 2);
            Assert.AreEqual(s[0], Def.Direction.Up);
            Assert.AreEqual(s[1], Def.Direction.Down);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Assert.AreNotEqual(new Def.Stroke(), null);

            Assert.AreEqual(new Def.Stroke(), new Def.Stroke());

            Assert.AreEqual(new Def.Stroke() { Def.Direction.Up }, new Def.Stroke() { Def.Direction.Up });
            Assert.AreEqual(new Def.Stroke() { Def.Direction.Down }, new Def.Stroke() { Def.Direction.Down });
            Assert.AreEqual(new Def.Stroke() { Def.Direction.Left }, new Def.Stroke() { Def.Direction.Left });
            Assert.AreEqual(new Def.Stroke() { Def.Direction.Right }, new Def.Stroke() { Def.Direction.Right });

            Assert.AreEqual(new Def.Stroke() { Def.Direction.Up, Def.Direction.Up }, new Def.Stroke() { Def.Direction.Up, Def.Direction.Up });
            Assert.AreEqual(new Def.Stroke() { Def.Direction.Up, Def.Direction.Down }, new Def.Stroke() { Def.Direction.Up, Def.Direction.Down });
            Assert.AreEqual(new Def.Stroke() { Def.Direction.Up, Def.Direction.Left }, new Def.Stroke() { Def.Direction.Up, Def.Direction.Left });
            Assert.AreEqual(new Def.Stroke() { Def.Direction.Up, Def.Direction.Right }, new Def.Stroke() { Def.Direction.Up, Def.Direction.Right });

            Assert.AreEqual(new Def.Stroke() { Def.Direction.Down, Def.Direction.Up }, new Def.Stroke() { Def.Direction.Down, Def.Direction.Up });
            Assert.AreEqual(new Def.Stroke() { Def.Direction.Down, Def.Direction.Down }, new Def.Stroke() { Def.Direction.Down, Def.Direction.Down });
            Assert.AreEqual(new Def.Stroke() { Def.Direction.Down, Def.Direction.Left }, new Def.Stroke() { Def.Direction.Down, Def.Direction.Left });
            Assert.AreEqual(new Def.Stroke() { Def.Direction.Down, Def.Direction.Right }, new Def.Stroke() { Def.Direction.Down, Def.Direction.Right });

            Assert.AreEqual(new Def.Stroke() { Def.Direction.Left, Def.Direction.Up }, new Def.Stroke() { Def.Direction.Left, Def.Direction.Up });
            Assert.AreEqual(new Def.Stroke() { Def.Direction.Left, Def.Direction.Down }, new Def.Stroke() { Def.Direction.Left, Def.Direction.Down });
            Assert.AreEqual(new Def.Stroke() { Def.Direction.Left, Def.Direction.Left }, new Def.Stroke() { Def.Direction.Left, Def.Direction.Left });
            Assert.AreEqual(new Def.Stroke() { Def.Direction.Left, Def.Direction.Right }, new Def.Stroke() { Def.Direction.Left, Def.Direction.Right });

            Assert.AreEqual(new Def.Stroke() { Def.Direction.Right, Def.Direction.Up }, new Def.Stroke() { Def.Direction.Right, Def.Direction.Up });
            Assert.AreEqual(new Def.Stroke() { Def.Direction.Right, Def.Direction.Down }, new Def.Stroke() { Def.Direction.Right, Def.Direction.Down });
            Assert.AreEqual(new Def.Stroke() { Def.Direction.Right, Def.Direction.Left }, new Def.Stroke() { Def.Direction.Right, Def.Direction.Left });
            Assert.AreEqual(new Def.Stroke() { Def.Direction.Right, Def.Direction.Right }, new Def.Stroke() { Def.Direction.Right, Def.Direction.Right });
        }

        [TestMethod()]
        public void EqualsTest1()
        {
            Assert.AreNotEqual(new Def.Stroke(), null);
            Assert.AreNotEqual(new Def.Stroke(), new Object());
        }

        [TestMethod()]
        public void GetHashCodeTest()
        {
            var s = new Def.Stroke();

            Assert.AreEqual(s.GetHashCode(), 0);
            s.Add(Def.Direction.Up);
            Assert.AreEqual(s.GetHashCode(), 0);
            s.Clear();
            s.Add(Def.Direction.Down);
            Assert.AreEqual(s.GetHashCode(), 1);
            s.Clear();
            s.Add(Def.Direction.Left);
            Assert.AreEqual(s.GetHashCode(), 2);
            s.Clear();
            s.Add(Def.Direction.Right);
            Assert.AreEqual(s.GetHashCode(), 3);

            s.Clear();
            s.Add(Def.Direction.Up);
            s.Add(Def.Direction.Up);
            Assert.AreEqual(s.GetHashCode(), 0);
            s.Clear();
            s.Add(Def.Direction.Up);
            s.Add(Def.Direction.Down);
            Assert.AreEqual(s.GetHashCode(), 1);
            s.Clear();
            s.Add(Def.Direction.Up);
            s.Add(Def.Direction.Left);
            Assert.AreEqual(s.GetHashCode(), 2);
            s.Clear();
            s.Add(Def.Direction.Up);
            s.Add(Def.Direction.Right);
            Assert.AreEqual(s.GetHashCode(), 3);

            s.Clear();
            s.Add(Def.Direction.Down);
            s.Add(Def.Direction.Up);
            Assert.AreEqual(s.GetHashCode(), 4);
            s.Clear();
            s.Add(Def.Direction.Down);
            s.Add(Def.Direction.Down);
            Assert.AreEqual(s.GetHashCode(), 5);
            s.Clear();
            s.Add(Def.Direction.Down);
            s.Add(Def.Direction.Left);
            Assert.AreEqual(s.GetHashCode(), 6);
            s.Clear();
            s.Add(Def.Direction.Down);
            s.Add(Def.Direction.Right);
            Assert.AreEqual(s.GetHashCode(), 7);
            
            s.Clear();
            s.Add(Def.Direction.Left);
            s.Add(Def.Direction.Up);
            Assert.AreEqual(s.GetHashCode(), 8);
            s.Clear();
            s.Add(Def.Direction.Left);
            s.Add(Def.Direction.Down);
            Assert.AreEqual(s.GetHashCode(), 9);
            s.Clear();
            s.Add(Def.Direction.Left);
            s.Add(Def.Direction.Left);
            Assert.AreEqual(s.GetHashCode(), 10);
            s.Clear();
            s.Add(Def.Direction.Left);
            s.Add(Def.Direction.Right);
            Assert.AreEqual(s.GetHashCode(), 11);

            s.Clear();
            s.Add(Def.Direction.Right);
            s.Add(Def.Direction.Up);
            Assert.AreEqual(s.GetHashCode(), 12);
            s.Clear();
            s.Add(Def.Direction.Right);
            s.Add(Def.Direction.Down);
            Assert.AreEqual(s.GetHashCode(), 13);
            s.Clear();
            s.Add(Def.Direction.Right);
            s.Add(Def.Direction.Left);
            Assert.AreEqual(s.GetHashCode(), 14);
            s.Clear();
            s.Add(Def.Direction.Right);
            s.Add(Def.Direction.Right);
            Assert.AreEqual(s.GetHashCode(), 15);
        }
    }
}