using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Breas.Device.Finder.Windows
{
    public class MockDeviceFinder : DeviceFinder
    {
        private bool foundit = false;

        public override List<Communication.ICommunication> FindComms(HashSet<IResolverInfo> activeDevices)
        {
            List<Communication.ICommunication> result = new List<Communication.ICommunication>();
            if (foundit)
                return result;
            result.Add(new MockCommunication(new MockResolverInfo("Stephen", "123545")));
            result.Add(new MockCommunication(new MockResolverInfo("Moyer", "123542")));
            foundit = true;
            return result;
        }

        public async override Task<bool> Initialize()
        {
            return true;
        }
    }
}