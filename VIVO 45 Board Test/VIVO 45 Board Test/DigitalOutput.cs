using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using NationalInstruments.DAQmx;

namespace VIVO_45_Board_Test
{
    public struct OutputLine
    {
        public string slot;
        public int port;
        public int line;
        public bool activeHigh;

        public OutputLine(string deviceSlot, int portNum, int lineNum, bool normalLogic)
        {
            slot = deviceSlot;
            port = portNum;
            line = lineNum;
            activeHigh = normalLogic;
        }
    }

    class DigitalOutput
    {
        public const int SwitchingMs = 50;
        public enum OutputSignals
        {
            ScopeAddr0 = 0, ScopeAddr1, ScopeAddr2, ScopeAddr3, ScopeAddr4, ScopeAddr5, ScopeAddr6,

            //Power source
            Ps4SelExtDC, PassMainstoExtDC,
            LoadResMuxPower, LoadResMainsDC, LoadResExtDC, LoadResExtBatt, LoadResIntBatt,
            SSLoadChan1, SSLoadChan2, SSLoadChan3, SSLoadChan4,
            LoadResEEPower,

            //Power supply loading resistors
            LoadRes1V2, LoadRes5VCFiltered, LoadRes3V3C, LoadRes1V5,
            LoadResVSNVS, LoadRes1V8, LoadRes1V375, LoadRes2V5,
            LoadRes12VC, LoadRes12VT, LoadRes5VC, LoadRes5VT,

            //Overloading resistors
            LoadRes12VCLim, OverloadRes12VCLim, LoadRes3V3CLim, OverloadRes3V3CLim,
            LoadRes5VCLim, OverloadRes5VCLim, LoadRes5VTLim, OverloadRes5VTLim,

            //Control lines
            RAPwrDisable,

            //Heater
            LoadResHeaterPlate, HumidifierSim, LoadResHeatedWire, HeatedWireSim,

            //Motor
            MotorSim,

            //Supercap lines
            SupercapDump1, SupercapDump2, SupercapDisconnect,

            //Boot pins
            BootMode0C, BootMode1C,

            Ps4SelRtcBatt, LoadResRtcBatt,

            LoadResCtrl5VC100, LoadResCtrl5VC10K,
            LoadResCtrl5VT100, LoadResCtrl5VT10K,
            LoadResRtc3V3C,

            ButtonDownSim, ButtonUpSim, ButtonISim,
            Button1Sim, Button2Sim, Button3Sim, Button4Sim, Button5Sim,
            ButtonMinusSim, ButtonPlusSim, ButtonStartSim, ButtonMuteSim,

            SdCardDetect, SDWriteProtect,
            MotorPowerDumpComp,

            JtagEn1, JtagEn2,

            LoadRes3V3T, EnableResetC,

            BootConfig1_5, BootConfig1_6, BootConfig1_7,
            BootConfig2_3, BootConfig2_4, BootConfig2_5, BootConfig2_6, BootConfig2_7
        }

        public const int MinDeviceSlot = 2;
        const int MaxDeviceSlot = 5;
        const int NumDevices = 2;
        const int PortsPerDevice = 8;
        const int LinesPerPort = 8;

        const int ScopeAddrPort = 5;
        const byte MaxAddr = 127;

        const ulong PrimaryStartupState = 0;
        const ulong SecondaryStartupState = ((ulong)0xFF << (8 * ScopeAddrPort));

        const int RelayWaitMs = 50;

        OutputLine[] outputs = new OutputLine[Enum.GetNames(typeof(OutputSignals)).Length];

        public DigitalOutput()
        {

        }

