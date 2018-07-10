using System;
using System.Linq;
using Breas.Device.Communication;

namespace Breas.Device.Vivo45.Messenger
{
    public class V45Packet
    {
        private class CRC16CITT
        {
            public enum InitialCrcValue { Zeros, NonZero1 = 0xffff, NonZero2 = 0x1D0F }

            private const ushort poly = 4129;
            private ushort[] table = new ushort[256];
            private ushort initialValue = 0;

            public ushort ComputeChecksum(byte[] bytes)
            {
                ushort crc = this.initialValue;
                for (int i = 0; i < bytes.Length; ++i)
                {
                    crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
                }
                return crc;
            }

            public byte[] ComputeChecksumBytes(byte[] bytes)
            {
                ushort crc = ComputeChecksum(bytes);
                return BitConverter.GetBytes(crc);
            }

            public CRC16CITT(InitialCrcValue initialValue)
            {
                this.initialValue = (ushort)initialValue;
                ushort temp, a;
                for (int i = 0; i < table.Length; ++i)
                {
                    temp = 0;
                    a = (ushort)(i << 8);
                    for (int j = 0; j < 8; ++j)
                    {
                        if (((temp ^ a) & 0x8000) != 0)
                        {
                            temp = (ushort)((temp << 1) ^ poly);
                        }
                        else
                        {
                            temp <<= 1;
                        }
                        a <<= 1;
                    }
                    table[i] = temp;
                }
            }
        }

        [Flags]
        public enum StatusFlags
        {
            Ack = 0x80,
            LastPacket = 0x40,
            AlarmAvalable = 0x08,
            EventAvalable = 0x10,
            None = 0x80
        }

        public enum V45Commands
        {
            cmdGetAvailableMeasPoints = 0x02,  /**< Get the available measurepoints */
            cmdGetMeasPointInfo       = 0x03,  /**< Get information about a measurepoint id */
            cmdGetMeasPointValue      = 0x04,  /**< Get the measurepoint value */
            cmdSetMeasPointValue      = 0x05,  /**< Set measurepoint value */
            cmdHandleMeasurePointSub  = 0x06,  /**< Handle measurepoint subscriptions */
            cmdGetAvailableSettings   = 0x07,  /**< Get a list of available settings */
            cmdGetSetting             = 0x08,  /**< Get setting value */
            cmdStepSetting            = 0x09,  /**< Step setting value */
            cmdSetSetting             = 0x0A,  /**< Set setting value */
            cmdCopyProfile            = 0x0B,  /**< Copy profile */
            cmdGetAvailableDeviceInfo = 0x0C,  /**< Get a list of available devices */
            cmdGetDeviceInfo          = 0x0D,  /**< Get information about the device information (read write access, explanatory text ) */
            cmdGetDeviceValue         = 0x0E,  /**< Get the value of the device information id */
            cmdSetDeviceValue         = 0x0F,  /**< Set the value of the device information id */
            cmdGetSettingInfo         = 0x10,  /**< Get information about a setting id */
            cmdSetSystemState         = 0x11,  /**<  */
            cmdHandleCalibration      = 0x12,  /**< Steps or sets value for calibration */
            cmdReadCalibrationValues  = 0x13,  /**< Reads calibration values */
            cmdStartStopTreatment     = 0x15,  /**< Starts or stops treatment */
            cmdGetUsageData           = 0x16,  /**< Get events */
            cmdGetActiveAlarms        = 0x17,  /**< Get a list of currently active alarms */
            cmdStream                 = 0x18,  /**< Stream */
            cmdHandleLogData          = 0x19,  /**< Handle log data */
            cmdEraseLogs              = 0x1A,  /**< Erase log files */
            cmdHandleTempCompensation = 0x1B,  /**< Handle temp compensation */

            cmdGetAllSettings         = 0x1C,  /**< Get all settings as a serialized allParameters struct */
            cmdHandleWifi             = 0x1D,  /**< Wrapper for wpa_ctrl command  SCAN, SCAN_RESULTS, ADD_NETWORK and such */
            cmdSendFile               = 0x1E,  /**< Send files command */

            cmdSetEncryptionKey       = 0x40,  /**< Set encryption key */
            cmdGetChallengeString     = 0x41,  /**< Get challenge string */
            cmdVerifyChallengeString  = 0x42,  /**< Verify challenge string */
            cmdEndSession             = 0x43,  /**< End authenticated session */

            cmdTestBuzzer             = 0x80,  /**<  */
            cmdNoCommand              = 0xFF   /**< No cmd */
        }

        public enum V45SystemStateRequests
        {
            pwrNO_CMD,        //Â§ No command
            pwrON,
            pwrOFF,
            pwrSTART,
            pwrSTOP,
            pwrCALIB_ON,      //Â§ Enter calibration state
            pwrCALIB_OFF,     //Â§ Exit calibration state
            pwrTEST_ON,       //Â§ Enter test state
            pwrTEST_OFF,      //Â§ Exit test state
        }

        private byte m_status = 0;
        private V45Commands m_command;
        private UInt16 m_length = 0;
        private UInt16 m_CRC;
        private byte[] m_payLoad;
        private byte m_sender = 0;
        private byte m_receiver = 0;
        private const byte m_headerSize = 6;
        private const byte m_CRCSize = 2;

