namespace Breas.Device.Finder
{
    public class MockResolverInfo : IResolverInfo
    {
        public string SerialNumber
        {
            get;
            set;
        }

        public string DeviceName
        {
            get;
            set;
        }

        public MockResolverInfo(string name, string serial)
        {
            this.DeviceName = name;
            this.SerialNumber = serial;
        }

        public bool Equals(IResolverInfo other)
        {
            return false;
        }
    }
}