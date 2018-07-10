using Breas.Device.Communication;
using Breas.Device.Finder;
using Breas.Device.Monitoring;
using System;
using System.Reflection;

namespace Breas.Device
{
    /// <summary>
    /// Defines parameters about a device to allow us to match a discovered device to an <see cref="Breas.Device.Device"/>
    /// </summary>
    public class Product
    {
        private string modelName;
        private string displayName;
        private string colorHex;
        private Type deviceType;
        private Func<IResolverInfo, bool> resolveFunc;

        public string ModelName { get { return modelName; } }

        public string DisplayName { get { return displayName; } }

        public string ColorHex { get { return colorHex; } }
        
        public Type DeviceType { get { return deviceType; } }

        /// <summary>
        /// Initialize a new Product.
        /// </summary>
        /// <param name="modelName">The model name for this product</param>
        /// <param name="displayName">
        /// The display name for this product.
        /// Multiple products can share a displayName.
        /// </param>
        /// <param name="deviceType">The device type for the device</param>
        public Product(string modelName,
                       string displayName,
                       string colorHex,
                       Type deviceType,
                       Func<IResolverInfo, bool> resolveFunc)
        {
            this.modelName = modelName;
            this.displayName = displayName;
            this.colorHex = colorHex;
            this.resolveFunc = resolveFunc;
            if (typeof(Device).GetTypeInfo().IsAssignableFrom(deviceType.GetTypeInfo()))
                this.deviceType = deviceType;
            else
                throw new ArgumentException("deviceType");
        }

        public Device CreateDevice(ICommunication communication)
        {
            return (Device)Activator.CreateInstance(deviceType, this, communication);
        }

        public bool Matches(IResolverInfo info)
        {
            return resolveFunc(info);
        }
        
    }
}