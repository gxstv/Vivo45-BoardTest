using Breas.Device.Communication;
using log4net;
using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Breas.Device.Finder.Windows
{
    public class V45NetworkCommunication : IVivo45Communication
    {        
        private const int Timeout = 10000;

        /// <summary>
        /// The ping timeout needs to be lower so we do not block the
        /// device finding thread
        /// </summary>
        private const int PingTimeout = 500;

        private static readonly ILog Log = LogManager.GetLogger(typeof(V45NetworkCommunication));

        private Socket commandsSocket;
        private Socket streamSocket;
        private Ping ping;

        public IResolverInfo ResolverInfo
        {
            get;
            set;
        }

        public bool Heartbeat
        {
            get
            {
                return true;
                int count = 0;
                while (count++ < 5)
                {
                    try {
                        if (ping.Send(NetworkResolverInfo.IpAddress, PingTimeout).Status == IPStatus.Success)
                            return true;
                    } catch
                    {
                    }
                    Task.Delay(50).Wait();
                }
                return false;
            }
        }

        public NetworkResolverInfo NetworkResolverInfo
        {
            get { return ResolverInfo as NetworkResolverInfo; }
        }

        public V45NetworkCommunication(NetworkResolverInfo networkInfo)
        {
            this.ping = new Ping();
            this.ResolverInfo = networkInfo;
        }

        public bool Connect()
        {
            if (commandsSocket != null)
                Disconnect();
            try
            {
                commandsSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                commandsSocket.ReceiveTimeout = Timeout;
                commandsSocket.SendTimeout = Timeout;
                streamSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                streamSocket.ReceiveTimeout = Timeout;
                streamSocket.SendTimeout = Timeout;
                // Connect the socket to the remote endpoint. Catch any errors.
                if (!commandsSocket.Connected)
                {
                    commandsSocket.Connect(NetworkResolverInfo.IpAddress, NetworkResolverInfo.Port);
                }
                if (!streamSocket.Connected)
                {
                    streamSocket.Connect(NetworkResolverInfo.IpAddress, NetworkResolverInfo.Port + 1);
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Error("Error opening Vivo45 network sockets", e);
            }
            return false;
        }

        public bool Disconnect()
        {
            try
            {

                  
            commandsSocket.Disconnect(true);
            streamSocket.Disconnect(true);
            }
            catch (Exception)
            {

                
            } 
            commandsSocket = null;
            streamSocket = null;
            return true;
        }

        public byte[] GetMessage(int timeout = Timeout)
        {           
            byte[] bytes = new byte[commandsSocket.ReceiveBufferSize];
            Console.WriteLine("Available b4: " + commandsSocket.Available.ToString());
            
            if (commandsSocket.Connected)
            {
                commandsSocket.ReceiveTimeout = timeout;

                int rcvd = commandsSocket.Receive(bytes);
                UInt16 expectedsize = BitConverter.ToUInt16(bytes, 4);
                if (expectedsize > bytes.Length)
                {
                    byte[] tmp = new byte[expectedsize];
                    Array.Copy(bytes, 0, tmp, 0, rcvd);
                    bytes = tmp;
                }
                while(rcvd < expectedsize)
                {
                    rcvd += commandsSocket.Receive(bytes,rcvd, (expectedsize-rcvd), SocketFlags.None );
                }
                byte[] actualBytes = new byte[rcvd];
                Array.Copy(bytes, 0, actualBytes, 0, rcvd);
                Console.WriteLine("Available after: " + commandsSocket.Available.ToString() + " Received: " + rcvd.ToString());
                return actualBytes;
            }
            throw new DisconnectedException("Disconnected from command socket");
        }

        public byte[] SendMessage(byte[] message, int timeout = Timeout)
        {
            commandsSocket.SendTimeout = timeout;
            try
            {
                if (commandsSocket.Connected)
                {
                    int sent = commandsSocket.Send(message);
                    if (sent == message.Length)
                    {
                        return GetMessage(timeout);
                    }
                    throw new CommunicationException("Error sending message, actual sent != packet length");
                }
                throw new DisconnectedException("Disconnected from command socket");
            }
            catch (Exception e)
            {
                throw new CommunicationException("Error sending message", e);
            }
        }

              
        public Task<byte[]> GetMessageAsync(int timeout = Timeout)
        {
            return Task.Factory.StartNew(() => GetMessage(timeout));
        }

        public Task<byte[]> SendMessageAsync(byte[] message, int timeout = Timeout)
        {
            return Task.Factory.StartNew(() => SendMessage(message));
        }

        public byte[] GetMessageStreamComm(int timeout = Timeout)
        {
            byte[] bytes = new byte[streamSocket.ReceiveBufferSize];
            if (streamSocket.Connected)
            {
                streamSocket.ReceiveTimeout = timeout;

                int rcvd = streamSocket.Receive(bytes);
                UInt16 expectedsize = BitConverter.ToUInt16(bytes, 4);
                if (expectedsize > bytes.Length)
                {
                    byte[] tmp = new byte[expectedsize];
                    Array.Copy(bytes, 0, tmp, 0, rcvd);
                    bytes = tmp;
                }
                while (rcvd < expectedsize)
                {
                    rcvd += streamSocket.Receive(bytes, rcvd, (expectedsize - rcvd), SocketFlags.None);
                }               
                return bytes;
            }
            throw new CommunicationException("Disconnected from stream socket");
        }

        public Task<byte[]> GetMessageStreamCommAsync(int timeout = Timeout)
        {
            return Task.Factory.StartNew(() => GetMessageStreamComm(timeout));
        }
    }
}