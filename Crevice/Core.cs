using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


 namespace Crevice.Future
{
    
}

namespace Crevice.Dev
{
    using System.Drawing;

    public abstract class ActionContext
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

    public class GestureManager<T>
        where T : ActionContext, new()
    {
        public Root<T> Root = new Root<T>();

        public List<GestureDefinition<T>> GetDefinitions()
        {
            return DSLTreeParser<T>.TreeToGestureDefinitions(Root)
                .Where(x => x.IsComplete)
                .ToList();
        }

        public GestureMachine<A, T> GetGestureMachine<A>(A config)
            where A : GestureMachineConfig
        {
            var gestureDef = GetDefinitions();
            return new GestureMachine<A, T>(config, gestureDef);
        }

    }

    public static class GestureManager
    {
        public static GestureManager<DefaultActionContext> Create()
        {
            return Create<DefaultActionContext>();
        }

        public static GestureManager<A> Create<A>()
            where A : ActionContext, new()
        {
            return new GestureManager<A>();
        }
    }


    // Configはやはり継承する場合があるのでジェネリクスを使わざるをえない
    // 各コンフィグの分のパラメーターが必要、かな


    // OnGestureStart()

    // OnGestureEnd (  )NeedToRestore~ をStateに生やす？ 

    // OnGestureCanelled

    // OnGestureTimeout

    // RestorePrimaryButtonDownEventHandler 

    // OnStrokeUudate


    // StrokeUpdateEventHandler

    // Configはデフォルトで見せる？見せない？とかそういうの


    // OnGestureTimeoutRequest(State1 S1) これは成否で2分岐する

    // OnGestureCancel() これは分岐なし // OnGestureEndでCancelかどうかを知れたほうがいいかな

    // これをGestureMachineにしてFSMのほうは単なるFSMに？
    public abstract class GestureMachineConfig
    {
        /*
        public static Func<System.Drawing.Point, System.Drawing.Point> DefaultPositionBinding
        {
            get
            {
                return (point) =>
                {
                    return point;
                };
            }
        }
        */

        //Todo: null check in bindings
        //public Def.PointBinding TooltipPositionBinding = Def.DefaultPointBinding;
        public event EventHandler OnMachineStart;
        public event EventHandler OnMachineReset;
        public event EventHandler OnMachineEnd;

        public event EventHandler OnGestureStart;
        public event EventHandler OnGestureCancel;
        public event System.ComponentModel.CancelEventHandler OnGestureTimeout;
        public event EventHandler OnGestureEnd;

        public int GestureTimeout = 1000;

        public event EventHandler OnStrokeUpdate;

        public int StrokeStartThreshold = 10;
        public int StrokeDirectionChangeThreshold = 20;
        public int StrokeExtensionThreshold = 10;
        public int StrokeWatchInterval = 10;
    }

    public class DefaultGestureMachineConfig : GestureMachineConfig
    { }


    // イミュータブルなConfigは微妙なので実装しない
    // Configはここではキャッシュせず、毎回アクセスすることを保証する。実装はConfigに任せる
    // ホットスワップ使うとよろし

    public class GestureMachine<A, B>
                : IDisposable
                where A : GestureMachineConfig
                where B : ActionContext, new()
    {
        public readonly StateGlobal Global;
        public IState State { get; private set; }
        public IEnumerable<GestureDefinition<B>> GestureDefinition { get; private set; }

        private System.Timers.Timer timer = new System.Timers.Timer();
        private readonly object lockObject = new object();

        public GestureMachine(A config, IEnumerable<GestureDefinition<B>> gestureDef)
        {
            this.Global = new StateGlobal(config); // will work?
            this.State = new State0<B>(Global, gestureDef);
            this.GestureDefinition = gestureDef;

            timer.Elapsed += new System.Timers.ElapsedEventHandler(TryGestureTimeout);
            timer.Interval = Global.Config.GestureTimeout;
            timer.AutoReset = false;
        }

        public bool Input(Core.Def.Event.IEvent evnt, System.Drawing.Point point)
        {
            lock (lockObject)
            {
                var res = State.Input(evnt, point);
                if (State.GetType() != res.NextState.GetType())
                {
                    Verbose.Print("The state of GestureMachine was changed: {0} -> {1}", State.GetType().Name, res.NextState.GetType().Name);

                    // Special side effect 1
                    if (res.NextState is State1<B> || res.NextState is State2<B>)
                    {
                        Global.ResetStrokeWatcher();
                    }

                    // Reset timer for gesture timeout
                    if (State is State0<B> && res.NextState is State1<B>)
                    {
                        timer.Stop();
                        // Reflect current config value
                        timer.Interval = Global.Config.GestureTimeout;
                        timer.Start();
                    }
                }
                State = res.NextState;
                return res.Event.IsConsumed;
            }
        }

        private void TryGestureTimeout(object sender, System.Timers.ElapsedEventArgs args)
        {
            lock (lockObject)
            {
                if (State is State1<B> && Global.StrokeWatcher.GetStorke().Count == 0)
                {
                    State = ((State1<B>)State).Cancel();
                }
            }
        }

        public void Reset()
        {
            lock (lockObject)
            {
                State = State.Reset();
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            lock (lockObject)
            {
                Global.Dispose();
            }
        }

        ~GestureMachine()
        {
            Dispose();
        }
    }

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
        public IState NextState { get; private set; }

        private Result(bool consumed, IState nextState)
        {
            this.Event = new EventResult(consumed);
            this.NextState = nextState;
        }

        public static Result EventIsConsumed(IState nextState)
        {
            return new Result(true, nextState);
        }

        public static Result EventIsRemained(IState nextState)
        {
            return new Result(false, nextState);
        }
    }

    public interface IState
    {
        Result Input(Core.Def.Event.IEvent evnt, System.Drawing.Point point);
        IState Reset();
    }

    public abstract class State<T>
        : IState
        where T : ActionContext, new()
    {
        protected internal readonly StateGlobal Global;

        public State(StateGlobal Global)
        {
            this.Global = Global;
        }

        public virtual Result Input(Core.Def.Event.IEvent evnt, System.Drawing.Point point)
        {
            return Result.EventIsRemained(nextState: this);
        }

        public virtual IState Reset()
        {
            throw new InvalidOperationException();
        }
        
        protected internal T CreateActionContext(System.Drawing.Point currentPoint, T previousActionContext = null)
        {
            var context = new T();
            context.Setup(previousActionContext, currentPoint);
            return context;
        }

        // TOdo move to Global?
        // 責任はそこではない。各State、が一番よさげ

        protected internal void ExecuteUserBeforeFuncInBackground(
            T ctx,
            IEnumerable<IBeforeExecutable<T>> gestureDef)
        {
            Global.UserActionTaskFactory.StartNew(() => {
                foreach (var gDef in gestureDef)
                {
                    gDef.ExecuteBeforeFunc(ctx);
                }
            });
        }

        protected internal void ExecuteUserDoFuncInBackground(
            T ctx,
            IEnumerable<IDoExecutable<T>> gestureDef)
        {
            Global.UserActionTaskFactory.StartNew(() => {
                foreach (var gDef in gestureDef)
                {
                    gDef.ExecuteDoFunc(ctx);
                }
            });
        }

        protected internal void ExecuteUserAfterFuncInBackground(
            T ctx,
            IEnumerable<IAfterExecutable<T>> gestureDef)
        {
            Global.UserActionTaskFactory.StartNew(() => {
                foreach (var gDef in gestureDef)
                {
                    gDef.ExecuteAfterFunc(ctx);
                }
            });
        }

        protected internal void ExecuteInBackground(
            Action action)
        {
            Global.UserActionTaskFactory.StartNew(action);
        }

        // Check whether given event must be ignored or not.
        // Return true if given event is in the ignore list, and remove it from the list.
        // Return false if the pair of given event is in the ignore list, and remove it from the list.
        // Otherwise return false.
        protected internal bool MustBeIgnored(Core.Def.Event.IEvent evnt)
        {
            if (evnt is Core.Def.Event.IDoubleActionRelease)
            {
                var ev = evnt as Core.Def.Event.IDoubleActionRelease;
                if (Global.IgnoreNext.Contains(ev))
                {
                    Global.IgnoreNext.Remove(ev);
                    return true;
                }
            }
            else if (evnt is Core.Def.Event.IDoubleActionSet)
            {
                var ev = evnt as Core.Def.Event.IDoubleActionSet;
                var p = ev.GetPair();
                if (Global.IgnoreNext.Contains(p))
                {
                    Global.IgnoreNext.Remove(p);
                    return false;
                }
            }
            return false;
        }

        protected internal void IgnoreNext(Core.Def.Event.IDoubleActionRelease evnt)
        {
            Verbose.Print("IgnoreNext flag is set for {0}. This event will be ignored next time.", evnt.GetType().Name);
            Global.IgnoreNext.Add(evnt);
        }
    }

