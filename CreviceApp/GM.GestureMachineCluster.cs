using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;


namespace Crevice.GestureMachine
{
    using System.Threading;

    public class GestureMachineCluster : IDisposable
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public IReadOnlyList<GestureMachineProfile> Profiles;

        public GestureMachineCluster(IReadOnlyList<GestureMachineProfile> profiles)
        {
            Profiles = profiles;
        }

        public void Run()
        {
            _semaphore.Wait();
            try
            {
                foreach (var profile in Profiles)
                {
                    profile.GestureMachine.Run(profile.RootElement);
                }
            }
            finally
            {
                _semaphore.Release();
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
            _semaphore.Wait();
            try
            {
                foreach (var profile in Profiles)
                {
                    profile.GestureMachine.Reset();
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Dispose()
        {
            _semaphore.Wait();
            try
            {
                GC.SuppressFinalize(this);
                foreach (var profile in Profiles)
                {
                    profile.GestureMachine.Dispose();
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        ~GestureMachineCluster()
        {
            Dispose();
        }
    }
}
