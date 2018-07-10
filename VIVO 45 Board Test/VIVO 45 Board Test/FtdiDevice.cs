#define FT232H

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FTD2XX_NET;

namespace VIVO_45_Board_Test
{
    class FtdiDevice
    {
        protected FTDI myFtdiDevice;
        protected FTDI.FT_STATUS ftStatus;

        const int BufferSize = 500;
        const int MaxFtdiDevices = 20;
        protected const int ReceiveRetries = 5000;

        protected uint numBytesToRead;
        protected uint numBytesRead;
        protected uint numBytesToSend;
        protected uint numBytesSent;
        protected byte[] outputBuffer = new byte[BufferSize];
        protected byte[] inputBuffer = new byte[BufferSize];
        protected byte[] receivedBuffer = new byte[BufferSize];

        protected uint locId;

        //I2C constants
        const byte I2C_Dir_SDAin_SCLin = 0x00;
        const byte I2C_Dir_SDAin_SCLout = 0x01;
        const byte I2C_Dir_SDAout_SCLout = 0x03;
        const byte I2C_Dir_SDAout_SCLin = 0x02;
        const byte I2C_Data_SDAhi_SCLhi = 0x03;
        const byte I2C_Data_SDAlo_SCLhi = 0x01;
        const byte I2C_Data_SDAlo_SCLlo = 0x00;
        const byte I2C_Data_SDAhi_SCLlo = 0x02;
        const byte MSB_FALLING_EDGE_CLOCK_BYTE_IN = 0x24;
        const byte MSB_RISING_EDGE_CLOCK_BYTE_IN = 0x20;
        const byte MSB_FALLING_EDGE_CLOCK_BYTE_OUT = 0x11;
        const byte MSB_DOWN_EDGE_CLOCK_BIT_IN = 0x26;
        const byte MSB_UP_EDGE_CLOCK_BYTE_IN = 0x20;
        const byte MSB_UP_EDGE_CLOCK_BYTE_OUT = 0x10;
        const byte MSB_RISING_EDGE_CLOCK_BIT_IN = 0x22;
        const byte MSB_FALLING_EDGE_CLOCK_BIT_OUT = 0x13;
        const int ClockDivisor = 199;
        byte GPIO_Low_Dat = 0;
        byte GPIO_Low_Dir = 0;

        bool i2cAck;

        public FtdiDevice()
        {
            myFtdiDevice = new FTDI();
            ftStatus = FTDI.FT_STATUS.FT_OK;
            numBytesToRead = 0;
            numBytesRead = 0;
            numBytesToSend = 0;
            numBytesSent = 0;
            locId = 0;
        }

        protected bool FtdiConnect(FTDI.FT_DEVICE deviceType)
        {
            ftStatus = FTDI.FT_STATUS.FT_OK;
            uint devCount = 0;

            //Find available FTDI devices
            try
            {
                ftStatus = myFtdiDevice.GetNumberOfDevices(ref devCount);
            }
            catch
            {
                //driver not loaded error
                MessageBox.Show("FTDI driver not loaded");
                return false;
            }

            //If no FTDI devices found return false
            if (devCount < 1 || ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                return false;
            }

            //Get the location ID of the first specified device type
            locId = 0;
            FTDI.FT_DEVICE_INFO_NODE[] myDeviceList = new FTDI.FT_DEVICE_INFO_NODE[MaxFtdiDevices];
            ftStatus = myFtdiDevice.GetDeviceList(myDeviceList);
            for (int i = 0; i < MaxFtdiDevices; i++)
            {
                if (myDeviceList[i] != null)
                {
                    if (myDeviceList[i].Type == deviceType)
                    {
                        locId = myDeviceList[i].LocId;
                        break;
                    }
                }
            }
            if (locId == 0)
            {
                return false;
            }

            //Open the device
            ftStatus = myFtdiDevice.OpenByLocation(locId);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                return false;
            }
            return true;
        }

