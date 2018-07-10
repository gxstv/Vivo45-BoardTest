using Breas.Device.DeviceClass;
using Breas.Device.Logs;

namespace Breas.Device.DeviceTypes
{
    /// <summary>
    /// Provides functionality for a device that has logs
    /// </summary>
    public interface ILogDevice : IDeviceClass
    {

        ILogReader LogReader { get; }

        string GetSerialNumber();

        string GetFirmware();

        string GetModel();
    }
}