using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Stroke
{
    using System.Linq;

    public class StrokeSequence : List<StrokeDirection>, IEquatable<StrokeSequence>
    {
        public StrokeSequence() : base() { }
        public StrokeSequence(int capacity) : base(capacity) { }
        public StrokeSequence(IEnumerable<StrokeDirection> dirs) : base(dirs) { }
        public StrokeSequence(params StrokeDirection[] dirs) : base(dirs) { }

        public bool Equals(StrokeSequence that)
        {
            if (that == null)
            {
                return false;
            }
            return (this.SequenceEqual(that));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }
            return Equals(obj as StrokeSequence);
        }

        public override int GetHashCode()
        {
            var hash = 0x00;
            foreach (var move in this)
            {
                hash = hash << 2;
                switch (move)
                {
                    case StrokeDirection.Up:
                        hash = hash | 0x00;
                        break;
                    case StrokeDirection.Down:
                        hash = hash | 0x01;
                        break;
                    case StrokeDirection.Left:
                        hash = hash | 0x02;
                        break;
                    case StrokeDirection.Right:
                        hash = hash | 0x03;
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            return hash;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var move in this)
            {
                switch (move)
                {
                    case StrokeDirection.Up:
                        sb.Append("U");
                        break;
                    case StrokeDirection.Down:
                        sb.Append("D");
                        break;
                    case StrokeDirection.Left:
                        sb.Append("L");
                        break;
                    case StrokeDirection.Right:
                        sb.Append("R");
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