        protected bool FtdiSync()
        {
            byte adBusVal = 0;
            byte adBusDir = 0;

            //Synchronize the MPSSE interface by sending bad command 0xAA
            numBytesToSend = 0;
            outputBuffer[numBytesToSend++] = 0xAA;
            if (!SendData(numBytesToSend))
            {
                return false;
            }
            numBytesToRead = 2;
            if (!ReceiveData(2))
            {
                return false;
            }
            if ((receivedBuffer[0] != 0xFA) || (receivedBuffer[1] != 0xAA))
            {
                return false;
            }

            //Synchronize the MPSSE interface by sending bad command 0xAB
            numBytesToSend = 0;
            outputBuffer[numBytesToSend++] = 0xAB;
            if (!SendData(numBytesToSend))
            {
                return false;
            }
            numBytesToRead = 2;
            if (!ReceiveData(2))
            {
                return false;
            }
            if ((receivedBuffer[0] != 0xFA) || (receivedBuffer[1] != 0xAB))
            {
                return false;
            }

            numBytesToSend = 0;
            outputBuffer[numBytesToSend++] = 0x8A; 	// Disable clock divide by 5 for 60Mhz master clock
            outputBuffer[numBytesToSend++] = 0x97;	// Turn off adaptive clocking
            outputBuffer[numBytesToSend++] = 0x8C; 	// Enable 3 phase data clock, used by I2C to allow data on both clock edges
            // The SK clock frequency can be worked out by below algorithm with divide by 5 set as off
            // SK frequency  = 60MHz /((1 +  [(1 +0xValueH*256) OR 0xValueL])*2)
            outputBuffer[numBytesToSend++] = 0x86; 	//Command to set clock divisor
            outputBuffer[numBytesToSend++] = (byte)(ClockDivisor & 0x00FF);	//Set 0xValueL of clock divisor
            outputBuffer[numBytesToSend++] = (byte)((ClockDivisor >> 8) & 0x00FF);	//Set 0xValueH of clock divisor
            outputBuffer[numBytesToSend++] = 0x85; 			// loopback off

#if (FT232H)
            outputBuffer[numBytesToSend++] = 0x9E;       //Enable the FT232H's drive-zero mode with the following enable mask...
            outputBuffer[numBytesToSend++] = 0x07;       // ... Low byte (ADx) enables - bits 0, 1 and 2 and ... 
            outputBuffer[numBytesToSend++] = 0x00;       //...High byte (ACx) enables - all off

            adBusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLhi | (GPIO_Low_Dat & 0xF8)); // SDA and SCL both output high (open drain)
            adBusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));
#else
            adBusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));  	// SDA and SCL set low but as input to mimic open drain
            adBusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLin | (GPIO_Low_Dir & 0xF8));	//

