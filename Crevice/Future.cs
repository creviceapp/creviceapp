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


    /* RootElement
     * 
     * .When() -> new WhenElement
     */
    public class RootElement<T>
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
     * .On(FireEvent) -> new SingleThrowElement
     * 
     * .On(PressEvent) -> new DoubleThrowElement
     */
    public class WhenElement<T>
        where T : ActionContext
    {
        public ActionEvaluator<T> WhenEvaluator { get; private set; }

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

        public WhenElement(ActionEvaluator<T> evaluator)
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
    public class SingleThrowElement<T>
        where T : ActionContext
    {
        public IFireEvent Trigger { get; private set; }
        
        private List<ActionExecutor<T>> doExecutors = new List<ActionExecutor<T>>();
        public IReadOnlyCollection<ActionExecutor<T>> DoExecutors
        {
            get { return doExecutors.ToList(); }
        }

        public SingleThrowElement(IFireEvent triggerEvent)
        {
            Trigger = triggerEvent;
        }

        public SingleThrowElement<T> Do(ActionExecutor<T> executor)
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
    public class DoubleThrowElement<T>
        where T : ActionContext
    {
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

        private List<ActionExecutor<T>> pressExecutors = new List<ActionExecutor<T>>();
        public IReadOnlyCollection<ActionExecutor<T>> PressExecutors
        {
            get { return pressExecutors.ToList(); }
        }

        private List<ActionExecutor<T>> doExecutors = new List<ActionExecutor<T>>();
        public IReadOnlyCollection<ActionExecutor<T>> DoExecutors
        {
            get { return doExecutors.ToList(); }
        }

        private List<ActionExecutor<T>> releaseExecutors = new List<ActionExecutor<T>>();
        public IReadOnlyCollection<ActionExecutor<T>> ReleaseExecutors
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

        public DoubleThrowElement<T> Press(ActionExecutor<T> executor)
        {
            pressExecutors.Add(executor);
            return this;
        }

        public DoubleThrowElement<T> Do(ActionExecutor<T> executor)
        {
            doExecutors.Add(executor);
            return this;
        }

        public DoubleThrowElement<T> Release(ActionExecutor<T> executor)
        {
            releaseExecutors.Add(executor);
            return this;
        }
    }

    /* 
     * .Do() -> this 
     */
    public class StrokeElement<T>
        where T : ActionContext
    {
        public IReadOnlyCollection<StrokeEvent.Direction> Strokes { get; private set; }

        private List<ActionExecutor<T>> doExecutors = new List<ActionExecutor<T>>();
        public IReadOnlyCollection<ActionExecutor<T>> DoExecutors
        {
            get { return doExecutors.ToList(); }
        }

        public StrokeElement(params StrokeEvent.Direction[] strokes)
        {
            Strokes = strokes;
        }

        public StrokeElement<T> Do(ActionExecutor<T> executor)
        {
            doExecutors.Add(executor);
            return this;
        }
    }


}
