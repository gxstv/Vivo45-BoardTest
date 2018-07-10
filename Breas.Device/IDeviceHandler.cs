using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Breas.Device.DeviceClass;
using Breas.Device.Finder;

namespace Breas.Device
{
    public delegate void DeviceDisconnected(Device device);

    public delegate void DeviceConnected(Device device);

    public interface IDeviceHandler
    {
        List<IDeviceFinder> DeviceFinders { get; }
        List<Device> Devices { get; }
        List<Product> Products { get; }

        event DeviceConnected DeviceFound;
        event DeviceDisconnected DeviceLost;

        List<Device> CheckForDisconnectedDevices();
        List<Device> FindDevices(Action<Device> init = null);
        List<Device> FindDevices<TFilter>(Action<Device> init = null) where TFilter : IDeviceClass;
        Task<List<Device>> FindDevicesAsync(Action<Device> init = null);
        Task<List<Device>> FindDevicesAsync<TFilter>(Action<Device> init = null) where TFilter : IDeviceClass;
        void RemoveDevice(Device device);
        void Cleanup();
    }
}