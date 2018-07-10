using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

using Breas.Device;
using Breas.Device.Finder;
using Breas.Device.Vivo;
using Breas.Device.Vivo45;
using Breas.Device.Vivo45.Messenger;
using Breas.Device.Finder.Windows.Usb;
using Breas.Device.Monitoring.Measurements;

using PS4000AImports;
using PicoStatus;
using PicoPinnedArray;

namespace VIVO_45_Board_Test
{
    class FixtureHardware
    {
        public DeviceUnderTest device;
        public ProgrammablePowerSupply pwrSupply;
        public PressureSensor atmSensor;
        public DigitalOutput outputController;
        public ScopeInput scopeInput;

        int mainsDcChannel = 1;
        int extDcChannel = 4;
        int extBattChannel = 2;
        int intBattChannel = 3;

        int mainScopeChannel = 0;
        int supercapHiScopeChannel = 1;
        int supercapMidScopeChannel = 2;
        int supercapReturnScopeChannel = 3;
        int speakerScopeChannel1 = 4;
        int speakerScopeChannel2 = 5;
        

        const double FixtureVoltage = 18.0;
        const double FixtureVoltageTolerance = 0.5;

        public FixtureHardware()
        {
            device = new DeviceUnderTest();
        }

        public int MainsDcChannel { get { return mainsDcChannel; } }
        public int ExtDcChannel { get { return extDcChannel; } }
        public int ExtBattChannel { get { return extBattChannel; } }
        public int IntBattChannel { get { return intBattChannel; } }
        public int MainScopeChannel { get { return mainScopeChannel; } }
        

        public bool ConnectToFixtureHw(BackgroundWorker bw, int[] pwrChannels, int[] scopeChannels, int[] outChannels, bool useSensor)
        {
            pwrSupply = new ProgrammablePowerSupply();
            atmSensor = new PressureSensor();
            outputController = new DigitalOutput();
            scopeInput = new ScopeInput();
            device = new DeviceUnderTest();

            //Setup power supplies
            bw.ReportProgress(0, "Initializing power supplies");

            //Set up power supply channels
            //Add one as power supply channels start at idx 1
            mainsDcChannel = pwrChannels[0] + 1;
            extDcChannel = pwrChannels[1] + 1;
            extBattChannel = pwrChannels[2] + 1;
            intBattChannel = pwrChannels[3] + 1;

            //Connect to power supplies
            if (!pwrSupply.Connect())
            {
                MessageBox.Show("Error communicating with power supply");
                return false;
            }

            //Ensure power removal is complete
            pwrSupply.DisableOutput();
            Thread.Sleep(500);

            //Set up PXI digital outputs
            bw.ReportProgress(0, "Initializing digital outputs");
            if(!outputController.Connect(outChannels[0], outChannels[1]))
            {
                return false;
            }

            outputController.EnableOutput(DigitalOutput.OutputSignals.Ps4SelExtDC, true);

            //Set power output settings
            bw.ReportProgress(0, "Enabling power");
            pwrSupply.EnableChannel(mainsDcChannel);
            pwrSupply.EnableChannel(extDcChannel);
            pwrSupply.EnableChannel(extBattChannel);
            pwrSupply.EnableChannel(intBattChannel);
            pwrSupply.SetCurrent(mainsDcChannel, 2.5);
            pwrSupply.SetCurrent(extDcChannel, 1.5);
            pwrSupply.SetCurrent(extBattChannel, 2);
            pwrSupply.SetCurrent(intBattChannel, 2);
            pwrSupply.SetVoltage(mainsDcChannel, FixtureVoltage);
            pwrSupply.SetVoltage(extDcChannel, FixtureVoltage);
            pwrSupply.SetVoltage(extBattChannel, FixtureVoltage);
            pwrSupply.SetVoltage(intBattChannel, FixtureVoltage);
            pwrSupply.EnableOutput();
            
            //Wait for power supplies to stabilize
            Thread.Sleep(2000);
            /*
            //Check power OK
            bool voltageOk = true;
            double voltageReading = pwrSupply.GetVoltageMeasurement(mainsDcChannel);
            voltageOk &= (voltageReading >= (FixtureVoltage - FixtureVoltageTolerance));

            voltageReading = pwrSupply.GetVoltageMeasurement(extDcChannel);
            voltageOk &= (voltageReading >= (FixtureVoltage - FixtureVoltageTolerance));
            
            voltageReading = pwrSupply.GetVoltageMeasurement(extBattChannel);
            voltageOk &= (voltageReading >= (FixtureVoltage - FixtureVoltageTolerance));
            
            voltageReading = pwrSupply.GetVoltageMeasurement(intBattChannel);
            voltageOk &= (voltageReading >= (FixtureVoltage - FixtureVoltageTolerance));
            
            if(!voltageOk)
            {
                pwrSupply.DisableOutput();
                return false;
            }*/

            outputController.EnableOutput(DigitalOutput.OutputSignals.SupercapDisconnect, true);

            //TODO potentially add in current measurements as well

            //Setup atmospheric sensor if required
            if (useSensor)
            {
                bw.ReportProgress(0, "Initializing pressure and temperature sensor");
                if (!atmSensor.Connect())
                {
                    MessageBox.Show("Error communicating with pressure sensor");
                    return false;
                }
            }

            //Set up scope
            bw.ReportProgress(0, "Initializing scope input");

            //Connect to scope
            if (!scopeInput.Connect())
            {
                MessageBox.Show("Error initializing scope");
                return false;
            }

            //Set scope channels
            mainScopeChannel = scopeChannels[0];
            speakerScopeChannel1 = scopeChannels[1];
            speakerScopeChannel2 = scopeChannels[2];
            supercapHiScopeChannel = scopeChannels[3];
            supercapMidScopeChannel = scopeChannels[4];
            supercapReturnScopeChannel = scopeChannels[5];

            //Wait for Vivo45 to come online
            bw.ReportProgress(0, "Waiting for unit startup");
            Thread.Sleep(20000);

            //Check for Vivo 45
            bw.ReportProgress(0, "Initializing Vivo45");

            if (!device.IsVivo45Connected())
            {
                //MessageBox.Show("VIVO 45 unit not found");
                //return false;
            }

            //Wait for MP communication to come online
            bw.ReportProgress(0, "Retrieving MP data");
            Thread.Sleep(3000);

            //Get MP list
            if (!device.ResetMPList() || !device.ResetDeviceInfoList() || !device.ResetSettingsList())
            {
                MessageBox.Show("Error communicating with VIVO 45 unit");
                //return false;
            }

            /*
            string commVersion = device.GetMpDeviceInfo("Communication SW version");

            string treatVersion = device.GetMpDeviceInfo("Treatment SW version");*/

            return true;
        }

