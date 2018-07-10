using Breas.Device.Communication;
using System;
using System.Threading.Tasks;

namespace Breas.Device
{
    public delegate void DeviceConnectionChanged(bool connected);

    public abstract class Device
    {
        
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// The Product of this device
        /// </summary>
        public Product Product { get; protected set; }

        /// <summary>
        /// This devices communication class
        /// </summary>
        public ICommunication Communication { get; protected set; }

        /// <summary>
        /// Weak way to check if the device is connected
        /// </summary>
        public bool Connected { get; protected set; }

        public object HeartbeatLock { get; private set; }

        /// <summary>
        /// Strong way to check if device is connected.
        /// Makes a request to the devices Communication class for data
        /// </summary>
        public abstract bool Heartbeat { get; }

        protected Device(Product product, ICommunication communication)
        {
            this.Product = product;
            this.Communication = communication;
            this.HeartbeatLock = new object();
        }

        /// <summary>
        /// Some devices need to connect to the device to pull info.
        /// Use this method for that. You can connect to the device without disconnecting,
        /// the handler will disconnect for you. It's preferred the implementation does 
        /// not disconnect so we can use that open connection to read the measurepoints
        /// </summary>
        /// <returns>false if initialization failed</returns>
        public virtual bool Initialize()
        {
            return true;
        }

        public Task<bool> ConnectAsync()
        {
            return Task.Factory.StartNew(() => Connect());
        }

        public Task<bool> DisconnectAsync()
        {
            return Task.Factory.StartNew(() => Disconnect());
        }

        /// <summary>
        /// Synchronously connects to the device
        /// </summary>
        /// <returns>If the device connection was successful</returns>
        public bool Connect()
        {
            return Connected = Communication.Connect();
        }

        /// <summary>
        /// Synchronously disconnected from the device
        /// </summary>
        /// <returns>
        /// If the device disconnection was successful. Connected will be set to false no matter
        /// what the result of the disconnection was
        /// </returns>
        public bool Disconnect()
        {
            Connected = false;
            return Communication.Disconnect();
        }
    }
}