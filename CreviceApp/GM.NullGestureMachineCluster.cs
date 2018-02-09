using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.GestureMachine
{
    public class NullGestureMachineCluster : GestureMachineCluster
    {
        public NullGestureMachineCluster()
            : base(new List<GestureMachineProfile>())
        { }
    }
}
