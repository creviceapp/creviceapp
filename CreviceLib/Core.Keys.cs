using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Keys
{
    /**
     * Lazyに初期値を割り当てて、上限なくイベント数を設定できるように
     * 
     * LogicalKeys[]
     * 
     * PhysicalKeys[]
     * 
     * IDは本体がシングルトンで管理しつつ、
     * それぞれの this[] へのアクセスをトラップして未定義なら動的生成、かな
     * 
     * Events.Logical[]
     * Events.Physical[][]
     * 
     * Events.LogicalSingle ~
     * Events.LogicalDoubleThrow[] // これをジェスチャ定義で使うようにするとPhysicalへの変換が整合する
     * Events.LogicalDoubleThrow[].PressEvent
     * Events.LogicalDoubleThrow[].ReleaseEvent
     * Events.LogicalDoubleThrow[].ToPhysical(n).PressEvent
     * Events.LogicalDoubleThrow[].ToPhysical(n).ReleaseEvent
     * Events.Physical ~
     * 
     * Events.LogicalSingleThrowKey[]
     * Events.LogicalDoubleThrowKey[]
     * Events.LogicalDoubleThrowSystemKey[] // Formsアセンブリが必要なので微妙。拡張の必要あり
     *                                      // シングルトンなので拡張は容易
     * 
     * ライブラリユーザー側は
     * int => IPressEventな関数を列挙したクラスで
     * 
     * Event.Keyboard.
     * Event.Mouse.
     * Event.Gamepad.
     * 
     * using M = Crevice.Event.MouseEvents
     * 
     * ゲームパッド対応を見込んで、このあたりを綺麗に整理したい
     *      Unityを参考に？
     *          Keysを必要なら拡張してマウスとゲームパッドに対応させるのがスマートかな
     *              Keysから純正Keysへの変換があれば名前の汚染は気にしなくてもいいのでは
     *              
     *              Keys + Pads でいいかな
     *                  →統合するのがよさげ
     *                  
     *                  MITライセンス
     *                  https://github.com/Microsoft/CodeContracts/blob/master/Microsoft.Research/Contracts/System.Windows.Forms/System.Windows.Forms.Keys.cs
     * 
     *                      →だがEnumを使わないなら関係はなさげ
     *                          型が揃わないのでクラスにせざるを得ない
     *                          
     *                          implicitなoperatorはLogicalDoubleThrowに仕込んでおいて、intに変換できればいいかな
     *                              →ひとまず必要なし　
     *                                  →intに変換可能かどうか、という型わけがあれば、コンパイル時エラーにできる
     *                                          →ついでにこれはFroms.Keysにもなる
     *                                                  →となるとFormsが必要なので、ライブラリ側では微妙
     *                                  
     *                  Keysの名称が微妙
     *                      using K = Crevice.Keys;
     *                      
     * 
     * VKからの変換
     * CreviceApp.WinAPI.Constants.VirtualKeys
     * 
     * Keysからの変換
     * System.Windows.Forms.Keys
     * 
     * マウスはキーボードフックも通る？　だとすればちょっと考慮が必要
     *      両方通ると動作がダブってマズい
     *      
     * ひとまずキーボードはVKとKeysからの変換のみでいいかな
     * 
     *      
     */
    using Crevice.Core.Events;
    
    public abstract class KeyGroup
    {
        public abstract int KeyId { get; }
    }

    public abstract class SingleThrowKey : KeyGroup
    {
        public abstract FireEvent FireEvent { get; }
    }
    
    public abstract class DoubleThrowKey : KeyGroup
    {
        public abstract PressEvent PressEvent { get; }
        public abstract ReleaseEvent ReleaseEvent { get; }
    }

    public abstract class Keys<TKey>
        where TKey : KeyGroup
    {
        public int MaxSize { get; }

        private readonly TKey[] keys;

        public Keys(int maxSize)
        {
            MaxSize = maxSize;
            keys = new TKey[maxSize];
        }

        public TKey this[int index]
        {
            get
            {
                var value = keys[index];
                if (value != null)
                {
                    return value;
                }
                else
                {
                    value = Create(index);
                    keys[index] = value;
                    return value;
                }
            }
        }

        public abstract TKey Create(int index);
    }

    public sealed class EventIdGenerator
    {
        private static EventIdGenerator Instance = new EventIdGenerator();

        private EventIdGenerator() { }

        private readonly object lockObject = new object();

        // EventId 0 is reverved for Core.Events.NullEvent.
        private int eventId = 1;

        private int GetAndIncrementEventId()
        {
            lock (lockObject)
            {
                try
                {
                    return eventId;
                }
                finally
                {
                    eventId++;
                }
            }
        }

        public static int Generate() => Instance.GetAndIncrementEventId();
    }

    public class LogicalSingleThrowKeys : Keys<LogicalSingleThrowKey>
    {
        public override LogicalSingleThrowKey Create(int index)
            => new LogicalSingleThrowKey(index);

        public LogicalSingleThrowKeys(int maxSize)
            : base(maxSize) { }
    }

    public class LogicalSingleThrowKey : SingleThrowKey
    {
        public override int KeyId { get; }

        public override FireEvent FireEvent => LogicalFireEvent;

        public LogicalFireEvent LogicalFireEvent { get; }

        public LogicalSingleThrowKey(int keyId)
        {
            KeyId = keyId;
            LogicalFireEvent = new LogicalSingleThrowFireEvent();
        }
    }

    public class LogicalSingleThrowFireEvent : LogicalFireEvent
    {
        public LogicalSingleThrowFireEvent()
            : base(EventIdGenerator.Generate()) { }
    }

    public class PhysicalSingleThrowKeys : Keys<PhysicalSingleThrowKey>
    {
        public override PhysicalSingleThrowKey Create(int index)
            => new PhysicalSingleThrowKey(logicalKeys[index], index);

        private LogicalSingleThrowKeys logicalKeys;

        public PhysicalSingleThrowKeys(LogicalSingleThrowKeys logicalKeys) 
            : base(logicalKeys.MaxSize)
        {
            this.logicalKeys = logicalKeys;
        }
    }

    public class PhysicalSingleThrowKey : SingleThrowKey
    {
        public override int KeyId { get; }

        public override FireEvent FireEvent => PhysicalFireEvent;

        public PhysicalFireEvent PhysicalFireEvent { get; }

        public PhysicalSingleThrowKey(LogicalSingleThrowKey logicalKey, int keyId)
        {
            KeyId = keyId;
            PhysicalFireEvent = new PhysicalSingleThrowFireEvent(logicalKey);
        }
    }

    public class PhysicalSingleThrowFireEvent : PhysicalFireEvent
    {
        public LogicalSingleThrowKey LogicalKey { get; }

        public override LogicalFireEvent LogicalNormalized
            => LogicalKey.LogicalFireEvent;

        public PhysicalSingleThrowFireEvent(LogicalSingleThrowKey logicalKey)
            : base(EventIdGenerator.Generate())
        {
            LogicalKey = logicalKey;
        }
    }

    public class LogicalDoubleThrowKeys : Keys<LogicalDoubleThrowKey>
    {
        public override LogicalDoubleThrowKey Create(int index)
            => new LogicalDoubleThrowKey(index);

        public LogicalDoubleThrowKeys(int maxSize)
            : base(maxSize) { }
    }

    public class LogicalDoubleThrowKey : DoubleThrowKey
    {
        public override int KeyId { get; }

        public override PressEvent PressEvent => LogicalPressEvent;
        public override ReleaseEvent ReleaseEvent => LogicalReleaseEvent;

        public LogicalPressEvent LogicalPressEvent { get; }
        public LogicalReleaseEvent LogicalReleaseEvent { get; }

        public LogicalDoubleThrowKey(int keyId)
        {
            KeyId = keyId;
            LogicalPressEvent = new LogicalDoubleThrowPressEvent(this);
            LogicalReleaseEvent = new LogicalDoubleThrowReleaseEvent(this);
        }
    }

    public class LogicalDoubleThrowPressEvent : LogicalPressEvent
    {
        public LogicalDoubleThrowKey LogicalKey { get; }

        public override LogicalReleaseEvent Opposition
            => LogicalKey.LogicalReleaseEvent;

        public LogicalDoubleThrowPressEvent(LogicalDoubleThrowKey logicalKey)
            : base(EventIdGenerator.Generate())
        {
            LogicalKey = logicalKey;
        }
    }

    public class LogicalDoubleThrowReleaseEvent : LogicalReleaseEvent
    {
        public LogicalDoubleThrowKey LogicalKey { get; }

        public override LogicalPressEvent Opposition
            => LogicalKey.LogicalPressEvent;

        public LogicalDoubleThrowReleaseEvent(LogicalDoubleThrowKey logicalKey)
            : base(EventIdGenerator.Generate())
        {
            LogicalKey = logicalKey;
        }
    }

    public class PhysicalDoubleThrowKeys : Keys<PhysicalDoubleThrowKey>
    {
        public override PhysicalDoubleThrowKey Create(int index)
            => new PhysicalDoubleThrowKey(logicalKeys[index], index);

        private LogicalDoubleThrowKeys logicalKeys;

        public PhysicalDoubleThrowKeys(LogicalDoubleThrowKeys logicalKeys)
            : base(logicalKeys.MaxSize)
        {
            this.logicalKeys = logicalKeys;
        }
    }

    public class PhysicalDoubleThrowKey : DoubleThrowKey
    {
        public override int KeyId { get; }

        public override PressEvent PressEvent => PhysicalPressEvent;
        public override ReleaseEvent ReleaseEvent => PhysicalReleaseEvent;

        public PhysicalPressEvent PhysicalPressEvent { get; }
        public PhysicalReleaseEvent PhysicalReleaseEvent { get; }

        public PhysicalDoubleThrowKey(LogicalDoubleThrowKey logicalKey, int keyId)
        {
            KeyId = keyId;
            PhysicalPressEvent = new PhysicalDoubleThrowPressEvent(logicalKey, this);
            PhysicalReleaseEvent = new PhysicalDoubleThrowReleaseEvent(logicalKey, this);
        }
    }

    public class PhysicalDoubleThrowPressEvent : PhysicalPressEvent
    {
        public LogicalDoubleThrowKey LogicalKey { get; }

        public PhysicalDoubleThrowKey PhysicalKey { get; }

        public override LogicalPressEvent LogicalNormalized
            => LogicalKey.LogicalPressEvent;

        public override PhysicalReleaseEvent Opposition
            => PhysicalKey.PhysicalReleaseEvent;

        public PhysicalDoubleThrowPressEvent(LogicalDoubleThrowKey logicalKey, PhysicalDoubleThrowKey physicalKey)
            : base(EventIdGenerator.Generate())
        {
            LogicalKey = logicalKey;
            PhysicalKey = physicalKey;
        }
    }

    public class PhysicalDoubleThrowReleaseEvent : PhysicalReleaseEvent
    {
        public LogicalDoubleThrowKey LogicalKey { get; }

        public PhysicalDoubleThrowKey PhysicalKey { get; }

        public override LogicalReleaseEvent LogicalNormalized
            => LogicalKey.LogicalReleaseEvent;

        public override PhysicalPressEvent Opposition
            => PhysicalKey.PhysicalPressEvent;

        public PhysicalDoubleThrowReleaseEvent(LogicalDoubleThrowKey logicalKey, PhysicalDoubleThrowKey physicalKey)
            : base(EventIdGenerator.Generate())
        {
            LogicalKey = logicalKey;
            PhysicalKey = physicalKey;
        }
    }
}
