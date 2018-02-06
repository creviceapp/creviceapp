using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp
{
    using System.Drawing;
    using Crevice.Core.Context;
    using Crevice.Core.DSL;
    using Crevice.Core.Keys;
    using Crevice.Core.FSM;
    using Crevice.Core.Callback;
    using Crevice.Core.Stroke;

    using CreviceApp.WinAPI.Window.Impl;
    using Crevice.Core.Events;

    public class LogicalSystemKeySet : KeySet<LogicalSystemKey> 
    {
        public override LogicalSystemKey Create(int index)
            => new LogicalSystemKey(index);

        public LogicalSystemKeySet(int maxSize)
            : base(maxSize) { }
    }

    public class LogicalSystemKey : LogicalDoubleThrowKey
    {
        public LogicalSystemKey(int keyId)
            : base(keyId) { }

        // todo implicit operater to Keys, int   
    }

    public class PhysicalSystemKeySet : KeySet<PhysicalSystemKey>
    {
        public override PhysicalSystemKey Create(int index)
            => new PhysicalSystemKey(logicalKeys[index], index);

        private LogicalSystemKeySet logicalKeys;

        public PhysicalSystemKeySet(LogicalSystemKeySet logicalKeys)
            : base(logicalKeys.MaxSize)
        {
            this.logicalKeys = logicalKeys;
        }
    }

    public class PhysicalSystemKey : PhysicalDoubleThrowKey
    {
        public PhysicalSystemKey(LogicalDoubleThrowKey logicalKey, int keyId)
            : base(logicalKey, keyId) { }

        // todo implicit operater to Keys, int   
    }

    public class SupportedKeys
    {
        private static SupportedKeys Instance = new SupportedKeys();

        private readonly LogicalKeyDeclaration logicalKeys;
        private readonly PhysicalKeyDeclaration physicalKeys;

        private SupportedKeys()
        {
            var logicalSingleThrowKeys = new LogicalSingleThrowKeySet(10);
            var physicalSingleThrowKeys = new PhysicalSingleThrowKeySet(logicalSingleThrowKeys);

            var logicalSystemKeys = new LogicalSystemKeySet(255);
            var physicalSystemKeys = new PhysicalSystemKeySet(logicalSystemKeys);

            logicalKeys = new LogicalKeyDeclaration(logicalSingleThrowKeys, logicalSystemKeys);
            physicalKeys = new PhysicalKeyDeclaration(physicalSingleThrowKeys, physicalSystemKeys);
        }

        public static LogicalKeyDeclaration Keys => Instance.logicalKeys;

        public static PhysicalKeyDeclaration PhysicalKeys => Instance.physicalKeys;

        public class KeyDeclaration<TKeyA, TKeyB>
            where TKeyA : KeyType
            where TKeyB : KeyType
        {
            public TKeyB this[int keyCode] => SystemKeySet[keyCode];

            public readonly KeySet<TKeyA> SingleThrowKeySet;
            public readonly KeySet<TKeyB> SystemKeySet;
            
            public KeyDeclaration(
                KeySet<TKeyA> singleThrowKeys,
                KeySet<TKeyB> systemKeys)
            {
                SingleThrowKeySet = singleThrowKeys;
                SystemKeySet = systemKeys;
            }

            public TKeyA WheelUp => SingleThrowKeySet[1];
            public TKeyA WheelDown => SingleThrowKeySet[2];
            public TKeyA WheelLeft => SingleThrowKeySet[3];
            public TKeyA WheelRight => SingleThrowKeySet[4];

            public StrokeDirection MoveUp => StrokeDirection.Up;
            public StrokeDirection MoveDown => StrokeDirection.Down;
            public StrokeDirection MoveLeft => StrokeDirection.Left;
            public StrokeDirection MoveRight => StrokeDirection.Right;

            public TKeyB LeftButton => SystemKeySet[1];
            public TKeyB RightButton => SystemKeySet[2];
            public TKeyB MiddleButton => SystemKeySet[4];
            public TKeyB X1Button => SystemKeySet[5];
            public TKeyB X2Button => SystemKeySet[6];

            public TKeyB None => SystemKeySet[0];
            public TKeyB LButton => SystemKeySet[1];
            public TKeyB RButton => SystemKeySet[2];
            public TKeyB Cancel => SystemKeySet[3];
            public TKeyB MButton => SystemKeySet[4];
            public TKeyB XButton1 => SystemKeySet[5];
            public TKeyB XButton2 => SystemKeySet[6];

            public TKeyB VirtualKey(int virtualKey) => SystemKeySet[virtualKey];

            public TKeyB Back => SystemKeySet[8];
            public TKeyB Tab => SystemKeySet[9];
            public TKeyB LineFeed => SystemKeySet[10];
            public TKeyB Clear => SystemKeySet[12];
            public TKeyB Enter => SystemKeySet[13];
            public TKeyB Return => SystemKeySet[13];
            public TKeyB ShiftKey => SystemKeySet[16];
            public TKeyB ControlKey => SystemKeySet[17];
            public TKeyB Menu => SystemKeySet[18];
            public TKeyB Pause => SystemKeySet[19];
            public TKeyB CapsLock => SystemKeySet[20];
            public TKeyB Capital => SystemKeySet[20];
            public TKeyB KanaMode => SystemKeySet[21];
            public TKeyB HangulMode => SystemKeySet[21];
            public TKeyB JunjaMode => SystemKeySet[23];
            public TKeyB FinalMode => SystemKeySet[24];
            public TKeyB KanjiMode => SystemKeySet[25];
            public TKeyB HanjaMode => SystemKeySet[25];
            public TKeyB Escape => SystemKeySet[27];
            public TKeyB IMEConvert => SystemKeySet[28];
            public TKeyB IMENonconvert => SystemKeySet[29];
            public TKeyB IMEAccept => SystemKeySet[30];
            public TKeyB IMEModeChange => SystemKeySet[31];
            public TKeyB Space => SystemKeySet[32];
            public TKeyB Prior => SystemKeySet[33];
            public TKeyB PageUp => SystemKeySet[33];
            public TKeyB Next => SystemKeySet[34];
            public TKeyB PageDown => SystemKeySet[34];
            public TKeyB End => SystemKeySet[35];
            public TKeyB Home => SystemKeySet[36];
            public TKeyB Left => SystemKeySet[37];
            public TKeyB Up => SystemKeySet[38];
            public TKeyB Right => SystemKeySet[39];
            public TKeyB Down => SystemKeySet[40];
            public TKeyB Select => SystemKeySet[41];
            public TKeyB Print => SystemKeySet[42];
            public TKeyB Execute => SystemKeySet[43];
            public TKeyB PrintScreen => SystemKeySet[44];
            public TKeyB Snapshot => SystemKeySet[44];
            public TKeyB Insert => SystemKeySet[45];
            public TKeyB Delete => SystemKeySet[46];
            public TKeyB Help => SystemKeySet[47];
            public TKeyB D0 => SystemKeySet[48];
            public TKeyB D1 => SystemKeySet[49];
            public TKeyB D2 => SystemKeySet[50];
            public TKeyB D3 => SystemKeySet[51];
            public TKeyB D4 => SystemKeySet[52];
            public TKeyB D5 => SystemKeySet[53];
            public TKeyB D6 => SystemKeySet[54];
            public TKeyB D7 => SystemKeySet[55];
            public TKeyB D8 => SystemKeySet[56];
            public TKeyB D9 => SystemKeySet[57];
            public TKeyB A => SystemKeySet[65];
            public TKeyB B => SystemKeySet[66];
            public TKeyB C => SystemKeySet[67];
            public TKeyB D => SystemKeySet[68];
            public TKeyB E => SystemKeySet[69];
            public TKeyB F => SystemKeySet[70];
            public TKeyB G => SystemKeySet[71];
            public TKeyB H => SystemKeySet[72];
            public TKeyB I => SystemKeySet[73];
            public TKeyB J => SystemKeySet[74];
            public TKeyB K => SystemKeySet[75];
            public TKeyB L => SystemKeySet[76];
            public TKeyB M => SystemKeySet[77];
            public TKeyB N => SystemKeySet[78];
            public TKeyB O => SystemKeySet[79];
            public TKeyB P => SystemKeySet[80];
            public TKeyB Q => SystemKeySet[81];
            public TKeyB R => SystemKeySet[82];
            public TKeyB S => SystemKeySet[83];
            public TKeyB T => SystemKeySet[84];
            public TKeyB U => SystemKeySet[85];
            public TKeyB V => SystemKeySet[86];
            public TKeyB W => SystemKeySet[87];
            public TKeyB X => SystemKeySet[88];
            public TKeyB Y => SystemKeySet[89];
            public TKeyB Z => SystemKeySet[90];
            public TKeyB LWin => SystemKeySet[91];
            public TKeyB RWin => SystemKeySet[92];
            public TKeyB Apps => SystemKeySet[93];
            public TKeyB Sleep => SystemKeySet[95];
            public TKeyB NumPad0 => SystemKeySet[96];
            public TKeyB NumPad1 => SystemKeySet[97];
            public TKeyB NumPad2 => SystemKeySet[98];
            public TKeyB NumPad3 => SystemKeySet[99];
            public TKeyB NumPad4 => SystemKeySet[100];
            public TKeyB NumPad5 => SystemKeySet[101];
            public TKeyB NumPad6 => SystemKeySet[102];
            public TKeyB NumPad7 => SystemKeySet[103];
            public TKeyB NumPad8 => SystemKeySet[104];
            public TKeyB NumPad9 => SystemKeySet[105];
            public TKeyB Multiply => SystemKeySet[106];
            public TKeyB Add => SystemKeySet[107];
            public TKeyB Separator => SystemKeySet[108];
            public TKeyB Subtract => SystemKeySet[109];
            public TKeyB Decimal => SystemKeySet[110];
            public TKeyB Divide => SystemKeySet[111];
            public TKeyB F1 => SystemKeySet[112];
            public TKeyB F2 => SystemKeySet[113];
            public TKeyB F3 => SystemKeySet[114];
            public TKeyB F4 => SystemKeySet[115];
            public TKeyB F5 => SystemKeySet[116];
            public TKeyB F6 => SystemKeySet[117];
            public TKeyB F7 => SystemKeySet[118];
            public TKeyB F8 => SystemKeySet[119];
            public TKeyB F9 => SystemKeySet[120];
            public TKeyB F10 => SystemKeySet[121];
            public TKeyB F11 => SystemKeySet[122];
            public TKeyB F12 => SystemKeySet[123];
            public TKeyB F13 => SystemKeySet[124];
            public TKeyB F14 => SystemKeySet[125];
            public TKeyB F15 => SystemKeySet[126];
            public TKeyB F16 => SystemKeySet[127];
            public TKeyB F17 => SystemKeySet[128];
            public TKeyB F18 => SystemKeySet[129];
            public TKeyB F19 => SystemKeySet[130];
            public TKeyB F20 => SystemKeySet[131];
            public TKeyB F21 => SystemKeySet[132];
            public TKeyB F22 => SystemKeySet[133];
            public TKeyB F23 => SystemKeySet[134];
            public TKeyB F24 => SystemKeySet[135];
            public TKeyB NumLock => SystemKeySet[144];
            public TKeyB Scroll => SystemKeySet[145];
            public TKeyB LShiftKey => SystemKeySet[160];
            public TKeyB RShiftKey => SystemKeySet[161];
            public TKeyB LControlKey => SystemKeySet[162];
            public TKeyB RControlKey => SystemKeySet[163];
            public TKeyB LMenu => SystemKeySet[164];
            public TKeyB RMenu => SystemKeySet[165];
            public TKeyB BrowserBack => SystemKeySet[166];
            public TKeyB BrowserForward => SystemKeySet[167];
            public TKeyB BrowserRefresh => SystemKeySet[168];
            public TKeyB BrowserStop => SystemKeySet[169];
            public TKeyB BrowserSearch => SystemKeySet[170];
            public TKeyB BrowserFavorites => SystemKeySet[171];
            public TKeyB BrowserHome => SystemKeySet[172];
            public TKeyB VolumeMute => SystemKeySet[173];
            public TKeyB VolumeDown => SystemKeySet[174];
            public TKeyB VolumeUp => SystemKeySet[175];
            public TKeyB MediaNextTrack => SystemKeySet[176];
            public TKeyB MediaPreviousTrack => SystemKeySet[177];
            public TKeyB MediaStop => SystemKeySet[178];
            public TKeyB MediaPlayPause => SystemKeySet[179];
            public TKeyB LaunchMail => SystemKeySet[180];
            public TKeyB SelectMedia => SystemKeySet[181];
            public TKeyB LaunchApplication1 => SystemKeySet[182];
            public TKeyB LaunchApplication2 => SystemKeySet[183];
            public TKeyB Oem1 => SystemKeySet[186];
            public TKeyB OemSemicolon => SystemKeySet[186];
            public TKeyB Oemplus => SystemKeySet[187];
            public TKeyB Oemcomma => SystemKeySet[188];
            public TKeyB OemMinus => SystemKeySet[189];
            public TKeyB OemPeriod => SystemKeySet[190];
            public TKeyB OemQuestion => SystemKeySet[191];
            public TKeyB Oem2 => SystemKeySet[191];
            public TKeyB Oemtilde => SystemKeySet[192];
            public TKeyB Oem3 => SystemKeySet[192];
            public TKeyB Oem4 => SystemKeySet[219];
            public TKeyB OemOpenBrackets => SystemKeySet[219];
            public TKeyB OemPipe => SystemKeySet[220];
            public TKeyB Oem5 => SystemKeySet[220];
            public TKeyB Oem6 => SystemKeySet[221];
            public TKeyB OemCloseBrackets => SystemKeySet[221];
            public TKeyB Oem7 => SystemKeySet[222];
            public TKeyB OemQuotes => SystemKeySet[222];
            public TKeyB Oem8 => SystemKeySet[223];
            public TKeyB Oem102 => SystemKeySet[226];
            public TKeyB OemBackslash => SystemKeySet[226];
            public TKeyB ProcessKey => SystemKeySet[229];
            public TKeyB Packet => SystemKeySet[231];
            public TKeyB Attn => SystemKeySet[246];
            public TKeyB Crsel => SystemKeySet[247];
            public TKeyB Exsel => SystemKeySet[248];
            public TKeyB EraseEof => SystemKeySet[249];
            public TKeyB Play => SystemKeySet[250];
            public TKeyB Zoom => SystemKeySet[251];
            public TKeyB NoName => SystemKeySet[252];
            public TKeyB Pa1 => SystemKeySet[253];
            public TKeyB OemClear => SystemKeySet[254];
        }

        public class LogicalKeyDeclaration : KeyDeclaration<LogicalSingleThrowKey, LogicalSystemKey>
        {
            public LogicalKeyDeclaration(
                LogicalSingleThrowKeySet singleThrowKeys,
                LogicalSystemKeySet logicalSystemKeys)
                : base(singleThrowKeys, logicalSystemKeys)
            {

            }
        }

        public class PhysicalKeyDeclaration : KeyDeclaration<PhysicalSingleThrowKey, PhysicalSystemKey>
        {
            public PhysicalKeyDeclaration(
                PhysicalSingleThrowKeySet singleThrowKeys, 
                PhysicalSystemKeySet systemKeys)
                : base(singleThrowKeys, systemKeys)
            {

            }
        }
    }

    public class CustomEvaluationContext : EvaluationContext
    {
        public readonly Point GestureStartPosition;
        public readonly ForegroundWindowInfo ForegroundWindow;
        public readonly PointedWindowInfo PointedWindow;

        public CustomEvaluationContext(Point gestureStartPosition)
        {
            this.GestureStartPosition = gestureStartPosition;
            this.ForegroundWindow = new ForegroundWindowInfo();
            this.PointedWindow = new PointedWindowInfo(gestureStartPosition);
        }
    }

    public class CustomExecutionContext : ExecutionContext
    {
        public readonly Point GestureStartPosition;
        public readonly Point GestureEndPosition;
        public readonly ForegroundWindowInfo ForegroundWindow;
        public readonly PointedWindowInfo PointedWindow;

        public CustomExecutionContext(CustomEvaluationContext evaluationContext, Point gestureEndPosition)
        {
            this.GestureStartPosition = evaluationContext.GestureStartPosition;
            this.GestureEndPosition = gestureEndPosition;
            this.ForegroundWindow = evaluationContext.ForegroundWindow;
            this.PointedWindow = evaluationContext.PointedWindow;
        }
    }

    public class CustomContextManager : ContextManager<CustomEvaluationContext, CustomExecutionContext>
    {
        public Point CursorPosition { get; set; }

        public override CustomEvaluationContext CreateEvaluateContext()
            => new CustomEvaluationContext(CursorPosition);

        public override CustomExecutionContext CreateExecutionContext(CustomEvaluationContext evaluationContext)
            => new CustomExecutionContext(evaluationContext, CursorPosition);

        public override bool Evaluate(
            CustomEvaluationContext evalContext,
            WhenElement<CustomEvaluationContext, CustomExecutionContext> whenElement)
        {
            var task = Task.Factory.StartNew(() => 
            {
                return whenElement.WhenEvaluator(evalContext);
            });
            try
            {
                return task.Wait(20);
            }
            catch(Exception)
            {
                return false;
            }
        }

        public override void Execute(
            CustomExecutionContext execContext, 
            ExecuteAction<CustomExecutionContext> executeAction)
        {
            Task.Factory.StartNew(() =>
            {
                executeAction(execContext);
            });
        }
    }
    
    public class CustomRootElement : RootElement<CustomEvaluationContext, CustomExecutionContext>
    {

    }

    public class CustomCallbackManager : CallbackManager<GestureMachineConfig, CustomContextManager, CustomEvaluationContext, CustomExecutionContext>
    {
        private readonly TaskFactory SystemKeyRestorationTaskFactory = Task.Factory;

        private readonly WinAPI.SendInput.SingleInputSender SingleInputSender = new WinAPI.SendInput.SingleInputSender();

        public override void OnGestureCancelled(
            StateN<GestureMachineConfig, CustomContextManager, CustomEvaluationContext, CustomExecutionContext> stateN)
        {
            Verbose.Print("GestureCancelled");
            var systemKey = stateN.NormalEndTrigger.PhysicalKey as PhysicalSystemKey;
            ExecuteInBackground(SystemKeyRestorationTaskFactory, RestoreKeyPressAndReleaseEvent(systemKey));
            base.OnGestureCancelled(stateN);
        }

        public override void OnGestureTimeout(
            StateN<GestureMachineConfig, CustomContextManager, CustomEvaluationContext, CustomExecutionContext> stateN)
        {
            Verbose.Print("GestureTimeout");
            var systemKey = stateN.NormalEndTrigger.PhysicalKey as PhysicalSystemKey;
            ExecuteInBackground(SystemKeyRestorationTaskFactory, RestoreKeyPressEvent(systemKey));
            base.OnGestureTimeout(stateN);
        }

        public override void OnMachineReset(
            IState state)
        {
            Verbose.Print("MachineReset");
            base.OnMachineReset(state);
        }

        protected internal void ExecuteInBackground(TaskFactory taskFactory, Action action)
            => taskFactory.StartNew(action);

        internal Action RestoreKeyPressEvent(PhysicalSystemKey systemKey)
        {
            return () =>
            {
                if (systemKey == SupportedKeys.PhysicalKeys.None)
                {

                }
                else if (systemKey == SupportedKeys.PhysicalKeys.LeftButton)
                {
                    SingleInputSender.LeftDown();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.RightButton)
                {
                    SingleInputSender.RightDown();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.MiddleButton)
                {
                    SingleInputSender.MiddleDown();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.X1Button)
                {
                    SingleInputSender.X1Down();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.X2Button)
                {
                    SingleInputSender.X2Down();
                }
                else
                {
                    SingleInputSender.ExtendedKeyDownWithScanCode((ushort)systemKey.KeyId);
                }
            };
        }

        internal Action RestoreKeyPressAndReleaseEvent(PhysicalSystemKey systemKey)
        {
            return () =>
            {
                if (systemKey == SupportedKeys.PhysicalKeys.None)
                {

                }
                else if (systemKey == SupportedKeys.PhysicalKeys.LeftButton)
                {
                    SingleInputSender.LeftClick();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.RightButton)
                {
                    SingleInputSender.RightClick();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.MiddleButton)
                {
                    SingleInputSender.MiddleClick();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.X1Button)
                {
                    SingleInputSender.X1Click();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.X2Button)
                {
                    SingleInputSender.X2Click();
                }
                else
                {
                    SingleInputSender.Multiple()
                        .ExtendedKeyDownWithScanCode((ushort)systemKey.KeyId)
                        .ExtendedKeyUpWithScanCode((ushort)systemKey.KeyId)
                        .Send();
                }
            };
        }
    }

    public class NullGestureMachine : CustomGestureMachine
    {
        public NullGestureMachine() : base(new CustomRootElement()) { }
    }
    
    public class CustomGestureMachine : GestureMachine<GestureMachineConfig, CustomContextManager, CustomEvaluationContext, CustomExecutionContext>
    {
        public CustomGestureMachine(CustomRootElement rootElement)
            : base(new GestureMachineConfig(), new CustomCallbackManager(), new CustomContextManager(), rootElement)
        {

        }

        public override bool Input(IPhysicalEvent evnt, Point? point)
        {
            lock (lockObject)
            {
                if (point.HasValue)
                {
                    ContextManager.CursorPosition = point.Value;
                }
                return base.Input(evnt, point);
            }
        }
    }
}