        public bool ConfirmOutputSlotConfig(string dev, ulong defaultState)
        {
            string[] channels;
            DigitalPowerUpState[] states;
            
            //Try to read the given device's default startup state
            try
            {
                DaqSystem.Local.GetDevicePowerUpState(dev, out channels, out states);
            }
            catch
            {
                return false;
            }

            //Check that the device has 64 outputs
            if (states.Length != 64)
            {
                return false;
            }

            //Compare startup states to expected startup states
            for (int i = 0; i < states.Length; i++)
            {
                bool defaultHigh = ((defaultState & ((ulong)0x01 << i)) != 0);
                bool startupHigh = (states[i] == DigitalPowerUpState.High);
                if(defaultHigh != startupHigh)
                {
                    return false;
                }
            }
            return true;
        }

        public bool Connect(int primary, int secondary)
        {
            bool primaryConfirmed = false;
            bool secondaryConfirmed = false;
            int primarySlot = primary + MinDeviceSlot;
            int secondarySlot = secondary + MinDeviceSlot;
            string primarySlotStr = "";
            string secondarySlotStr = "";

            string[] deviceList = DaqSystem.Local.Devices;
            string[] portList = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOLine, PhysicalChannelAccess.External);


            foreach (string device in deviceList)
            {
                //Check that device is properly labeled
                if(!device.Contains("BRE6513"))
                {
                    continue;
                }

                //Check for given primary slot
                if(device.Contains("slot"+primarySlot.ToString()))
                {
                    if (ConfirmOutputSlotConfig(device, PrimaryStartupState))
                    {
                        primaryConfirmed = true;
                        primarySlotStr = device;                        
                    }
                }

                //Check for given secondary slot
                if (device.Contains("slot" + secondarySlot.ToString()))
                {
                    if (ConfirmOutputSlotConfig(device, SecondaryStartupState))
                    {
                        secondaryConfirmed = true;
                        secondarySlotStr = device;
                    }
                }
            }

            if(!primaryConfirmed)
            {
                MessageBox.Show("Could not identify primary digital output card. " +
                    "Please ensure card is seated and preconfigured correctly. " +
                    "Hardware rescan may be required.");
                return false;
            }

            if(!secondaryConfirmed)
            {
                MessageBox.Show("Could not identify secondary digital output card. " +
                    "Please ensure card is seated and preconfigured correctly. " +
                    "Hardware rescan may be required.");
                return false;
            }

            InitOutputDefinitions(primarySlotStr, secondarySlotStr);

            for (int i = 0; i < outputs.Length; i++)
            {
                EnableOutput((OutputSignals)i, false);
            }

            SetScopeAddr(39);

            Thread.Sleep(50);

            SetScopeAddr(0);

            return true;
        }

        public void Disconnect()
        {
            for (int i = 0; i < outputs.Length; i++)
            {
                EnableOutput((OutputSignals)i, false);
            }

            SetScopeAddr(0);
        }

