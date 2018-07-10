using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breas.Device.Finder
{
    public class UsbResolverInfo : IResolverInfo
    {
        public string SerialNumber { get; set; }

        public string DeviceName { get; set; }

        public int VendorId { get; set; }

        public int ProductId { get; set; }

        public string SymbolicName { get; set; }

        public bool V45Endpoints { get; set; }
        
        public UsbResolverInfo(int vendorId, int productId, string symbolicName)
        {
            this.VendorId = vendorId;
            this.ProductId = productId;
            this.SymbolicName = symbolicName;
        }

        public bool Equals(IResolverInfo other)
        {
            if (other is UsbResolverInfo)
            {
                var otherUsb = (UsbResolverInfo)other;
                return otherUsb.ProductId == ProductId && otherUsb.VendorId == VendorId && otherUsb.SymbolicName == otherUsb.SymbolicName;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ("USB" + SymbolicName).GetHashCode();
        }
    }
}