    public class State0<T> : State<T>
        where T : ActionContext, new()
    {
        internal readonly IDictionary<Core.Def.Event.ISingleAction, IEnumerable<IfButtonGestureDefinition<T>>> T0;
        internal readonly IDictionary<Core.Def.Event.IDoubleActionSet, IEnumerable<OnButtonGestureDefinition<T>>> T1;
        internal readonly IDictionary<Core.Def.Event.IDoubleActionSet, IEnumerable<IfButtonGestureDefinition<T>>> T2;

        public State0(
            StateGlobal Global,
            IEnumerable<GestureDefinition<T>> gestureDef)
            : base(Global)
        {
            this.T0 = Transition<T>.Gen0_0(gestureDef);
            this.T1 = Transition<T>.Gen0_1(gestureDef);
            this.T2 = Transition<T>.Gen1_3(gestureDef);
        }

        public override Result Input(Core.Def.Event.IEvent evnt, System.Drawing.Point point)
        {
            // Special side effect 3, 4
            if (MustBeIgnored(evnt))
            {
                return Result.EventIsConsumed(nextState: this);
            }

            // todo ビット演算でやるとよさげ 
            // ただし、プラットフォーム互換になる仕組みが必要
            // GestureMachineは消費したかどうかだけをboolで返すので、ジェスチャ内

            if (evnt is Core.Def.Event.ISingleAction)
            {
                var ev = evnt as Core.Def.Event.ISingleAction;
                if (T0.Keys.Contains(ev))
                {
                    // Whenで使われたこのctxを以降も保持する
                    var ctx = CreateActionContext(point);
                    var _T0 = FilterByWhenClause(ctx, T0[ev]);
                    if (_T0.Count() > 0)
                    {
                        Verbose.Print("[Transition 0_0]");
                        ExecuteUserDoFuncInBackground(ctx, _T0);
                        return Result.EventIsConsumed(nextState: this);
                    }
                }
            }
            else if (evnt is Core.Def.Event.IDoubleActionSet)
            {
                var ev = evnt as Core.Def.Event.IDoubleActionSet;
                if (T1.Keys.Contains(ev) || T2.Keys.Contains(ev))
                {
                    // Whenで使われたこのctxを以降も保持する
                    var ctx = CreateActionContext(point);
                    var cache = new Dictionary<Def<T>.WhenFunc, bool>();
                    var _T1 = T1.Keys.Contains(ev) ? FilterByWhenClause(ctx, T1[ev], cache) : new List<OnButtonGestureDefinition<T>>();
                    var _T2 = T2.Keys.Contains(ev) ? FilterByWhenClause(ctx, T2[ev], cache) : new List<IfButtonGestureDefinition<T>>();
                    if (_T1.Count() > 0 || _T2.Count() > 0)
                    {
                        Verbose.Print("[Transition 0_1]");
                        ExecuteUserBeforeFuncInBackground(ctx, _T2);
                        return Result.EventIsConsumed(nextState: new State1<T>(Global, this, ctx, ev, _T1, _T2));
                    }
                }
            }
            return base.Input(evnt, point);
        }

        public override IState Reset()
        {
            Verbose.Print("[Transition 0_2]");
            return this;
        }


        // tupleで戻り値を扱うとスマートでは
        internal static IEnumerable<A> FilterByWhenClause<A>(
            T ctx,
            IEnumerable<A> gestureDef)
            where A : IWhenEvaluatable<T>
        {
            return FilterByWhenClause(ctx, gestureDef, new Dictionary<Def<T>.WhenFunc, bool>());
        }

        internal static IEnumerable<A> FilterByWhenClause<A>(
            T ctx,
            IEnumerable<A> gestureDef,
            Dictionary<Def<T>.WhenFunc, bool> cache)
            where A : IWhenEvaluatable<T>
        {
            // This evaluation of functions given as the parameter of `@when` clause can be executed in parallel, 
            // but executing it in sequential order here for simplicity.
            return gestureDef
                .Where(x => x.EvaluateWhenFunc(ctx, cache))
                .ToList();
        }
    }

    public class State1<T> : State<T>
        where T : ActionContext, new()
    {
        internal readonly State0<T> S0;
        internal readonly State2<T> S2;
        internal readonly T ctx;
        internal readonly Core.Def.Event.IDoubleActionSet primaryEvent;
        internal readonly IDictionary<Core.Def.Event.ISingleAction, IEnumerable<OnButtonWithIfButtonGestureDefinition<T>>> T0;
        internal readonly IDictionary<Core.Def.Event.IDoubleActionSet, IEnumerable<OnButtonWithIfButtonGestureDefinition<T>>> T1;
        internal readonly IDictionary<Core.Def.Stroke, IEnumerable<OnButtonWithIfStrokeGestureDefinition<T>>> T2;
        internal readonly IEnumerable<IfButtonGestureDefinition<T>> T3;

        //todo
        //private readonly SingleInputSender InputSender = new SingleInputSender();

        public State1(
            StateGlobal Global,
            State0<T> S0,
            T ctx,
            Core.Def.Event.IDoubleActionSet primaryEvent,
            IEnumerable<OnButtonGestureDefinition<T>> T1,
            IEnumerable<IfButtonGestureDefinition<T>> T2
            ) : base(Global)
        {
            this.S0 = S0;
            this.ctx = ctx;
            this.primaryEvent = primaryEvent;
            this.T0 = Transition<T>.Gen1_0(T1);
            this.T1 = Transition<T>.Gen1_1(T1);
            this.T2 = Transition<T>.Gen1_2(T1);
            this.T3 = T2;
            this.S2 = new State2<T>(Global, S0, ctx, primaryEvent, this.T0, this.T1, this.T2, this.T3);
        }

        public override Result Input(Core.Def.Event.IEvent evnt, System.Drawing.Point point)
        {
            // Special side effect 3, 4
            if (MustBeIgnored(evnt))
            {
                return Result.EventIsConsumed(nextState: this);
            }
            // Special side effect 2
            Global.StrokeWatcher.Queue(point);

            if (evnt is Core.Def.Event.ISingleAction)
            {
                var ev = evnt as Core.Def.Event.ISingleAction;
                if (T0.Keys.Contains(ev))
                {
                    Verbose.Print("[Transition 1_0]");
                    // todo ctx
                    // var ctx = CreateActionContext(point, previousActionContext: ctx);
                    ExecuteUserDoFuncInBackground(ctx, T0[ev]);
                    return Result.EventIsConsumed(nextState: S2);
                }
            }
            else if (evnt is Core.Def.Event.IDoubleActionSet)
            {
                var ev = evnt as Core.Def.Event.IDoubleActionSet;
                if (T1.Keys.Contains(ev))
                {
                    Verbose.Print("[Transition 1_1]");
                    ExecuteUserBeforeFuncInBackground(ctx, T1[ev]);
                    return Result.EventIsConsumed(nextState: new State3<T>(Global, S0, S2, ctx, primaryEvent, ev, T3, T1[ev]));
                }
            }
            else if (evnt is Core.Def.Event.IDoubleActionRelease)
            {
                var ev = evnt as Core.Def.Event.IDoubleActionRelease;
                if (ev == primaryEvent.GetPair())
                {
                    var stroke = Global.StrokeWatcher.GetStorke();
                    if (stroke.Count() > 0)
                    {
                        Verbose.Print("Stroke: {0}", stroke.ToString());
                        if (T2.Keys.Contains(stroke))
                        {
                            Verbose.Print("[Transition 1_2]");
                            ExecuteUserDoFuncInBackground(ctx, T2[stroke]);
                            ExecuteUserAfterFuncInBackground(ctx, T3);
                        }
                    }
                    else
                    {
                        if (T3.Count() > 0)
                        {
                            Verbose.Print("[Transition 1_3]");
                            ExecuteUserDoFuncInBackground(ctx, T3);
                            ExecuteUserAfterFuncInBackground(ctx, T3);
                        }
                        else
                        {
                            Verbose.Print("[Transition 1_4]");
                            //todo
                            //ExecuteInBackground(ctx, RestorePrimaryButtonClickEvent());
                        }
                    }
                    return Result.EventIsConsumed(nextState: S0);
                }
            }
            return base.Input(evnt, point);
        }

        // TOdo: 

        // Todo: IsCancellable を共通インターフェイスにする or IsTimeoutable

        // Todo: んで、IsRestorable

        public IState Cancel()
        {
            if (!HasBeforeOrAfter) // Todo: IsCancelable がいいかな
            {
                // リストア可能でないなら、IgnoreNext、かな。IgnoreNextがデフォルト？ eventhanderかな

                // これはストロークの数が０であることも復元の条件（なのでポインタの原点を持っていなくてもうまくいっている）
                // ポインタの初期位置を復元するのもおかしいので難しいところだが、この実装が正しいと思う

                Verbose.Print("[Transition 1_5]");
                //todo
                //ExecuteInBackground(ctx, RestorePrimaryButtonDownEvent()); 
                return S0;
            }
            else
            {
                // キャンセルを無視するフロー
                return this;
            }
        }

        internal bool HasBeforeOrAfter
        {
            get
            {
                return T3
                    .Where(x => x.beforeFunc != null || x.afterFunc != null)
                    .Count() > 0;
            }
        }

        public override IState Reset()
        {
            Verbose.Print("[Transition 1_6]");
            IgnoreNext(primaryEvent.GetPair());
            ExecuteUserAfterFuncInBackground(ctx, T3);
            return S0;
        }

        /*
        internal Action RestorePrimaryButtonDownEvent()
        {
            return () =>
            {
                if (primaryEvent == Def.Constant.LeftButtonDown)
                {
                    InputSender.LeftDown();
                }
                else if (primaryEvent == Def.Constant.MiddleButtonDown)
                {
                    InputSender.MiddleDown();
                }
                else if (primaryEvent == Def.Constant.RightButtonDown)
                {
                    InputSender.RightDown();
                }
                else if (primaryEvent == Def.Constant.X1ButtonDown)
                {
                    InputSender.X1Down();
                }
                else if (primaryEvent == Def.Constant.X2ButtonDown)
                {
                    InputSender.X2Down();
                }
            };
        }
        */

        /*
        internal Action RestorePrimaryButtonClickEvent()
        {
            return () =>
            {
                if (primaryEvent == Def.Constant.LeftButtonDown)
                {
                    InputSender.LeftClick();
                }
                else if (primaryEvent == Def.Constant.MiddleButtonDown)
                {
                    InputSender.MiddleClick();
                }
                else if (primaryEvent == Def.Constant.RightButtonDown)
                {
                    InputSender.RightClick();
                }
                else if (primaryEvent == Def.Constant.X1ButtonDown)
                {
                    InputSender.X1Click();
                }
                else if (primaryEvent == Def.Constant.X2ButtonDown)
                {
                    InputSender.X2Click();
                }
            };
        }
        */
    }

