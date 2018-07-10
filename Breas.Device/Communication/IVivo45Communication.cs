using System.Threading.Tasks;

namespace Breas.Device.Communication
{
    public interface IVivo45Communication : ICommunication
    {
        byte[] GetMessageStreamComm(int timeout = 1000);

        Task<byte[]> GetMessageStreamCommAsync(int timeout = 1000);
    }
}