        private void InitOutputDefinitions(string primary, string secondary)
        {
            //Set up output definitions for primary slot
            outputs[(int)OutputSignals.Ps4SelExtDC] = new OutputLine(primary, 0, 0, false);
            outputs[(int)OutputSignals.PassMainstoExtDC] = new OutputLine(primary, 0, 1, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 0, 2, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 0, 3, false);
            outputs[(int)OutputSignals.LoadRes1V2] = new OutputLine(primary, 0, 4, false);
            outputs[(int)OutputSignals.LoadRes5VCFiltered] = new OutputLine(primary, 0, 5, false);
            outputs[(int)OutputSignals.LoadRes3V3C] = new OutputLine(primary, 0, 6, false);
            outputs[(int)OutputSignals.LoadRes1V5] = new OutputLine(primary, 0, 7, false);

            outputs[(int)OutputSignals.LoadResExtBatt] = new OutputLine(primary, 1, 0, false);
            outputs[(int)OutputSignals.LoadResIntBatt] = new OutputLine(primary, 1, 1, false);
            outputs[(int)OutputSignals.LoadRes5VC] = new OutputLine(primary, 1, 2, false);
            outputs[(int)OutputSignals.LoadRes5VT] = new OutputLine(primary, 1, 3, false);
            outputs[(int)OutputSignals.LoadRes12VT] = new OutputLine(primary, 1, 4, false);
            outputs[(int)OutputSignals.LoadRes5VCLim] = new OutputLine(primary, 1, 5, false);
            outputs[(int)OutputSignals.OverloadRes5VCLim] = new OutputLine(primary, 1, 6, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 1, 7, false);

            outputs[(int)OutputSignals.LoadResVSNVS] = new OutputLine(primary, 2, 0, false);
            outputs[(int)OutputSignals.LoadRes1V8] = new OutputLine(primary, 2, 1, false);
            outputs[(int)OutputSignals.LoadRes1V375] = new OutputLine(primary, 2, 2, false);
            outputs[(int)OutputSignals.LoadRes2V5] = new OutputLine(primary, 2, 3, false);
            outputs[(int)OutputSignals.LoadRes12VCLim] = new OutputLine(primary, 2, 4, false);
            outputs[(int)OutputSignals.OverloadRes12VCLim] = new OutputLine(primary, 2, 5, false);
            outputs[(int)OutputSignals.LoadRes5VTLim] = new OutputLine(primary, 2, 6, false);
            outputs[(int)OutputSignals.OverloadRes5VTLim] = new OutputLine(primary, 2, 7, false);

            outputs[(int)OutputSignals.SSLoadChan1] = new OutputLine(primary, 3, 0, false);
            outputs[(int)OutputSignals.SSLoadChan2] = new OutputLine(primary, 3, 1, false);
            outputs[(int)OutputSignals.SSLoadChan3] = new OutputLine(primary, 3, 2, false);
            outputs[(int)OutputSignals.SSLoadChan4] = new OutputLine(primary, 3, 3, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 3, 4, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 3, 5, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 3, 6, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 3, 7, false);

            outputs[(int)OutputSignals.LoadRes12VC] = new OutputLine(primary, 4, 0, false);
            outputs[(int)OutputSignals.RAPwrDisable] = new OutputLine(primary, 4, 1, false);
            outputs[(int)OutputSignals.LoadRes3V3CLim] = new OutputLine(primary, 4, 2, false);
            outputs[(int)OutputSignals.OverloadRes3V3CLim] = new OutputLine(primary, 4, 3, false);
            outputs[(int)OutputSignals.LoadResEEPower] = new OutputLine(primary, 4, 4, false);
            outputs[(int)OutputSignals.SupercapDump1] = new OutputLine(primary, 4, 5, false);
            outputs[(int)OutputSignals.SupercapDump2] = new OutputLine(primary, 4, 6, false);
            outputs[(int)OutputSignals.SupercapDisconnect] = new OutputLine(primary, 4, 7, false);

            //outputs[(int)OutputSignals.] = new OutputLine(primary, 5, 0, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 5, 1, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 5, 2, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 5, 3, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 5, 4, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 5, 5, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 5, 6, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 5, 7, false);

            outputs[(int)OutputSignals.LoadResHeaterPlate] = new OutputLine(primary, 6, 0, false);
            outputs[(int)OutputSignals.HumidifierSim] = new OutputLine(primary, 6, 1, false);
            outputs[(int)OutputSignals.LoadResHeatedWire] = new OutputLine(primary, 6, 2, false);
            outputs[(int)OutputSignals.HeatedWireSim] = new OutputLine(primary, 6, 3, false);
            outputs[(int)OutputSignals.MotorSim] = new OutputLine(primary, 6, 4, false);
            outputs[(int)OutputSignals.LoadResMainsDC] = new OutputLine(primary, 6, 5, false);
            outputs[(int)OutputSignals.LoadResMuxPower] = new OutputLine(primary, 6, 6, false);
            outputs[(int)OutputSignals.LoadResExtDC] = new OutputLine(primary, 6, 7, false);

            //outputs[(int)OutputSignals.] = new OutputLine(primary, 7, 0, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 7, 1, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 7, 2, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 7, 3, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 7, 4, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 7, 5, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 7, 6, false);
            //outputs[(int)OutputSignals.] = new OutputLine(primary, 7, 7, false);

            
            //Set up output definitions for secondary slot
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 0, 0, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 0, 1, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 0, 2, false);
            outputs[(int)OutputSignals.BootMode0C] = new OutputLine(secondary, 0, 3, false);
            outputs[(int)OutputSignals.BootMode1C] = new OutputLine(secondary, 0, 4, false);
            outputs[(int)OutputSignals.Ps4SelRtcBatt] = new OutputLine(secondary, 0, 5, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 0, 6, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 0, 7, false);

