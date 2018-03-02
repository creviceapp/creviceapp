using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;


namespace Crevice.GestureMachine
{
    using Crevice.Core.FSM;

    public class GestureMachineCluster 
        : IGestureMachine, IDisposable
    {
        public readonly IReadOnlyList<GestureMachineProfile> Profiles;

        public GestureMachineCluster(IReadOnlyList<GestureMachineProfile> profiles)
        {
            Profiles = profiles;
            Console.WriteLine($"{profiles.Count}");
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
            foreach (var profile in Profiles)
            {
                profile.GestureMachine.Reset();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var profile in Profiles)
                {
                    profile.Dispose();
                }
            }
        }

        ~GestureMachineCluster() => Dispose(false);
    }
}
