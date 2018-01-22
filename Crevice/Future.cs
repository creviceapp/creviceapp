using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.Future
{
    using System.Drawing;

    // interface ISetupable

    public abstract class ActionContext // todo ISetupable<T> 
    {
        public abstract void Setup(object gestureStartContext, Point currentPoint);
    }

    public class DefaultActionContext : ActionContext
    {
        public Point StartPoint { get; private set; }

        // このシグネチャのコンストラクタを持つ、という制約よりはわかりやすいかな

        // これもイベントにすれば？
        // 無意味っぽい

        // ここにプラットフォームごとに拡張したInputで与えられる、Pointを統合したクラスが
        // 来るなら拡張がさらに容易かな
        public override void Setup(object gestureStartContext, Point currentPoint)
        {
            if (gestureStartContext == null)
            {
                StartPoint = currentPoint;
            }
        }

        //
        // Setup(Point currentPoint)
        // Setup(object, Point currentPoint)
        //
    }


    public delegate bool EvaluateAction<in T>(T ctx);
    public delegate void ExecuteAction<in T>(T ctx);

    public class Result
    {
        public class EventResult
        {
            public readonly bool IsConsumed;
            public EventResult(bool consumed)
            {
                IsConsumed = consumed;
            }
        }

        public EventResult Event;
        public State NextState { get; private set; }

        private Result(bool consumed, State nextState)
        {
            this.Event = new EventResult(consumed);
            this.NextState = nextState;
        }

        public static Result EventIsConsumed(State nextState)
        {
            return new Result(true, nextState);
        }

        public static Result EventIsRemained(State nextState)
        {
            return new Result(false, nextState);
        }
    }

    public class GestureMachine
    {
        public IEnumerable<StrokeEvent.Direction> GetStroke()
        {
            return new List<StrokeEvent.Direction>();
        }

        private readonly HashSet<IReleaseEvent> invalidReleaseEvents = new HashSet<IReleaseEvent>();

        public bool IsIgnored(IReleaseEvent releaseEvent)
        {
            if (invalidReleaseEvents.Contains(releaseEvent))
            {
                invalidReleaseEvents.Remove(releaseEvent);
                return true;
            }
            return false;
        }
        


        public void IgnoreNext(IReleaseEvent releaseEvent)
        {
            if (!invalidReleaseEvents.Add(releaseEvent))
            {
                throw new InvalidOperationException();
            }
        }

        public void IgnoreNext(IEnumerable<IReleaseEvent> releaseEvents)
        {
            foreach (var releaseEvent in releaseEvents)
            {
                IgnoreNext(releaseEvent);
            }
        }
    }


    public abstract class State
    {
        // global variable
        public GestureMachine Machine;

        public virtual Result Input(Event evnt, Point point) // ここのPointは抽象化するとよさげ
        {
            return Result.EventIsRemained(nextState: this);
        }
    }
    
    public class State0<T> : State
        where T : ActionContext, new()
    {
        public readonly RootElement<T> RootElement;

        public State0(
            GestureMachine gestureMachine,
            RootElement<T> rootElement)
        {
            Machine = gestureMachine;
            RootElement = rootElement; 
        }

        public override Result Input(Event evnt, Point point)
        {
            if (evnt is IFireEvent fireEvent && SingleThrowTriggers.Contains(fireEvent))
            {
                var ctx = CreateActionContext();
                foreach(var st in GetActiveSingleThrowElements(ctx, fireEvent))
                {
                    foreach (var doExecutor in st.DoExecutors)
                    {
                        doExecutor(ctx);
                    }
                }
                return Result.EventIsConsumed(nextState: this);

            }
            else if (evnt is IPressEvent pressEvent && DoubleThrowTriggers.Contains(pressEvent))
            {
                var ctx = CreateActionContext();
                var doubleThrowElements = GetActiveDoubleThrowElements(ctx, pressEvent);
                if (doubleThrowElements.Count() > 0)
                {
                    foreach (var dt in doubleThrowElements)
                    {
                        foreach (var pressExecutor in dt.PressExecutors)
                        {
                            pressExecutor(ctx);
                        }
                    }
                    var state = new StateN<T>(
                        ctx,
                        CreateHistory(pressEvent, this),
                        doubleThrowElements,
                        allowCancel: true
                        );

                    return Result.EventIsConsumed(nextState: state);
                }
            }
            else if (evnt is IReleaseEvent releaseEvent)
            {
                if (Machine.IsIgnored(releaseEvent))
                {
                    return Result.EventIsConsumed(nextState: this);
                }
            }
            
            return base.Input(evnt, point);
        }

        public T CreateActionContext()
        {
            // Todo: setup
            return new T();
        }

        public IReadOnlyList<Tuple<IReleaseEvent, State>> CreateHistory(IPressEvent pressEvent, State state)
        {
            return new List<Tuple<IReleaseEvent, State>>() {
                Tuple.Create(pressEvent.Opposition, state)
            };
        }

        public IReadOnlyList<DoubleThrowElement<T>> GetActiveDoubleThrowElements(T ctx, IPressEvent triggerEvent)
        {
            return (
                from w in RootElement.WhenElements
                where w.IsFull &&
                      w.WhenEvaluator(ctx)
                select (
                    from d in w.DoubleThrowElements
                    where d.IsFull && d.Trigger == triggerEvent
                    select d))
                .Aggregate(new List<DoubleThrowElement<T>>(), (a, b) => { a.AddRange(b); return a; });
        }

        public IReadOnlyList<SingleThrowElement<T>> GetActiveSingleThrowElements(T ctx, IFireEvent triggerEvent)
        {
            return (
                from w in RootElement.WhenElements
                where w.IsFull && 
                      w.WhenEvaluator(ctx)
                select (
                    from s in w.SingleThrowElements
                    where s.IsFull && s.Trigger == triggerEvent
                    select s))
                .Aggregate(new List<SingleThrowElement<T>>(), (a, b) => { a.AddRange(b); return a; } );
        }

        public IReadOnlyCollection<IFireEvent> SingleThrowTriggers
        {
            get
            {
                return (
                    from w in RootElement.WhenElements
                    where w.IsFull
                    select (
                        from s in w.SingleThrowElements
                        where s.IsFull
                        select s.Trigger))
                    .Aggregate(new HashSet<IFireEvent>(), (a, b) => { a.UnionWith(b); return a; });
            }
        }

        public IReadOnlyCollection<IPressEvent> DoubleThrowTriggers
        {
            get
            {
                return (
                    from w in RootElement.WhenElements
                    where w.IsFull
                    select (
                        from d in w.DoubleThrowElements
                        where d.IsFull
                        select d.Trigger))
                    .Aggregate(new HashSet<IPressEvent>(), (a, b) => { a.UnionWith(b); return a; });
            }
        }
    }
    
    public class StateN<T> : State
        where T : ActionContext
    {
        public readonly T Ctx;
        public readonly IReadOnlyList<Tuple<IReleaseEvent, State>> History;
        public readonly IReadOnlyList<DoubleThrowElement<T>> DoubleThrowElements;
        public readonly bool AllowCancel;

        public StateN(
            T ctx,
            IReadOnlyList<Tuple<IReleaseEvent, State>> history,
            IReadOnlyList<DoubleThrowElement<T>> doubleThrowElements,
            bool allowCancel = true
            )
        {
            Ctx = ctx;
            History = history;
            DoubleThrowElements = doubleThrowElements;
            AllowCancel = allowCancel;
        }

        public override Result Input(Event evnt, Point point)
        {
            // Todo: storkewatcher

            if (evnt is IFireEvent fireEvent)
            {
                if (SingleThrowTriggers.Contains(fireEvent))
                {
                    foreach (var st in GetSingleThrowElements(fireEvent))
                    {
                        foreach (var doExecutor in st.DoExecutors)
                        {
                            doExecutor(Ctx);
                        }
                    }
                    var notCancellableCopyState = new StateN<T>(
                        Ctx,
                        History,
                        DoubleThrowElements,
                        allowCancel: false);
                    return Result.EventIsConsumed(nextState: notCancellableCopyState);
                }
            }
            else if (evnt is IPressEvent pressEvent)
            {
                if (DoubleThrowTriggers.Contains(pressEvent))
                {
                    foreach (var dt in GetDoubleThrowElements(pressEvent))
                    {
                        foreach (var pressExecutor in dt.PressExecutors)
                        {
                            pressExecutor(Ctx);
                        }
                    }
                    var nextState = new StateN<T>(
                        Ctx,
                        CreateHistory(History, pressEvent, this),
                        DoubleThrowElements,
                        allowCancel: true);
                    return Result.EventIsConsumed(nextState: nextState);
                }
            }
            else if (evnt is IReleaseEvent releaseEvent)
            {
                if (Machine.IsIgnored(releaseEvent))
                {
                    return Result.EventIsConsumed(nextState: this);
                }
                else if (IsNormalEndTrigger(releaseEvent))
                {
                    var strokes = Machine.GetStroke();
                    if (strokes.Count() > 0)
                    {
                        // if match
                        // Do strokeElements -> DoExecutors
                    }
                    else
                    {
                        //normal end
                        if (ShouldFinalize)
                        {
                            // execute Do and Press 
                        }
                        else
                        {
                            if (AllowCancel)
                            {
                                // Machine.OnCancel()
                            }
                        }
                    }
                    return Result.EventIsConsumed(nextState: LastState);
                }
                else if (AbnormalEndTriggers.Contains(releaseEvent))
                {
                    var (oldState, skippedReleaseEvents) = FindStateFromHistory(releaseEvent);
                    Machine.IgnoreNext(skippedReleaseEvents);
                    return Result.EventIsConsumed(nextState: oldState);
                }
            }

            return base.Input(evnt, point);
        }

        public State LastState
        {
            get { return History.Last().Item2; }
        }
        
        public bool IsCancelable
        {
            get
            {
                return AllowCancel && !ShouldFinalize;
            }
        }

        public bool ShouldFinalize
        {
            get
            {
                return DoubleThrowElements.All(d =>
                        d.DoExecutors.Count == 0 &&
                        d.PressExecutors.Count == 0);
            }
        }

        public IReadOnlyList<Tuple<IReleaseEvent, State>> CreateHistory(
            IReadOnlyList<Tuple<IReleaseEvent, State>> history, 
            IPressEvent pressEvent, 
            State state)
        {
            var newHistory = history.ToList();
            newHistory.Add(Tuple.Create(pressEvent.Opposition, state));
            return newHistory;
        }

        public IReleaseEvent NormalEndTrigger
        {
            get { return History.Last().Item1; }
        }

        public bool IsNormalEndTrigger(IReleaseEvent releaseEvent)
        {
            return releaseEvent == NormalEndTrigger;
        }


        public IReadOnlyCollection<IReleaseEvent> AbnormalEndTriggers
        {
            get
            {
                return new HashSet<IReleaseEvent>(from h in History select h.Item1);
            }
        }

        public (State, IReadOnlyList<IReleaseEvent>) FindStateFromHistory(IReleaseEvent releaseEvent)
        {

            var nextHistory = History.TakeWhile(t => t.Item1 != releaseEvent);
            var nextState = History[nextHistory.Count()].Item2;
            var skippedReleaseEvents = History.Skip(nextHistory.Count()).Select(t => t.Item1).ToList();
            return (nextState, skippedReleaseEvents);
        }


        public IReadOnlyList<DoubleThrowElement<T>> GetDoubleThrowElements(IPressEvent triggerEvent)
        {
            return (
                from d in DoubleThrowElements
                where d.IsFull
                select (
                    from dd in d.DoubleThrowElements
                    where dd.IsFull && dd.Trigger == triggerEvent
                    select dd))
                .Aggregate(new List<DoubleThrowElement<T>>(), (a, b) => { a.AddRange(b); return a; });
        }

        public IReadOnlyList<SingleThrowElement<T>> GetSingleThrowElements(IFireEvent triggerEvent)
        {
            return (
                from d in DoubleThrowElements
                where d.IsFull
                select (
                    from s in d.SingleThrowElements
                    where s.IsFull && s.Trigger == triggerEvent
                    select s))
                .Aggregate(new List<SingleThrowElement<T>>(), (a, b) => { a.AddRange(b); return a; });
        }

        public IEnumerable<IFireEvent> SingleThrowTriggers
        {
            get
            {
                return (
                    from d in DoubleThrowElements
                    where d.IsFull
                    select (
                        from s in d.SingleThrowElements
                        where s.IsFull
                        select s.Trigger))
                    .Aggregate(new HashSet<IFireEvent>(), (a, b) => { a.UnionWith(b); return a; });
            }
        }

        public IEnumerable<IPressEvent> DoubleThrowTriggers
        {
            get
            {
                return (
                    from d in DoubleThrowElements
                    where d.IsFull
                    select (
                        from dd in d.DoubleThrowElements
                        where dd.IsFull
                        select dd.Trigger))
                    .Aggregate(new HashSet<IPressEvent>(), (a, b) => { a.UnionWith(b); return a; });
            }
        }

    }

    public abstract class Element
    {
        public abstract bool IsFull { get; }
    }

    /* RootElement
     * 
     * .When() -> new WhenElement
     */
    public class RootElement<T> : Element
        where T : ActionContext
    {
        public override bool IsFull
        {
            get => WhenElements.Any(e => e.IsFull);
        }

        private List<WhenElement<T>> whenElements = new List<WhenElement<T>>();

        public IReadOnlyCollection<WhenElement<T>> WhenElements
        {
            get { return whenElements.ToList(); }
        }

        public WhenElement<T> When(EvaluateAction<T> evaluator)
        {
            var elm = new WhenElement<T>(evaluator);
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
    public class WhenElement<T> : Element
        where T : ActionContext
    {
        public override bool IsFull
        {
            get => WhenEvaluator != null &&
                SingleThrowElements.Any(e => e.IsFull) ||
                DoubleThrowElements.Any(e => e.IsFull);
        }

        public EvaluateAction<T> WhenEvaluator { get; private set; }

        private List<SingleThrowElement<T>> singleThrowElements = new List<SingleThrowElement<T>>();
        public IReadOnlyCollection<SingleThrowElement<T>> SingleThrowElements
        {
            get { return singleThrowElements.ToList(); }
        }

        private List<DoubleThrowElement<T>> doubleThrowElements = new List<DoubleThrowElement<T>>();
        public IReadOnlyCollection<DoubleThrowElement<T>> DoubleThrowElements
        {
            get { return doubleThrowElements.ToList(); }
        }

        public WhenElement(EvaluateAction<T> evaluator)
        {
            WhenEvaluator = evaluator;
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
    }

    /* SingleThrowElement
     * 
     * .Do() -> this
     */
    public class SingleThrowElement<T> : Element
        where T : ActionContext
    {
        public override bool IsFull
        {
            get => Trigger != null &&
                DoExecutors.Count > 0 && DoExecutors.Any(e => e != null);
        }

        public IFireEvent Trigger { get; private set; }
        
        private List<ExecuteAction<T>> doExecutors = new List<ExecuteAction<T>>();
        public IReadOnlyCollection<ExecuteAction<T>> DoExecutors
        {
            get { return doExecutors.ToList(); }
        }

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
        where T : ActionContext
    {
        public override bool IsFull
        {
            get => Trigger != null &&
                PressExecutors.Count > 0 && PressExecutors.Any(e => e != null) ||
                DoExecutors.Count > 0 && DoExecutors.Any(e => e != null) ||
                ReleaseExecutors.Count > 0 && ReleaseExecutors.Any(e => e != null) ||
                SingleThrowElements.Any(e => e.IsFull) ||
                DoubleThrowElements.Any(e => e.IsFull) ||
                StrokeElements.Any(e => e.IsFull);
        }

        public IPressEvent Trigger { get; private set; }

        private List<SingleThrowElement<T>> singleThrowElements = new List<SingleThrowElement<T>>();
        public IReadOnlyCollection<SingleThrowElement<T>> SingleThrowElements
        {
            get { return singleThrowElements.ToList(); }
        }

        private List<DoubleThrowElement<T>> doubleThrowElements = new List<DoubleThrowElement<T>>();
        public IReadOnlyCollection<DoubleThrowElement<T>> DoubleThrowElements
        {
            get { return doubleThrowElements.ToList(); }
        }

        private List<StrokeElement<T>> strokeElements = new List<StrokeElement<T>>();
        public IReadOnlyCollection<StrokeElement<T>> StrokeElements
        {
            get { return strokeElements.ToList(); }
        }

        private List<ExecuteAction<T>> pressExecutors = new List<ExecuteAction<T>>();
        public IReadOnlyCollection<ExecuteAction<T>> PressExecutors
        {
            get { return pressExecutors.ToList(); }
        }

        private List<ExecuteAction<T>> doExecutors = new List<ExecuteAction<T>>();
        public IReadOnlyCollection<ExecuteAction<T>> DoExecutors
        {
            get { return doExecutors.ToList(); }
        }

        private List<ExecuteAction<T>> releaseExecutors = new List<ExecuteAction<T>>();
        public IReadOnlyCollection<ExecuteAction<T>> ReleaseExecutors
        {
            get { return releaseExecutors.ToList(); }
        }

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
        where T : ActionContext
    {
        public override bool IsFull
        {
            get => Strokes != null && Strokes.Count > 0 &&
                DoExecutors.Count > 0 && DoExecutors.Any(e => e != null); 
        }

        public IReadOnlyCollection<StrokeEvent.Direction> Strokes { get; private set; }

        private List<ExecuteAction<T>> doExecutors = new List<ExecuteAction<T>>();
        public IReadOnlyCollection<ExecuteAction<T>> DoExecutors
        {
            get { return doExecutors.ToList(); }
        }

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
