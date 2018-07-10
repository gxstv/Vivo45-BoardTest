using Breas.Device.Finder;
using System.Threading.Tasks;

namespace Breas.Device.Communication
{
    public interface ICommunication
    {
        IResolverInfo ResolverInfo { get; }

        bool Heartbeat { get; }

        bool StayConnected { get; }

        bool FailedInit { get; set; }

        bool Connect();

        bool Disconnect();

        byte[] GetMessage(int timeout = 5000);

        byte[] SendMessage(byte[] message, int timeout = 5000);

        Task<byte[]> GetMessageAsync(int timeout = 1000);

        Task<byte[]> SendMessageAsync(byte[] message, int timeout = 1000);
    }
}