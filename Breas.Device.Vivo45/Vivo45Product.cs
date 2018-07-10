using Breas.Device.Finder;
using System;

namespace Breas.Device.Vivo45
{
    public class Vivo45Product
    {
        public const string Vivo45Color = "#808080"; // Gray
        public const string Vivo45LSColor = "#1E90FF"; // DodgerBlue
        public const string Nippy4Color = "#00008B"; // DarkBlue
        public const string Nippy4PlusColor = "#808080"; // Gray
        public const string Vivo35Color = "#800080"; // Purple

        public static readonly Product Vivo45 = new Product(
            "VIVO45",
            "Vivo 45",
            Vivo45Color,
            typeof(Vivo45Device),
            ResolveVivo45);

        public static readonly Product Vivo45LS = new Product(
            "VIVO45LS",
            "Vivo 45 LS",
            Vivo45LSColor,
            typeof(Vivo45Device),
            ResolveVivo45LS);

        public static readonly Product Nippy4 = new Product(
            "NIPPY4",
            "NIPPY 4",
            Nippy4Color,
            typeof(Vivo45Device),
            ResolveNippy4);

        public static readonly Product Nippy4Plus = new Product(
            "NIPPY4+",
            "NIPPY 4+",
            Nippy4PlusColor,
            typeof(Vivo45Device),
            ResolveNippy4Plus);

        public static readonly Product Vivo35 = new Product(
            "VIVO35",
            "Vivo 35",
            Vivo35Color,
            typeof(Vivo45Device),
            ResolveVivo35);

        private static bool ResolveVivo45(IResolverInfo resolverInfo)
        {
            return ResolveVivo(resolverInfo, Vivo45);
        }

        private static bool ResolveVivo45LS(IResolverInfo resolverInfo)
        {
            return ResolveVivo(resolverInfo, Vivo45LS);
        }

        private static bool ResolveNippy4(IResolverInfo resolverInfo)
        {
            return ResolveVivo(resolverInfo, Nippy4);
        }

        private static bool ResolveNippy4Plus(IResolverInfo resolverInfo)
        {
            return ResolveVivo(resolverInfo, Nippy4Plus);
        }

        private static bool ResolveVivo35(IResolverInfo resolverInfo)
        {
            return ResolveVivo(resolverInfo, Vivo35);
        }

        private static bool ResolveVivo(IResolverInfo resolverInfo, Product product)
        {
            if (resolverInfo is NetworkResolverInfo)
            {
                var networkInfo = resolverInfo as NetworkResolverInfo;

                return product.ModelName.Equals(networkInfo.DeviceName, StringComparison.OrdinalIgnoreCase);
            }
            else if (resolverInfo is UsbResolverInfo)
            {
                var usbInfo = resolverInfo as UsbResolverInfo;
                return usbInfo.VendorId == 0x1DAF && usbInfo.ProductId == 45;
            }

            return false;
        }

        public static readonly Product[] All = new Product[]
        {
            Vivo45, Vivo45LS, Nippy4, Nippy4Plus, Vivo35
        };
    }
}