using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crevice.Core
{
    namespace UserActionContext
    {
        using System.Drawing;
        using System.Reflection;
        using System.Linq.Expressions;
        
        public class UserActionEvaluationContext
        {
            public Point EvaluatedPoint;

            public UserActionEvaluationContext(
                Point evaluatedPoint)
            {
                this.EvaluatedPoint = evaluatedPoint;
            }
        }

        public class UserActionExecutionContext
        {
            public Point EvaluatedPoint;
            public Point ExcutedPoint;

            public UserActionExecutionContext(
                Point evaluatedPoint,
                Point excutedPoint)
            {
                this.EvaluatedPoint = evaluatedPoint;
                this.ExcutedPoint = excutedPoint;
            }
        }

        public class UserActionEvaluationContextFactory<T>
            where T : UserActionEvaluationContext
        {
            public readonly Func<Point, T> CreateInstance = GetFactory();

            private static Func<Point, T> GetFactory()
            {
                var argsTypes = new[] { typeof(Point) };
                var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
                var args = argsTypes.Select(Expression.Parameter).ToArray();
                return Expression.Lambda<Func<Point, T>>(Expression.New(constructor, args), args).Compile();
            }

            public T Create(
                Point evaluatedPoint)
            {
                return CreateInstance(evaluatedPoint);
            }
        }

        public class UserActionExecutionContextFactory<T>
            where T : UserActionExecutionContext
        {
            public readonly Func<Point, Point, T> CreateInstance = GetFactory();

            private static Func<Point, Point, T> GetFactory()
            {
                var argsTypes = new[] { typeof(Point) , typeof(Point) };
                var constructor = typeof(T).GetConstructor(BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
                var args = argsTypes.Select(Expression.Parameter).ToArray();
                return Expression.Lambda<Func<Point, Point, T>>(Expression.New(constructor, args), args).Compile();
            }

            public T Create(
                Point evaluatedPoint,
                Point executedPoint)
            {
                return CreateInstance(evaluatedPoint, executedPoint);
            }
        }
    }
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
