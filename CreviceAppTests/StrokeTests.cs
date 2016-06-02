using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.Tests
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
            var s = new Def.Stroke() { Def.Move.Up };
            Assert.AreEqual(s.Count, 1);
            Assert.AreEqual(s[0], Def.Move.Up);

            s = new Def.Stroke() { Def.Move.Up, Def.Move.Down };
            Assert.AreEqual(s.Count, 2);
            Assert.AreEqual(s[0], Def.Move.Up);
            Assert.AreEqual(s[1], Def.Move.Down);
        }
        
        [TestMethod()]
        public void Stroke3Test()
        {
            var s = new Def.Stroke(new List<Def.Move>() { Def.Move.Up });
            Assert.AreEqual(s.Count, 1);
            Assert.AreEqual(s[0], Def.Move.Up);

            s = new Def.Stroke(new List<Def.Move>() { Def.Move.Up, Def.Move.Down });
            Assert.AreEqual(s.Count, 2);
            Assert.AreEqual(s[0], Def.Move.Up);
            Assert.AreEqual(s[1], Def.Move.Down);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            Assert.AreNotEqual(new Def.Stroke(), null);

            Assert.AreEqual(new Def.Stroke(), new Def.Stroke());

            Assert.AreEqual(new Def.Stroke() { Def.Move.Up }, new Def.Stroke() { Def.Move.Up });
            Assert.AreEqual(new Def.Stroke() { Def.Move.Down }, new Def.Stroke() { Def.Move.Down });
            Assert.AreEqual(new Def.Stroke() { Def.Move.Left }, new Def.Stroke() { Def.Move.Left });
            Assert.AreEqual(new Def.Stroke() { Def.Move.Right }, new Def.Stroke() { Def.Move.Right });

            Assert.AreEqual(new Def.Stroke() { Def.Move.Up, Def.Move.Up }, new Def.Stroke() { Def.Move.Up, Def.Move.Up });
            Assert.AreEqual(new Def.Stroke() { Def.Move.Up, Def.Move.Down }, new Def.Stroke() { Def.Move.Up, Def.Move.Down });
            Assert.AreEqual(new Def.Stroke() { Def.Move.Up, Def.Move.Left }, new Def.Stroke() { Def.Move.Up, Def.Move.Left });
            Assert.AreEqual(new Def.Stroke() { Def.Move.Up, Def.Move.Right }, new Def.Stroke() { Def.Move.Up, Def.Move.Right });

            Assert.AreEqual(new Def.Stroke() { Def.Move.Down, Def.Move.Up }, new Def.Stroke() { Def.Move.Down, Def.Move.Up });
            Assert.AreEqual(new Def.Stroke() { Def.Move.Down, Def.Move.Down }, new Def.Stroke() { Def.Move.Down, Def.Move.Down });
            Assert.AreEqual(new Def.Stroke() { Def.Move.Down, Def.Move.Left }, new Def.Stroke() { Def.Move.Down, Def.Move.Left });
            Assert.AreEqual(new Def.Stroke() { Def.Move.Down, Def.Move.Right }, new Def.Stroke() { Def.Move.Down, Def.Move.Right });

            Assert.AreEqual(new Def.Stroke() { Def.Move.Left, Def.Move.Up }, new Def.Stroke() { Def.Move.Left, Def.Move.Up });
            Assert.AreEqual(new Def.Stroke() { Def.Move.Left, Def.Move.Down }, new Def.Stroke() { Def.Move.Left, Def.Move.Down });
            Assert.AreEqual(new Def.Stroke() { Def.Move.Left, Def.Move.Left }, new Def.Stroke() { Def.Move.Left, Def.Move.Left });
            Assert.AreEqual(new Def.Stroke() { Def.Move.Left, Def.Move.Right }, new Def.Stroke() { Def.Move.Left, Def.Move.Right });

            Assert.AreEqual(new Def.Stroke() { Def.Move.Right, Def.Move.Up }, new Def.Stroke() { Def.Move.Right, Def.Move.Up });
            Assert.AreEqual(new Def.Stroke() { Def.Move.Right, Def.Move.Down }, new Def.Stroke() { Def.Move.Right, Def.Move.Down });
            Assert.AreEqual(new Def.Stroke() { Def.Move.Right, Def.Move.Left }, new Def.Stroke() { Def.Move.Right, Def.Move.Left });
            Assert.AreEqual(new Def.Stroke() { Def.Move.Right, Def.Move.Right }, new Def.Stroke() { Def.Move.Right, Def.Move.Right });
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
            s.Add(Def.Move.Up);
            Assert.AreEqual(s.GetHashCode(), 0);
            s.Clear();
            s.Add(Def.Move.Down);
            Assert.AreEqual(s.GetHashCode(), 1);
            s.Clear();
            s.Add(Def.Move.Left);
            Assert.AreEqual(s.GetHashCode(), 2);
            s.Clear();
            s.Add(Def.Move.Right);
            Assert.AreEqual(s.GetHashCode(), 3);

            s.Clear();
            s.Add(Def.Move.Up);
            s.Add(Def.Move.Up);
            Assert.AreEqual(s.GetHashCode(), 0);
            s.Clear();
            s.Add(Def.Move.Up);
            s.Add(Def.Move.Down);
            Assert.AreEqual(s.GetHashCode(), 1);
            s.Clear();
            s.Add(Def.Move.Up);
            s.Add(Def.Move.Left);
            Assert.AreEqual(s.GetHashCode(), 2);
            s.Clear();
            s.Add(Def.Move.Up);
            s.Add(Def.Move.Right);
            Assert.AreEqual(s.GetHashCode(), 3);

            s.Clear();
            s.Add(Def.Move.Down);
            s.Add(Def.Move.Up);
            Assert.AreEqual(s.GetHashCode(), 4);
            s.Clear();
            s.Add(Def.Move.Down);
            s.Add(Def.Move.Down);
            Assert.AreEqual(s.GetHashCode(), 5);
            s.Clear();
            s.Add(Def.Move.Down);
            s.Add(Def.Move.Left);
            Assert.AreEqual(s.GetHashCode(), 6);
            s.Clear();
            s.Add(Def.Move.Down);
            s.Add(Def.Move.Right);
            Assert.AreEqual(s.GetHashCode(), 7);
            
            s.Clear();
            s.Add(Def.Move.Left);
            s.Add(Def.Move.Up);
            Assert.AreEqual(s.GetHashCode(), 8);
            s.Clear();
            s.Add(Def.Move.Left);
            s.Add(Def.Move.Down);
            Assert.AreEqual(s.GetHashCode(), 9);
            s.Clear();
            s.Add(Def.Move.Left);
            s.Add(Def.Move.Left);
            Assert.AreEqual(s.GetHashCode(), 10);
            s.Clear();
            s.Add(Def.Move.Left);
            s.Add(Def.Move.Right);
            Assert.AreEqual(s.GetHashCode(), 11);

            s.Clear();
            s.Add(Def.Move.Right);
            s.Add(Def.Move.Up);
            Assert.AreEqual(s.GetHashCode(), 12);
            s.Clear();
            s.Add(Def.Move.Right);
            s.Add(Def.Move.Down);
            Assert.AreEqual(s.GetHashCode(), 13);
            s.Clear();
            s.Add(Def.Move.Right);
            s.Add(Def.Move.Left);
            Assert.AreEqual(s.GetHashCode(), 14);
            s.Clear();
            s.Add(Def.Move.Right);
            s.Add(Def.Move.Right);
            Assert.AreEqual(s.GetHashCode(), 15);
        }
    }
}