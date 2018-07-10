using System.Collections.Generic;
using System.Threading.Tasks;

namespace Breas.Device.Finder
{
    public interface IDeviceFinder
    {
        List<Product> Products { get; set; }

        Task<bool> Initialize();

        List<Device> CheckForDisconnectedDevices();

        /// <summary>
        /// Finds all of the active devices under this finder.
        /// </summary>
        /// <param name="activeDevices">A list of resolverinfos of the attached devices that allows you to filter</param>
        /// <returns></returns>
        List<Device> FindDevices(HashSet<IResolverInfo> activeDevices);

        Task<List<Device>> FindDevicesAsync(HashSet<IResolverInfo> activeDevices);

        void Cleanup();
    }
}