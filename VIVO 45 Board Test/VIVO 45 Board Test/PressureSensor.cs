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
    class PressureSensor : FtdiDevice
    {
        //Sensor register values
        const byte MPL3115A2_ADDRESS = 0x60;    // 1100000
        const byte MPL3115A2_REGISTER_STATUS = 0x00;
        const byte MPL3115A2_REGISTER_STATUS_TDR = 0x02;
        const byte MPL3115A2_REGISTER_STATUS_PDR = 0x04;
        const byte MPL3115A2_REGISTER_STATUS_PTDR = 0x08;
        const byte MPL3115A2_REGISTER_PRESSURE_MSB = 0x01;
        const byte MPL3115A2_REGISTER_PRESSURE_CSB = 0x02;
        const byte MPL3115A2_REGISTER_PRESSURE_LSB = 0x03;
        const byte MPL3115A2_REGISTER_TEMP_MSB = 0x04;
        const byte MPL3115A2_REGISTER_TEMP_LSB = 0x05;
        const byte MPL3115A2_REGISTER_DR_STATUS = 0x06;
        const byte MPL3115A2_OUT_P_DELTA_MSB = 0x07;
        const byte MPL3115A2_OUT_P_DELTA_CSB = 0x08;
        const byte MPL3115A2_OUT_P_DELTA_LSB = 0x09;
        const byte MPL3115A2_OUT_T_DELTA_MSB = 0x0A;
        const byte MPL3115A2_OUT_T_DELTA_LSB = 0x0B;
        const byte MPL3115A2_WHOAMI = 0x0C;
        const byte MPL3115A2_PT_DATA_CFG = 0x13;
        const byte MPL3115A2_PT_DATA_CFG_TDEFE = 0x01;
        const byte MPL3115A2_PT_DATA_CFG_PDEFE = 0x02;
        const byte MPL3115A2_PT_DATA_CFG_DREM = 0x04;
        const byte MPL3115A2_CTRL_REG1 = 0x26;
        const byte MPL3115A2_CTRL_REG1_SBYB = 0x01;
        const byte MPL3115A2_CTRL_REG1_OST = 0x02;
        const byte MPL3115A2_CTRL_REG1_RST = 0x04;
        const byte MPL3115A2_CTRL_REG1_OS1 = 0x00;
        const byte MPL3115A2_CTRL_REG1_OS2 = 0x08;
        const byte MPL3115A2_CTRL_REG1_OS4 = 0x10;
        const byte MPL3115A2_CTRL_REG1_OS8 = 0x18;
        const byte MPL3115A2_CTRL_REG1_OS16 = 0x20;
        const byte MPL3115A2_CTRL_REG1_OS32 = 0x28;
        const byte MPL3115A2_CTRL_REG1_OS64 = 0x30;
        const byte MPL3115A2_CTRL_REG1_OS128 = 0x38;
        const byte MPL3115A2_CTRL_REG1_RAW = 0x40;
        const byte MPL3115A2_CTRL_REG1_ALT = 0x80;
        const byte MPL3115A2_CTRL_REG1_BAR = 0x00;
        const byte MPL3115A2_CTRL_REG2 = 0x27;
        const byte MPL3115A2_CTRL_REG3 = 0x28;
        const byte MPL3115A2_CTRL_REG4 = 0x29;
        const byte MPL3115A2_CTRL_REG5 = 0x2A;
        const byte MPL3115A2_REGISTER_STARTCONVERSION = 0x12;

        

        public PressureSensor() : base()
        {            
        }

        public bool Connect()
        {
            return true;
            //Connect the FTDI device
            if (!FtdiConnect(FTDI.FT_DEVICE.FT_DEVICE_232H))
            {
                MessageBox.Show("Error communicating to FTDI interface");
                myFtdiDevice.Close();
                return false;
            }

            //Set FTDI device settings
            ftStatus |= myFtdiDevice.SetTimeouts(5000, 5000);
            ftStatus |= myFtdiDevice.SetLatency(16);
            ftStatus |= myFtdiDevice.SetFlowControl(FTDI.FT_FLOW_CONTROL.FT_FLOW_RTS_CTS, 0x00, 0x00);
            ftStatus |= myFtdiDevice.SetBitMode(0x00, 0x00);
            ftStatus |= myFtdiDevice.SetBitMode(0x00, 0x02);         // MPSSE mode
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                MessageBox.Show("Error communicating to FTDI interface");
                myFtdiDevice.Close();
                return false;
            }

            //Flush the buffer
            if (!FlushBuffer())
            {
                MessageBox.Show("Error flushing FTDI buffer");
                myFtdiDevice.Close();
                return false;
            }

            //Sync the FTDI and configure the clocks
            if(!FtdiSync())
            {
                MessageBox.Show("Error syncing the FTDI device");
                myFtdiDevice.Close();
                return false;
            }

            return InitPressureSensor();
        }

        public bool InitPressureSensor()
        {
            //Check that i2c we are attached to is pressure sensor
            if (ReadPressureSensorRegister(MPL3115A2_WHOAMI) != 0xC4)
            {
                return false;
            }

            //Configure control register
            WritePressureSensorRegister(MPL3115A2_CTRL_REG1,
                MPL3115A2_CTRL_REG1_OS128 | MPL3115A2_CTRL_REG1_SBYB);

            //Configure data
            WritePressureSensorRegister(MPL3115A2_PT_DATA_CFG,
                MPL3115A2_PT_DATA_CFG_DREM | MPL3115A2_PT_DATA_CFG_PDEFE | MPL3115A2_PT_DATA_CFG_TDEFE);

            return true;
        }

        private byte ReadPressureSensorRegister(byte register)
        {
            receivedBuffer[0] = 0;

            //Set register address we want to read
            I2CSetStart();
            I2CSendDeviceAddrAndCheckACK(MPL3115A2_ADDRESS, false);
            I2CSendByteAndCheckACK(register);

            //Read register value
            I2CSetStart();
            I2CSendDeviceAddrAndCheckACK(MPL3115A2_ADDRESS, true);
            I2CReadByte(false); //Read, sending NACK to end read
            I2CSetStop();
            
            return receivedBuffer[0];
        }

        private void WritePressureSensorRegister(byte register, byte value)
        {
            //Start I2C communication
            I2CSetStart();

            //Send address, start write mode
            I2CSendDeviceAddrAndCheckACK(MPL3115A2_ADDRESS, false);

            //Send register address
            I2CSendByteAndCheckACK(register);

            //Send register value
            I2CSendByteAndCheckACK(value);

            //Finalize packet
            I2CSetStop();
        }

        public double GetPressure()
        {
            int pressure = 0;

            WritePressureSensorRegister(MPL3115A2_CTRL_REG1,
                MPL3115A2_CTRL_REG1_BAR | MPL3115A2_CTRL_REG1_OS128 | MPL3115A2_CTRL_REG1_SBYB);

            byte status = 0;
            int retries = 50;

            //Wait until pressure data is available
            while((status & MPL3115A2_REGISTER_STATUS_PDR) == 0 && retries > 0)
            {
                status = ReadPressureSensorRegister(MPL3115A2_REGISTER_STATUS);
                retries--;
                Thread.Sleep(10);
            }
            if(retries < 0)
            {
                return -1;
            }

            //Set register start
            I2CSetStart();
            I2CSendDeviceAddrAndCheckACK(MPL3115A2_ADDRESS, false);
            I2CSendByteAndCheckACK(MPL3115A2_REGISTER_PRESSURE_MSB);

            //Read pressure value
            I2CSetStart();
            I2CSendDeviceAddrAndCheckACK(MPL3115A2_ADDRESS, true);
            I2CReadByte(true);
            pressure += receivedBuffer[0];
            pressure <<= 8;
            I2CReadByte(true);
            pressure += receivedBuffer[0];
            pressure <<= 8;
            I2CReadByte(false);
            pressure += receivedBuffer[0];
            pressure >>= 4;
            I2CSetStop();

            //Returns Pa
            return ((double)pressure)/4.0;
        }

        public double GetTemperature()
        {
            int temperature = 0;
            byte status = 0;
            int retries = 50;

            //Wait until pressure data is available
            while ((status & MPL3115A2_REGISTER_STATUS_TDR) == 0 && retries > 0)
            {
                status = ReadPressureSensorRegister(MPL3115A2_REGISTER_STATUS);
                retries--;
                Thread.Sleep(10);
            }
            if (retries < 0)
            {
                return -1;
            }

            //Set register start
            I2CSetStart();
            I2CSendDeviceAddrAndCheckACK(MPL3115A2_ADDRESS, false);
            I2CSendByteAndCheckACK(MPL3115A2_REGISTER_TEMP_MSB);

            //Read pressure value
            I2CSetStart();
            I2CSendDeviceAddrAndCheckACK(MPL3115A2_ADDRESS, true);
            I2CReadByte(true);
            temperature += receivedBuffer[0];
            temperature <<= 8;
            I2CReadByte(false);
            temperature += receivedBuffer[0];
            temperature >>= 4;
            I2CSetStop();

            //Returns degrees C
            return ((double)temperature) / 16.0;
        }
    }
}
