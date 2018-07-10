using Bonjour;
using Breas.Device.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Breas.Device.Finder.Shared;

namespace Breas.Device.Finder.Windows
{
    public class BonjourDeviceFinder : DeviceFinder
    {

        private const string SerialNumberKey = "serial";
        private const string DeviceNameKey = "device";

        private List<NetworkCommunication> foundComms = new List<NetworkCommunication>();

        private DNSSDEventManager eventManager;
        private DNSSDService service;
        private DNSSDService browser;

        public BonjourDeviceFinder()
        {
        }

        public async override Task<bool> Initialize()
        {
            this.service = new DNSSDService();
            this.eventManager = new DNSSDEventManager();
            this.eventManager.ServiceFound += new _IDNSSDEvents_ServiceFoundEventHandler(this.ServiceFound);
            this.eventManager.ServiceLost += new _IDNSSDEvents_ServiceLostEventHandler(this.ServiceLost);
            this.eventManager.ServiceResolved += new _IDNSSDEvents_ServiceResolvedEventHandler(this.ServiceResolved);
            this.eventManager.OperationFailed += new _IDNSSDEvents_OperationFailedEventHandler(this.OperationFailed);
            this.eventManager.AddressFound += EventManager_AddressFound;

            //NetworkChange.NetworkAddressChanged += OnNetworkAddressChanged;
            browser = service.Browse(0, 0, "_Breas._tcp", null, eventManager);

            return true;
        }
        
        private void OnNetworkAddressChanged(object sender, EventArgs e)
        {
            browser.Stop();
            browser = service.Browse(0, 0, "_Breas._tcp", null, eventManager);
        }

        public override List<ICommunication> FindComms(HashSet<IResolverInfo> activeDevices)
        {
            //Bonjour is very tricky to keep the devices in sync
            var comms = new List<NetworkCommunication>();
            foreach (var comm in foundComms)
            {
                if (comms.Any(c => c.NetworkResolverInfo.IpAddress == comm.NetworkResolverInfo.IpAddress))
                    continue;
                comms.Add(comm);
            }
            var result = new List<ICommunication>();
            foreach (var comm in comms)
            {
                if (!activeDevices.Contains(comm.NetworkResolverInfo) && comm.NetworkResolverInfo.Active)
                    result.Add(comm);
            }
            return result;
        }

        public void ServiceFound(DNSSDService sref, DNSSDFlags flags, uint ifIndex, string serviceName, string regType, string domain)
        {
            if (foundComms.Any(r => r.NetworkResolverInfo.Name == serviceName)) //TODO we have an issue with same device discovered many times
                return;
            NetworkCommunication comm = new NetworkCommunication(new NetworkResolverInfo());

            comm.NetworkResolverInfo.InterfaceIndex = ifIndex;
            comm.NetworkResolverInfo.Name = serviceName;
            comm.NetworkResolverInfo.Type = regType;
            comm.NetworkResolverInfo.Domain = domain;
            comm.NetworkResolverInfo.Refs = 1;
            comm.NetworkResolverInfo.Connected = true;

            foundComms.Add(comm);
            comm.NetworkResolverInfo.Resolver = service.Resolve(0, comm.NetworkResolverInfo.InterfaceIndex, comm.NetworkResolverInfo.Name, comm.NetworkResolverInfo.Type, comm.NetworkResolverInfo.Domain, eventManager);
        }

        public void ServiceLost(DNSSDService sref, DNSSDFlags flags, uint ifIndex, string serviceName, string regType, string domain)
        {
            var itemToRemove = foundComms.SingleOrDefault(r => r.NetworkResolverInfo.Name == serviceName);
            if (itemToRemove != null)
            {
                if (itemToRemove.NetworkResolverInfo.Resolver != null)
                {
                    (itemToRemove.NetworkResolverInfo.Resolver as DNSSDService).Stop();
                }
                itemToRemove.NetworkResolverInfo.Connected = false;
                itemToRemove.NetworkResolverInfo.Active = false;
                foundComms.Remove(itemToRemove);
            }
        }

        public void ServiceResolved(DNSSDService sref, DNSSDFlags flags, uint ifIndex, string serviceName, string hostName, ushort port, TXTRecord txtRecord)
        {
            var comm = foundComms.SingleOrDefault(r => r.NetworkResolverInfo.Resolver == sref);

            comm.NetworkResolverInfo.InterfaceIndex = ifIndex;
            comm.NetworkResolverInfo.HostName = hostName;
            comm.NetworkResolverInfo.Port = port;

            if (txtRecord != null)
            {
                try
                {
                    comm.NetworkResolverInfo.DeviceName = Encoding.ASCII.GetString((byte[])txtRecord.GetValueForKey(DeviceNameKey));
                    comm.NetworkResolverInfo.SerialNumber = Encoding.ASCII.GetString((byte[])txtRecord.GetValueForKey(SerialNumberKey));
                }
                catch (Exception e)
                {
                    //Log.Error("Error reading address info", e);
                }
            }

            (comm.NetworkResolverInfo.Resolver as DNSSDService).Stop();

            comm.NetworkResolverInfo.Resolver = sref.GetAddrInfo(flags, ifIndex, DNSSDAddressFamily.kDNSSDAddressFamily_IPv4, hostName, this.eventManager);
        }

        private void EventManager_AddressFound(DNSSDService service, DNSSDFlags flags, uint ifIndex, string hostname, DNSSDAddressFamily addressFamily, string address, uint ttl)
        {
            var comm = foundComms.SingleOrDefault(r => r.NetworkResolverInfo.Resolver == service);
            if (comm != null)
            {
                comm.NetworkResolverInfo.IpAddress = address;
                comm.NetworkResolverInfo.Active = true;
                (comm.NetworkResolverInfo.Resolver as DNSSDService).Stop();
                comm.NetworkResolverInfo.Resolver = null;
            }
        }

        public void OperationFailed(DNSSDService sref, DNSSDError error)
        {
            //Log.ErrorFormat("Bonjour Operation Failed: {0}", error);
        }
    }
}