#endif

            outputBuffer[numBytesToSend++] = 0x80; 	//Command to set directions of lower 8 pins and force value on bits set as output 
            outputBuffer[numBytesToSend++] = (byte)(adBusVal);
            outputBuffer[numBytesToSend++] = (byte)(adBusDir);

            if (!SendData(numBytesToSend))
            {
                return false;
            }
            return true;
        }

        public void Disconnect()
        {
            try
            {
                myFtdiDevice.Close();
            }
            catch
            {

            }
        }

        protected bool FlushBuffer()
        {
            // Get the number of bytes available
            uint bytesAvailable = 0;
            ftStatus = myFtdiDevice.GetRxBytesAvailable(ref bytesAvailable);
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                return false;
            }

            if (bytesAvailable > 0)
            {
                //Read out the data from device
                ftStatus = myFtdiDevice.Read(inputBuffer, bytesAvailable, ref numBytesRead);
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    return false;
                }
            }
            return true;
        }

        protected bool SendData(uint bytesToSend)
        {
            numBytesToSend = bytesToSend;

            // Send data. This will return once all sent or if times out
            ftStatus = myFtdiDevice.Write(outputBuffer, numBytesToSend, ref numBytesSent);

            //If not all bytes sent or an error occurs return false
            if ((numBytesSent != numBytesToSend) || (ftStatus != FTDI.FT_STATUS.FT_OK))
            {
                return false;
            }
            return true;
        }

        protected bool ReceiveData(uint bytesToRead)
        {
            uint numBytesInQueue = 0;
            uint inputIndex = 0;
            uint receivedIndex = 0;
            uint totalBytesRead = 0;
            uint queueTimeOut = 0;
            bool queueTimeoutFlag = false;
            uint numBytesRxd = 0;

            //Try to receive all bytes or timeout trying
            while ((totalBytesRead < bytesToRead) && (queueTimeoutFlag == false))
            {
                ftStatus = myFtdiDevice.GetRxBytesAvailable(ref numBytesInQueue);
                //If bytes are available, read them in
                if ((numBytesInQueue > 0) && (ftStatus == FTDI.FT_STATUS.FT_OK))
                {
                    ftStatus = myFtdiDevice.Read(inputBuffer, numBytesInQueue, ref numBytesRxd);
                    if ((numBytesInQueue == numBytesRxd) && (ftStatus == FTDI.FT_STATUS.FT_OK))
                    {
                        inputIndex = 0;

                        //Copy driver populated rx buffer into application rx buffer
                        while (inputIndex < numBytesRxd)
                        {
                            receivedBuffer[receivedIndex] = inputBuffer[inputIndex];
                            inputIndex++;
                            receivedIndex++;
                        }
                        //Update total bytes read
                        totalBytesRead += numBytesRxd;
                    }
                    else
                    {
                        return false;
                    }
                }

                //Increment timeout tries
                queueTimeOut++;
                if (queueTimeOut == ReceiveRetries)
                {
                    queueTimeoutFlag = true;
                }
                else
                {
                    Thread.Sleep(0);
                }
            }
            //update the class bytes read variable
            numBytesRead = totalBytesRead;

            if (queueTimeoutFlag)
            {
                return false;
            }

            return true;
        }

        public bool I2CSetStart()
        {
            byte Count = 0;
            byte ADbusVal = 0;
            byte ADbusDir = 0;
            numBytesToSend = 0;


#if (FT232H)
            // SDA high, SCL high
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLhi | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));    // on FT232H lines always output

            for (Count = 0; Count < 6; Count++)
            {
                outputBuffer[numBytesToSend++] = 0x80;	    // ADbus GPIO command
                outputBuffer[numBytesToSend++] = ADbusVal;   // Set data value
                outputBuffer[numBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA lo, SCL high
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLhi | (GPIO_Low_Dat & 0xF8));

            for (Count = 0; Count < 6; Count++)	// Repeat commands to ensure the minimum period of the start setup time ie 600ns is achieved
            {
                outputBuffer[numBytesToSend++] = 0x80;	    // ADbus GPIO command
                outputBuffer[numBytesToSend++] = ADbusVal;   // Set data value
                outputBuffer[numBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA lo, SCL lo
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));

            for (Count = 0; Count < 6; Count++)	// Repeat commands to ensure the minimum period of the start setup time ie 600ns is achieved
            {
                outputBuffer[numBytesToSend++] = 0x80;	    // ADbus GPIO command
                outputBuffer[numBytesToSend++] = ADbusVal;   // Set data value
                outputBuffer[numBytesToSend++] = ADbusDir;	// Set direction
            }

            // Release SDA
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLlo | (GPIO_Low_Dat & 0xF8));

            outputBuffer[numBytesToSend++] = 0x80;	    // ADbus GPIO command
            outputBuffer[numBytesToSend++] = ADbusVal;   // Set data value
            outputBuffer[numBytesToSend++] = ADbusDir;	// Set direction


# else

            // Both SDA and SCL high (setting to input simulates open drain high)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLin | (GPIO_Low_Dir & 0xF8));

            for (Count = 0; Count < 6; Count++)
            {
                outputBuffer[numBytesToSend++] = 0x80;	    // ADbus GPIO command
                outputBuffer[numBytesToSend++] = ADbusVal;   // Set data value
                outputBuffer[numBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA low, SCL high (setting to input simulates open drain high)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLin | (GPIO_Low_Dir & 0xF8));

            for (Count = 0; Count < 6; Count++)	// Repeat commands to ensure the minimum period of the start setup time
            {
                outputBuffer[numBytesToSend++] = 0x80;	    // ADbus GPIO command
                outputBuffer[numBytesToSend++] = ADbusVal;   // Set data value
                outputBuffer[numBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA low, SCL low
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));//
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));//as above

            for (Count = 0; Count < 6; Count++)	// Repeat commands to ensure the minimum period of the start setup time
            {
                outputBuffer[numBytesToSend++] = 0x80;	    // ADbus GPIO command
                outputBuffer[numBytesToSend++] = ADbusVal;   // Set data value
                outputBuffer[numBytesToSend++] = ADbusDir;	// Set direction
            }

            // Release SDA (setting to input simulates open drain high)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));//
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8));//as above

            outputBuffer[numBytesToSend++] = 0x80;	    // ADbus GPIO command
            outputBuffer[numBytesToSend++] = ADbusVal;   // Set data value
            outputBuffer[numBytesToSend++] = ADbusDir;	// Set direction
