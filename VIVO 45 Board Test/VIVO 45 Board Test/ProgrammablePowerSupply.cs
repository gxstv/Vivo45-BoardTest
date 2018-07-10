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
    class ProgrammablePowerSupply : FtdiDevice
    {
        const string QueryDeviceIdStr = "*IDN?";
        const string ResetDeviceStr = "*RST";
        const string EnableOutputStr = "OUTPut:GENeral ON";
        const string DisableOutputStr = "OUTPut:GENeral OFF";
        const string GetChannelStr = "INSTrument:NSELect?";
        const string SetChannelStr = "INSTrument:NSELect ";
        const string EnableChannelStr = "OUTPut:SELect ON";
        const string DisableChannelStr = "OUTPut:SELect OFF";
        const string MeasureVoltageStr = "MEASure:VOLTage?";
        const string MeasureCurrentStr = "MEASure:CURRent?";
        const string SetVoltageStr = "VOLTage ";
        const string SetCurrentStr = "CURRent ";

        const string SupplyIdStr = "HAMEG,HMP4040";

        const double MaxAmps = 5;
        const double MaxVoltage = 20;


        public ProgrammablePowerSupply() : base()
        {
            
        }
        
        public bool Connect()
        {
            //Connect the FTDI device
            if (!FtdiConnect(FTDI.FT_DEVICE.FT_DEVICE_BM))
            {
                myFtdiDevice.Close();
                return false;
            }

            if (!FlushBuffer())
            {
                myFtdiDevice.Close();
                return false;
            }

            //TODO initialize
            if (!SupplyInit())
            {
                myFtdiDevice.Close();
                return false;
            }
            return true;
        }

        private bool SendCommand(string command)
        {
            string output = command + "\n";
            // Send data. This will return once all sent or if times out
            ftStatus = myFtdiDevice.Write(output, output.Length, ref numBytesSent);

            if(numBytesSent != output.Length || (ftStatus != FTDI.FT_STATUS.FT_OK))
            {
                return false;
            }
            return true;
        }

        private bool SendCommandValue(string command, double value)
        {
            string output = command + value.ToString() + "\n";
            // Send data. This will return once all sent or if times out
            ftStatus = myFtdiDevice.Write(output, output.Length, ref numBytesSent);

            if (numBytesSent != output.Length || (ftStatus != FTDI.FT_STATUS.FT_OK))
            {
                return false;
            }
            return true;
        }

        public string ReadResponse()
        {
            uint inputIndex = 0;
            uint numBytesInQueue = 0;
            uint numBytesRxd = 0;
            bool lfReceived = false;
            string output = "";
            uint queueTimeOut = 0;
            bool queueTimeoutFlag = false;

            while (!lfReceived && !queueTimeoutFlag)
            {
                ftStatus = myFtdiDevice.GetRxBytesAvailable(ref numBytesInQueue);
                //If bytes are available, read them in
                if ((numBytesInQueue > 0) && (ftStatus == FTDI.FT_STATUS.FT_OK))
                {
                    ftStatus = myFtdiDevice.Read(inputBuffer, numBytesInQueue, ref numBytesRxd);

                    if ((numBytesInQueue != numBytesRxd) || (ftStatus != FTDI.FT_STATUS.FT_OK))
                    {
                        return "";
                    }

                    inputIndex = 0;

                    //Copy new bytes to output string until line feed is reached
                    while (inputIndex < numBytesRxd)
                    {
                        if((char)inputBuffer[inputIndex] == '\n')
                        {
                            lfReceived = true;
                            break;
                        }
                        output += (char)inputBuffer[inputIndex];
                        inputIndex++;
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
                    Thread.Sleep(1);
                }
            }
            return output;
        }

        public bool SupplyInit()
        {
            //Query the device ID and make sure it matches the expected supply
            if (!SendCommand(QueryDeviceIdStr))
            {
                return false;
            }
            string id = ReadResponse();
            if (!id.Contains(SupplyIdStr))
            {
                return false;
            }

            //Reset the device
            if (!SendCommand(ResetDeviceStr))
            {
                return false;
            }

            return true;
        }

        public bool EnableOutput()
        {
            if(!SendCommand(EnableOutputStr))
            {
                return false;
            }
            return true;
        }

        public bool DisableOutput()
        {
            if (!SendCommand(DisableOutputStr))
            {
                return false;
            }
            return true;
        }

        public bool DisableChannel(int channel)
        {
            if (channel <= 0 || channel > 4) return false;
            if (!SendCommandValue(SetChannelStr, channel)) return false;
            if (!SendCommand(DisableChannelStr)) return false;
            return true;
        }

        public bool EnableChannel(int channel)
        {
            //Ensure channel is valid
            if (channel <= 0 || channel > 4) return false;
            //Try to set the channel
            if (!SendCommandValue(SetChannelStr, channel)) return false;
            //Try to enable the channel
            if (!SendCommand(EnableChannelStr)) return false;
            return true;
        }

        public bool SetVoltage(int channel, double voltage)
        {
            //Ensure channel is valid
            //if(channel!=1&& voltage!=18.0) return false;
            if (channel <= 0 || channel > 4) return false;
            //Ensure voltage is valid
            if (voltage < 0 || voltage > MaxVoltage) return false;
            //Try to set the channel
            if (!SendCommandValue(SetChannelStr, channel)) return false;
            //Try to set the voltage
            if (!SendCommandValue(SetVoltageStr, voltage)) return false;
            return true;
        }
        
        public double GetVoltageMeasurement(int channel)
        {
            double val = -1;
            //Ensure channel is valid
            if (channel <= 0 || channel > 4) return -1;
            //Try to set the channel
            if (!SendCommandValue(SetChannelStr, channel)) return -1;
            //Request voltage measurement
            if (!SendCommand(MeasureVoltageStr)) return -1;
            //Read response and return if it is a number
            string response = ReadResponse();
            if(Double.TryParse(response, out val)) return val;
            else return -1;
        }

        public bool SetCurrent(int channel, double amps)
        {
            //Ensure channel is valid
            if (channel <= 0 || channel > 4) return false;
            //Ensure current if valid
            if (amps < 0 || amps > MaxAmps) return false;
            //Try to set the channel
            if (!SendCommandValue(SetChannelStr, channel)) return false;
            //Try to set the current
            if (!SendCommandValue(SetCurrentStr, amps)) return false;
            return true;
        }

        public double GetCurrentMeasurement(int channel)
        {
            double val = -1;
            //Ensure channel is valid
            if (channel <= 0 || channel > 4) return -1;
            //Try to set the channel
            if (!SendCommandValue(SetChannelStr, channel)) return -1;
            //Request voltage measurement
            if (!SendCommand(MeasureCurrentStr)) return -1;
            //Read response and return if it is a number
            string response = ReadResponse();
            if (Double.TryParse(response, out val)) return val;
            else return -1;
        }
    }
}
