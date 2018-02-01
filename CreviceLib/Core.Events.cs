using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Events
{
    using Crevice.Core.Types;

    public sealed class EventIdGenerator
    {
        private static EventIdGenerator Instance = new EventIdGenerator();

        private EventIdGenerator() { }

        private readonly object lockObject = new object();

        private int eventId = 0;

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

    public abstract class KeyGroup
    {
        public abstract int KeyId { get; }
    }

    public abstract class LogicalKeyGroup : KeyGroup { }
    public abstract class PhysicalKeyGroup : KeyGroup { }


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

    public class LogicalSingleThrowKeys : Keys<LogicalSingleThrowKeys.LogicalKey>
    {
        public override LogicalKey Create(int index)
            => new LogicalKey(index);

        public LogicalSingleThrowKeys(int maxSize)
            : base(maxSize) { }

        public class LogicalKey : LogicalKeyGroup
        {
            public override int KeyId { get; }

            public readonly Fire FireEvent;

            public LogicalKey(int keyId)
            {
                KeyId = keyId;
                FireEvent = new Fire();
            }

            public class Fire : LogicalFireEvent<SingleThrowSwitch>
            {
                public Fire()
                    : base(EventIdGenerator.Generate()) { }
            }
        }
    }

    public class PhysicalSingleThrowKeys : Keys<PhysicalSingleThrowKeys.PhysicalKey>
    {
        public override PhysicalKey Create(int index)
            => new PhysicalKey(logicalKeys[index], index);

        private LogicalSingleThrowKeys logicalKeys;

        public PhysicalSingleThrowKeys(LogicalSingleThrowKeys logicalKeys)
            : base(logicalKeys.MaxSize)
        {
            this.logicalKeys = logicalKeys;
        }

        public class PhysicalKey : PhysicalKeyGroup
        {
            public override int KeyId { get; }

            public readonly Fire FireEvent;

            public PhysicalKey(LogicalSingleThrowKeys.LogicalKey logicalKey, int keyId)
            {
                KeyId = keyId;
                FireEvent = new Fire(logicalKey);
            }

            public class Fire : PhysicalFireEvent<SingleThrowSwitch>
            {
                public LogicalSingleThrowKeys.LogicalKey LogicalKey { get; }

                public override LogicalFireEvent<SingleThrowSwitch> LogicalEquivalentFireEvent
                    => LogicalKey.FireEvent;

                public Fire(LogicalSingleThrowKeys.LogicalKey logicalKey)
                    : base(EventIdGenerator.Generate())
                {
                    LogicalKey = logicalKey;
                }
            }
        }
    }

    public class LogicalDoubleThrowKeys : Keys<LogicalDoubleThrowKeys.LogicalKey>
    {
        public override LogicalKey Create(int index)
            => new LogicalKey(index);

        public LogicalDoubleThrowKeys(int maxSize)
            : base(maxSize) { }

        public class LogicalKey : LogicalKeyGroup
        {
            public override int KeyId { get; }

            public readonly Press PressEvent;
            public readonly Release ReleaseEvent;

            public LogicalKey(int keyId)
            {
                KeyId = keyId;
                PressEvent = new Press(this);
                ReleaseEvent = new Release(this);
            }

            public class Press : LogicalPressEvent<DoubleThrowSwitch>
            {
                public LogicalKey LogicalKey { get; }

                public override LogicalReleaseEvent<DoubleThrowSwitch> OppositeReleaseEvent
                    => LogicalKey.ReleaseEvent;

                public Press(LogicalKey logicalKey)
                    : base(EventIdGenerator.Generate())
                {
                    LogicalKey = logicalKey;
                }
            }

            public class Release : LogicalReleaseEvent<DoubleThrowSwitch>
            {
                public LogicalKey LogicalKey { get; }

                public override LogicalPressEvent<DoubleThrowSwitch> OppositePressEvent
                    => LogicalKey.PressEvent;

                public Release(LogicalKey logicalKey)
                    : base(EventIdGenerator.Generate())
                {
                    LogicalKey = logicalKey;
                }
            }
        }
    }

    public class PhysicalDoubleThrowKeys : Keys<PhysicalDoubleThrowKeys.PhysicalKey>
    {
        public override PhysicalKey Create(int index)
            => new PhysicalKey(logicalKeys[index], index);

        private LogicalDoubleThrowKeys logicalKeys;

        public PhysicalDoubleThrowKeys(LogicalDoubleThrowKeys logicalKeys)
            : base(logicalKeys.MaxSize)
        {
            this.logicalKeys = logicalKeys;
        }

        public class PhysicalKey : PhysicalKeyGroup
        {
            public override int KeyId { get; }

            public readonly Press PressEvent;
            public readonly Release ReleaseEvent;

            public PhysicalKey(LogicalDoubleThrowKeys.LogicalKey logicalKey, int keyId)
            {
                KeyId = keyId;
                PressEvent = new Press(logicalKey, this);
                ReleaseEvent = new Release(logicalKey, this);
            }

            public class Press : PhysicalPressEvent<DoubleThrowSwitch>
            {
                public LogicalDoubleThrowKeys.LogicalKey LogicalKey { get; }

                public PhysicalKey PhysicalKey { get; }

                public override LogicalPressEvent<DoubleThrowSwitch> LogicalEquivalentPressEvent
                    => LogicalKey.PressEvent;

                public override PhysicalReleaseEvent<DoubleThrowSwitch> OppositePhysicalReleaseEvent
                    => PhysicalKey.ReleaseEvent;

                public Press(LogicalDoubleThrowKeys.LogicalKey logicalKey, PhysicalKey physicalKey)
                    : base(EventIdGenerator.Generate())
                {
                    LogicalKey = logicalKey;
                    PhysicalKey = physicalKey;
                }
            }

            public class Release : PhysicalReleaseEvent<DoubleThrowSwitch>
            {
                public LogicalDoubleThrowKeys.LogicalKey LogicalKey { get; }

                public PhysicalKey PhysicalKey { get; }

                public override LogicalReleaseEvent<DoubleThrowSwitch> LogicalEquivalentReleaseEvent
                    => LogicalKey.ReleaseEvent;

                public override PhysicalPressEvent<DoubleThrowSwitch> OppositePhysicalPressEvent
                    => PhysicalKey.PressEvent;

                public Release(LogicalDoubleThrowKeys.LogicalKey logicalKey, PhysicalKey physicalKey)
                    : base(EventIdGenerator.Generate())
                {
                    LogicalKey = logicalKey;
                    PhysicalKey = physicalKey;
                }
            }
        }
    }
}