# endif
            return SendData(numBytesToSend);
        }

        public bool I2CSetStop()
        {
            byte Count = 0;
            byte ADbusVal = 0;
            byte ADbusDir = 0;
            numBytesToSend = 0;

#if (FT232H)
            // SDA low, SCL low
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));    // on FT232H lines always output

            for (Count = 0; Count < 6; Count++)
            {
                outputBuffer[numBytesToSend++] = 0x80;	    // ADbus GPIO command
                outputBuffer[numBytesToSend++] = ADbusVal;   // Set data value
                outputBuffer[numBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA low, SCL high
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLhi | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));    // on FT232H lines always output

            for (Count = 0; Count < 6; Count++)
            {
                outputBuffer[numBytesToSend++] = 0x80;	    // ADbus GPIO command
                outputBuffer[numBytesToSend++] = ADbusVal;   // Set data value
                outputBuffer[numBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA high, SCL high
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLhi | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));        // on FT232H lines always output

            for (Count = 0; Count < 6; Count++)
            {
                outputBuffer[numBytesToSend++] = 0x80;	    // ADbus GPIO command
                outputBuffer[numBytesToSend++] = ADbusVal;   // Set data value
                outputBuffer[numBytesToSend++] = ADbusDir;	// Set direction
            }

#else
            // SDA low, SCL low
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));

            for (Count = 0; Count < 6; Count++)
            {
                outputBuffer[numBytesToSend++] = 0x80;	    // ADbus GPIO command
                outputBuffer[numBytesToSend++] = ADbusVal;   // Set data value
                outputBuffer[numBytesToSend++] = ADbusDir;	// Set direction
            }


            // SDA low, SCL high (note: setting to input simulates open drain high)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLin | (GPIO_Low_Dir & 0xF8));

            for (Count = 0; Count < 6; Count++)
            {
                outputBuffer[numBytesToSend++] = 0x80;	    // ADbus GPIO command
                outputBuffer[numBytesToSend++] = ADbusVal;   // Set data value
                outputBuffer[numBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA high, SCL high (note: setting to input simulates open drain high)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLin | (GPIO_Low_Dir & 0xF8));

            for (Count = 0; Count < 6; Count++)	// Repeat commands to hold states for longer time
            {
                outputBuffer[numBytesToSend++] = 0x80;	    // ADbus GPIO command
                outputBuffer[numBytesToSend++] = ADbusVal;   // Set data value
                outputBuffer[numBytesToSend++] = ADbusDir;	// Set direction
            }
#endif
            // send the buffer of commands to the chip 
            return SendData(numBytesToSend);
        }

        public bool I2CSendDeviceAddrAndCheckACK(byte Address, bool Read)
        {


            byte ADbusVal = 0;
            byte ADbusDir = 0;
            numBytesToSend = 0;

            Address <<= 1;
            if (Read == true)
                Address |= 0x01;

#if (FT232H)
            outputBuffer[numBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BYTE_OUT;        // clock data byte out
            outputBuffer[numBytesToSend++] = 0x00;                                   // 
            outputBuffer[numBytesToSend++] = 0x00;                                   //  Data length of 0x0000 means 1 byte data to clock in
            outputBuffer[numBytesToSend++] = Address;           //  Byte to send

            // Put line back to idle (data released, clock pulled low)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));// make data input
            outputBuffer[numBytesToSend++] = 0x80;                                   // Command - set low byte
            outputBuffer[numBytesToSend++] = ADbusVal;                               // Set the values
            outputBuffer[numBytesToSend++] = ADbusDir;                               // Set the directions

            // CLOCK IN ACK
            outputBuffer[numBytesToSend++] = MSB_RISING_EDGE_CLOCK_BIT_IN;           // clock data bits in
            outputBuffer[numBytesToSend++] = 0x00;                                   // Length of 0 means 1 bit
#else

            // Set directions of clock and data to output in preparation for clocking out a byte
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));// back to output
            outputBuffer[numBytesToSend++] = 0x80;                                   // Command - set low byte
            outputBuffer[numBytesToSend++] = ADbusVal;                               // Set the values
            outputBuffer[numBytesToSend++] = ADbusDir;                               // Set the directions
            // clock out one byte
            outputBuffer[numBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BYTE_OUT;        // clock data byte out
            outputBuffer[numBytesToSend++] = 0x00;                                   // 
            outputBuffer[numBytesToSend++] = 0x00;                                   // Data length of 0x0000 means 1 byte data to clock in
            outputBuffer[numBytesToSend++] = Address;                         // Byte to send

            // Put line back to idle (data released, clock pulled low) so that sensor can drive data line
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8)); // make data input
            outputBuffer[numBytesToSend++] = 0x80;                                   // Command - set low byte
            outputBuffer[numBytesToSend++] = ADbusVal;                               // Set the values
            outputBuffer[numBytesToSend++] = ADbusDir;                               // Set the directions

            // CLOCK IN ACK
            outputBuffer[numBytesToSend++] = MSB_RISING_EDGE_CLOCK_BIT_IN;           // clock data byte in
            outputBuffer[numBytesToSend++] = 0x00;                                   // Length of 0 means 1 bit