        public void DisconnectFixtureHW()
        {
            //Disable and disconnect programmable power supplies
            pwrSupply.DisableOutput();
            pwrSupply.Disconnect();

            //Disable all digital outputs
            outputController.Disconnect();

            outputController.EnableOutput(DigitalOutput.OutputSignals.SupercapDisconnect, true);

            //Disconnect atmospheric pressure sensor
            atmSensor.Disconnect();

            //Disconnect scope
            scopeInput.Disconnect();

            device.Disconnect();

            //TODO any other disconnects here
        }

        public string GetFixtureHwRev()
        {
            //TODO set back to 0 default
            string rev = "5.0";
            /*
            double revSignal = GetScopeMeterReading(ScopeInput.MuxedInputSignal.FixtureRevision, Imports.Range.Range_10V);*/

            //TODO based on revSignal reading, set rev

            return rev;
        }        

        public double GetScopeMeterReading(ScopeInput.MuxedInputSignal signal, Imports.Range range)
        {
            //Set the scope address
            outputController.SetScopeAddr((int)signal);

            Thread.Sleep(50);

            double reading = scopeInput.GetScopeQuickValue(mainScopeChannel, range);

            //Reset the scope address
            outputController.SetScopeAddr(0);

            return reading;
        }

        public double GetScopePwmFrequency(ScopeInput.MuxedInputSignal signal, double expectedVoltage, int sampleTimeMs)
        {
            double pwmFreq = 0;

            //Set the scope address
            outputController.SetScopeAddr((int)signal);

            //Get crossing counts for given time frame
            pwmFreq = scopeInput.GetScopePwmFrequency(mainScopeChannel, expectedVoltage, sampleTimeMs);

            //Reset the scope address
            outputController.SetScopeAddr(0);

            return pwmFreq;
        }

        public double[] GetAudioWaveform(int sampleMs)
        {
            double[] waveform = { };

            int[] channels = { speakerScopeChannel1, speakerScopeChannel2 };

            //Try to capture scope input
            if(!scopeInput.CaptureScopeSignal(channels, Imports.Range.Range_10V, sampleMs))
            {
                return null;
            }

            short[] audio1 = scopeInput.GetScopeBuffer(speakerScopeChannel1);

            short[] audio2 = scopeInput.GetScopeBuffer(speakerScopeChannel2);

            if(audio1.Length != audio2.Length)
            {
                return null;
            }
            waveform = new double[audio1.Length];

            for(int i = 0; i < audio1.Length; i++)
            {
                waveform[i] = audio1[i] - audio2[i];
            }
            
            return waveform;
        }
    }
}
