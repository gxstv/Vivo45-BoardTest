using Breas.Device.DeviceClass;
using Breas.Device.Finder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Breas.Device
{

    public class DeviceHandler : IDeviceHandler
    {
        public event DeviceConnected DeviceFound;

        public event DeviceDisconnected DeviceLost;

        private List<Product> _products;
        private List<Finder.IDeviceFinder> _deviceFinders;
        private bool _multiThreadedDiscovery;

        private List<Device> devices;
        
        public List<Finder.IDeviceFinder> DeviceFinders
        {
            get
            {
                return _deviceFinders;
            }
        }

        public List<Product> Products
        {
            get
            {
                return _products;
            }
        }

        public List<Device> Devices { get { return devices; } }

        public DeviceHandler(List<Product> products, List<Finder.IDeviceFinder> deviceFinders, bool multiThreadedDiscovery = false)
            : this(products.ToArray(), deviceFinders.ToArray(), multiThreadedDiscovery)
        {
        }

        public DeviceHandler(Product[] products, Finder.IDeviceFinder[] deviceFinders, bool multiThreadedDiscovery = false)
        {
            this.devices = new List<Device>();
            this._products = products.ToList();
            this._deviceFinders = deviceFinders.ToList();
            this._multiThreadedDiscovery = multiThreadedDiscovery;
            foreach (var df in deviceFinders)
                df.Products = products.ToList();
            Initialize();
        }

        private void Initialize()
        {
            foreach (var df in _deviceFinders.ToList())
            {
                try
                {
					if (!df.Initialize().Result)
                        _deviceFinders.Remove(df);
                } catch
                {
                    _deviceFinders.Remove(df);
                }
            }
        }

        /// <summary>
        /// Returns a list of newly found devices matching the filter
        ///
        /// </summary>
        public List<Device> FindDevices<TFilter>(Action<Device> init = null) where TFilter : DeviceClass.IDeviceClass
        {
            RemoveDisconnected();
            List<Device> newDevices = _multiThreadedDiscovery ? FindNewDevicesMultiThreaded() : FindNewDevices();
            foreach (var device in newDevices)
            {
                if (!device.GetType().GetTypeInfo().ImplementedInterfaces.Contains(typeof(TFilter)))
                {
                    continue;
                }
                bool succeeded = false;
                for (int i = 0; i < 3; i++)
                {
                    if (!device.Initialize())
                    {
                        if (device.Connected)
                        {
                            device.Disconnect();
                        }
                    } 
                    else
                    {
                        succeeded = true;
                        break;
                    }
                }
                if (!succeeded)
                {
                    //its up to the communication impl to react to failedint.(ex: USB resets the port)
                    device.Communication.FailedInit = true;
                    if (device.Connected)
                    {
                        device.Disconnect();
                    }
                    continue;
                }
                if (init != null)
                {
                    init(device);
                }
                if (device.Connected && !device.Communication.StayConnected)
                {
                    device.Disconnect();
                }
                else if (!device.Connected && device.Communication.StayConnected)
                {
                    device.Connect();
                }
                if (DeviceFound != null)
                {
                    DeviceFound(device);
                }
                Devices.Add(device);
            }
            return newDevices;
        }

        private List<Device> FindNewDevices()
        {
            var devicesSet = new HashSet<IResolverInfo>(devices.Select(s => s.Communication.ResolverInfo));
            List<Device> newDevices = new List<Device>();
            foreach (var finder in _deviceFinders)
            {
                try
                {
                    //we dont want a single device handler to break the rest of our device handlers so we wrap in a catch
                    newDevices.AddRange(finder.FindDevices(devicesSet));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return newDevices;
        }

        private List<Device> FindNewDevicesMultiThreaded()
        {
            var devicesSet = new HashSet<IResolverInfo>(devices.Select(s => s.Communication.ResolverInfo));
            List<Device> newDevices = new List<Device>();
            var tasks = new List<Task>();
            foreach (var finder in _deviceFinders)
            {
                tasks.Add(Task.Run(() => {
                    try
                    {
                        //we dont want a single device handler to break the rest of our device handlers so we wrap in a catch
                        newDevices.AddRange(finder.FindDevices(devicesSet));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            return newDevices;
        }

        private void RemoveDisconnected()
        {
            var dcedDevices = new List<Device>();
            foreach (var device in devices)
            {
                
                lock (device.HeartbeatLock)
                {
                    if (!device.Heartbeat)
                        dcedDevices.Add(device);
                }
            }
            foreach (var device in dcedDevices)
            {
                RemoveDevice(device);
            }
        }

        public List<Device> CheckForDisconnectedDevices()
        {
            //think about this impl
            return null;
        }

        public void RemoveDevice(Device device)
        {
            devices.Remove(device);
            try
            {
                device.Disconnect();
            }
            catch
            {

            }
            if (DeviceLost != null)
                DeviceLost(device);
        }
        
        /// <summary>
        /// Finds a list of all devices regardless of type
        /// </summary>
        /// <returns></returns>
        public List<Device> FindDevices(Action<Device> init = null)
        {
            return FindDevices<IDeviceClass>();
        }

        public Task<List<Device>> FindDevicesAsync<TFilter>(Action<Device> init = null) where TFilter : IDeviceClass
        {
            return Task.Run(() => FindDevices<TFilter>(init));
        }

        public Task<List<Device>> FindDevicesAsync(Action<Device> init = null)
        {
            return FindDevicesAsync<IDeviceClass>(init);
        }

        public void Cleanup()
        {
            foreach (var deviceFinder in _deviceFinders)
            {
                deviceFinder.Cleanup();
            }
        }

    }
}