#endif
            // This command then tells the MPSSE to send any results gathered (in this case the ack bit) back immediately
            outputBuffer[numBytesToSend++] = 0x87;                                //  ' Send answer back immediate command

            // send commands to chip
            if (!SendData(numBytesToSend))
            {
                return false;
            }

            if (!ReceiveData(1))
            {
                return false;
            }

            // if ack bit is 0 then sensor acked the transfer, otherwise it nak'd the transfer
            if ((receivedBuffer[0] & 0x01) == 0)
            {
                i2cAck = true;
            }
            else
            {
                i2cAck = false;
            }

            return true;
        }

        public bool I2CSendByteAndCheckACK(byte DataByteToSend)
        {
            byte ADbusVal = 0;
            byte ADbusDir = 0;
            numBytesToSend = 0;

#if (FT232H)
            outputBuffer[numBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BYTE_OUT;        // clock data byte out
            outputBuffer[numBytesToSend++] = 0x00;                                   // 
            outputBuffer[numBytesToSend++] = 0x00;                                   //  Data length of 0x0000 means 1 byte data to clock in
            outputBuffer[numBytesToSend++] = DataByteToSend;// DataSend[0];          //  Byte to send

            // Put line back to idle (data released, clock pulled low)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));// make data input
            outputBuffer[numBytesToSend++] = 0x80;                                   // Command - set low byte
            outputBuffer[numBytesToSend++] = ADbusVal;                               // Set the values
            outputBuffer[numBytesToSend++] = ADbusDir;                               // Set the directions

            // CLOCK IN ACK
            outputBuffer[numBytesToSend++] = MSB_RISING_EDGE_CLOCK_BIT_IN;           // clock data bits in
            outputBuffer[numBytesToSend++] = 0x00;                                   // Length of 0 means 1 bit
#else

            // Set directions of clock and data to output in preparation for clocking out a byte
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));// back to output
            outputBuffer[numBytesToSend++] = 0x80;                                   // Command - set low byte
            outputBuffer[numBytesToSend++] = ADbusVal;                               // Set the values
            outputBuffer[numBytesToSend++] = ADbusDir;                               // Set the directions
            // clock out one byte
            outputBuffer[numBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BYTE_OUT;        // clock data byte out
            outputBuffer[numBytesToSend++] = 0x00;                                   // 
            outputBuffer[numBytesToSend++] = 0x00;                                   // Data length of 0x0000 means 1 byte data to clock in
            outputBuffer[numBytesToSend++] = DataByteToSend;                         // Byte to send

            // Put line back to idle (data released, clock pulled low) so that sensor can drive data line
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8)); // make data input
            outputBuffer[numBytesToSend++] = 0x80;                                   // Command - set low byte
            outputBuffer[numBytesToSend++] = ADbusVal;                               // Set the values
            outputBuffer[numBytesToSend++] = ADbusDir;                               // Set the directions

            // CLOCK IN ACK
            outputBuffer[numBytesToSend++] = MSB_RISING_EDGE_CLOCK_BIT_IN;           // clock data byte in
            outputBuffer[numBytesToSend++] = 0x00;                                   // Length of 0 means 1 bit

