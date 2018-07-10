using Breas.Device.Communication;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Breas.Device.Finder
{
    public abstract class DeviceFinder : IDeviceFinder
    {
        public virtual List<Product> Products { get; set; }

        public List<Device> FindDevices(HashSet<IResolverInfo> activeDevices)
        {
            List<Device> devices = new List<Device>();
            List<ICommunication> results = FindComms(activeDevices);
            if (results == null)
            {
                return devices;
            }
            foreach (var findResult in results)
            {
                foreach (var product in Products)
                {
                    //if (findResult.ResolverInfo.Equals("NIPPY4"))
                    //{
                    //    devices.Add(product.CreateDevice(findResult));
                    //    break;
                    //}

                    //if (findResult.ResolverInfo.DeviceName.Equals("NIPPY4+"))
                    //{
                    //    devices.Add(product.CreateDevice(findResult));
                    //    break;
                    //}

                    if (product.Matches(findResult.ResolverInfo))
                    {
                        devices.Add(product.CreateDevice(findResult));
                        break;
                    }
                }
            }
            return devices;
        }

        public Task<List<Device>> FindDevicesAsync(HashSet<IResolverInfo> activeDevices)
        {
            return Task.Factory.StartNew(() => FindDevices(activeDevices));
        }

        public List<Device> CheckForDisconnectedDevices()
        {
            //think about this implementation
            return new List<Device>();
        }

        public virtual void Cleanup()
        {
        }

        public abstract Task<bool> Initialize();

		public abstract List<ICommunication> FindComms(HashSet<IResolverInfo> activeDevices);
    }
}