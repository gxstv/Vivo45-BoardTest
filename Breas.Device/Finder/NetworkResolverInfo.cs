namespace Breas.Device.Finder
{
    public class NetworkResolverInfo : IResolverInfo
    {
        public string Name { get; set; }

        public int Port { get; set; }

        public uint InterfaceIndex { get; set; }

        public string Type { get; set; }

        public string Domain { get; set; }

        public int Refs { get; set; }

        public bool Active { get; set; }

        public string HostName { get; set; }

        public string IpAddress { get; set; }

        public object Resolver { get; set; }

        public string SerialNumber { get; set; }

        public string DeviceName { get; set; }

        public bool Equals(IResolverInfo other)
        {
            var otherNetwork = other as NetworkResolverInfo;
            return otherNetwork != null && otherNetwork.IpAddress == IpAddress && otherNetwork.Port == Port;
        }

        public bool Same (string address, int port)
        {
            return address == IpAddress && port == Port;
        }

        public override int GetHashCode()
        {
            return ("NETWORK" + IpAddress + Port).GetHashCode();
        }

        public bool Connected { get; set; }
    }
}