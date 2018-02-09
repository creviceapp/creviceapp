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
        public IReadOnlyList<GestureMachineProfile> Profiles;

        public GestureMachineCluster(IReadOnlyList<GestureMachineProfile> gestureMachineProfiles)
        {
            Profiles = gestureMachineProfiles;
        }

        public void Run()
        {
            foreach (var profile in Profiles)
            {
                profile.GestureMachine.Run(profile.RootElement);
            }
        }

        public bool Input(Core.Events.IPhysicalEvent physicalEvent, System.Drawing.Point? point)
        {
            foreach (var profile in Profiles)
            {
                var eventIsConsumed = profile.GestureMachine.Input(physicalEvent, point);
                if (eventIsConsumed == true)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Input(Core.Events.IPhysicalEvent physicalEvent)
            => Input(physicalEvent, null);

        public void Reset()
        {
            foreach(var profile in Profiles)
            {
                profile.GestureMachine.Reset();
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            foreach (var profile in Profiles)
            {
                profile.GestureMachine.Dispose();
            }
        }

        ~GestureMachineCluster()
        {
            Dispose();
        }
    }
}
