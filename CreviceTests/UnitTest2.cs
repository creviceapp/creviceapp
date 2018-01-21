using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceTests
{
    using Crevice.Future;

    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestTypeSystem()
        {
            Assert.IsTrue(Events.Constants.Move is FireEvent<MoveSwich>);
            Assert.IsTrue(Events.Constants.Move is ILogicalEvent);
            Assert.IsTrue(Events.Constants.Move is IPhysicalEvent == false);
            Assert.IsTrue(Events.Constants.Move.EventId == 1000);

            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is PressEvent<LeftButtonSwitch>);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is ILogicalEvent);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is IPhysicalEvent == false);
            Assert.AreEqual(Events.Constants.LeftButtonDownEvent.OppositePressEvent, Events.Constants.LeftButtonUpEvent);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent.EventId == 2000);

            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is PressEvent<LeftButtonSwitch>);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is ILogicalEvent);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is IPhysicalEvent == false);
            Assert.AreEqual(Events.Constants.LeftButtonDownEvent.OppositePressEvent, Events.Constants.LeftButtonUpEvent);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent.EventId == 2000);

            Assert.IsTrue(Events.Constants.LeftButtonDownP0Event is PhysicalPressEvent<LeftButtonSwitch>);
            Assert.IsTrue(Events.Constants.LeftButtonDownP0Event is ILogicalEvent == false);
            Assert.IsTrue(Events.Constants.LeftButtonDownP0Event is IPhysicalEvent);
            Assert.AreEqual(Events.Constants.LeftButtonDownP0Event.OppositeEvent, Events.Constants.LeftButtonUpP0Event);
            Assert.IsTrue(Events.Constants.LeftButtonDownP0Event.EventId == 2001);
            Assert.IsTrue(Events.Constants.LeftButtonDownP0Event.LogicallyEquals(Events.Constants.LeftButtonDownEvent));
        }

        [TestMethod]
        public void TestDSLSyntex()
        {
            var r = new RootElement<DefaultActionContext>();

            Assert.AreEqual(r.WhenElements.Count, 0);
            var w = r.When(ctx => { return true; });
            Assert.AreEqual(r.WhenElements.Count, 1);
            
            {
                Assert.AreEqual(w.SingleThrowElements.Count, 0);
                var f = w.On(Events.Constants.WheelDownEvent);
                Assert.AreEqual(w.SingleThrowElements.Count, 1);

                {
                    Assert.AreEqual(f.DoExecutors.Count, 0);
                    f.Do(ctx => { });
                    Assert.AreEqual(f.DoExecutors.Count, 1);
                }
            }

            {
                Assert.AreEqual(w.DoubleThrowElements.Count, 0);
                var p = w.On(Events.Constants.LeftButtonDownEvent);
                Assert.AreEqual(w.DoubleThrowElements.Count, 1);

                {
                    Assert.AreEqual(p.SingleThrowElements.Count, 0);
                    var f = p.On(Events.Constants.WheelDownEvent);
                    Assert.AreEqual(p.SingleThrowElements.Count, 1);

                    {
                        Assert.AreEqual(f.DoExecutors.Count, 0);
                        f.Do(ctx => { });
                        Assert.AreEqual(f.DoExecutors.Count, 1);
                    }
                }

                { 
                    Assert.AreEqual(p.PressExecutors.Count, 0);
                    p.Press(ctx => { });
                    Assert.AreEqual(p.PressExecutors.Count, 1);

                    Assert.AreEqual(p.DoExecutors.Count, 0);
                    p.Do(ctx => { });
                    Assert.AreEqual(p.DoExecutors.Count, 1);

                    Assert.AreEqual(p.ReleaseExecutors.Count, 0);
                    p.Release(ctx => { });
                    Assert.AreEqual(p.ReleaseExecutors.Count, 1);
                }

                {
                    Assert.AreEqual(p.StrokeElements.Count, 0);
                    var s = p.On(StrokeEvent.Direction.Up);
                    Assert.AreEqual(p.StrokeElements.Count, 1);

                    Assert.AreEqual(s.DoExecutors.Count, 0);
                    s.Do(ctx => { });
                    Assert.AreEqual(s.DoExecutors.Count, 1);
                }

                {
                    Assert.AreEqual(p.DoubleThrowElements.Count, 0);
                    var pp = p.On(Events.Constants.LeftButtonDownEvent);
                    Assert.AreEqual(p.DoubleThrowElements.Count, 1);

                    {
                        Assert.AreEqual(pp.SingleThrowElements.Count, 0);
                        var f = pp.On(Events.Constants.WheelDownEvent);
                        Assert.AreEqual(pp.SingleThrowElements.Count, 1);

                        {
                            Assert.AreEqual(f.DoExecutors.Count, 0);
                            f.Do(ctx => { });
                            Assert.AreEqual(f.DoExecutors.Count, 1);
                        }
                    }

                    {
                        Assert.AreEqual(pp.PressExecutors.Count, 0);
                        pp.Press(ctx => { });
                        Assert.AreEqual(pp.PressExecutors.Count, 1);

                        Assert.AreEqual(pp.DoExecutors.Count, 0);
                        pp.Do(ctx => { });
                        Assert.AreEqual(pp.DoExecutors.Count, 1);

                        Assert.AreEqual(pp.ReleaseExecutors.Count, 0);
                        pp.Release(ctx => { });
                        Assert.AreEqual(pp.ReleaseExecutors.Count, 1);
                    }

                    {
                        Assert.AreEqual(pp.StrokeElements.Count, 0);
                        var s = pp.On(StrokeEvent.Direction.Up);
                        Assert.AreEqual(pp.StrokeElements.Count, 1);

                        Assert.AreEqual(s.DoExecutors.Count, 0);
                        s.Do(ctx => { });
                        Assert.AreEqual(s.DoExecutors.Count, 1);
                    }
                }

                
            }
        }

        [TestMethod]
        public void TestState()
        {
            
        }
    }
}
