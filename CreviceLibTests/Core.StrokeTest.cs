using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using System.Linq;
    using System.Drawing;
    using Crevice.Core.Events;
    using Crevice.Core.Stroke;

    using TestRootElement = Crevice.Core.DSL.RootElement<Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;
    
    [TestClass]
    public class StrokeTest
    {
        [TestMethod]
        public void StrokeResetTest()
        {
            var root = new TestRootElement();
            root.When((ctx) => { return true; })
                .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Do((ctx) => { });
            var callback = new TestCallbackManager(
                enableStrokeResetCallback: true,
                enableStrokeUpdatedCallback: true,
                enableStateChangedCallback: true);
            using (var gm = new TestGestureMachine(root, callback))
            {
                gm.Config.StrokeWatchInterval = 0;
                gm.Config.GestureTimeout = 0;

                Assert.AreEqual(callback.OnStrokeResetCDE.Wait(10000), true);
                callback.OnStrokeResetCDE.Reset();

                Assert.AreEqual(callback.OnStateChangedCDE.Wait(10000), true);
                callback.OnStateChangedCDE.Reset();

                gm.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);

                Assert.AreEqual(callback.OnStrokeResetCDE.Wait(10000), true);
                callback.OnStrokeResetCDE.Reset();

                Assert.AreEqual(callback.OnStateChangedCDE.Wait(10000), true);
                callback.OnStateChangedCDE.Reset();

                // move
                gm.Input(new NullEvent(), new Point(100, 100));
                gm.Input(new NullEvent(), new Point(100, 150));
                Assert.AreEqual(callback.OnStrokeUpdatedCDE.Wait(10000), true);
                callback.OnStrokeUpdatedCDE.Reset();

                // move
                gm.Input(new NullEvent(), new Point(100, 150));
                gm.Input(new NullEvent(), new Point(150, 150));
                Assert.AreEqual(callback.OnStrokeUpdatedCDE.Wait(10000), true);
                callback.OnStrokeUpdatedCDE.Reset();

                var strokes = gm.StrokeWatcher.GetStorkes();
                Assert.AreEqual(strokes.SequenceEqual(new List<StrokeDirection>() { StrokeDirection.Down, StrokeDirection.Right }), true);

                gm.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                Assert.AreEqual(callback.OnStateChangedCDE.Wait(10000), true);
                callback.OnStateChangedCDE.Reset();
            }
        }
    }
}