            outputs[(int)OutputSignals.JtagEn1] = new OutputLine(secondary, 1, 0, false);
            outputs[(int)OutputSignals.JtagEn2] = new OutputLine(secondary, 1, 1, false);
            outputs[(int)OutputSignals.LoadRes3V3T] = new OutputLine(secondary, 1, 2, false);
            outputs[(int)OutputSignals.EnableResetC] = new OutputLine(secondary, 1, 3, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 1, 4, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 1, 5, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 1, 6, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 1, 7, false);

            outputs[(int)OutputSignals.LoadResRtcBatt] = new OutputLine(secondary, 2, 0, false);
            outputs[(int)OutputSignals.LoadResCtrl5VC100] = new OutputLine(secondary, 2, 1, false);
            outputs[(int)OutputSignals.LoadResCtrl5VC10K] = new OutputLine(secondary, 2, 2, false);
            outputs[(int)OutputSignals.LoadResCtrl5VT100] = new OutputLine(secondary, 2, 3, false);
            outputs[(int)OutputSignals.LoadResCtrl5VT10K] = new OutputLine(secondary, 2, 4, false);
            outputs[(int)OutputSignals.LoadResRtc3V3C] = new OutputLine(secondary, 2, 5, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 2, 6, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 2, 7, false);

            outputs[(int)OutputSignals.BootConfig1_5] = new OutputLine(secondary, 3, 0, false);
            outputs[(int)OutputSignals.BootConfig1_6] = new OutputLine(secondary, 3, 1, false);
            outputs[(int)OutputSignals.BootConfig1_7] = new OutputLine(secondary, 3, 2, false);
            outputs[(int)OutputSignals.BootConfig2_3] = new OutputLine(secondary, 3, 3, false);
            outputs[(int)OutputSignals.BootConfig2_4] = new OutputLine(secondary, 3, 4, false);
            outputs[(int)OutputSignals.BootConfig2_5] = new OutputLine(secondary, 3, 5, false);
            outputs[(int)OutputSignals.BootConfig2_6] = new OutputLine(secondary, 3, 6, false);
            outputs[(int)OutputSignals.BootConfig2_7] = new OutputLine(secondary, 3, 7, false);

            outputs[(int)OutputSignals.ButtonDownSim] = new OutputLine(secondary, 4, 0, false);
            outputs[(int)OutputSignals.ButtonUpSim] = new OutputLine(secondary, 4, 1, false);
            outputs[(int)OutputSignals.ButtonISim] = new OutputLine(secondary, 4, 2, false);
            outputs[(int)OutputSignals.Button1Sim] = new OutputLine(secondary, 4, 3, false);
            outputs[(int)OutputSignals.Button2Sim] = new OutputLine(secondary, 4, 4, false);
            outputs[(int)OutputSignals.Button3Sim] = new OutputLine(secondary, 4, 5, false);
            outputs[(int)OutputSignals.Button4Sim] = new OutputLine(secondary, 4, 6, false);
            outputs[(int)OutputSignals.Button5Sim] = new OutputLine(secondary, 4, 7, false);

