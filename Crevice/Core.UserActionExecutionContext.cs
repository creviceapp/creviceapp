using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crevice.Core
{
    using System.Drawing;
    using System.Reflection;
    using System.Linq.Expressions;

    // Todo generics

        // ExecutionPoint
        // EvaluationPoint
    public abstract class ActionContext
    {
        public Point EvaluatePoint;
        public Point ExecutePoint;

        public ActionContext(Point evaluatePoint)
        { 
            this.EvaluatePoint = evaluatePoint;
        }

        public ActionContext(ActionContext evaluationContext, Point executePoint)
        {
            this.EvaluatePoint = evaluationContext.EvaluatePoint;
            this.ExecutePoint = executePoint;
        }
    }

    public class ActionContextFactory<T>
        where T : ActionContext
    {
        public readonly Func<Point, T> CreateEvaluationContext = GetEvaluationContextFactory();

        private static Func<Point, T> GetEvaluationContextFactory()
        {
            var argsTypes = new[] { typeof(Point) };
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
            var args = argsTypes.Select(Expression.Parameter).ToArray();
            return Expression.Lambda<Func<Point, T>>(Expression.New(constructor, args), args).Compile();
        }

        public T Create(
            Point evaluatePoint)
        {
            return CreateEvaluationContext(evaluatePoint);
        }

        public readonly Func<T, Point, T> CreateExecutionContext = GetExecutionContextFactory();

        private static Func<T, Point, T> GetExecutionContextFactory()
        {
            var argsTypes = new[] { typeof(T), typeof(Point) };
            var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
            var args = argsTypes.Select(Expression.Parameter).ToArray();
            return Expression.Lambda<Func<T, Point, T>>(Expression.New(constructor, args), args).Compile();
        }

        public T Create(
            T evaluationContext,
            Point executePoint)
        {
            return CreateExecutionContext(evaluationContext, executePoint);
        }

    }
    
    public class DefaultActionContext : ActionContext
    {
        public DefaultActionContext(Point evaluatePoint) : base(evaluatePoint)
        { }

        public DefaultActionContext(DefaultActionContext evaluationContext, Point executePoint) : base(evaluationContext, executePoint)
        { }
    }

    /*

    public abstract class GestureExecutionContext
    {
        public Point EvaluatePoint;
        public Point ExcutePoint;

        public GestureExecutionContext(
            GestureEvaluationContext evaluateContext,
            Point excutePoint)
        {
            this.EvaluatePoint = evaluateContext.EvaluatePoint;
            this.ExcutePoint = excutePoint;
        }
    }

    public class ExecutionContextFactory<A, B>
        where A : GestureEvaluationContext
        where B : GestureExecutionContext
    {
        public readonly Func<A, Point, B> CreateInstance = GetFactory();
 
        private static Func<A, Point, B> GetFactory()
        {
            var argsTypes = new[] { typeof(A) , typeof(Point) };
            var constructor = typeof(B).GetConstructor(BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
            var args = argsTypes.Select(Expression.Parameter).ToArray();
            return Expression.Lambda<Func<A, Point, B>>(Expression.New(constructor, args), args).Compile();
        }
 
        public B Create(
            A evaluationContext,
            Point executePoint)
        {
            return CreateInstance(evaluationContext, executePoint);
        }
    }
         

    public class ActionContextFactory<A, B>
        where A : GestureEvaluationContext
        where B : GestureExecutionContext
    {
        public readonly EvaluationContextFactory<A> EvaluationContextFactory
          = new EvaluationContextFactory<A>();

        public readonly ExecutionContextFactory<A, B> ExecutionContextFactory
            = new ExecutionContextFactory<A, B>();

        public GestureEvaluationContext CreateEvaluationContext(Point evaluatePoint)
        {
            return EvaluationContextFactory.Create(evaluatePoint);
        }

        public GestureExecutionContext CreateExcutionContext(A evaluationContext, Point excutionPoint)
        {
            return ExecutionContextFactory.Create(evaluationContext, excutionPoint);
        }
    }
    */


    namespace GestureActionContext
    {
        /*
        using System.Drawing;
        
        public class EvaluationContext
        {
            public Point EvaluatePoint;

            public EvaluationContext(
                Point evaluatePoint)
            {
                this.EvaluatePoint = evaluatePoint;
            }
        }

        public class ExecutionContext
        {
            public Point EvaluatePoint;
            public Point ExcutePoint;

            public ExecutionContext(
                Point evaluatedPoint,
                Point excutedPoint)
            {
                this.EvaluatePoint = evaluatedPoint;
                this.ExcutePoint = excutedPoint;
            }
        }
        */

        // GestureConditionEvaluationContext
        // SingleActionGestureExecutionActionContext
        // DoubleActionGestureExecutionActionContext
        // StrokeGestureExecutionActionContext
    }

    /*
    public abstract class UserActionExecutionContextCreator<T>
        where T : UserActionExecutionContextBase
    {
        public abstract T Create(System.Drawing.Point point);
    }


    public class DefaultUserActionExecutionContext 
        : UserActionExecutionContextBase
    {
        private Lazy<System.Drawing.Point> _startPoint = new Lazy<System.Drawing.Point>();
        public System.Drawing.Point StartPoint
        {
            get { return _startPoint.Value ; }
        }

        public void Setup(System.Drawing.Point startPoint)
        {
            this._startPoint.Value = startPoint;
        }
    }

    

    public class UserActionExecutionContextFactory<T>
    where T : UserActionExecutionContextBase, new()
    {
        public static T Create(System.Drawing.Point endPoint)
        {
            var 
            return new T();
        }
    }


    public class UserActionExecutionContextFactoryCreator<T>
        where T : UserActionExecutionContextBase
    {
        public T Create(System.Drawing.Point startPoint)
        {
            return new T();
        }
    }



    public class DefaultUserActionExecutionContextFactory 
        : UserActionExecutionContextFactoryBase<DefaultUserActionExecutionContext>
    {
        public override DefaultUserActionExecutionContext Create(System.Drawing.Point point)
        {
            return new DefaultUserActionExecutionContext();
        }
    }
    */

    /*
    public class UserActionExecutionContext
    {
        // Todo 多相化が必要では
        // UserActionExecutionContextFactory[T] として .Create() で確定

        public readonly Point GestureStartPoint;
        public readonly ForegroundWindowInfo ForegroundWindow;
        public readonly PointedWindowInfo PointedWindow;

        public UserActionExecutionContext(System.Drawing.Point point)
        {
            // Todo: GestureStartPosition
            // Todo: GestureEndPosition // いらない？
            // Todo: CurrentPosition
            this.GestureStartPoint = point;
            this.ForegroundWindow = new ForegroundWindowInfo();
            this.PointedWindow = new PointedWindowInfo(point);
        }
        // Todo: UserActionExecutionContextFactory, UserActionExecutionContext

    }
    */
}
