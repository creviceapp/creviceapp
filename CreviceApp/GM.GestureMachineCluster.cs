using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;


namespace Crevice.GestureMachine
{
    public class GestureMachineCluster : IDisposable
    {
        public IReadOnlyList<GestureMachine> GestureMachines;

        public GestureMachineCluster(IReadOnlyList<GestureMachine> gestureMachines)
        {
            GestureMachines = gestureMachines;
        }
        public bool Input(Crevice.Core.Events.IPhysicalEvent physicalEvent, System.Drawing.Point? point)
        {
            foreach (var gm in GestureMachines)
            {
                var eventIsConsumed = gm.Input(physicalEvent, point);
                if (eventIsConsumed == true)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Input(Crevice.Core.Events.IPhysicalEvent physicalEvent)
            => Input(physicalEvent, null);

        public void Reset()
        {
            foreach(var gm in GestureMachines)
            {
                gm.Reset();
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            foreach (var gm in GestureMachines)
            {
                gm.Dispose();
            }
        }

        ~GestureMachineCluster()
        {
            Dispose();
        }
    }
}
