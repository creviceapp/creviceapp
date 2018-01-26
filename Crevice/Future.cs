using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.Future
{
    using System.Drawing;


    /**
     * Todo: キーボードと組み合わせる関係で、
     * SendInput関係に、recapture = false 的な機能が必要っぽい
     * 例えばあるマウスボタンをCtrlとして使うようなシーンで、Ctrlをジェスチャに組み込む場合、
     * アプリケーション側でReCaptureしないといけない
     * 
     * Todo: 現在のコンテキストを表示するようなビュー。コールバックだけあればユーザーサイドでやれる？
     * 
     * Todo: カーソルの軌跡的なビュー。これもジェスチャ状態だけ貰えればあとは定期的にポーリングすればいける
     *          あるいはストローク情報に開始終了位置を含むとか
     * 
     * Todo: 複数のプロファイルを読み込むような機能。これはAppで実装できるかな
     *          複数のアプリを実行するより、優先度の管理がしやすい
     *              DeclareProfile("ProfileName");
     *              SubProfile("ProfileName");
     *              Using(new Profile("ProfileName")) {}
     *              
     * Todo: ベンチマーク
     * 
     * Todo: On() と If() を区別するのは？ SingleThrowとStrokeはIfのほうがよいような…
     * 
     */


    // interface ISetupable


    // EvaluationContextはVoid -> EvaluationContextはVoidな任意のGeneratorを渡して生成できる？
    // ExecutionContextは同様にEvaluationContext -> ExecutionContextで？

    // todo
    public class EvaluationContext { }
    public class ExecutionContext { }

    public delegate bool EvaluateAction<in T>(T ctx);
    public delegate void ExecuteAction<in T>(T ctx);

    public class NaturalNumberCounter<T>
    {
        private readonly Dictionary<T, int> Dictionary = new Dictionary<T, int>();

        public int this[T key]
        {
            get
            {
                return Dictionary.TryGetValue(key, out int count) ? count : 0;
            }
            set
            {
                if (value < 0)
                {
                    throw new InvalidOperationException("Natural number >= 0");
                }
                Dictionary[key] = value;
            }
        }

        public void CountDown(T key)
        {
            Dictionary[key] = this[key] - 1;
        }

        public void CountUp(T key)
        {
            Dictionary[key] = this[key] + 1;
        }
    }

    public abstract class GestureMachine<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        private readonly object LockObject = new object();
        private readonly System.Timers.Timer GestureTimeoutTimer = new System.Timers.Timer();
        private readonly NaturalNumberCounter<IReleaseEvent> InvalidReleaseEvents = new NaturalNumberCounter<IReleaseEvent>();

        public int GestureTimeout { get; set; } = 1000; // ms

        public int StrokeStartThreshold { get; set; } = 10; // px
        public int StrokeDirectionChangeThreshold { get; set; } = 20; // px
        public int StrokeExtensionThreshold { get; set; } = 10; // px
        public int StrokeWatchInterval { get; set; } = 10; // ms

        public StrokeWatcher StrokeWatcher { get; private set; } = null;

        public readonly RootElement<TEvalContext, TExecContext> RootElement;

        public IState CurrentState { get; private set; }

        public virtual TEvalContext CreateEvaluateContext()
            => throw new NotImplementedException();

        public virtual TExecContext CreateExecutionContext(TEvalContext evaluationContext)
            => throw new NotImplementedException();

        public virtual TaskFactory StrokeWatcherTaskFactory => Task.Factory;
        public virtual TaskFactory LowPriorityTaskFactory => Task.Factory;

        public GestureMachine(RootElement<TEvalContext, TExecContext> rootElement)
        {
            RootElement = rootElement;
            CurrentState = new State0<TEvalContext, TExecContext>(this, rootElement);

            GestureTimeoutTimer.Elapsed += new System.Timers.ElapsedEventHandler(TryTimeout);
            GestureTimeoutTimer.Interval = GestureTimeout;
            GestureTimeoutTimer.AutoReset = false;
        }

        public bool Input(IPhysicalEvent evnt) => Input(evnt, null);

        public bool Input(IPhysicalEvent evnt, Point? point)
        {
            lock (LockObject)
            {
                if (evnt is IReleaseEvent releaseEvent && InvalidReleaseEvents[releaseEvent] > 0)
                {
                    InvalidReleaseEvents.CountDown(releaseEvent);
                    return true;
                }

                if (point.HasValue && CurrentState is StateN<TEvalContext, TExecContext>)
                {
                    StrokeWatcher.Queue(point.Value);
                }

                var res = CurrentState.Input(evnt);
                if (CurrentState != res.NextState)
                {
                    if (res.NextState is State0<TEvalContext, TExecContext> S0)
                    {
                        ReleaseStrokeWatcher();

                        GestureTimeoutTimer.Stop();
                    }
                    else if (res.NextState is StateN<TEvalContext, TExecContext> SN)
                    {
                        ResetStrokeWatcher();

                        GestureTimeoutTimer.Stop();
                        GestureTimeoutTimer.Interval = GestureTimeout;
                        GestureTimeoutTimer.Start();
                    }
                }
                CurrentState = res.NextState;
                return res.EventIsConsumed;
            }
        }

        private StrokeWatcher CreateStrokeWatcher()
            => new StrokeWatcher(
                StrokeWatcherTaskFactory,
                StrokeStartThreshold,
                StrokeDirectionChangeThreshold,
                StrokeExtensionThreshold,
                StrokeWatchInterval);

        private void ReleaseStrokeWatcher() => LazyRelease(StrokeWatcher);

        private void LazyRelease(StrokeWatcher strokeWatcher)
        {
            if (strokeWatcher != null)
            {
                LowPriorityTaskFactory.StartNew(() => {
                    strokeWatcher.Dispose();
                });
            }
        }

        private void ResetStrokeWatcher()
        {
            var strokeWatcher = StrokeWatcher;
            StrokeWatcher = CreateStrokeWatcher();
            LazyRelease(strokeWatcher);
        }

        private void TryTimeout(object sender, System.Timers.ElapsedEventArgs args)
        {
            lock (LockObject)
            {
                if (CurrentState is StateN<TEvalContext, TExecContext>)
                {
                    var state = CurrentState;
                    var _state = CurrentState.Timeout();
                    while (state != _state)
                    {
                        state = _state;
                        _state = state.Timeout();
                    }
                    if (CurrentState != state)
                    {
                        // OnGestureTimeout
                        CurrentState = state;
                    }
                }
            }
        }

        public void Reset()
        {
            lock (LockObject)
            {
                if (CurrentState is StateN<TEvalContext, TExecContext>)
                {
                    var state = CurrentState;
                    var _state = CurrentState.Reset();
                    while (state != _state)
                    {
                        state = _state;
                        _state = state.Reset();
                    }
                    CurrentState = state;
                }
                // OnReset
            }
        }

        public void IgnoreNext(IReleaseEvent releaseEvent)
        {
            InvalidReleaseEvents.CountUp(releaseEvent);
        }

        public void IgnoreNext(IEnumerable<IReleaseEvent> releaseEvents)
        {
            foreach (var releaseEvent in releaseEvents)
            {
                IgnoreNext(releaseEvent);
            }
        }

        public bool EvaluateWhenEvaluator(TEvalContext evalContext, WhenElement<TEvalContext, TExecContext> whenElement)
            => whenElement.WhenEvaluator(evalContext);

        public void ExecuteExcutor(TExecContext execContext, ExecuteAction<TExecContext> executeAction)
            => executeAction(execContext);

        public void ExecutePressExecutors(TExecContext execContext, IEnumerable<DoubleThrowElement<TExecContext>> doubleThrowElements)
        {
            foreach (var element in doubleThrowElements)
            {
                foreach (var executor in element.PressExecutors)
                {
                    ExecuteExcutor(execContext, executor);
                }
            }
        }

        public void ExecuteDoExecutors(TExecContext execContext, IEnumerable<DoubleThrowElement<TExecContext>> doubleThrowElements)
        {
            foreach (var element in doubleThrowElements)
            {
                foreach (var executor in element.DoExecutors)
                {
                    ExecuteExcutor(execContext, executor);
                }
            }
        }

        public void ExecuteDoExecutors(TExecContext execContext, IEnumerable<SingleThrowElement<TExecContext>> singleThrowElements)
        {
            foreach (var element in singleThrowElements)
            {
                foreach (var executor in element.DoExecutors)
                {
                    ExecuteExcutor(execContext, executor);
                }
            }
        }

        public void ExecuteDoExecutors(TExecContext execContext, IEnumerable<StrokeElement<TExecContext>> strokeElements)
        {
            foreach (var element in strokeElements)
            {
                foreach (var executor in element.DoExecutors)
                {
                    ExecuteExcutor(execContext, executor);
                }
            }
        }

        public void ExecuteReleaseExecutors(TExecContext execContext, IEnumerable<DoubleThrowElement<TExecContext>> doubleThrowElements)
        {
            foreach (var element in doubleThrowElements)
            {
                foreach (var executor in element.ReleaseExecutors)
                {
                    ExecuteExcutor(execContext, executor);
                }
            }
        }
    }

    public class Stroke
    {
        public readonly StrokeEvent.Direction Direction;
        internal readonly int strokeDirectionChangeThreshold;
        internal readonly int strokeExtensionThreshold;

        private readonly List<System.Drawing.Point> points = new List<System.Drawing.Point>();

        public Stroke(
            int strokeDirectionChangeThreshold,
            int strokeExtensionThreshold,
            StrokeEvent.Direction dir)
        {
            this.Direction = dir;
            this.strokeDirectionChangeThreshold = strokeDirectionChangeThreshold;
            this.strokeExtensionThreshold = strokeExtensionThreshold;
        }

        public Stroke(
            int strokeDirectionChangeThreshold,
            int strokeExtensionThreshold,
            List<System.Drawing.Point> input)
        {
            this.Direction = NextDirection(GetAngle(input.First(), input.Last()));
            this.strokeDirectionChangeThreshold = strokeDirectionChangeThreshold;
            this.strokeExtensionThreshold = strokeExtensionThreshold;
            Absorb(input);
        }

        public virtual Stroke Input(List<System.Drawing.Point> input)
        {
            var p0 = input.First();
            var p1 = input.Last();
            var dx = Math.Abs(p0.X - p1.X);
            var dy = Math.Abs(p0.Y - p1.Y);
            var angle = GetAngle(p0, p1);
            if (dx > strokeDirectionChangeThreshold || dy > strokeDirectionChangeThreshold)
            {
                var dir = NextDirection(angle);
                if (IsSameDirection(dir))
                {
                    Absorb(input);
                    return this;
                }
                var stroke = CreateNew(dir);
                stroke.Absorb(input);
                return stroke;
            }

            if (dx > strokeExtensionThreshold || dy > strokeExtensionThreshold)
            {
                if (IsExtensionable(angle))
                {
                    Absorb(input);
                }
            }
            return this;
        }

        public void Absorb(List<System.Drawing.Point> points)
        {
            this.points.AddRange(points);
            points.Clear();
        }

        private static StrokeEvent.Direction NextDirection(double angle)
        {
            if (-135 <= angle && angle < -45)
            {
                return StrokeEvent.Direction.Up;
            }
            else if (-45 <= angle && angle < 45)
            {
                return StrokeEvent.Direction.Right;
            }
            else if (45 <= angle && angle < 135)
            {
                return StrokeEvent.Direction.Down;
            }
            else // if (135 <= angle || angle < -135)
            {
                return StrokeEvent.Direction.Left;
            }
        }

        private bool IsSameDirection(StrokeEvent.Direction dir)
        {
            return dir == Direction;
        }

        private bool IsExtensionable(double angle)
        {
            return Direction == NextDirection(angle);
        }

        private Stroke CreateNew(StrokeEvent.Direction dir)
        {
            return new Stroke(strokeDirectionChangeThreshold, strokeExtensionThreshold, dir);
        }

        public static bool CanCreate(int initialStrokeThreshold, System.Drawing.Point p0, System.Drawing.Point p1)
        {
            var dx = Math.Abs(p0.X - p1.X);
            var dy = Math.Abs(p0.Y - p1.Y);
            if (dx > initialStrokeThreshold || dy > initialStrokeThreshold)
            {
                return true;
            }
            return false;
        }

        private static double GetAngle(System.Drawing.Point p0, System.Drawing.Point p1)
        {
            return Math.Atan2(p1.Y - p0.Y, p1.X - p0.X) * 180 / Math.PI;
        }
    }

    public abstract class PointProcessor
    {
        public readonly int WatchInterval;

        private int lastTickCount;

        public PointProcessor(int watchInterval)
        {
            this.WatchInterval = watchInterval;
            this.lastTickCount = 0;
        }

        private bool MustBeProcessed(int currentTickCount)
        {
            if (WatchInterval == 0)
            {
                return true;
            }
            if (currentTickCount < lastTickCount || lastTickCount + WatchInterval < currentTickCount)
            {
                lastTickCount = currentTickCount;
                return true;
            }
            return false;
        }

        public bool Process(System.Drawing.Point point)
        {
            if (!MustBeProcessed(currentTickCount: Environment.TickCount))
            {
                return false;
            }
            OnProcess(point);
            return true;
        }

        internal abstract void OnProcess(System.Drawing.Point point);
    }

    public abstract class QueuedPointProcessor : PointProcessor
    {
        internal readonly System.Collections.Concurrent.BlockingCollection<System.Drawing.Point> queue 
            = new System.Collections.Concurrent.BlockingCollection<System.Drawing.Point>();

        public QueuedPointProcessor(int watchInterval) : base(watchInterval) { }

        internal override void OnProcess(System.Drawing.Point point)
        {
            queue.Add(point);
        }
    }
    
    public class StrokeWatcher : QueuedPointProcessor, IDisposable
    {
        internal readonly TaskFactory taskFactory;
        internal readonly int initialStrokeThreshold;
        internal readonly int strokeDirectionChangeThreshold;
        internal readonly int strokeExtensionThreshold;

        internal readonly List<Stroke> strokes = new List<Stroke>();
        internal readonly Task task;

        public StrokeWatcher(
            TaskFactory taskFactory,
            int initialStrokeThreshold,
            int strokeDirectionChangeThreshold,
            int strokeExtensionThreshold,
            int watchInterval) : base(watchInterval)
        {
            this.taskFactory = taskFactory;
            this.initialStrokeThreshold = initialStrokeThreshold;
            this.strokeDirectionChangeThreshold = strokeDirectionChangeThreshold;
            this.strokeExtensionThreshold = strokeExtensionThreshold;
            this.task = Start();
        }

        public virtual void Queue(System.Drawing.Point point)
        {
            if (!IsDisposed)
            {
                Process(point);
            }
        }

        internal readonly List<System.Drawing.Point> buffer = new List<System.Drawing.Point>();

        private Task Start()
        {
            return taskFactory.StartNew(() =>
            {
                try
                {
                    foreach (var point in queue.GetConsumingEnumerable())
                    {
                        buffer.Add(point);
                        if (buffer.Count < 2)
                        {
                            continue;
                        }
                        if (strokes.Count == 0)
                        {
                            if (Stroke.CanCreate(initialStrokeThreshold, buffer.First(), buffer.Last()))
                            {
                                // OnStroke~
                                var stroke = new Stroke(strokeDirectionChangeThreshold, strokeExtensionThreshold, buffer);
                                Verbose.Print("Stroke[0]: {0}", Enum.GetName(typeof(StrokeEvent.Direction), stroke.Direction));
                                strokes.Add(stroke);
                            }
                        }
                        else
                        {
                            var stroke = strokes.Last();
                            var res = stroke.Input(buffer);
                            if (stroke != res)
                            {
                                // OnStroke~
                                Verbose.Print("Stroke[{0}]: {1}", strokes.Count, Enum.GetName(typeof(StrokeEvent.Direction), res.Direction));
                                strokes.Add(res);
                            }
                        }
                    }
                }
                catch (OperationCanceledException) { }
            });
        }

        public IReadOnlyList<StrokeEvent.Direction> GetStorkes()
            => strokes.Select(x => x.Direction).ToList();

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            IsDisposed = true;
            queue.CompleteAdding();
            Verbose.Print("StrokeWatcher(HashCode: 0x{0:X}) was released.", GetHashCode());
        }

        ~StrokeWatcher()
        {
            Dispose();
        }
    }



    public interface IState
    {
        (bool EventIsConsumed, IState NextState) Input(IPhysicalEvent evnt);
        IState Timeout();
        IState Reset();
    }

    public abstract class State<TEvalContext, TExecContext> : IState
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public virtual (bool EventIsConsumed, IState NextState) Input(IPhysicalEvent evnt)
        {
            return (EventIsConsumed: false, NextState: this);
        }

        public virtual IState Timeout()
        {
            return this;
        }

        public virtual IState Reset()
        {
            return this;
        }
    }
    
    public class State0<TEvalContext, TExecContext> : State<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public readonly GestureMachine<TEvalContext, TExecContext> Machine;
        public readonly RootElement<TEvalContext, TExecContext> RootElement;

        public State0(
            GestureMachine<TEvalContext, TExecContext> gestureMachine,
            RootElement<TEvalContext, TExecContext> rootElement)
        {
            Machine = gestureMachine;
            RootElement = rootElement; 
        }

        public override (bool EventIsConsumed, IState NextState) Input(IPhysicalEvent evnt)
        {
            if (evnt is IFireEvent fireEvent && 
                    (SingleThrowTriggers.Contains(fireEvent) || 
                     SingleThrowTriggers.Contains(fireEvent.LogicalNormalized)))
            {
                var evalContext = Machine.CreateEvaluateContext();
                var singleThrowElements = GetActiveSingleThrowElements(evalContext, fireEvent);
                if (singleThrowElements.Any())
                {
                    var execContext = Machine.CreateExecutionContext(evalContext);
                    Machine.ExecuteDoExecutors(execContext, singleThrowElements);
                    return (EventIsConsumed: true, NextState: this);
                }
            }
            else if (evnt is IPressEvent pressEvent && 
                        (DoubleThrowTriggers.Contains(pressEvent) ||
                         DoubleThrowTriggers.Contains(pressEvent.LogicalNormalized)))
            {
                var evalContext = Machine.CreateEvaluateContext();
                var doubleThrowElements = GetActiveDoubleThrowElements(evalContext, pressEvent);
                if (doubleThrowElements.Any())
                {
                    var execContext = Machine.CreateExecutionContext(evalContext);
                    Machine.ExecutePressExecutors(execContext, doubleThrowElements);
                    var nextState = new StateN<TEvalContext, TExecContext>(
                        Machine,
                        evalContext,
                        CreateHistory(pressEvent),
                        doubleThrowElements,
                        canCancel: true
                        );
                    return (EventIsConsumed: true, NextState: nextState);
                }
            }
            
            return base.Input(evnt);
        }

        public IReadOnlyList<(IReleaseEvent, IState)> CreateHistory(IPressEvent pressEvent)
            => new List<(IReleaseEvent, IState)>() { (pressEvent.Opposition, this) };
        
        // Filter
        public IReadOnlyList<DoubleThrowElement<TExecContext>> GetActiveDoubleThrowElements(TEvalContext ctx, IPressEvent triggerEvent)
            => (from w in RootElement.WhenElements
                where w.IsFull && Machine.EvaluateWhenEvaluator(ctx, w)
                select (from d in w.DoubleThrowElements
                        where d.IsFull && (d.Trigger == triggerEvent ||
                                           d.Trigger == triggerEvent.LogicalNormalized)
                        select d))
            .Aggregate(new List<DoubleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        public IReadOnlyList<SingleThrowElement<TExecContext>> GetActiveSingleThrowElements(TEvalContext ctx, IFireEvent triggerEvent)
            => (from w in RootElement.WhenElements
                where w.IsFull && Machine.EvaluateWhenEvaluator(ctx, w)
                select (from s in w.SingleThrowElements
                        where s.IsFull && (s.Trigger == triggerEvent ||
                                           s.Trigger == triggerEvent.LogicalNormalized)
                        select s))
                .Aggregate(new List<SingleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; } );

        public IReadOnlyCollection<IFireEvent> SingleThrowTriggers
            => (from w in RootElement.WhenElements
                where w.IsFull
                select (
                    from s in w.SingleThrowElements
                    where s.IsFull
                    select s.Trigger))
                .Aggregate(new HashSet<IFireEvent>(), (a, b) => { a.UnionWith(b); return a; });

        public IReadOnlyCollection<IPressEvent> DoubleThrowTriggers
            => (from w in RootElement.WhenElements
                where w.IsFull
                select (
                    from d in w.DoubleThrowElements
                    where d.IsFull
                    select d.Trigger))
                .Aggregate(new HashSet<IPressEvent>(), (a, b) => { a.UnionWith(b); return a; });
    }

    public class StateN<TEvalContext, TExecContext> : State<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public readonly GestureMachine<TEvalContext, TExecContext> Machine;
        public readonly TEvalContext EvaluationContext;
        public readonly IReadOnlyList<(IReleaseEvent, IState)> History;
        public readonly IReadOnlyList<DoubleThrowElement<TExecContext>> DoubleThrowElements;
        public readonly bool CanCancel;

        public StateN(
            GestureMachine<TEvalContext, TExecContext> machine,
            TEvalContext ctx,
            IReadOnlyList<(IReleaseEvent, IState)> history,
            IReadOnlyList<DoubleThrowElement<TExecContext>> doubleThrowElements,
            bool canCancel = true)
        {
            Machine = machine;
            EvaluationContext = ctx;
            History = history;
            DoubleThrowElements = doubleThrowElements;
            CanCancel = canCancel;
        }

        public override (bool EventIsConsumed, IState NextState) Input(IPhysicalEvent evnt)
        {
            if (evnt is IFireEvent fireEvent)
            {
                var singleThrowElements = GetSingleThrowElements(fireEvent);
                if (singleThrowElements.Any())
                {
                    var execContext = Machine.CreateExecutionContext(EvaluationContext);
                    Machine.ExecuteDoExecutors(execContext, singleThrowElements);
                    var notCancellableCopyState = new StateN<TEvalContext, TExecContext>(
                        Machine,
                        EvaluationContext,
                        History,
                        DoubleThrowElements,
                        canCancel: false);
                    return (EventIsConsumed: true, NextState: notCancellableCopyState);
                }
            }
            else if (evnt is IPressEvent pressEvent)
            {
                var doubleThrowElements = GetDoubleThrowElements(pressEvent);
                if (doubleThrowElements.Any())
                {
                    var execContext = Machine.CreateExecutionContext(EvaluationContext);
                    Machine.ExecutePressExecutors(execContext, doubleThrowElements);
                    var nextState = new StateN<TEvalContext, TExecContext>(
                        Machine,
                        EvaluationContext,
                        CreateHistory(History, pressEvent, this),
                        doubleThrowElements,
                        canCancel: CanCancel);
                    return (EventIsConsumed: true, NextState: nextState);
                }
            }
            else if (evnt is IReleaseEvent releaseEvent)
            {
                if (IsNormalEndTrigger(releaseEvent))
                {
                    var strokes = Machine.StrokeWatcher.GetStorkes();
                    if (strokes.Any())
                    {
                        var strokeElements = GetStrokeElements(strokes);
                        if (strokeElements.Any())
                        {
                            var execContext = Machine.CreateExecutionContext(EvaluationContext);
                            Machine.ExecuteDoExecutors(execContext, strokeElements);
                            Machine.ExecuteReleaseExecutors(execContext, DoubleThrowElements);
                        }
                    }
                    else if (HasDoExecutors || HasReleaseExecutors)
                    {
                        //normal end
                        var execContext = Machine.CreateExecutionContext(EvaluationContext);
                        Machine.ExecuteDoExecutors(execContext, DoubleThrowElements);
                        Machine.ExecuteReleaseExecutors(execContext, DoubleThrowElements);
                    }
                    else if (/* !HasDoExecutors && !HasReleaseExecutors && */ CanCancel)
                    {
                        // Machine.OnGestureCancel()

                        //何のインスタンスが来るかによって対応を変える必要がある
                        //例えばゲームパッドであれば何もする必要がない

                        // ボタンであればクリックの復元を
                    }
                    return (EventIsConsumed: true, NextState: LastState);
                }
                else if (AbnormalEndTriggers.Contains(releaseEvent))
                {
                    var (oldState, skippedReleaseEvents) = FindStateFromHistory(releaseEvent);
                    Machine.IgnoreNext(skippedReleaseEvents);
                    return (EventIsConsumed: true, NextState: oldState);
                }
            }

            return base.Input(evnt);
        }

        public override IState Timeout()
        {
            if (!HasDoExecutors && !HasReleaseExecutors && CanCancel)
            {
                //Machine.OnGestureTimeout()
                return LastState;
            }
            return this;
        }

        public override IState Reset()
        {
            // Machine.OnGestureReset()
            Machine.IgnoreNext(NormalEndTrigger);
            // 再帰的に行う必要がある。
            // Machineのイベントを扱わないといけないので、Machine側で舐めて処理する？
            if (HasReleaseExecutors)
            {
                var execContext = Machine.CreateExecutionContext(EvaluationContext);
                Machine.ExecuteReleaseExecutors(execContext, DoubleThrowElements);
            }
            return LastState;
        }
        
        public IReleaseEvent NormalEndTrigger => History.Last().Item1;

        public IState LastState => History.Last().Item2;

        public bool HasPressExecutors => DoubleThrowElements.Any(d => d.PressExecutors.Any());

        public bool HasDoExecutors => DoubleThrowElements.Any(d => d.DoExecutors.Any());

        public bool HasReleaseExecutors => DoubleThrowElements.Any(d => d.ReleaseExecutors.Any());

        public IReadOnlyList<(IReleaseEvent, IState)> CreateHistory(
            IReadOnlyList<(IReleaseEvent, IState)> history, 
            IPressEvent pressEvent,
            IState state)
        {
            var newHistory = history.ToList();
            newHistory.Add((pressEvent.Opposition, state));
            return newHistory;
        }
        
        public bool IsNormalEndTrigger(IReleaseEvent releaseEvent)
            => releaseEvent == NormalEndTrigger;

        public IReadOnlyCollection<IReleaseEvent> AbnormalEndTriggers
            => new HashSet<IReleaseEvent>(from h in History.Reverse().Skip(1) select h.Item1);

        public (IState, IReadOnlyList<IReleaseEvent>) FindStateFromHistory(IReleaseEvent releaseEvent)
        {
            var nextHistory = History.TakeWhile(t => t.Item1 != releaseEvent);
            var foundState = History[nextHistory.Count()].Item2;
            var skippedReleaseEvents = History.Skip(nextHistory.Count()).Select(t => t.Item1).ToList();
            return (foundState, skippedReleaseEvents);
        }

        public IReadOnlyList<DoubleThrowElement<TExecContext>> GetDoubleThrowElements(IPressEvent triggerEvent)
            => (from d in DoubleThrowElements
                where d.IsFull
                select (
                    from dd in d.DoubleThrowElements
                    where dd.IsFull && (dd.Trigger == triggerEvent ||
                                        dd.Trigger == triggerEvent.LogicalNormalized)
                    select dd))
                .Aggregate(new List<DoubleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        public IReadOnlyList<StrokeElement<TExecContext>> GetStrokeElements(IReadOnlyList<StrokeEvent.Direction> strokes)
            => (from d in DoubleThrowElements
                where d.IsFull
                select (
                    from s in d.StrokeElements
                    where s.IsFull && s.Strokes.SequenceEqual(strokes)
                    select s))
                .Aggregate(new List<StrokeElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        public IReadOnlyList<SingleThrowElement<TExecContext>> GetSingleThrowElements(IFireEvent triggerEvent)
            => (from d in DoubleThrowElements
                where d.IsFull
                select (
                    from s in d.SingleThrowElements
                    where s.IsFull && (s.Trigger == triggerEvent ||
                                       s.Trigger == triggerEvent.LogicalNormalized)
                    select s))
                .Aggregate(new List<SingleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });
    }

    public abstract class Element
    {
        public abstract bool IsFull { get; }
    }

    /* RootElement
     * 
     * .When() -> new WhenElement
     */
    public class RootElement<TEvalContext, TExecContext> : Element
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public override bool IsFull => WhenElements.Any(e => e.IsFull);
        
        private readonly List<WhenElement<TEvalContext, TExecContext>> whenElements = new List<WhenElement<TEvalContext, TExecContext>>();
        public IReadOnlyList<WhenElement<TEvalContext, TExecContext>> WhenElements => whenElements.ToList();

        public WhenElement<TEvalContext, TExecContext> When(EvaluateAction<TEvalContext> evaluator)
        {
            var elm = new WhenElement<TEvalContext, TExecContext>(evaluator);
            whenElements.Add(elm);
            return elm;
        }
    }

    /*
     *  Triggers | When | Element | Element | ... 
     * 
     *  State
     *      when_elements := when要素の集合
     *      active_when_elements := 現在のwhen要素の集合
     *      cursor := 現在のマシンの深さ (初期値: 0)
     *      triggers := 反応するべきトリガのセット
     *      
     *      Init()
     *      
     *      
     *      
     *      Input(input = 入力されたイベント)
     *      
     *          if input in triggers:
     *              active_when_elements = filter(when_elements, _.evaluator() )
     *          
     *              if input is FireEvent:
     *                  この深さの
     *                  for elm in active_when_elements[cursor+1].on_fire_elements.trigger == input:
     *                      elm.executor()
     *                  
     *              elif input is PressEvent:
     *                  次の深さのwhen_elementsでOnPressかOnStrokeに対応していれば、
     *                      フィルタリングして次の深さへ
     *
     *      
     * 
     */

    /* WhenElement
     * 
     * .On(FireEvent) -> new SingleThrowElement
     * 
     * .On(PressEvent) -> new DoubleThrowElement
     */
    public class WhenElement<TEvalContext, TExecContext> : Element
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public override bool IsFull 
            => WhenEvaluator != null &&
                (SingleThrowElements.Any(e => e.IsFull) ||
                 DoubleThrowElements.Any(e => e.IsFull));

        public readonly EvaluateAction<TEvalContext> WhenEvaluator;

        private readonly List<SingleThrowElement<TExecContext>> singleThrowElements = new List<SingleThrowElement<TExecContext>>();
        public IReadOnlyList<SingleThrowElement<TExecContext>> SingleThrowElements => singleThrowElements.ToList();
        
        private readonly List<DoubleThrowElement<TExecContext>> doubleThrowElements = new List<DoubleThrowElement<TExecContext>>();
        public IReadOnlyList<DoubleThrowElement<TExecContext>> DoubleThrowElements => doubleThrowElements.ToList();
        
        public WhenElement(EvaluateAction<TEvalContext> evaluator)
        {
            WhenEvaluator = evaluator;
        }

        public SingleThrowElement<TExecContext> On(IFireEvent triggerEvent)
        {
            var elm = new SingleThrowElement<TExecContext>(triggerEvent);
            singleThrowElements.Add(elm);
            return elm;
        }

        public DoubleThrowElement<TExecContext> On(IPressEvent triggerEvent)
        {
            var elm = new DoubleThrowElement<TExecContext>(triggerEvent);
            doubleThrowElements.Add(elm);
            return elm;
        }
    }

    /* SingleThrowElement
     * 
     * .Do() -> this
     */
    public class SingleThrowElement<T> : Element
        where T : ExecutionContext
    {
        public override bool IsFull => Trigger != null && DoExecutors.Any(e => e != null);

        public readonly IFireEvent Trigger;
        
        private readonly List<ExecuteAction<T>> doExecutors = new List<ExecuteAction<T>>();
        public IReadOnlyList<ExecuteAction<T>> DoExecutors => doExecutors.ToList();
        
        public SingleThrowElement(IFireEvent triggerEvent)
        {
            Trigger = triggerEvent;
        }

        public SingleThrowElement<T> Do(ExecuteAction<T> executor)
        {
            doExecutors.Add(executor);
            return this;
        }
    }

    /* 
     * .Press() -> this 
     * 
     * .Do() -> this 
     * 
     * .Release() -> this 
     * 
     * .On(FireEvent) -> new SingleThrowElement
     * 
     * .On(PressEvent) -> new DoubleThrowElement
     * 
     * .On(StrokeEvent) -> new StrokeEelement
     */
    public class DoubleThrowElement<T> : Element
        where T : ExecutionContext
    {
        public override bool IsFull 
            => Trigger != null && 
                (PressExecutors.Any(e => e != null) ||
                 DoExecutors.Any(e => e != null) ||
                 ReleaseExecutors.Any(e => e != null) ||
                 SingleThrowElements.Any(e => e.IsFull) ||
                 DoubleThrowElements.Any(e => e.IsFull) ||
                 StrokeElements.Any(e => e.IsFull));
        
        public readonly IPressEvent Trigger;

        private readonly List<SingleThrowElement<T>> singleThrowElements = new List<SingleThrowElement<T>>();
        public IReadOnlyList<SingleThrowElement<T>> SingleThrowElements => singleThrowElements.ToList(); 

        private readonly List<DoubleThrowElement<T>> doubleThrowElements = new List<DoubleThrowElement<T>>();
        public IReadOnlyList<DoubleThrowElement<T>> DoubleThrowElements => doubleThrowElements.ToList(); 

        private readonly List<StrokeElement<T>> strokeElements = new List<StrokeElement<T>>();
        public IReadOnlyList<StrokeElement<T>> StrokeElements => strokeElements.ToList();

        private readonly List<ExecuteAction<T>> pressExecutors = new List<ExecuteAction<T>>();
        public IReadOnlyList<ExecuteAction<T>> PressExecutors => pressExecutors.ToList();

        private readonly List<ExecuteAction<T>> doExecutors = new List<ExecuteAction<T>>();
        public IReadOnlyList<ExecuteAction<T>> DoExecutors => doExecutors.ToList();

        private readonly List<ExecuteAction<T>> releaseExecutors = new List<ExecuteAction<T>>();
        public IReadOnlyList<ExecuteAction<T>> ReleaseExecutors => releaseExecutors.ToList(); 

        public DoubleThrowElement(IPressEvent triggerEvent)
        {
            Trigger = triggerEvent;
        }

        public SingleThrowElement<T> On(IFireEvent triggerEvent)
        {
            var elm = new SingleThrowElement<T>(triggerEvent);
            singleThrowElements.Add(elm);
            return elm;
        }

        public DoubleThrowElement<T> On(IPressEvent triggerEvent)
        {
            var elm = new DoubleThrowElement<T>(triggerEvent);
            doubleThrowElements.Add(elm);
            return elm;
        }

        public StrokeElement<T> On(params StrokeEvent.Direction[] strokeDirections)
        {
            var elm = new StrokeElement<T>(strokeDirections);
            strokeElements.Add(elm);
            return elm;
        }

        public DoubleThrowElement<T> Press(ExecuteAction<T> executor)
        {
            pressExecutors.Add(executor);
            return this;
        }

        public DoubleThrowElement<T> Do(ExecuteAction<T> executor)
        {
            doExecutors.Add(executor);
            return this;
        }

        public DoubleThrowElement<T> Release(ExecuteAction<T> executor)
        {
            releaseExecutors.Add(executor);
            return this;
        }
    }

    /* 
     * .Do() -> this 
     */
    public class StrokeElement<T> : Element
        where T : ExecutionContext
    {
        public override bool IsFull => Strokes.Any() && DoExecutors.Any(e => e != null);

        public readonly IReadOnlyList<StrokeEvent.Direction> Strokes;

        private readonly List<ExecuteAction<T>> doExecutors = new List<ExecuteAction<T>>();
        public IReadOnlyList<ExecuteAction<T>> DoExecutors => doExecutors.ToList();

        public StrokeElement(params StrokeEvent.Direction[] strokes)
        {
            Strokes = strokes;
        }

        public StrokeElement<T> Do(ExecuteAction<T> executor)
        {
            doExecutors.Add(executor);
            return this;
        }
    }


}
