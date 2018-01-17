using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
            public delegate System.Drawing.Point PositionBinding(System.Drawing.Point point);

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
