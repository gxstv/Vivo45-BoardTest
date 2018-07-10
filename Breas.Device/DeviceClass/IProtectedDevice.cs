using System;

namespace Breas.Device.DeviceClass
{
    public interface IProtectedDevice : IDeviceClass
    {
        string LastPassword { get; }

        bool LoggedIn { get; }

        bool Login(string password);

    }
}