#endif
            // This command then tells the MPSSE to send any results gathered (in this case the ack bit) back immediately
            outputBuffer[numBytesToSend++] = 0x87;                                //  ' Send answer back immediate command

            // send commands to chip
            if (!SendData(numBytesToSend))
            {
                return false;
            }

            // read back byte containing ack
            if (!ReceiveData(1))
            {
                return false;
            }

            // if ack bit is 0 then sensor acked the transfer, otherwise it nak'd the transfer
            if ((receivedBuffer[0] & 0x01) == 0)
            {
                i2cAck = true;
            }
            else
            {
                i2cAck = false;
            }

            return true;
        }

        public bool I2CReadByte(bool ACK)
        {
            byte ADbusVal = 0;
            byte ADbusDir = 0;
            numBytesToSend = 0;

#if (FT232H)
            // Clock in one data byte
            outputBuffer[numBytesToSend++] = MSB_RISING_EDGE_CLOCK_BYTE_IN;      // Clock data byte in
            outputBuffer[numBytesToSend++] = 0x00;
            outputBuffer[numBytesToSend++] = 0x00;                               // Data length of 0x0000 means 1 byte data to clock in

            // clock out one bit as ack/nak bit
            outputBuffer[numBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BIT_OUT;     // Clock data bit out
            outputBuffer[numBytesToSend++] = 0x00;                               // Length of 0 means 1 bit
            if (ACK == true)
                outputBuffer[numBytesToSend++] = 0x00;                           // Data bit to send is a '0'
            else
                outputBuffer[numBytesToSend++] = 0xFF;                           // Data bit to send is a '1'

            // I2C lines back to idle state 
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));
            outputBuffer[numBytesToSend++] = 0x80;                               //       ' Command - set low byte
            outputBuffer[numBytesToSend++] = ADbusVal;                            //      ' Set the values
            outputBuffer[numBytesToSend++] = ADbusDir;                             //     ' Set the directions
#else          
            // Ensure line is definitely an input since FT2232H and FT4232H don't have open drain
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8)); // make data input
            outputBuffer[numBytesToSend++] = 0x80;                                   // command - set low byte
            outputBuffer[numBytesToSend++] = ADbusVal;                               // Set the values
            outputBuffer[numBytesToSend++] = ADbusDir;                               // Set the directions
            // Clock one byte of data in from the sensor
            outputBuffer[numBytesToSend++] = MSB_RISING_EDGE_CLOCK_BYTE_IN;      // Clock data byte in
            outputBuffer[numBytesToSend++] = 0x00;
            outputBuffer[numBytesToSend++] = 0x00;                               // Data length of 0x0000 means 1 byte data to clock in

            // Change direction back to output and clock out one bit. If ACK is true, we send bit as 0 as an acknowledge
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));    // back to output
            outputBuffer[numBytesToSend++] = 0x80;                               // Command - set low byte
            outputBuffer[numBytesToSend++] = ADbusVal;                           // set the values
            outputBuffer[numBytesToSend++] = ADbusDir;                           // set the directions

            outputBuffer[numBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BIT_OUT;    // Clock data bit out
            outputBuffer[numBytesToSend++] = 0x00;                              // Length of 0 means 1 bit
            if (ACK == true)
            {
                outputBuffer[numBytesToSend++] = 0x00;                          // Data bit to send is a '0'
            }
            else
            {
                outputBuffer[numBytesToSend++] = 0xFF;                          // Data bit to send is a '1'
            }

            // Put line states back to idle with SDA open drain high (set to input) 
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8));//make data input
            outputBuffer[numBytesToSend++] = 0x80;                               //       ' Command - set low byte
            outputBuffer[numBytesToSend++] = ADbusVal;                            //      ' Set the values
            outputBuffer[numBytesToSend++] = ADbusDir;                             //     ' Set the directions


#endif
            // This command then tells the MPSSE to send any results gathered back immediately
            outputBuffer[numBytesToSend++] = 0x87;                                  //    ' Send answer back immediate command

            // send commands to chip
            if (!SendData(numBytesToSend))
            {
                return false;
            }

            // get the byte which has been read from the driver's receive buffer
            return ReceiveData(1);  // receivedBuffer[0] now contains the results
        }
    }
}