        public V45Packet(V45Commands command)
        {
            this.m_status = 0;
            this.m_command = command;
            this.m_length = 6;
            this.m_CRC = 0;
            m_sender = 0;
            m_receiver = 0;
        }

        public V45Packet(V45Commands command, byte[] data, byte sender = 0, byte receiver = 0)
        {
            this.m_status = 0;
            this.m_command = command;
            this.m_payLoad = data;
            this.m_sender = sender;
            this.m_receiver = receiver;
            this.m_length = this.getLength();

            this.m_CRC = this.getCRC();
        }

        public V45Packet(byte[] indata, int start = 0, bool ignoreLength = false)
        {
            try
            {
                this.m_status = indata[start];
                this.m_command = (V45Commands)indata[start + 1];
                this.m_sender = indata[start + 2];
                this.m_receiver = indata[start + 3];
                this.m_length = BitConverter.ToUInt16(indata, start + 4);

                if (m_length != indata.Length && !ignoreLength)
                {
                    throw new LengthException("Length error. Suggested: " + this.m_length.ToString() + " Received: " + indata.Length.ToString());
                }

                this.m_payLoad = new byte[m_length - 8];

                Array.Copy(indata, start + 6, m_payLoad, 0, m_length - 8);
                //.AddRange(indata.Skip(6).Take(this.Length - 8).ToArray());

                CRC16CITT tmpCRC = new CRC16CITT(CRC16CITT.InitialCrcValue.Zeros);
                this.m_CRC = BitConverter.ToUInt16(indata, start + 6 + this.Payload.Length);
                UInt16 tmprc = tmpCRC.ComputeChecksum(indata.Skip(start).Take(this.Length - 2).ToArray());

                if (this.m_CRC != tmprc)//steve--skip calibration commmand
                {
                    throw new CrcException("CRC error. Computed: " + tmprc.ToString() + " Received: " + this.m_CRC.ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private ushort getLength()
        {
            ushort rtn = 0;

            rtn++; //Status
            rtn++; //Command
            rtn++; //Sender
            rtn++; //receiver
            rtn += 2; //length
            rtn += (ushort)this.m_payLoad.Length; //length
            rtn += 2; //CRC

            return rtn;
        }

        private ushort getCRC()
        {
            ushort rtn = 0;
            byte[] tmpdata = new byte[6 + m_payLoad.Length];

            tmpdata[0] = this.m_status;
            tmpdata[1] = (byte)this.m_command;
            tmpdata[2] = this.m_sender;
            tmpdata[3] = this.m_receiver;
            tmpdata[4] = (byte)this.m_length;
            tmpdata[5] = (byte)(this.m_length >> 8);
            Array.Copy(m_payLoad, 0, tmpdata, 6, m_payLoad.Length);

            CRC16CITT tmpCRC = new CRC16CITT(CRC16CITT.InitialCrcValue.Zeros);
            rtn = tmpCRC.ComputeChecksum(tmpdata);
            return rtn;
        }

        public byte[] toArray()
        {
            byte[] tmpdata = new byte[8 + m_payLoad.Length];

            tmpdata[0] = this.m_status;
            tmpdata[1] = (byte)this.m_command;
            tmpdata[2] = this.m_sender;
            tmpdata[3] = this.m_receiver;
            tmpdata[4] = (byte)this.m_length;
            tmpdata[5] = (byte)(this.m_length >> 8);
            Array.Copy(m_payLoad, 0, tmpdata, 6, m_payLoad.Length);

            this.m_CRC = getCRC();
            tmpdata[6 + m_payLoad.Length] = (byte)this.m_CRC;
            tmpdata[7 + m_payLoad.Length] = (byte)(this.m_CRC >> 8);

            return tmpdata;
        }

        public V45Commands Commad
        {
            get { return this.m_command; }
            set { this.m_command = value; }
        }

        public UInt16 Length
        {
            get { return this.m_length; }
            set { this.m_length = value; }
        }

        public UInt16 Checksum
        {
            get { return this.m_CRC; }
            set { this.m_CRC = value; }
        }

        public byte[] Payload
        {
            get { return this.m_payLoad.ToArray(); }
            set { m_payLoad = value; }
        }

        public int PayloadLength
        {
            get { return this.m_payLoad.Length; }
        }

        public byte Status
        {
            get { return this.m_status; }
            set { this.m_status = value; }
        }

        public byte Sender
        {
            get { return this.m_sender; }
            set { this.m_sender = value; }
        }

        public byte Receiver
        {
            get { return this.m_receiver; }
            set { this.m_receiver = value; }
        }

        public bool LastPacket
        {
            get 
            { 
                return ( Convert.ToBoolean((StatusFlags)this.m_status & StatusFlags.LastPacket) ); 
            }
        }
        
        public bool Ack
        {
            get 
            { 
                return (Convert.ToBoolean((StatusFlags)this.m_status & StatusFlags.Ack)); 
            }            
        }

        public bool Nack
        {
            get
            {
                return !(Convert.ToBoolean((StatusFlags)this.m_status & StatusFlags.Ack));
            }
        }

        public bool AlarmAvailable
        {
            get 
            { 
                return (Convert.ToBoolean((StatusFlags)this.m_status & StatusFlags.AlarmAvalable)); 
            }
        }

        public bool EventAvailable
        {
            get 
            { 
                return (Convert.ToBoolean((StatusFlags)this.m_status & StatusFlags.EventAvalable) ); 
            }
        }
    }
}