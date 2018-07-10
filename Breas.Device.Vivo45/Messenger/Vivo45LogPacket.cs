using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breas.Device.Vivo45.Messenger
{
    public class Vivo45LogPacket
    {
        private enum StatusFlags
        {
            AckNack = 0x80,
            LastPacket = 0x40,
            AlarmAvalable = 0x08,
            None = 0x00
        }

        public enum Vivo45LogAction
        {
            sendLog = 0,     /**< Send log to external storage, this could be SD or extcom  */
            eraseLog = 1,    /**< Erase log */
            stopLog = 2,     /**< Stop current log process */
            continueLog = 3  /**< Continue current log process. Use from Extcom to get next packet */
        }

        public enum Vivo45LogLevel
        {
            Level1 = 1,
            Level2 = 2,
            Level3 = 3
        }

        private List<byte> m_data;
        private byte m_status;

        public Vivo45LogAction Action { get; set; }
        public Vivo45LogLevel LogLevel { get; set; }
        public UInt32 TimeStamp { get; set; }
        public byte[] data
        {
            get { return m_data.ToArray(); }
        }


        public bool LastPacket
        {
            get { return (((StatusFlags)this.m_status & StatusFlags.LastPacket) == StatusFlags.LastPacket); }
        }

        public bool Ack
        {
            get { return (((StatusFlags)this.m_status & StatusFlags.AckNack) == StatusFlags.AckNack); }
        }

        public Vivo45LogPacket(byte[] inData, byte status)
        {
            int i = 0;
            m_status = status;
            Action = (Vivo45LogAction)inData[i++];
            LogLevel = (Vivo45LogLevel)inData[i++];
            TimeStamp = BitConverter.ToUInt32(inData, i);
            i += 4;
            m_data = new List<byte>();
            m_data.AddRange(inData.Skip(i).ToArray());
        }

        public void AddData(byte[] inData)
        {
            m_data.AddRange(inData);
        }
    }
}
