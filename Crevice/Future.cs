using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.Future
{
    using System.Drawing;

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



    public delegate bool ActionEvaluator<in T>(T ctx);
    public delegate void ActionExecutor<in T>(T ctx);



    /* Root
     * 
     * .When() -> new WhenElement
     */
    public class Root<T>
        where T : ActionContext
    {
        private List<WhenElement<T>> whenElements = new List<WhenElement<T>>();
        public IReadOnlyCollection<WhenElement<T>> WhenElements
        {
            get { return whenElements.ToList(); }
        }

        public WhenElement<T> When(ActionEvaluator<T> evaluator)
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
     * .On(FireEvent) -> new OnFireElement
     * 
     * .On(PressEvent) -> new OnPressElement
     */
    public class WhenElement<T>
        where T : ActionContext
    {
        public ActionEvaluator<T> WhenEvaluator { get; private set; }

        private List<OnFireElement<T>> onFireElements = new List<OnFireElement<T>>();
        public IReadOnlyCollection<OnFireElement<T>> OnFireElements
        {
            get { return onFireElements.ToList(); }
        }

        private List<OnPressElement<T>> onPressElements = new List<OnPressElement<T>>();
        public IReadOnlyCollection<OnPressElement<T>> OnPressElements
        {
            get { return onPressElements.ToList(); }
        }

        public WhenElement(ActionEvaluator<T> evaluator)
        {
            WhenEvaluator = evaluator;
        }

        public OnFireElement<T> On(IFireEvent triggerEvent)
        {
            var elm = new OnFireElement<T>(triggerEvent);
            onFireElements.Add(elm);
            return elm;
        }

        public OnPressElement<T> On(IPressEvent triggerEvent)
        {
            var elm = new OnPressElement<T>(triggerEvent);
            onPressElements.Add(elm);
            return elm;
        }
    }

    /* OnFireElement
     * 
     * .Do() -> this OnFireElement
     */
    public class OnFireElement<T>
        where T : ActionContext
    {
        public IFireEvent Trigger { get; private set; }
        
        private List<ActionExecutor<T>> doExecutors = new List<ActionExecutor<T>>();
        public IReadOnlyCollection<ActionExecutor<T>> DoExecutors
        {
            get { return doExecutors.ToList(); }
        }

        public OnFireElement(IFireEvent triggerEvent)
        {
            Trigger = triggerEvent;
        }

        public OnFireElement<T> Do(ActionExecutor<T> executor)
        {
            doExecutors.Add(executor);
            return this;
        }
    }

    /* OnPressElement
     * 
     * .DoBefore() -> this OnPressElement
     * 
     * .Do() -> this OnPressElement
     * 
     * .DoAfter() -> this OnPressElement
     * 
     * .On(FireEvent) -> new OnFireEvent
     * 
     * .On(PressEvent) -> new OnPressEvent
     * 
     * .On(StrokeEvent) -> new StrokeEvent
     */
    public class OnPressElement<T>
        where T : ActionContext
    {
        public IPressEvent Trigger { get; private set; }

        private List<OnFireElement<T>> onFireElements = new List<OnFireElement<T>>();
        public IReadOnlyCollection<OnFireElement<T>> OnFireElements
        {
            get { return onFireElements.ToList(); }
        }

        private List<OnPressElement<T>> onPressElements = new List<OnPressElement<T>>();
        public IReadOnlyCollection<OnPressElement<T>> OnPressElements
        {
            get { return onPressElements.ToList(); }
        }

        private List<OnStrokeElement<T>> onStrokeElements = new List<OnStrokeElement<T>>();
        public IReadOnlyCollection<OnStrokeElement<T>> OnStrokeElements
        {
            get { return onStrokeElements.ToList(); }
        }

        private List<ActionExecutor<T>> doBeforeExecutors = new List<ActionExecutor<T>>();
        public IReadOnlyCollection<ActionExecutor<T>> DoBeforeExecutors
        {
            get { return doBeforeExecutors.ToList(); }
        }

        private List<ActionExecutor<T>> doExecutors = new List<ActionExecutor<T>>();
        public IReadOnlyCollection<ActionExecutor<T>> DoExecutors
        {
            get { return doExecutors.ToList(); }
        }

        private List<ActionExecutor<T>> doAfterExecutors = new List<ActionExecutor<T>>();
        public IReadOnlyCollection<ActionExecutor<T>> DoAfterExecutors
        {
            get { return doAfterExecutors.ToList(); }
        }

        public OnPressElement(IPressEvent triggerEvent)
        {
            Trigger = triggerEvent;
        }

        public OnFireElement<T> On(IFireEvent triggerEvent)
        {
            var elm = new OnFireElement<T>(triggerEvent);
            onFireElements.Add(elm);
            return elm;
        }

        public OnPressElement<T> On(IPressEvent triggerEvent)
        {
            var elm = new OnPressElement<T>(triggerEvent);
            onPressElements.Add(elm);
            return elm;
        }

        public OnStrokeElement<T> On(params StrokeEvent.Direction[] strokeDirections)
        {
            var elm = new OnStrokeElement<T>(strokeDirections);
            onStrokeElements.Add(elm);
            return elm;
        }

        public OnPressElement<T> DoBefore(ActionExecutor<T> executor)
        {
            doBeforeExecutors.Add(executor);
            return this;
        }

        public OnPressElement<T> Do(ActionExecutor<T> executor)
        {
            doExecutors.Add(executor);
            return this;
        }

        public OnPressElement<T> DoAfter(ActionExecutor<T> executor)
        {
            doAfterExecutors.Add(executor);
            return this;
        }
    }

    /* StrokeEvent
     * 
     * .Do() -> this StrokeEvent
     */
    public class OnStrokeElement<T>
        where T : ActionContext
    {
        public IReadOnlyCollection<StrokeEvent.Direction> Strokes { get; private set; }

        private List<ActionExecutor<T>> doExecutors = new List<ActionExecutor<T>>();
        public IReadOnlyCollection<ActionExecutor<T>> DoExecutors
        {
            get { return doExecutors.ToList(); }
        }

        public OnStrokeElement(params StrokeEvent.Direction[] strokes)
        {
            Strokes = strokes;
        }

        public OnStrokeElement<T> Do(ActionExecutor<T> executor)
        {
            doExecutors.Add(executor);
            return this;
        }
    }


}
