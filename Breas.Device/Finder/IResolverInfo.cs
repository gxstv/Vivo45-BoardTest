using System;

namespace Breas.Device.Finder
{
    public interface IResolverInfo : IEquatable<IResolverInfo>
    {
        string SerialNumber { get; set;  }

        string DeviceName { get; set; }

    }
}