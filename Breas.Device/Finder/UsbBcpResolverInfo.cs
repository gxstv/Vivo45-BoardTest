namespace Breas.Device.Finder
{
    public class UsbBcpResolverInfo : IResolverInfo
    {
        public string SerialNumber { get; set; }

        public string DeviceName { get; set; }

        public int VendorId { get; set; }

        public int ProductId { get; set; }

        public int InterfaceVersion { get; set; }

        public UsbBcpResolverInfo(string name, string serial, int interfaceVersion, int vendorId, int productId)
        {
            this.DeviceName = name;
            this.SerialNumber = serial;
            this.InterfaceVersion = interfaceVersion;
            this.VendorId = vendorId;
            this.ProductId = productId;
        }

        public bool Equals(IResolverInfo other)
        {
            if (other is UsbBcpResolverInfo)
            {
                var otherUsb = (UsbBcpResolverInfo)other;
                return otherUsb.ProductId == ProductId && otherUsb.VendorId == VendorId && other.SerialNumber == SerialNumber;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ("USB" + SerialNumber).GetHashCode();
        }
    }
}