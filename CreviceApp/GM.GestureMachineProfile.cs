using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crevice.GestureMachine
{
    using Crevice.Config;
    using Crevice.DSL;

    public class GestureMachineProfile
        : IDisposable
    {
        public readonly RootElement RootElement = new RootElement();

        public readonly GestureMachine GestureMachine;

        public readonly UserConfig UserConfig;

        public readonly string ProfileName;

        public GestureMachineProfile(string profileName)
        {
            ProfileName = profileName;
            var callbackManager = new CallbackManager();
            UserConfig = new UserConfig(callbackManager.Receiver);
            GestureMachine = new GestureMachine(UserConfig.Core, callbackManager);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GestureMachine?.Dispose();
        }

        ~GestureMachineProfile() => Dispose();
    }

    public class GestureMachineProfileManager
    {
        private readonly List<GestureMachineProfile> profiles = new List<GestureMachineProfile>();

        public IReadOnlyList<GestureMachineProfile> Profiles => profiles;

        public void DeclareProfile(string profileName)
            => profiles.Add(new GestureMachineProfile(profileName));
    }
}