    public class State2<T> : State<T>
        where T : ActionContext, new()
    {
        internal readonly State0<T> S0;
        internal readonly T ctx;
        internal readonly Core.Def.Event.IDoubleActionSet primaryEvent;
        internal readonly IDictionary<Core.Def.Event.ISingleAction, IEnumerable<OnButtonWithIfButtonGestureDefinition<T>>> T0;
        internal readonly IDictionary<Core.Def.Event.IDoubleActionSet, IEnumerable<OnButtonWithIfButtonGestureDefinition<T>>> T1;
        internal readonly IDictionary<Core.Def.Stroke, IEnumerable<OnButtonWithIfStrokeGestureDefinition<T>>> T2;
        internal readonly IEnumerable<IfButtonGestureDefinition<T>> T3;

        public State2(
            StateGlobal Global,
            State0<T> S0,
            T ctx,
            Core.Def.Event.IDoubleActionSet primaryEvent,
            IDictionary<Core.Def.Event.ISingleAction, IEnumerable<OnButtonWithIfButtonGestureDefinition<T>>> T0,
            IDictionary<Core.Def.Event.IDoubleActionSet, IEnumerable<OnButtonWithIfButtonGestureDefinition<T>>> T1,
            IDictionary<Core.Def.Stroke, IEnumerable<OnButtonWithIfStrokeGestureDefinition<T>>> T2,
            IEnumerable<IfButtonGestureDefinition<T>> T3
            ) : base(Global)
        {
            this.S0 = S0;
            this.ctx = ctx;
            this.primaryEvent = primaryEvent;
            this.T0 = T0;
            this.T1 = T1;
            this.T2 = T2;
            this.T3 = T3;
        }

        public override Result Input(Core.Def.Event.IEvent evnt, System.Drawing.Point point)
        {
            // Special side effect 3, 4
            if (MustBeIgnored(evnt))
            {
                return Result.EventIsConsumed(nextState: this);
            }
            // Special side effect 2
            Global.StrokeWatcher.Queue(point);

            if (evnt is Core.Def.Event.ISingleAction)
            {
                var ev = evnt as Core.Def.Event.ISingleAction;
                if (T0.Keys.Contains(ev))
                {
                    Verbose.Print("[Transition 2_0]");
                    ExecuteUserDoFuncInBackground(ctx, T0[ev]);
                    return Result.EventIsConsumed(nextState: this);
                }
            }
            else if (evnt is Core.Def.Event.IDoubleActionSet)
            {
                var ev = evnt as Core.Def.Event.IDoubleActionSet;
                if (T1.Keys.Contains(ev))
                {
                    Verbose.Print("[Transition 2_1]");
                    ExecuteUserBeforeFuncInBackground(ctx, T1[ev]);
                    return Result.EventIsConsumed(nextState: new State3<T>(Global, S0, this, ctx, primaryEvent, ev, T3, T1[ev]));
                }
            }
            else if (evnt is Core.Def.Event.IDoubleActionRelease)
            {
                var ev = evnt as Core.Def.Event.IDoubleActionRelease;
                if (ev == primaryEvent.GetPair())
                {
                    var stroke = Global.StrokeWatcher.GetStorke();
                    if (stroke.Count() > 0)
                    {
                        Verbose.Print("Stroke: {0}", stroke.ToString());
                        if (T2.Keys.Contains(stroke))
                        {
                            Verbose.Print("[Transition 2_2]");
                            ExecuteUserDoFuncInBackground(ctx, T2[stroke]);
                            ExecuteUserAfterFuncInBackground(ctx, T3);
                        }
                    }
                    else
                    {
                        Verbose.Print("[Transition 2_3]");
                        ExecuteUserAfterFuncInBackground(ctx, T3);
                    }
                    return Result.EventIsConsumed(nextState: S0);
                }
            }
            return base.Input(evnt, point);
        }

        public override IState Reset()
        {
            Verbose.Print("[Transition 2_4]");
            IgnoreNext(primaryEvent.GetPair());
            ExecuteUserAfterFuncInBackground(ctx, T3);
            return S0;
        }
    }
    public class State3<T> : State<T>
        where T : ActionContext, new()
    {
        internal readonly State0<T> S0;
        internal readonly State2<T> S2;
        internal readonly T ctx;
        internal readonly Core.Def.Event.IDoubleActionSet primaryEvent;
        internal readonly Core.Def.Event.IDoubleActionSet secondaryEvent;
        internal readonly IEnumerable<IfButtonGestureDefinition<T>> T0;
        internal readonly IEnumerable<OnButtonWithIfButtonGestureDefinition<T>> T1;

        public State3(
            StateGlobal Global,
            State0<T> S0,
            State2<T> S2,
            T ctx,
            Core.Def.Event.IDoubleActionSet primaryEvent,
            Core.Def.Event.IDoubleActionSet secondaryEvent,
            IEnumerable<IfButtonGestureDefinition<T>> T0,
            IEnumerable<OnButtonWithIfButtonGestureDefinition<T>> T1
            ) : base(Global)
        {
            this.S0 = S0;
            this.S2 = S2;
            this.ctx = ctx;
            this.primaryEvent = primaryEvent;
            this.secondaryEvent = secondaryEvent;
            this.T0 = T0;
            this.T1 = T1;
        }

        public override Result Input(Core.Def.Event.IEvent evnt, System.Drawing.Point point)
        {
            // Special side effect 3, 4
            if (MustBeIgnored(evnt))
            {
                return Result.EventIsConsumed(nextState: this);
            }

            if (evnt is Core.Def.Event.IDoubleActionRelease)
            {
                var ev = evnt as Core.Def.Event.IDoubleActionRelease;
                if (ev == secondaryEvent.GetPair())
                {
                    Verbose.Print("[Transition 3_0]");
                    ExecuteUserDoFuncInBackground(ctx, T1);
                    ExecuteUserAfterFuncInBackground(ctx, T1);
                    return Result.EventIsConsumed(nextState: S2);
                }
                else if (ev == primaryEvent.GetPair())
                {
                    Verbose.Print("[Transition 3_1]");
                    IgnoreNext(secondaryEvent.GetPair());
                    ExecuteUserAfterFuncInBackground(ctx, T1);
                    ExecuteUserAfterFuncInBackground(ctx, T0);
                    return Result.EventIsConsumed(nextState: S0);
                }
            }
            return base.Input(evnt, point);
        }

        public override IState Reset()
        {
            Verbose.Print("[Transition 3_2]");
            IgnoreNext(primaryEvent.GetPair());
            IgnoreNext(secondaryEvent.GetPair());
            ExecuteUserAfterFuncInBackground(ctx, T1);
            ExecuteUserAfterFuncInBackground(ctx, T0);
            return S0;
        }
    }
    public class StateGlobal
    : IDisposable
    {
        public readonly TaskFactory StrokeWatcherTaskFactory;
        public readonly TaskFactory LowPriorityTaskFactory;
        public readonly TaskFactory UserActionTaskFactory;

        public readonly GestureMachineConfig Config;

        public readonly HashSet<Core.Def.Event.IDoubleActionRelease> IgnoreNext = new HashSet<Core.Def.Event.IDoubleActionRelease>();

        public Core.Stroke.StrokeWatcher StrokeWatcher { get; internal set; }

        public StateGlobal(
            GestureMachineConfig config,
            TaskFactory strokeWatcherTaskFactory,
            TaskFactory userActionTaskFactory,
            TaskFactory lowPriorityTaskFactory)
        {
            this.Config = config;
            this.StrokeWatcherTaskFactory = strokeWatcherTaskFactory;
            this.LowPriorityTaskFactory = lowPriorityTaskFactory;
            this.UserActionTaskFactory = userActionTaskFactory;
            ResetStrokeWatcher();
        }

        public StateGlobal(GestureMachineConfig config) : this(
            config,
            Task.Factory,
            Task.Factory,
            Task.Factory
            )
        { }

        private Core.Stroke.StrokeWatcher NewStrokeWatcher()
        {
            return new Core.Stroke.StrokeWatcher(
                StrokeWatcherTaskFactory,
                Config.StrokeStartThreshold,
                Config.StrokeDirectionChangeThreshold,
                Config.StrokeExtensionThreshold,
                Config.StrokeWatchInterval);
        }

        public void ResetStrokeWatcher()
        {
            var _StrokeWatcher = StrokeWatcher;
            StrokeWatcher = NewStrokeWatcher();
            if (_StrokeWatcher != null)
            {
                Verbose.Print("StrokeWatcher was reset; {0} -> {1}", _StrokeWatcher.GetHashCode(), StrokeWatcher.GetHashCode());
                LowPriorityTaskFactory.StartNew(() => {
                    _StrokeWatcher.Dispose();
                });
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            StrokeWatcher.Dispose();
        }

        ~StateGlobal()
        {
            Dispose();
        }
    }

    public static class Transition<T>
        where T : ActionContext
    {
        #region Transition Definition
        #region State0
        // Transition 0_0 (single action gesture established)
        //
        // State0 -> State0
        //
        // This transition happends when `fire` event of single action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@do` clause of IfButtonGestureDefinition are executed.
        public static IDictionary<Core.Def.Event.ISingleAction, IEnumerable<IfButtonGestureDefinition<T>>>
            Gen0_0(IEnumerable<GestureDefinition<T>> gestureDef)
        {
            return gestureDef
                .Select(x => x as IfButtonGestureDefinition<T>)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.ifButton))
                .Where(x => x.Key is Core.Def.Event.ISingleAction)
                .ToDictionary(x => x.Key as Core.Def.Event.ISingleAction, x => x.Select(y => y));
        }

