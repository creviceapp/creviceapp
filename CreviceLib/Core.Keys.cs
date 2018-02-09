using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Keys
{
    using Crevice.Core.Events;
    
    public abstract class KeyType
    {
        public abstract int KeyId { get; }
    }

    public abstract class SingleThrowKey<TFireEvent> : KeyType
        where TFireEvent : FireEvent
    {
        public abstract TFireEvent FireEvent { get; }
    }
    
    public abstract class DoubleThrowKey<TPressEvent, TReleaseEvent> : KeyType
        where TPressEvent : PressEvent
        where TReleaseEvent : ReleaseEvent
    {
        public abstract TPressEvent PressEvent { get; }
        public abstract TReleaseEvent ReleaseEvent { get; }
    }

    public abstract class KeySet<TKey>
        where TKey : KeyType
    {
        public int MaxSize { get; }

        private readonly TKey[] keys;

        public KeySet(int maxSize)
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

    public class LogicalSingleThrowKeySet : KeySet<LogicalSingleThrowKey>
    {
        public override LogicalSingleThrowKey Create(int index)
            => new LogicalSingleThrowKey(index);

        public LogicalSingleThrowKeySet(int maxSize)
            : base(maxSize) { }
    }

    public class LogicalSingleThrowKey : SingleThrowKey<LogicalFireEvent>
    {
        public override int KeyId { get; }

        public override LogicalFireEvent FireEvent { get; }

        public LogicalSingleThrowKey(int keyId)
        {
            KeyId = keyId;
            FireEvent = new LogicalFireEvent();
        }
    }
    
    public class PhysicalSingleThrowKeySet : KeySet<PhysicalSingleThrowKey>
    {
        public override PhysicalSingleThrowKey Create(int index)
            => new PhysicalSingleThrowKey(logicalKeys[index], index);

        private LogicalSingleThrowKeySet logicalKeys;

        public PhysicalSingleThrowKeySet(LogicalSingleThrowKeySet logicalKeys) 
            : base(logicalKeys.MaxSize)
        {
            this.logicalKeys = logicalKeys;
        }
    }

    public class PhysicalSingleThrowKey : SingleThrowKey<PhysicalFireEvent>
    {
        public override int KeyId { get; }

        public override PhysicalFireEvent FireEvent { get; }
        
        public PhysicalSingleThrowKey(LogicalSingleThrowKey logicalKey, int keyId)
        {
            KeyId = keyId;
            FireEvent = new PhysicalFireEvent(logicalKey);
        }
    }
    
    public class LogicalDoubleThrowKeySet : KeySet<LogicalDoubleThrowKey>
    {
        public override LogicalDoubleThrowKey Create(int index)
            => new LogicalDoubleThrowKey(index);

        public LogicalDoubleThrowKeySet(int maxSize)
            : base(maxSize) { }
    }

    public class LogicalDoubleThrowKey : DoubleThrowKey<LogicalPressEvent, LogicalReleaseEvent>
    {
        public override int KeyId { get; }

        public override LogicalPressEvent PressEvent { get; }
        public override LogicalReleaseEvent ReleaseEvent { get; }
        
        public LogicalDoubleThrowKey(int keyId)
        {
            KeyId = keyId;
            PressEvent = new LogicalPressEvent(this);
            ReleaseEvent = new LogicalReleaseEvent(this);
        }
    }
    
    public class PhysicalDoubleThrowKeySet : KeySet<PhysicalDoubleThrowKey>
    {
        public override PhysicalDoubleThrowKey Create(int index)
            => new PhysicalDoubleThrowKey(logicalKeys[index], index);

        private LogicalDoubleThrowKeySet logicalKeys;

        public PhysicalDoubleThrowKeySet(LogicalDoubleThrowKeySet logicalKeys)
            : base(logicalKeys.MaxSize)
        {
            this.logicalKeys = logicalKeys;
        }
    }

    public class PhysicalDoubleThrowKey : DoubleThrowKey<PhysicalPressEvent, PhysicalReleaseEvent>
    {
        public override int KeyId { get; }

        public override PhysicalPressEvent PressEvent { get; }
        public override PhysicalReleaseEvent ReleaseEvent { get; }
        
        public PhysicalDoubleThrowKey(LogicalDoubleThrowKey logicalKey, int keyId)
        {
            KeyId = keyId;
            PressEvent = new PhysicalPressEvent(logicalKey, this);
            ReleaseEvent = new PhysicalReleaseEvent(logicalKey, this);
        }
    }
}
