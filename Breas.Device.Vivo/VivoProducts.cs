using Breas.Device.Finder;
using Breas.Device.Monitoring;
using System.Linq;

namespace Breas.Device.Vivo
{
    public class VivoProducts
    {
        public const string Vivo50DeviceName = "Vivo 50";
        public const string Vivo60DeviceName = "Vivo 60";

        public const string VivoColor = "#C41230";
        
        private static int[] vivo50InterfaceVersions = new int[] { 1, 2, 3, 4, 5, 6 ,9, 12};
        private static int[] vivo50V7InterfaceVersions = new int[] { 7 };
        private static int[] vivo50USInterfaceVersions = new int[] { 7 };
        private static int[] vivo60InterfaceVersions = new int[] { 8,10,13 };
        private static int[] vivo65InterfaceVersions = new int[] { 11 };
        private static int[] vivoVendorIds = new int[] { 0x1DAF };
        private static int[] vivoProductIds = new int[] { 20576, 5060 };

        public static readonly Product Vivo50 = new Product(
            "VIVO50",
            Vivo50DeviceName,
            VivoColor,
            typeof(VivoDevice),
            ResolveUSB);
        
        //Vivo50V7 adds some new measurements
        public static readonly Product Vivo50V7 = new Product(
            "VIVO50V7",
            Vivo50DeviceName,
            VivoColor,
            typeof(VivoDevice),
            ResolveUSB);

        private static int[] vivo50V7ProductIds = new int[] { /*TODO*/ };
        
        
        public static readonly Product Vivo50US = new Product(
            "VIVO50 US",
            Vivo50DeviceName,
            VivoColor,
            typeof(VivoDevice),
            ResolveUSB);

        private static int[] vivo50USProductIds = new int[] { /*TODO*/ };
                
        public static readonly Product Vivo60 = new Product(
            "VIVO60",
            Vivo60DeviceName,
            VivoColor,
            typeof(VivoDevice),
            ResolveUSB);

        public static readonly Product Vivo65 = new Product(
        "VIVO65",
        Vivo60DeviceName,
        VivoColor,
        typeof(VivoDevice),
        ResolveUSB);

        public static bool ResolveUSB(IResolverInfo resolverInfo)
        {
            var usbResolverInfo = resolverInfo as UsbResolverInfo;
            if (usbResolverInfo == null)
                return false;
            return vivoVendorIds.Contains(usbResolverInfo.VendorId)
                    && vivoProductIds.Contains(usbResolverInfo.ProductId);
        }

        public static Product GetRealVivoProduct(int interfaceVersion, string name)
        {

            //all interfaceversion are different now!
            if (vivo50InterfaceVersions.Contains(interfaceVersion) && name == Vivo50.ModelName)
                return Vivo50;
            else if (vivo50V7InterfaceVersions.Contains(interfaceVersion) && name == Vivo50.ModelName)
                return Vivo50V7;
            else if (vivo50USInterfaceVersions.Contains(interfaceVersion) && name == Vivo50US.ModelName)
                return Vivo50US;
            else if ((vivo65InterfaceVersions.Contains(interfaceVersion)||vivo60InterfaceVersions.Contains(interfaceVersion)) && (name == Vivo65.ModelName|| name == Vivo60.ModelName))
                return Vivo60;
            return null; //support base model if we cant find out what we are connected to
        }

        public static readonly Product[] All = new Product[]
        {
            Vivo60,Vivo50, Vivo50US, Vivo50V7,  Vivo65
        };

    }
}