using log4net;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Breas.Device.Finder.Windows
{
    public class UsbBcpDeviceFinder : DeviceFinder
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UsbBcpDeviceFinder));

        public const string InterfaceVersionKey = "Measure point version";

        public override List<Communication.ICommunication> FindComms(HashSet<IResolverInfo> activeDevices)
        {
            var comms = new List<Communication.ICommunication>();
            for (int i = 0; i < 10; i++)
            {
                string port = "EZUSB-" + i;
                var resolverInfo = new UsbBcpResolverInfo()
                {
                    Port = port
                };
                if (activeDevices.Contains(resolverInfo))
                    continue;
                try
                {
                    UsbBcpCommunication comm = new UsbBcpCommunication(resolverInfo);
                    if (!comm.Connect())
                        continue;
                    Task.Delay(50);//delaying 50 ms can avoid some BCP errors
                    try {
                        resolverInfo.DeviceName = comm.Version.Substring(0, comm.Version.LastIndexOf(' '));
                        resolverInfo.InterfaceVersion = comm.GetMeasurePointValue(0);
                        resolverInfo.SerialNumber = comm.GetStringValue(0);
                        Log.InfoFormat("Found USB through BCP. Port: {0}, Device: {1}, Interface Version: {2}", port, resolverInfo.DeviceName, resolverInfo.InterfaceVersion);
                        comms.Add(comm);
                    } catch (BCPException e)
                    {
                        Log.Error("BCP Error", e);
                        //hopefully disconnecting will clear any errors
                        comm.Disconnect();
                    }
                }
                catch (DllNotFoundException e)
                {
                    Log.ErrorFormat("No bcp.dll", e);
                    throw e;
                }
            }
            return comms;
        }

        public async override Task<bool> Initialize()
        {
            return true;
        }
    }
}