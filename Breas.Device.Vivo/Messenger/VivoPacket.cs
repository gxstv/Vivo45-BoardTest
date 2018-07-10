using System;

namespace Breas.Device.Vivo.Messenger
{
    public struct VivoPacket
    {
        private const int PacketHeaderLength = 3;
        private const int PacketFooterLength = 1;

        public VivoPacket(VivoCommand command, byte[] data)
            : this()
        {
            Command = command;
            Data = data;
        }

        public VivoPacket(byte[] data)
            : this()
        {
            Data = null;
            Command = default(VivoCommand);

            if (data == null)
            {
                return;
            }

            // Process received response
            Command = (VivoCommand)data[0];

            if (!IsShortCommand(Command))
            {
                int receivedLength = ((ushort)data[1] << 8) + data[2];
                byte receivedCheckSum = data[receivedLength + PacketHeaderLength];

                // Check calculated checksum against received checksum
                byte calculatedCheckSum = 0;

                for (int i = 0; i < receivedLength + PacketHeaderLength; i++)
                {
                    calculatedCheckSum += data[i];
                }

                if (receivedCheckSum == calculatedCheckSum)
                {
                    Data = new byte[receivedLength];
                    Array.Copy(data, PacketHeaderLength, Data, 0, receivedLength);
                }
            }
            else
            {
                Data = new byte[0];
            }
        }

        public VivoCommand Command { get; private set; }

        public byte[] Data { get; private set; }

        public byte[] ToArray()
        {
            if (IsShortCommand(Command))
            {
                byte[] packet = new byte[1];

                packet[0] = (byte)Command;

                return packet;
            }
            else
            {
                int dataLength = Data.Length;
                byte[] packet = new byte[dataLength + PacketHeaderLength + PacketFooterLength];
                byte checkSum = 0;

                // Build packet header
                packet[0] = (byte)Command;
                packet[1] = (byte)((dataLength >> 8) & 0xFF); // MSB Length
                packet[2] = (byte)(dataLength & 0xFF);        // LSB Length

                // Calculate checksum on packet header
                checkSum += packet[0];
                checkSum += packet[1];
                checkSum += packet[2];

                // Add data to packet
                for (int i = 0; i < dataLength; i++)
                {

                    packet[i + PacketHeaderLength] = Data[i]; // Copy packet content
                    checkSum += Data[i ];                     // Calculate checksum on body
                }

                packet[dataLength + PacketHeaderLength] = checkSum; // Build packet footer

                return packet;
            }
        }

        private bool IsShortCommand(VivoCommand command)
        {
            return (byte)command < 128;
        }

        private byte Rotate(byte data, int x)
        {
            while (x > 0)
            {
                int tmp;
                tmp = (byte)(data >> 7 & 1);
                data = (byte)(data << 1);
                data = (byte)(data | tmp);
                x--;
            }
            return data;
        }

        public void decode(int shift)
        {
            
            for (int i = 0; i < this.Data.Length; i++)
            {
                this.Data[i] = Rotate(this.Data[i], shift);
            }
        }
}
}