            outputs[(int)OutputSignals.ScopeAddr0] = new OutputLine(secondary, ScopeAddrPort, 0, true);
            outputs[(int)OutputSignals.ScopeAddr1] = new OutputLine(secondary, ScopeAddrPort, 1, true);
            outputs[(int)OutputSignals.ScopeAddr2] = new OutputLine(secondary, ScopeAddrPort, 2, true);
            outputs[(int)OutputSignals.ScopeAddr3] = new OutputLine(secondary, ScopeAddrPort, 3, true);
            outputs[(int)OutputSignals.ScopeAddr4] = new OutputLine(secondary, ScopeAddrPort, 4, true);
            outputs[(int)OutputSignals.ScopeAddr5] = new OutputLine(secondary, ScopeAddrPort, 5, true);
            outputs[(int)OutputSignals.ScopeAddr6] = new OutputLine(secondary, ScopeAddrPort, 6, true);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 5, 7, false);

            outputs[(int)OutputSignals.ButtonMinusSim] = new OutputLine(secondary, 6, 0, false);
            outputs[(int)OutputSignals.ButtonPlusSim] = new OutputLine(secondary, 6, 1, false);
            outputs[(int)OutputSignals.ButtonStartSim] = new OutputLine(secondary, 6, 2, false);
            outputs[(int)OutputSignals.ButtonMuteSim] = new OutputLine(secondary, 6, 3, false);
            outputs[(int)OutputSignals.SdCardDetect] = new OutputLine(secondary, 6, 4, false);
            outputs[(int)OutputSignals.SDWriteProtect] = new OutputLine(secondary, 6, 5, false);
            outputs[(int)OutputSignals.MotorPowerDumpComp] = new OutputLine(secondary, 6, 6, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 6, 7, false);

            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 7, 0, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 7, 1, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 7, 2, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 7, 3, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 7, 4, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 7, 5, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 7, 6, false);
            //outputs[(int)OutputSignals.] = new OutputLine(secondary, 7, 7, false);

            //For debug checking if all index values have an assigned output
            for (int i = 0; i < outputs.Length; i++)
            {
                if(outputs[i].slot != primary && outputs[i].slot != secondary)
                {
                    //MessageBox.Show("Unitialized PXI output");
                }
            }
        }


        //Funtion to enable the specified output signal
        public bool EnableOutput(OutputSignals signal, bool enable)
        {
            OutputLine output = outputs[(int)signal];
            
            //Check if signal has a valid device association
            if(output.slot == "")
            {
                return false;
            }

            //Check if signal has a valid port association
            if(output.port < 0 || output.port >= PortsPerDevice)
            {
                return false;
            }

            //Check if signal has a valid line association
            if(output.line < 0 || output.line >= LinesPerPort)
            {
                return false;
            }

            //Enable or disable the signal based on the assigned device slot, port, and line
            bool outputValue = (enable != outputs[(int)signal].activeHigh);
            string outputChannel = outputs[(int)signal].slot;
            outputChannel += "/port" + outputs[(int)signal].port.ToString();
            outputChannel += "/line" + outputs[(int)signal].line.ToString();
            try
            {
                using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                {
                    digitalWriteTask.DOChannels.CreateChannel(outputChannel, "", ChannelLineGrouping.OneChannelForEachLine);
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSampleSingleLine(true, outputValue);
                }
            }
            catch
            {

            }

            return true;
        }

        public void SetScopeAddr(int addr)
        {
            byte addrOut = (byte)(~addr & 0xFF);

            //Ignore any invalid addresses and set to 0
            if(addr > MaxAddr)
            {
                addrOut = 0;
            }

            //Set address using a single port write
            try
            {
                using (NationalInstruments.DAQmx.Task digitalWriteTask = new NationalInstruments.DAQmx.Task())
                {
                    string addressCh = outputs[(int)OutputSignals.ScopeAddr0].slot;
                    addressCh += "/port" + outputs[(int)OutputSignals.ScopeAddr0].port.ToString();
                    //addressCh += "/line0:7";
                    digitalWriteTask.DOChannels.CreateChannel(addressCh, "", ChannelLineGrouping.OneChannelForAllLines);
                    DigitalSingleChannelWriter writer = new DigitalSingleChannelWriter(digitalWriteTask.Stream);
                    writer.WriteSingleSamplePort(true, addrOut);
                }
            }
            catch
            {

            }

            //Wait for relays to switch
            Thread.Sleep(RelayWaitMs);
        }
    }
}
