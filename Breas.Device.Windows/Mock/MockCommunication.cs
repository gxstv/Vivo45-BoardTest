using Breas.Device.Communication;
using System;

namespace Breas.Device.Finder.Windows
{
    internal class MockCommunication : ICommunication
    {
        private IResolverInfo _resolverInfo;

        public IResolverInfo ResolverInfo
        {
            get { return _resolverInfo; }
        }

        public bool Heartbeat
        {
            get { return true; }
        }

        public bool FailedInit { get; set; }

        public bool StayConnected
        {
            get
            {
                return true;
            }
        }

        public MockCommunication(IResolverInfo resolverInfo)
        {
            this._resolverInfo = resolverInfo;
        }

        public bool Connect()
        {
            return true;
        }

        public bool Disconnect()
        {
            return true;
        }

        public byte[] GetMessage(int timeout = 1000)
        {
            throw new NotImplementedException();
        }

        public byte[] SendMessage(byte[] message, int timeout = 1000)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<byte[]> GetMessageAsync(int timeout = 1000)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<byte[]> SendMessageAsync(byte[] message, int timeout = 1000)
        {
            throw new NotImplementedException();
        }
    }
}