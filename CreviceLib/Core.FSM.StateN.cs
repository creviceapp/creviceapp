using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.FSM
{
    using System.Linq;
    using Crevice.Core.Types;
    using Crevice.Core.Context;
    using Crevice.Core.DSL;
    using Crevice.Core.Stroke;

    public class StateN<TConfig, TContextManager, TEvalContext, TExecContext> : State
        where TConfig : GestureMachineConfig
        where TContextManager : ContextManager<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public readonly GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> Machine;
        public readonly TEvalContext Ctx;
        public readonly IReadOnlyList<(IReleaseEvent, IState)> History;
        public readonly IReadOnlyList<DoubleThrowElement<TExecContext>> DoubleThrowElements;
        public readonly bool CanCancel;

        public StateN(
            GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> machine,
            TEvalContext ctx,
            IReadOnlyList<(IReleaseEvent, IState)> history,
            IReadOnlyList<DoubleThrowElement<TExecContext>> doubleThrowElements,
            bool canCancel = true)
        {
            Machine = machine;
            Ctx = ctx;
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
                    Machine.ContextManager.ExecuteDoExecutors(Ctx, singleThrowElements);
                    var notCancellableCopyState = new StateN<TConfig, TContextManager, TEvalContext, TExecContext>(
                        Machine,
                        Ctx,
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
                    Machine.ContextManager.ExecutePressExecutors(Ctx, doubleThrowElements);
                    var nextState = new StateN<TConfig, TContextManager, TEvalContext, TExecContext>(
                        Machine,
                        Ctx,
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
                        Machine.ContextManager.ExecuteDoExecutors(Ctx, strokeElements);
                        Machine.ContextManager.ExecuteReleaseExecutors(Ctx, DoubleThrowElements);
                    }
                    else if (HasPressExecutors || HasDoExecutors || HasReleaseExecutors)
                    {
                        //normal end
                        Machine.ContextManager.ExecuteDoExecutors(Ctx, DoubleThrowElements);
                        Machine.ContextManager.ExecuteReleaseExecutors(Ctx, DoubleThrowElements);
                    }
                    else if (/* !HasPressExecutors && !HasDoExecutors && !HasReleaseExecutors && */
                             CanCancel)
                    {
                        Machine.OnGestureCancelled();
                        //何のインスタンスが来るかによって対応を変える必要がある
                        //例えばゲームパッドであれば何もする必要がない

                        // ボタンであればクリックの復元を
                    }
                    return (EventIsConsumed: true, NextState: LastState);
                }
                else if (AbnormalEndTriggers.Contains(releaseEvent))
                {
                    var (oldState, skippedReleaseEvents) = FindStateFromHistory(releaseEvent);
                    Machine.invalidReleaseEvents.IgnoreNext(skippedReleaseEvents);
                    return (EventIsConsumed: true, NextState: oldState);
                }
            }
            return base.Input(evnt);
        }

        public override IState Timeout()
        {
            if (!HasPressExecutors && !HasDoExecutors && !HasReleaseExecutors && CanCancel)
            {
                return LastState;
            }
            return this;
        }

        public override IState Reset()
        {
            Machine.invalidReleaseEvents.IgnoreNext(NormalEndTrigger);
            Machine.ContextManager.ExecuteReleaseExecutors(Ctx, DoubleThrowElements);
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

        // このあたりを扱いやすい型に変換してユーザーサイドで取れるように

        public IReadOnlyList<DoubleThrowElement<TExecContext>> GetDoubleThrowElements(IPressEvent triggerEvent)
            => (from d in DoubleThrowElements
                where d.IsFull
                select (
                    from dd in d.DoubleThrowElements
                    where dd.IsFull && (dd.Trigger.Equals(triggerEvent) ||
                                        dd.Trigger.Equals(triggerEvent.LogicalNormalized))
                    select dd))
                .Aggregate(new List<DoubleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        public IReadOnlyList<StrokeElement<TExecContext>> GetStrokeElements(IReadOnlyList<StrokeDirection> strokes)
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
                    where s.IsFull && (s.Trigger.Equals(triggerEvent) ||
                                       s.Trigger.Equals(triggerEvent.LogicalNormalized))
                    select s))
                .Aggregate(new List<SingleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });
    }
}
