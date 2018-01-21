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
    }


    public abstract class State
    {
        // global variable
        public GestureMachine Machine;
        public NaturalNumberCounter<IReleaseEvent> InvalidReleaseEvents;

        public virtual Result Input(Event evnt, Point point) // ここのPointは抽象化するとよさげ
        {
            return Result.EventIsRemained(nextState: this);
        }

        public bool MustBeIgnored(IReleaseEvent releaseEvent)
        {
            if (InvalidReleaseEvents[releaseEvent] > 0)
            {
                InvalidReleaseEvents.CountDown(releaseEvent);
                return true;
            }
            else if (releaseEvent is IPhysicalEvent)
            {
                var logicalEquivalent = releaseEvent.LogicalNormalized;

                if (InvalidReleaseEvents[logicalEquivalent] > 0)
                {
                    InvalidReleaseEvents.CountDown(logicalEquivalent);
                }
                return true;
            }
            return false;
        }


    }

    public class NaturalNumberCounter<T> {
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
            Dictionary[key] = Dictionary[key] + 1;
        }

        public void CountUp(T key)
        {
            Dictionary[key] = Dictionary[key] - 1;
        }
    }


    public class State0<T> : State
        where T : ActionContext, new()
    {
        public readonly RootElement<T> RootElement;

        public State0(
            GestureMachine gestureMachine,
            RootElement<T> rootElement
            ) : this(
            gestureMachine,
            rootElement,
            new NaturalNumberCounter<IReleaseEvent>())
        {

        }

        public State0(
            GestureMachine gestureMachine,
            RootElement<T> rootElement,
            NaturalNumberCounter<IReleaseEvent> invalidReleaseEvents)
        {
            Machine = gestureMachine;
            RootElement = rootElement; 
            InvalidReleaseEvents = invalidReleaseEvents;
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
                    return Result.EventIsConsumed(nextState: new StateN<T>(this, ));
                }
            }
            else if (evnt is IReleaseEvent releaseEvent)
            {
                if (MustBeIgnored(releaseEvent))
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

        public IEnumerable<DoubleThrowElement<T>> GetActiveDoubleThrowElements(T ctx, IPressEvent triggerEvent)
        {
            return (
                from w in RootElement.WhenElements
                where w.IsFull &&
                      w.WhenEvaluator(ctx)
                select (
                    from d in w.DoubleThrowElements
                    where d.IsFull && (
                              d.Trigger == triggerEvent ||
                              d.Trigger == triggerEvent.LogicalNormalized)
                    select d))
                .Aggregate(new List<DoubleThrowElement<T>>(), (a, b) => { a.AddRange(b); return a; });
        }

        public IEnumerable<SingleThrowElement<T>> GetActiveSingleThrowElements(T ctx, IFireEvent triggerEvent)
        {
            return (
                from w in RootElement.WhenElements
                where w.IsFull && 
                      w.WhenEvaluator(ctx)
                select (
                    from s in w.SingleThrowElements
                    where s.IsFull && (
                              s.Trigger == triggerEvent || 
                              s.Trigger == triggerEvent.LogicalNormalized)
                    select s))
                .Aggregate(new List<SingleThrowElement<T>>(), (a, b) => { a.AddRange(b); return a; } );
        }

        public IEnumerable<IFireEvent> SingleThrowTriggers
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

        public IEnumerable<IPressEvent> DoubleThrowTriggers
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
        public readonly IReleaseEvent EndTrigger;
        public readonly T Ctx;
        public readonly IReadOnlyList<Tuple<IReleaseEvent, State>> History;
        public readonly IReadOnlyList<DoubleThrowElement<T>> DoubleThrowElements;

        public bool IsCancelable { get; private set; }

        public StateN(
            IReleaseEvent endTrigger,
            T ctx,
            IReadOnlyList<Tuple<IReleaseEvent, State>> history,
            IReadOnlyList<DoubleThrowElement<T>> doubleThrowElements
            )
        {
            EndTrigger = endTrigger;
            Ctx = ctx;
            History = history;
            DoubleThrowElements = doubleThrowElements;
        }

        public override Result Input(Event evnt, Point point)
        {
            // Todo: storkewatcher

            if (evnt is IFireEvent fireEvent)
            {
                foreach (var st in GetActiveSingleThrowElements(Ctx, fireEvent))
                {
                    foreach (var doExecutor in st.DoExecutors)
                    {
                        doExecutor(Ctx);
                    }
                }
                return Result.EventIsConsumed(nextState: this);

            }
            else if (evnt is IPressEvent pressEvent)
            {
                var doubleThrowElements = GetActiveDoubleThrowElements(Ctx, pressEvent);
                if (doubleThrowElements.Count() > 0)
                {
                    foreach (var dt in doubleThrowElements)
                    {
                        foreach (var pressExecutor in dt.PressExecutors)
                        {
                            pressExecutor(Ctx);
                        }
                    }
                    return Result.EventIsConsumed(nextState: new StateN<T>(this, ));
                }
            }
            else if (evnt is IReleaseEvent releaseEvent)
            {
                if (MustBeIgnored(releaseEvent))
                {
                    return Result.EventIsConsumed(nextState: this);
                }

                if (releaseEvent == EndTrigger)
                {
                    var (_, previousState) = History.Last();
                    return Result.EventIsConsumed(nextState: previousState);
                }
                
                if (AbnormalEndTriggers.Contains(releaseEvent))
                {


                    // Append EndTrigger
                }

                var 
                var (nextStack, broken) = SplitHistory(releaseEvent);

                    
                return Result.EventIsConsumed(nextState: this);
             


                var strokes = Machine.GetStroke();
                if (strokes.Count() > 0)
                {
                    // if match
                    // Do strokeElements -> DoExecutors
                    return Result.EventIsConsumed
                }
                else
                {

                }
            }

            return base.Input(evnt, point);
        }

        public IReadOnlyCollection<IReleaseEvent> AbnormalEndTriggers
        {
            get
            {
                return new HashSet<IReleaseEvent>(from h in History select h.Item1);
            }
        }

        public (State, IReadOnlyList<IReleaseEvent>) FindFromHistory(IReleaseEvent releaseEvent)
        {

            var skipped = History
                .Reverse()
                .TakeWhile(t =>
                    t.Item1 != releaseEvent &&
                    t.Item1 != releaseEvent.LogicalNormalized);
            var skippedReleaseEvents = from r in skipped select r.Item1;
            var state = 

            return state;
        }


        public IReadOnlyList<DoubleThrowElement<T>> GetActiveDoubleThrowElements(T ctx, IPressEvent triggerEvent)
        {
            return (
                from d in DoubleThrowElements
                where d.IsFull
                select (
                    from dd in d.DoubleThrowElements
                    where dd.IsFull && (
                              dd.Trigger == triggerEvent ||
                              dd.Trigger == triggerEvent.LogicalNormalized)

                    select dd))
                .Aggregate(new List<DoubleThrowElement<T>>(), (a, b) => { a.AddRange(b); return a; });
        }

        public IReadOnlyList<SingleThrowElement<T>> GetActiveSingleThrowElements(T ctx, IFireEvent triggerEvent)
        {
            return (
                from d in DoubleThrowElements
                where d.IsFull
                select (
                    from st in d.SingleThrowElements
                    where st.IsFull && (
                              st.Trigger == triggerEvent ||
                              st.Trigger == triggerEvent.LogicalNormalized)
                    select st))
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
