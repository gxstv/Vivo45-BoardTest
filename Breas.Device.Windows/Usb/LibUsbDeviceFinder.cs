using Breas.Device.Communication;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Breas.Device.Finder.Windows.Usb
{
    public class LibUsbDeviceFinder : DeviceFinder
    {
        public const string ResolverInfoProp = "BreasResolverInfoProperty";

        public override List<ICommunication> FindComms(HashSet<IResolverInfo> activeDevices)
        {
            List<ICommunication> comms = new List<ICommunication>();

            foreach (UsbRegistry registry in UsbDevice.AllDevices)
            {
                var resolverInfo = new UsbResolverInfo(registry.Vid, registry.Pid, registry.SymbolicName);

                if (activeDevices.Contains(resolverInfo))
                {
                    continue;
                }

                ICommunication comm = new LibUsbCommunication(registry, resolverInfo);
                comms.Add(comm);
            }

            return comms;
        }

        public override void Cleanup()
        {
        }

        public override Task<bool> Initialize()
        {
            return Task.FromResult(true);
        }
    }
}