        // Transition 0_1 (gesture with primary double action mouse button start)
        //
        // State0 -> State1
        //
        // Transition from the state(S0) to the state(S1).
        // This transition happends when `set` event of double action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@before` clause of IfButtonGestureDefinition are executed.
        public static IDictionary<Core.Def.Event.IDoubleActionSet, IEnumerable<OnButtonGestureDefinition<T>>>
            Gen0_1(IEnumerable<GestureDefinition<T>> gestureDef)
        {
            return gestureDef
                .Select(x => x as OnButtonGestureDefinition<T>)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.onButton))
                .ToDictionary(x => x.Key, x => x.Select(y => y));
        }

        // Transition 0_2 (forced reset)
        //
        // State0 -> State0
        // 
        // Transition from the state(S0) to the state(S0).
        // This event happens when a `reset` command given.
        // This transition have no side effect.
        #endregion

        #region State1
        // Transition 1_0 (single action gesture established)
        //
        // State1 -> State2
        //
        // Transition from the state(S1) to the state(S2). 
        // This transition happends when `fire` event of single action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@do` clause of OnButtonWithIfButtonGestureDefinition are executed.
        public static IDictionary<Core.Def.Event.ISingleAction, IEnumerable<OnButtonWithIfButtonGestureDefinition<T>>>
            Gen1_0(IEnumerable<OnButtonGestureDefinition<T>> gestureDef)
        {
            return gestureDef
                .Select(x => x as OnButtonWithIfButtonGestureDefinition<T>)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.ifButton))
                .Where(x => x.Key is Core.Def.Event.ISingleAction)
                .ToDictionary(x => x.Key as Core.Def.Event.ISingleAction, x => x.Select(y => y));
        }

        // Transition 1_1 (gesture with primary and secondary double action mouse button start)
        //
        // State1 -> State3
        //
        // Transition from the state(S1) to the state(S3).
        // This transition happends when `set` event of double action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@before` clause of OnButtonWithIfButtonGestureDefinition are executed.
        public static IDictionary<Core.Def.Event.IDoubleActionSet, IEnumerable<OnButtonWithIfButtonGestureDefinition<T>>>
            Gen1_1(IEnumerable<OnButtonGestureDefinition<T>> gestureDef)
        {
            return gestureDef
                .Select(x => x as OnButtonWithIfButtonGestureDefinition<T>)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.ifButton))
                .Where(x => x.Key is Core.Def.Event.IDoubleActionSet)
                .ToDictionary(x => x.Key as Core.Def.Event.IDoubleActionSet, x => x.Select(y => y));
        }

        // Transition 1_2 (stroke gesture established)
        //
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0).
        // This transition happends when `release` event of primary double action mouse button
        // and a gesture stroke existing in OnButtonWithIfStrokeGestureDefinition are given.
        // This transition has two side effects.
        // 1. Functions given as the parameter of `@do` clause of StrokeGestureDefinition are executed.
        // 2. Functions given as the parameter of `@after` clause of IfButtonGestureDefinition are executed.
        public static IDictionary<Core.Def.Stroke, IEnumerable<OnButtonWithIfStrokeGestureDefinition<T>>>
            Gen1_2(IEnumerable<OnButtonGestureDefinition<T>> gestureDef)
        {
            return gestureDef
                .Select(x => x as OnButtonWithIfStrokeGestureDefinition<T>)
                .Where(x => x != null)
                .ToLookup(x => x.stroke)
                .ToDictionary(x => x.Key, x => x.Select(y => y));
        }

        // Transition 1_3 (default gesture established)
        // 
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0).
        // This transition happens when `release` event of primary double action mouse button is given and 
        // there have not been any actions executed.
        // This transition has two side effects.
        // 1. Functions given as the parameter of `@do` clause of IfButtonGestureDefinition are executed.
        // 2. Functions given as the parameter of `@after` clause of IfButtonGestureDefinition are executed.
        public static IDictionary<Core.Def.Event.IDoubleActionSet, IEnumerable<IfButtonGestureDefinition<T>>>
            Gen1_3(IEnumerable<GestureDefinition<T>> gestureDef)
        {
            return gestureDef
                .Select(x => x as IfButtonGestureDefinition<T>)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.ifButton))
                .Where(x => x.Key is Core.Def.Event.IDoubleActionSet)
                .ToDictionary(x => x.Key as Core.Def.Event.IDoubleActionSet, x => x.Select(y => y));
        }

        // Transition 1_4 (restoration of the `set` event) // Todo: `set` ?  or `set` and `release` ?
        //
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0).
        // This transition happends when `release` event of primary double action mouse button is given and 
        // there have not been any actions executed.
        // This transition has one side effect.
        // 1. The `set` and `release` events of primary double action mouse button will be restored.

        // Transition 1_5 (forced cancel)
        //
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0). 
        // This event happens when a `cancel` command given and functions given as the parameter of
        // `@before` and `@after` clauses of IfButtonGestureDefinition do not exist.
        // This transition has one side effect.
        // 1. The `set` and `release` events of primary double action mouse button will be restored.

        // Transition 1_6 (forced reset)
        //
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0). 
        // This event happens when a `reset` command given.
        // This transition has two side effects.
        // 1. Primary double action mouse button left holding is marked as irregularly holding by the user.
        // 2. Functions given as the parameter of `@after` clause of IfButtonGestureDefinition are executed.
        #endregion

        #region State2
        // Transition 2_0 (single action gesture established)
        //
        // State2 -> State2
        //
        // Transition from the state(S2) to the state(S2). 
        // This transition happends when `fire` event of single action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@do` clause of OnButtonWithIfButtonGestureDefinition are executed.

        // Transition 2_1 (gesture with primary and secondary double action mouse button start)
        //
        // State2 -> State3
        //
        // Transition from the state(S2) to the state(S2).
        // This transition happends when `set` event of double action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@before` clause of OnButtonWithIfButtonGestureDefinition are executed.

        // Transition 2_2 (stroke gesture established)
        //
        // State2 -> State0
        //
        // Transition from the state(S2) to the state(S0).
        // This transition happends when `release` event of primary double action mouse button is given
        // and a gesture stroke existing in OnButtonWithIfStrokeGestureDefinition is established.
        // This transition has two side effects.
        // 1. Functions given as the parameter of `@do` clause of StrokeGestureDefinition are executed.
        // 2. Functions given as the parameter of `@after` clause of IfButtonGestureDefinition are executed.

        // Transition 2_3 (normal end)
        // 
        // State2 -> State0
        //
        // Transition from the state(S2) to the state(S0).
        // This transition happens when `release` event of primary double action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@after` clause of IfButtonGestureDefinition are executed.

        // Transition 2_4 (forced reset)
        //
        // State2 -> State0
        //
        // Transition from the state(S2) to the state(S0). 
        // This event happens when a `reset` command given.
        // This transition has two side effects.
        // 1. Primary double action mouse button left holding is marked as irregularly holding by the user.
        // 2. Functions given as the parameter of `@after` clause of IfButtonGestureDefinition are executed.
        #endregion

        #region State3
        // Transition 3_0 (double action gesture established)
        //
        // State3 -> State2
        //
        // Transition from the state(S3) to the state(S2).
        // This transition happends when `release` event of secondary double action mouse button is given.
        // This transition has two side effects.
        // 1. Functions given as the parameter of `@do` clause of OnButtonWithIfButtonGestureDefinition are executed. 
        // 2. Functions given as the parameter of `@after` clause of OnButtonWithIfButtonGestureDefinition are executed.

        // Transition 3-1 (irregular end)
        //
        // State3 -> State0
        //
        // Transition from the state(S3) to the state(S0).
        // This transition happends when primary double action mouse button is released in irregular order. 
        // This transition has three side effects.
        // 1. Secondary double action mouse button left holding will be marked as irreggularly holding by the user.
        // 2. Functions given as the parameter of `@after` clause of IfButtonGestureDefinition are executed.
        // 3. Functions given as the parameter of `@after` clause of OnButtonWithIfButtonGestureDefinition are executed.

        // Transition 3-2 (forced reset)
        //
        // State3 -> State0
        //
        // Transition from the state(S3) to the state(S0).
        // This event happens when a `reset` command given.
        // This transition has three side effects.
        // 1. Primary and secondly double action mouse buttons left holding is marked as irregularly 
        // holding by the user.
        // 2. Functions given as the parameter of `@after` clause of IfButtonGestureDefinition are executed.
        // 3. Functions given as the parameter of `@after` clause of OnButtonWithIfButtonGestureDefinition are executed.
        #endregion

        #region Special side effects
        // Special side effects
        //
        // 1. Transition any state to the state S1 and S2 will reset the gesture stroke.
        // 2. Input given to the state S1 and S2 is intepreted as a gesture stroke.
        // 3. Each state will remove the mark of irregularly holding from double action mouse button when 
        //    `set` event of it to be given. 
        // 4. Each state will remove the mark of irregularly holding from double action mouse button and ignore it when 
        //    `release` event of it to be given.
        #endregion
        #endregion
    }

    public static class Def<T>
        where T : ActionContext
    {
        public delegate bool WhenFunc(T ctx);
        public delegate void BeforeFunc(T ctx);
        public delegate void DoFunc(T ctx);
        public delegate void AfterFunc(T ctx);
    }

    public static class Def
    {
        public interface Button { }
        public interface AcceptableInOnClause : Button { }
        public interface AcceptableInIfButtonClause : Button { }
        public interface AcceptableInIfSingleTriggerButtonClause : AcceptableInIfButtonClause { }
        public interface AcceptableInIfDoubleTriggerButtonClause : AcceptableInIfButtonClause { }

        public class LeftButton : AcceptableInOnClause, AcceptableInIfDoubleTriggerButtonClause { }
        public class MiddleButton : AcceptableInOnClause, AcceptableInIfDoubleTriggerButtonClause { }
        public class RightButton : AcceptableInOnClause, AcceptableInIfDoubleTriggerButtonClause { }
        public class WheelUp : AcceptableInIfSingleTriggerButtonClause { }
        public class WheelDown : AcceptableInIfSingleTriggerButtonClause { }
        public class WheelLeft : AcceptableInIfSingleTriggerButtonClause { }
        public class WheelRight : AcceptableInIfSingleTriggerButtonClause { }
        public class X1Button : AcceptableInOnClause, AcceptableInIfDoubleTriggerButtonClause { }
        public class X2Button : AcceptableInOnClause, AcceptableInIfDoubleTriggerButtonClause { }

        public interface Move { }
        public interface AcceptableInIfStrokeClause : Move { }
        public class MoveUp : AcceptableInIfStrokeClause { }
        public class MoveDown : AcceptableInIfStrokeClause { }
        public class MoveLeft : AcceptableInIfStrokeClause { }
        public class MoveRight : AcceptableInIfStrokeClause { }

        public class ConstantSingleton
        {
            private static ConstantSingleton singleton = new ConstantSingleton();

            public readonly LeftButton LeftButton = new LeftButton();
            public readonly MiddleButton MiddleButton = new MiddleButton();
            public readonly RightButton RightButton = new RightButton();
            public readonly WheelDown WheelDown = new WheelDown();
            public readonly WheelUp WheelUp = new WheelUp();
            public readonly WheelLeft WheelLeft = new WheelLeft();
            public readonly WheelRight WheelRight = new WheelRight();
            public readonly X1Button X1Button = new X1Button();
            public readonly X2Button X2Button = new X2Button();

            public readonly MoveUp MoveUp = new MoveUp();
            public readonly MoveDown MoveDown = new MoveDown();
            public readonly MoveLeft MoveLeft = new MoveLeft();
            public readonly MoveRight MoveRight = new MoveRight();

            public static ConstantSingleton GetInstance()
            {
                return singleton;
            }
        }

        public static ConstantSingleton Constant
        {
            get { return ConstantSingleton.GetInstance(); }
        }
    }

    public class Root<T>
        where T : ActionContext
    {
        public readonly List<WhenElement<T>.Value> whenElements = new List<WhenElement<T>.Value>();
        
        public WhenElement<T> @when(Def<T>.WhenFunc func)
        {
            return new WhenElement<T>(whenElements, func);
        }
    }

    public class WhenElement<T>
        where T : ActionContext
    {
        public class Value
        {
            public readonly List<IfSingleTriggerButtonElement<T>.Value> ifSingleTriggerButtonElements = new List<IfSingleTriggerButtonElement<T>.Value>();
            public readonly List<IfDoubleTriggerButtonElement<T>.Value> ifDoubleTriggerButtonElements = new List<IfDoubleTriggerButtonElement<T>.Value>();
            public readonly List<OnElement<T>.Value> onElements = new List<OnElement<T>.Value>();
            public readonly Def<T>.WhenFunc func;

            public Value(Def<T>.WhenFunc func)
            {
                this.func = func;
            }
        }

        private readonly Value value;

        public WhenElement(List<Value> parent, Def<T>.WhenFunc func)
        {
            this.value = new Value(func);
            parent.Add(this.value);
        }

        public OnElement<T> @on(Def.AcceptableInOnClause button)
        {
            return new OnElement<T>(value.onElements, button);
        }

        public IfSingleTriggerButtonElement<T> @if(Def.AcceptableInIfSingleTriggerButtonClause button)
        {
            return new IfSingleTriggerButtonElement<T>(value.ifSingleTriggerButtonElements, button);
        }

        public IfDoubleTriggerButtonElement<T> @if(Def.AcceptableInIfDoubleTriggerButtonClause button)
        {
            return new IfDoubleTriggerButtonElement<T>(value.ifDoubleTriggerButtonElements, button);
        }
    }

    public class DoubleTriggerAfterElement<T>
        where T : ActionContext
    {
        public class Value
        {
            public readonly Def<T>.AfterFunc func;

            public Value(Def<T>.AfterFunc func)
            {
                this.func = func;
            }
        }

        public DoubleTriggerAfterElement(List<Value> parent, Def<T>.AfterFunc func)
        {
            parent.Add(new Value(func));
        }
    }

    public class DoubleTriggerBeforeElement<T>
        where T : ActionContext
    {
        public class Value
        {
            public readonly Def<T>.BeforeFunc func;

            public Value(Def<T>.BeforeFunc func)
            {
                this.func = func;
            }
        }

        private readonly List<DoubleTriggerDoElement<T>.Value> doParent;
        private readonly List<DoubleTriggerAfterElement<T>.Value> afterParent;

        public DoubleTriggerBeforeElement(
            List<Value> parentA,
            List<DoubleTriggerDoElement<T>.Value> parentB,
            List<DoubleTriggerAfterElement<T>.Value> parentC,
            Def<T>.BeforeFunc func)
        {
            parentA.Add(new Value(func));
            doParent = parentB;
            afterParent = parentC;
        }

        public DoubleTriggerDoElement<T> @do(Def<T>.DoFunc func)
        {
            return new DoubleTriggerDoElement<T>(doParent, afterParent, func);
        }

        public DoubleTriggerAfterElement<T> @after(Def<T>.AfterFunc func)
        {
            return new DoubleTriggerAfterElement<T>(afterParent, func);
        }
    }

    public class DoubleTriggerDoElement<T>
        where T : ActionContext
    {
        public class Value
        {
            public readonly Def<T>.DoFunc func;

            public Value(Def<T>.DoFunc func)
            {
                this.func = func;
            }
        }

        private readonly List<DoubleTriggerAfterElement<T>.Value> afterParent;

        public DoubleTriggerDoElement(
            List<Value> parentA,
            List<DoubleTriggerAfterElement<T>.Value> parentB,
            Def<T>.DoFunc func)
        {
            parentA.Add(new Value(func));
            afterParent = parentB;
        }

        public DoubleTriggerAfterElement<T> @after(Def<T>.AfterFunc func)
        {
            return new DoubleTriggerAfterElement<T>(afterParent, func);
        }
    }


    public class IfDoubleTriggerButtonElement<T>
        where T : ActionContext
    {
        public class Value
        {
            public readonly List<DoubleTriggerBeforeElement<T>.Value> beforeElements = new List<DoubleTriggerBeforeElement<T>.Value>();
            public readonly List<DoubleTriggerDoElement<T>.Value> doElements = new List<DoubleTriggerDoElement<T>.Value>();
            public readonly List<DoubleTriggerAfterElement<T>.Value> afterElements = new List<DoubleTriggerAfterElement<T>.Value>();

            public readonly Def.AcceptableInIfDoubleTriggerButtonClause button;

            public Value(Def.AcceptableInIfDoubleTriggerButtonClause button)
            {
                this.button = button;
            }
        }

        private readonly Value value;

        public IfDoubleTriggerButtonElement(List<Value> parent, Def.AcceptableInIfDoubleTriggerButtonClause button)
        {
            this.value = new Value(button);
            parent.Add(this.value);
        }

        public DoubleTriggerBeforeElement<T> @before(Def<T>.BeforeFunc func)
        {
            return new DoubleTriggerBeforeElement<T>(value.beforeElements, value.doElements, value.afterElements, func);
        }

        public DoubleTriggerDoElement<T> @do(Def<T>.DoFunc func)
        {
            return new DoubleTriggerDoElement<T>(value.doElements, value.afterElements, func);
        }

        public DoubleTriggerAfterElement<T> @after(Def<T>.AfterFunc func)
        {
            return new DoubleTriggerAfterElement<T>(value.afterElements, func);
        }
    }


    public class IfSingleTriggerButtonElement<T>
        where T : ActionContext
    {
        public class Value
        {
            public readonly List<SingleTriggerDoElement<T>.Value> doElements = new List<SingleTriggerDoElement<T>.Value>();

            public readonly Def.AcceptableInIfSingleTriggerButtonClause button;

            public Value(Def.AcceptableInIfSingleTriggerButtonClause button)
            {
                this.button = button;
            }
        }

        private readonly Value value;

        public IfSingleTriggerButtonElement(List<Value> parent, Def.AcceptableInIfSingleTriggerButtonClause button)
        {
            this.value = new Value(button);
            parent.Add(this.value);
        }

        public SingleTriggerDoElement<T> @do(Def<T>.DoFunc func)
        {
            return new SingleTriggerDoElement<T>(value.doElements, func);
        }
    }

    public class IfStrokeElement<T>
        where T : ActionContext
    {
        public class Value
        {
            public readonly List<SingleTriggerDoElement<T>.Value> doElements = new List<SingleTriggerDoElement<T>.Value>();

            public readonly IEnumerable<Def.AcceptableInIfStrokeClause> moves;

            public Value(IEnumerable<Def.AcceptableInIfStrokeClause> moves)
            {
                this.moves = moves;
            }
        }

        private readonly Value value;

        public IfStrokeElement(List<Value> parent, params Def.AcceptableInIfStrokeClause[] moves)
        {
            this.value = new Value(moves);
            parent.Add(this.value);
        }

        public SingleTriggerDoElement<T> @do(Def<T>.DoFunc func)
        {
            return new SingleTriggerDoElement<T>(value.doElements, func);
        }
    }

    public class OnElement<T>
        where T : ActionContext
    {
        public class Value
        {
            public readonly List<IfSingleTriggerButtonElement<T>.Value> ifSingleTriggerButtonElements = new List<IfSingleTriggerButtonElement<T>.Value>();
            public readonly List<IfDoubleTriggerButtonElement<T>.Value> ifDoubleTriggerButtonElements = new List<IfDoubleTriggerButtonElement<T>.Value>();
            public readonly List<IfStrokeElement<T>.Value> ifStrokeElements = new List<IfStrokeElement<T>.Value>();
            public readonly Def.AcceptableInOnClause button;

            public Value(Def.AcceptableInOnClause button)
            {
                this.button = button;
            }
        }

        private readonly Value value;

        public OnElement(List<Value> parent, Def.AcceptableInOnClause button)
        {
            this.value = new Value(button);
            parent.Add(this.value);
        }

        public IfSingleTriggerButtonElement<T> @if(Def.AcceptableInIfSingleTriggerButtonClause button)
        {
            return new IfSingleTriggerButtonElement<T>(value.ifSingleTriggerButtonElements, button);
        }

        public IfDoubleTriggerButtonElement<T> @if(Def.AcceptableInIfDoubleTriggerButtonClause button)
        {
            return new IfDoubleTriggerButtonElement<T>(value.ifDoubleTriggerButtonElements, button);
        }

        public IfStrokeElement<T> @if(params Def.AcceptableInIfStrokeClause[] moves)
        {
            return new IfStrokeElement<T>(value.ifStrokeElements, moves);
        }
    }

    public class SingleTriggerDoElement<T>
        where T : ActionContext
    {
        public class Value
        {
            public readonly Def<T>.DoFunc func;

            public Value(Def<T>.DoFunc func)
            {
                this.func = func;
            }
        }

        public SingleTriggerDoElement(List<Value> parent, Def<T>.DoFunc func)
        {
            parent.Add(new Value(func));
        }
    }


    public static class DSLTreeParser<T>
        where T : ActionContext
    {
        public static IEnumerable<GestureDefinition<T>> TreeToGestureDefinitions(Root<T> root)
        {
            using (Verbose.PrintElapsed("Parse GestureConfigTree"))
            {
                return Parse(root).ToList();
            }
        }

        internal static IEnumerable<GestureDefinition<T>> Parse(
            Root<T> root)
        {
            foreach (var elm in root.whenElements)
            {
                foreach (var def in Parse(elm)) { yield return def; }
            }
        }

        internal static IEnumerable<GestureDefinition<T>> Parse(
            WhenElement<T>.Value whenElement)
        {
            if (whenElement.onElements.Count == 0 &&
                whenElement.ifSingleTriggerButtonElements.Count == 0 &&
                whenElement.ifDoubleTriggerButtonElements.Count == 0)
            {
                yield return new GestureDefinition<T>(whenElement.func);
            }
            else
            {
                foreach (var elm in whenElement.onElements)
                {
                    foreach (var def in Parse(whenElement, elm)) { yield return def; }
                }
                foreach (var elm in whenElement.ifSingleTriggerButtonElements)
                {
                    foreach (var def in Parse(whenElement, elm)) { yield return def; }
                }
                foreach (var elm in whenElement.ifDoubleTriggerButtonElements)
                {
                    foreach (var def in Parse(whenElement, elm)) { yield return def; }
                }
            }
        }

        internal static IEnumerable<GestureDefinition<T>> Parse(
            WhenElement<T>.Value whenElement,
            IfSingleTriggerButtonElement<T>.Value ifSingleTriggerButtonElement)
        {
            if (ifSingleTriggerButtonElement.doElements.Count == 0)
            {
                yield return new IfButtonGestureDefinition<T>(whenElement.func, ifSingleTriggerButtonElement.button, null, null, null);
            }
            else
            {
                foreach (var doElement in ifSingleTriggerButtonElement.doElements)
                {
                    yield return new IfButtonGestureDefinition<T>(whenElement.func, ifSingleTriggerButtonElement.button, null, doElement.func, null);
                }
            }
        }

        internal static IEnumerable<GestureDefinition<T>> Parse(
             WhenElement<T>.Value whenElement,
             IfDoubleTriggerButtonElement<T>.Value ifDoubleTriggerButtonElement)
        {
            if (ifDoubleTriggerButtonElement.beforeElements.Count == 0 &&
                ifDoubleTriggerButtonElement.doElements.Count == 0 &&
                ifDoubleTriggerButtonElement.afterElements.Count == 0
                )
            {
                yield return new IfButtonGestureDefinition<T>(whenElement.func, ifDoubleTriggerButtonElement.button, null, null, null);
            }
            else
            {
                foreach (var elm in ifDoubleTriggerButtonElement.beforeElements)
                {
                    yield return new IfButtonGestureDefinition<T>(whenElement.func, ifDoubleTriggerButtonElement.button, elm.func, null, null);
                }
                foreach (var elm in ifDoubleTriggerButtonElement.doElements)
                {
                    yield return new IfButtonGestureDefinition<T>(whenElement.func, ifDoubleTriggerButtonElement.button, null, elm.func, null);
                }
                foreach (var elm in ifDoubleTriggerButtonElement.afterElements)
                {
                    yield return new IfButtonGestureDefinition<T>(whenElement.func, ifDoubleTriggerButtonElement.button, null, null, elm.func);
                }
            }
        }

        internal static IEnumerable<GestureDefinition<T>> Parse(
             WhenElement<T>.Value whenElement,
             OnElement<T>.Value onElement)
        {
            if (onElement.ifSingleTriggerButtonElements.Count == 0 &&
                onElement.ifDoubleTriggerButtonElements.Count == 0 &&
                onElement.ifStrokeElements.Count == 0)
            {
                yield return new OnButtonGestureDefinition<T>(whenElement.func, onElement.button);
            }
            else
            {
                foreach (var elm in onElement.ifSingleTriggerButtonElements)
                {
                    foreach (var def in Parse(whenElement, onElement, elm)) { yield return def; }
                }
                foreach (var elm in onElement.ifDoubleTriggerButtonElements)
                {
                    foreach (var def in Parse(whenElement, onElement, elm)) { yield return def; }
                }
                foreach (var elm in onElement.ifStrokeElements)
                {
                    foreach (var def in Parse(whenElement, onElement, elm)) { yield return def; }
                }
            }
        }

        internal static IEnumerable<GestureDefinition<T>> Parse(
            WhenElement<T>.Value whenElement,
            OnElement<T>.Value onElement,
            IfSingleTriggerButtonElement<T>.Value ifSingleTriggerButtonElement)
        {
            if (ifSingleTriggerButtonElement.doElements.Count == 0)
            {
                yield return new OnButtonWithIfButtonGestureDefinition<T>(whenElement.func, onElement.button, ifSingleTriggerButtonElement.button, null, null, null);
            }
            else
            {
                foreach (var doElement in ifSingleTriggerButtonElement.doElements)
                {
                    yield return new OnButtonWithIfButtonGestureDefinition<T>(whenElement.func, onElement.button, ifSingleTriggerButtonElement.button, null, doElement.func, null);
                }
            }
        }

        internal static IEnumerable<GestureDefinition<T>> Parse(
             WhenElement<T>.Value whenElement,
             OnElement<T>.Value onElement,
             IfDoubleTriggerButtonElement<T>.Value ifDoubleTriggerButtonElement)
        {
            if (ifDoubleTriggerButtonElement.beforeElements.Count == 0 &&
                ifDoubleTriggerButtonElement.doElements.Count == 0 &&
                ifDoubleTriggerButtonElement.afterElements.Count == 0
                )
            {
                yield return new OnButtonWithIfButtonGestureDefinition<T>(whenElement.func, onElement.button, ifDoubleTriggerButtonElement.button, null, null, null);
            }
            else
            {
                foreach (var elm in ifDoubleTriggerButtonElement.beforeElements)
                {
                    yield return new OnButtonWithIfButtonGestureDefinition<T>(whenElement.func, onElement.button, ifDoubleTriggerButtonElement.button, elm.func, null, null);
                }
                foreach (var elm in ifDoubleTriggerButtonElement.doElements)
                {
                    yield return new OnButtonWithIfButtonGestureDefinition<T>(whenElement.func, onElement.button, ifDoubleTriggerButtonElement.button, null, elm.func, null);
                }
                foreach (var elm in ifDoubleTriggerButtonElement.afterElements)
                {
                    yield return new OnButtonWithIfButtonGestureDefinition<T>(whenElement.func, onElement.button, ifDoubleTriggerButtonElement.button, null, null, elm.func);
                }
            }
        }

        internal static IEnumerable<GestureDefinition<T>> Parse(
            WhenElement<T>.Value whenElement,
            OnElement<T>.Value onElement,
            IfStrokeElement<T>.Value ifStrokeElement)
        {
            var stroke = Dev.Helper.Convert(ifStrokeElement.moves);
            if (ifStrokeElement.doElements.Count == 0)
            {
                yield return new OnButtonWithIfStrokeGestureDefinition<T>(whenElement.func, onElement.button, stroke, null);
            }
            else
            {
                foreach (var doElement in ifStrokeElement.doElements)
                {
                    yield return new OnButtonWithIfStrokeGestureDefinition<T>(whenElement.func, onElement.button, stroke, doElement.func);
                }
            }
        }

        public static IEnumerable<GestureDefinition<T>> FilterComplete(IEnumerable<GestureDefinition<T>> gestureDef)
        {
            return gestureDef
                .Where(x => x.IsComplete)
                .ToList();
        }
    }

    public class GestureDefinition<T> : IWhenEvaluatable<T>
        where T : ActionContext
    {
        public readonly Def<T>.WhenFunc whenFunc;

        public GestureDefinition(
            Def<T>.WhenFunc whenFunc
            )
        {
            this.whenFunc = whenFunc;
        }

        virtual public bool IsComplete //HasAction
        {
            get { return false; }
        }

        public bool EvaluateWhenFunc(T ctx)
        {
            return whenFunc(ctx);
        }

        public bool EvaluateWhenFunc(T ctx, Dictionary<Def<T>.WhenFunc, bool> cache)
        {
            if (!cache.Keys.Contains(whenFunc))
            {
                cache[whenFunc] = EvaluateWhenFunc(ctx);
            }
            return cache[whenFunc];
        }
    }

    public interface IWhenEvaluatable<T>
        where T : ActionContext
    {
        bool EvaluateWhenFunc(T ctx);
        bool EvaluateWhenFunc(T ctx, Dictionary<Def<T>.WhenFunc, bool> cache);
    }

    public interface IBeforeExecutable<T>
        where T : ActionContext
    {
        void ExecuteBeforeFunc(T ctx);
    }

    public interface IDoExecutable<T>
        where T : ActionContext
    {
        void ExecuteDoFunc(T ctx);
    }

    public interface IAfterExecutable<T>
        where T : ActionContext
    {
        void ExecuteAfterFunc(T ctx);
    }


    public class IfButtonGestureDefinition<T>
        : GestureDefinition<T>, IBeforeExecutable<T>, IDoExecutable<T>, IAfterExecutable<T>
        where T : ActionContext
    {
        public readonly Def.AcceptableInIfButtonClause ifButton;
        public readonly Def<T>.BeforeFunc beforeFunc;
        public readonly Def<T>.DoFunc doFunc;
        public readonly Def<T>.AfterFunc afterFunc;

        public IfButtonGestureDefinition(
            Def<T>.WhenFunc whenFunc,
            Def.AcceptableInIfButtonClause ifButton,
            Def<T>.BeforeFunc beforeFunc,
            Def<T>.DoFunc doFunc,
            Def<T>.AfterFunc afterFunc
            ) : base(whenFunc)
        {
            this.ifButton = ifButton;
            this.beforeFunc = beforeFunc;
            this.doFunc = doFunc;
            this.afterFunc = afterFunc;
        }

        override public bool IsComplete
        {
            get
            {
                return whenFunc != null &&
                       ifButton != null &&
                       (beforeFunc != null || doFunc != null || afterFunc != null);
            }
        }

        public void ExecuteBeforeFunc(T ctx)
        {
            beforeFunc(ctx);
        }

        public void ExecuteDoFunc(T ctx)
        {
            doFunc(ctx);
        }

        public void ExecuteAfterFunc(T ctx)
        {
            afterFunc(ctx);
        }
    }

    public class OnButtonGestureDefinition<T>
        : GestureDefinition<T>
        where T : ActionContext
    {
        public readonly Def.AcceptableInOnClause onButton;

        public OnButtonGestureDefinition(
            Def<T>.WhenFunc whenFunc,
            Def.AcceptableInOnClause onButton
            ) : base(whenFunc)
        {
            this.onButton = onButton;
        }
    }

    public class OnButtonWithIfButtonGestureDefinition<T>
        : OnButtonGestureDefinition<T>, IBeforeExecutable<T>, IDoExecutable<T>, IAfterExecutable<T>
        where T : ActionContext
    {
        public readonly Def.AcceptableInIfButtonClause ifButton;
        public readonly Def<T>.BeforeFunc beforeFunc;
        public readonly Def<T>.DoFunc doFunc;
        public readonly Def<T>.AfterFunc afterFunc;

        public OnButtonWithIfButtonGestureDefinition(
            Def<T>.WhenFunc whenFunc,
            Def.AcceptableInOnClause onButton,
            Def.AcceptableInIfButtonClause ifButton,
            Def<T>.BeforeFunc beforeFunc,
            Def<T>.DoFunc doFunc,
            Def<T>.AfterFunc afterFunc
            ) : base(whenFunc, onButton)
        {
            this.ifButton = ifButton;
            this.beforeFunc = beforeFunc;
            this.doFunc = doFunc;
            this.afterFunc = afterFunc;
        }

        override public bool IsComplete
        {
            get
            {
                return whenFunc != null &&
                       onButton != null &&
                       ifButton != null &&
                       (beforeFunc != null || doFunc != null || afterFunc != null);
            }
        }

        public void ExecuteBeforeFunc(T ctx)
        {
            beforeFunc(ctx);
        }

        public void ExecuteDoFunc(T ctx)
        {
            doFunc(ctx);
        }

        public void ExecuteAfterFunc(T ctx)
        {
            afterFunc(ctx);
        }
    }

    public class OnButtonWithIfStrokeGestureDefinition<T>
        : OnButtonGestureDefinition<T>, IDoExecutable<T>
        where T : ActionContext
    {
        public readonly Core.Def.Stroke stroke;
        public readonly Def<T>.DoFunc doFunc;

        public OnButtonWithIfStrokeGestureDefinition(
            Def<T>.WhenFunc whenFunc,
            Def.AcceptableInOnClause onButton,
            Core.Def.Stroke stroke,
            Def<T>.DoFunc doFunc
            ) : base(whenFunc, onButton)
        {
            this.stroke = stroke;
            this.doFunc = doFunc;
        }

        override public bool IsComplete
        {
            get
            {
                return whenFunc != null &&
                       onButton != null &&
                       stroke != null &&
                       doFunc != null;
            }
        }

        public void ExecuteDoFunc(T ctx)
        {
            doFunc(ctx);
        }
    }

    public static class Helper
    {
        private static Core.Def.Direction Convert(Def.AcceptableInIfStrokeClause move)
        {
            if (move is Def.MoveUp)
            {
                return Core.Def.Direction.Up;
            }
            else if (move is Def.MoveDown)
            {
                return Core.Def.Direction.Down;
            }
            else if (move is Def.MoveLeft)
            {
                return Core.Def.Direction.Left;
            }
            else if (move is Def.MoveRight)
            {
                return Core.Def.Direction.Right;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static Core.Def.Stroke Convert(IEnumerable<Def.AcceptableInIfStrokeClause> moves)
        {
            return new Core.Def.Stroke(moves.Select(m => Convert(m)));
        }

        public static Core.Def.Event.IDoubleActionSet Convert(Def.AcceptableInOnClause onButton)
        {
            if (onButton is Def.LeftButton)
            {
                return Core.Def.Constant.LeftButtonDown;
            }
            else if (onButton is Def.MiddleButton)
            {
                return Core.Def.Constant.MiddleButtonDown;
            }
            else if (onButton is Def.RightButton)
            {
                return Core.Def.Constant.RightButtonDown;
            }
            else if (onButton is Def.X1Button)
            {
                return Core.Def.Constant.X1ButtonDown;
            }
            else if (onButton is Def.X2Button)
            {
                return Core.Def.Constant.X2ButtonDown;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static Core.Def.Event.IEvent Convert(Def.AcceptableInIfButtonClause ifButton)
        {
            if (ifButton is Def.LeftButton)
            {
                return Core.Def.Constant.LeftButtonDown;
            }
            else if (ifButton is Def.MiddleButton)
            {
                return Core.Def.Constant.MiddleButtonDown;
            }
            else if (ifButton is Def.RightButton)
            {
                return Core.Def.Constant.RightButtonDown;
            }
            else if (ifButton is Def.WheelUp)
            {
                return Core.Def.Constant.WheelUp;
            }
            else if (ifButton is Def.WheelDown)
            {
                return Core.Def.Constant.WheelDown;
            }
            else if (ifButton is Def.WheelLeft)
            {
                return Core.Def.Constant.WheelLeft;
            }
            else if (ifButton is Def.WheelRight)
            {
                return Core.Def.Constant.WheelRight;
            }
            else if (ifButton is Def.X1Button)
            {
                return Core.Def.Constant.X1ButtonDown;
            }
            else if (ifButton is Def.X2Button)
            {
                return Core.Def.Constant.X2ButtonDown;
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}

// TOdo パッケージ名はいいんだけど、ディレクトリはCreviceLib? nugetでわかりやすければそれでいい

namespace Crevice.Core
{
    public static class TestClass
    {
        public static void Test()
        {
            var context = new ActionContextFactory<DefaultActionContext>();

            var config = new DefaultGestureMachineConfig();
            var gestureDef = new List<GestureDefinition>();

            var gestureMachine = new FSM.GestureMachine<DefaultGestureMachineConfig, DefaultActionContext>(config, gestureDef);
            var global = gestureMachine.Global;



            var nullGestureMachine0 = FSM.NullGestureMachine.Create();
            var nullGestureMachine1 = FSM.NullGestureMachine.Create(new DefaultGestureMachineConfig());

        }
    }

    // Configはやはり継承する場合があるのでジェネリクスを使わざるをえない
    // 各コンフィグの分のパラメーターが必要、かな


    // OnGestureStart()

    // OnGestureEnd (  )NeedToRestore~ をStateに生やす？ 

    // OnGestureCanelled

    // OnGestureTimeout

    // RestorePrimaryButtonDownEventHandler 

    // OnStrokeUudate


    // StrokeUpdateEventHandler

    // Configはデフォルトで見せる？見せない？とかそういうの


    // OnGestureTimeoutRequest(State1 S1) これは成否で2分岐する

    // OnGestureCancel() これは分岐なし // OnGestureEndでCancelかどうかを知れたほうがいいかな

    // これをGestureMachineにしてFSMのほうは単なるFSMに？
    public abstract class GestureMachineConfig
    {
        /*
        public static Func<System.Drawing.Point, System.Drawing.Point> DefaultPositionBinding
        {
            get
            {
                return (point) =>
                {
                    return point;
                };
            }
        }
        */

        //Todo: null check in bindings
        //public Def.PointBinding TooltipPositionBinding = Def.DefaultPointBinding;
        public event EventHandler OnMachineStart;
        public event EventHandler OnMachineReset;
        public event EventHandler OnMachineEnd;

        public event EventHandler OnGestureStart;
        public event EventHandler OnGestureCancel;
        public event System.ComponentModel.CancelEventHandler OnGestureTimeout;
        public event EventHandler OnGestureEnd;

        public int GestureTimeout = 1000;

        public event EventHandler OnStrokeUpdate;

        public int StrokeStartThreshold = 10;
        public int StrokeDirectionChangeThreshold = 20;
        public int StrokeExtensionThreshold = 10;
        public int StrokeWatchInterval = 10;
    }

    public class DefaultGestureMachineConfig : GestureMachineConfig
    { }

    namespace Config
    {
        // Defは悪い文化

        public static class Def
        {
            //public delegate System.Drawing.Point PositionBinding(System.Drawing.Point point);

                /*
            public static PositionBinding DefaultPositionBinding
            {
                get
                {
                    return (point) => 
                    {
                        return point;
                    };
                }
            }
            */

            /*
            public delegate bool GestureTimeoutBinding(FSM.State1 S1);

            
            public static GestureTimeoutBinding DefaultGestureTimeoutBinding
            {
                get
                {
                    // Todo test
                    return (S1) => 
                    {
                        S1.IgnoreNext(S1.primaryEvent.GetPair());
                        return S1.S0;
                    };
                }
            }
            
            public delegate FSM.State GestureCancelBinding(FSM.State1 S1);

            public static GestureCancelBinding DefaultGestureCancelBinding
            {
                get
                {
                    // Todo test
                    return (S1) =>
                    {
                        return S1.S0;
                    };
                }
            }
            */
        }

        /*
        public class GestureConfig
        {
            public int InitialStrokeThreshold = 10;
            public int StrokeDirectionChangeThreshold = 20;
            public int StrokeExtensionThreshold = 10;
            public int WatchInterval = 10;
            public int Timeout = 1000;


        }

        public class UserInterfaceConfig
        {
            public Def.PositionBinding TooltipPositionBinding = Def.DefaultPositionBinding;
            public int TooltipTimeout = 3000;
            public int BalloonTimeout = 10000;
        }
        */


        /*

        public class UserConfig
        {
            public SystemConfig System = new SystemConfig();
            public GestureConfig Gesture = new GestureConfig();
            public UserInterfaceConfig UI = new UserInterfaceConfig();
        }
        */
    }
}
