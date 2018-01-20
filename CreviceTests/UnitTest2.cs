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
            Assert.IsTrue(Events.Constants.Move is IPysicalEvent == false);
            Assert.IsTrue(Events.Constants.Move.EventId == 1000);

            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is PressEvent<LeftButtonSwitch>);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is ILogicalEvent);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is IPysicalEvent == false);
            Assert.AreEqual(Events.Constants.LeftButtonDownEvent.OppositeEvent, Events.Constants.LeftButtonUpEvent);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent.EventId == 2000);

            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is PressEvent<LeftButtonSwitch>);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is ILogicalEvent);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is IPysicalEvent == false);
            Assert.AreEqual(Events.Constants.LeftButtonDownEvent.OppositeEvent, Events.Constants.LeftButtonUpEvent);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent.EventId == 2000);

            Assert.IsTrue(Events.Constants.LeftButtonDownP0Event is PhysicalPressEvent<LeftButtonSwitch>);
            Assert.IsTrue(Events.Constants.LeftButtonDownP0Event is ILogicalEvent == false);
            Assert.IsTrue(Events.Constants.LeftButtonDownP0Event is IPysicalEvent);
            Assert.AreEqual(Events.Constants.LeftButtonDownP0Event.OppositeEvent, Events.Constants.LeftButtonUpP0Event);
            Assert.IsTrue(Events.Constants.LeftButtonDownP0Event.EventId == 2001);
            Assert.IsTrue(Events.Constants.LeftButtonDownP0Event.LogicallyEquals(Events.Constants.LeftButtonDownEvent));
        }

        [TestMethod]
        public void TestDSLSyntex()
        {
            var r = new Root<DefaultActionContext>();

            Assert.AreEqual(r.WhenElements.Count, 0);
            var w = r.When(ctx => { return true; });
            Assert.AreEqual(r.WhenElements.Count, 1);
            
            {
                Assert.AreEqual(w.OnFireElements.Count, 0);
                var f = w.On(Events.Constants.WheelDownEvent);
                Assert.AreEqual(w.OnFireElements.Count, 1);

                {
                    Assert.AreEqual(f.DoExecutors.Count, 0);
                    f.Do(ctx => { });
                    Assert.AreEqual(f.DoExecutors.Count, 1);
                }
            }

            {
                Assert.AreEqual(w.OnPressElements.Count, 0);
                var p = w.On(Events.Constants.LeftButtonDownEvent);
                Assert.AreEqual(w.OnPressElements.Count, 1);

                {
                    Assert.AreEqual(p.OnFireElements.Count, 0);
                    var f = p.On(Events.Constants.WheelDownEvent);
                    Assert.AreEqual(p.OnFireElements.Count, 1);

                    {
                        Assert.AreEqual(f.DoExecutors.Count, 0);
                        f.Do(ctx => { });
                        Assert.AreEqual(f.DoExecutors.Count, 1);
                    }
                }

                { 
                    Assert.AreEqual(p.DoBeforeExecutors.Count, 0);
                    p.DoBefore(ctx => { });
                    Assert.AreEqual(p.DoBeforeExecutors.Count, 1);

                    Assert.AreEqual(p.DoExecutors.Count, 0);
                    p.Do(ctx => { });
                    Assert.AreEqual(p.DoExecutors.Count, 1);

                    Assert.AreEqual(p.DoAfterExecutors.Count, 0);
                    p.DoAfter(ctx => { });
                    Assert.AreEqual(p.DoAfterExecutors.Count, 1);
                }

                {
                    Assert.AreEqual(p.OnStrokeElements.Count, 0);
                    var s = p.On(StrokeEvent.Direction.Up);
                    Assert.AreEqual(p.OnStrokeElements.Count, 1);

                    Assert.AreEqual(s.DoExecutors.Count, 0);
                    s.Do(ctx => { });
                    Assert.AreEqual(s.DoExecutors.Count, 1);
                }

                {
                    Assert.AreEqual(p.OnPressElements.Count, 0);
                    var pp = p.On(Events.Constants.LeftButtonDownEvent);
                    Assert.AreEqual(p.OnPressElements.Count, 1);

                    {
                        Assert.AreEqual(pp.OnFireElements.Count, 0);
                        var f = pp.On(Events.Constants.WheelDownEvent);
                        Assert.AreEqual(pp.OnFireElements.Count, 1);

                        {
                            Assert.AreEqual(f.DoExecutors.Count, 0);
                            f.Do(ctx => { });
                            Assert.AreEqual(f.DoExecutors.Count, 1);
                        }
                    }

                    {
                        Assert.AreEqual(pp.DoBeforeExecutors.Count, 0);
                        pp.DoBefore(ctx => { });
                        Assert.AreEqual(pp.DoBeforeExecutors.Count, 1);

                        Assert.AreEqual(pp.DoExecutors.Count, 0);
                        pp.Do(ctx => { });
                        Assert.AreEqual(pp.DoExecutors.Count, 1);

                        Assert.AreEqual(pp.DoAfterExecutors.Count, 0);
                        pp.DoAfter(ctx => { });
                        Assert.AreEqual(pp.DoAfterExecutors.Count, 1);
                    }

                    {
                        Assert.AreEqual(pp.OnStrokeElements.Count, 0);
                        var s = pp.On(StrokeEvent.Direction.Up);
                        Assert.AreEqual(pp.OnStrokeElements.Count, 1);

                        Assert.AreEqual(s.DoExecutors.Count, 0);
                        s.Do(ctx => { });
                        Assert.AreEqual(s.DoExecutors.Count, 1);
                    }
                }

                
            }
            



        }
    }
}
