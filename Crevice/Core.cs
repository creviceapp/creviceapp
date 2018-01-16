using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.Core
{
    using UserActionContext;

    public static class TestClass
    {
        public static void Test()
        {
            var userConfig = new Config.UserConfig();
            var global = new FSM.StateGlobal<UserActionEvaluationContext, UserActionExecutionContext>(userConfig);
            var gestureDef = new List<GestureDefinition>();
            var gestureMachine = new FSM.GestureMachine<UserActionEvaluationContext, UserActionExecutionContext>(userConfig, gestureDef);


        }
    }

    namespace Config
    {
        public static class Def
        {
            public delegate System.Drawing.Point PointBinding(System.Drawing.Point point);
            public delegate bool WhenClauseBinding(UserActionEvaluationContext ctx, DSL.Def.WhenFunc whenFunc);
            public delegate void BeforeClauseBinding(UserActionExecutionContext ctx, DSL.Def.BeforeFunc beforeFunc);
            public delegate void DoClauseBinding(UserActionExecutionContext ctx, DSL.Def.DoFunc doFunc);
            public delegate void AfterClauseBinding(UserActionExecutionContext ctx, DSL.Def.AfterFunc afterFunc);


            public static PointBinding DefaultPointBinding
            {
                get { return (point) => { return point; }; }
            }

            public static WhenClauseBinding DefaultWhenClauseBinding
            {
                get
                {
                    return (ctx, whenFunc) =>
                    {
                        try
                        {
                            return whenFunc(ctx);
                        }
                        catch (Exception ex)
                        {
                            Verbose.Print(
                                "An exception was thrown when executing a WhenFunc of a gesture. " +
                                "This error may automatically be recovered.\n{0} :\n{1}",
                                ex.GetType().Name,
                                ex.StackTrace);
                        }
                        return false;
                    };
                }
            }

            public static BeforeClauseBinding DefaultBeforeClauseBinding
            {
                get
                {
                    return (ctx, beforeFunc) =>
                    {
                        try
                        {
                            beforeFunc(ctx);
                        }
                        catch (Exception ex)
                        {
                            Verbose.Print(
                                "An exception was thrown when executing a BeforeFunc of a gesture. " +
                                "This error may automatically be recovered.\n{0} :\n{1}",
                                ex.GetType().Name,
                                ex.StackTrace);
                        }
                    };
                }
            }


            public static DoClauseBinding DefaultDoClauseBinding
            {
                get
                {
                    return (ctx, doFunc) =>
                    {
                        try
                        {
                            return doFunc(ctx);
                        }
                        catch (Exception ex)
                        {
                            Verbose.Print(
                                "An exception was thrown when executing a DoFunc of a gesture. " +
                                "This error may automatically be recovered.\n{0} :\n{1}",
                                ex.GetType().Name,
                                ex.StackTrace);
                        }
                    };
                }
            }


            public static AfterClauseBinding DefaultAfterClauseBinding
            {
                get
                {
                    return (ctx, afterFunc) =>
                    {
                        try
                        {
                            return afterFunc(ctx);
                        }
                        catch (Exception ex)
                        {
                            Verbose.Print(
                                "An exception was thrown when executing a AfterFunc of a gesture. " +
                                "This error may automatically be recovered.\n{0} :\n{1}",
                                ex.GetType().Name,
                                ex.StackTrace);
                        }
                    };
                }
            }

        }

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
            public Def.PointBinding TooltipPositionBinding = Def.DefaultPointBinding;
            public int TooltipTimeout = 3000;
            public int BalloonTimeout = 10000;
        }

        public class SystemConfig
        {
            //Todo: null check in bindings
            public Def.PointBinding TooltipPositionBinding = Def.DefaultPointBinding;

            public Def.WhenClauseBinding WhenClauseBinding = Def.DefaultWhenClauseBinding;
            public Def.BeforeClauseBinding BeforeClauseBinding = Def.DefaultBeforeClauseBinding;
            public Def.DoClauseBinding DoClauseBinding = Def.DefaultDoClauseBinding;
            public Def.AfterClauseBinding AfterClauseBinding = Def.DefaultAfterClauseBinding;
        }

        public class UserConfig
        {
            public readonly SystemConfig System = new SystemConfig();
            public readonly GestureConfig Gesture = new GestureConfig();
            public readonly UserInterfaceConfig UI = new UserInterfaceConfig();
        }
    }
}
