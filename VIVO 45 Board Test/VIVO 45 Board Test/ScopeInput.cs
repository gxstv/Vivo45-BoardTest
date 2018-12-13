using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using PS4000AImports;
using PicoStatus;
using PicoPinnedArray;

namespace VIVO_45_Board_Test
{
    class ScopeInput
    {
        const Imports.Range MaxRange = Imports.Range.Range_50V;
        const Imports.Range MinxRange = Imports.Range.Range_10MV;
        const int MaxSampleRate = 80000000;
        double[] inputRanges = { 0.01, 0.02, 0.05, 0.1, 0.2, 0.5, 1, 2, 5, 10, 20, 50 };

        bool blockReady;
        short handle;
        uint status;

        PinnedArray<short>[] dataBuffers;
        List<int> crossingIdx;

        //list of scope inputs based on the mux address of test fixture
        public enum MuxedInputSignal
        {
            //0-7
            InvalidSignal = 0,
            PowerOnExtBatt,
            PowerOnIntBatt,
            Supply1V2,
            Overlay5VC,
            Overlay3V3C,
            Led4Mains,
            ButtonLed,

            //8-15
            TermRegDDR,
            Supply1V8,
            Supply2V5,
            CapVddSOC,
            CapVddPU,
            CapVddARM,
            CapVddHIGH,
            CapVddSNVS,

            //16-23
            CapVddIN,
            RtcOneHz,
            RtcSupply,
            RtcBatt,
            Supply4V5C,
            Supply5VCDiv,
            SupplyRaPower,
            Supply5VTLimiter,

            //24-31
            NurseCall,
            Supply12VC,
            Supply3V3CLimiter,
            SourceEePower,
            SupercapCapStack,
            SupercapNtc,
            HeaterPlateOutput,
            HeaterPlate5VT,

            //32-39
            HeaterPlateGnd,
            HeatedWireSupply,
            HeatedWireControl,
            CoolingFanSupply,
            HeatedWirePcbPower,
            Transducer,
            FixtureRevision,
            ControllerRevision,

            //40-47
            FixtureTP806,
            MotorPower,
            MotorPowerDump,
            PowerMuxMainsDc,
            PowerMuxExtDc,
            PowerMuxExtBatt,
            PowerMuxIntBatt,
            Supply5VC,

            //48-55
            Supply5VCEnable,
            Supply5VT,
            SensorBoard5VC,
            SensorBoard5VT,
            Supply4V5T,
            BoardLedT,
            Supply5VTDiv,
            BoardLedC,

            //56-63
            Supply3V3T,       //Note this is a rev6+ only signal
            MotorPowerDiv,    //Note this is a rev6+ only signal
            Supply5VTEnable,
            Lcd3V3C,          //Note this is a rev6+ only signal
            Supply5VCLimiter, //Note this is a rev6+ only signal
            PiezoValveCtrl,   //Note this is a rev6+ only signal
            PiezoValve12VT,   //Note this is a rev6+ only signal
            PiezoValve5VT,    //Note this is a rev6+ only signal

            //64-71
            SupplyExtBattCharger,
            SupplyIntBattCharger,
            SupplyEmmcVdd,
            RemotteAlarmTxParsed,
            MuxedPwr,
            ExtBatVolt = 72,
            InBatVolt = 73
        }

        public ScopeInput()
        {
        }

        public bool Connect()
        {
            dataBuffers = new PinnedArray<short>[Imports.OCTO_SCOPE];
            status = Imports.OpenUnit(out handle, null);

            if (handle <= 0)
            {
                return false;
            }

            if (status == StatusCodes.PICO_USB3_0_DEVICE_NON_USB3_0_PORT || status == StatusCodes.PICO_POWER_SUPPLY_NOT_CONNECTED)
            {
                MessageBox.Show("Please connect scope to USB 3.0 port");
                return false;
            }

            //Disable all scope channels
            if(!DisableAllScopeChannels())
            {
                return false;
            }

            //Disable trigger
            if (Imports.SetSimpleTrigger(handle, 0, Imports.Channel.CHANNEL_A, 0, Imports.ThresholdDirection.Rising, 0, 0) != 0)
            {
                return false;
            }

            return true;
        }

        public void Disconnect()
        {
            try
            {
                DisableAllScopeChannels();
                Imports.CloseUnit(handle);
            }
            catch
            {
            }
            
        }

        private bool DisableAllScopeChannels()
        {
            for (int i = 0; i < Imports.OCTO_SCOPE; i++)
            {
                if (!EnableScopeChannel(i, 0, false))
                {
                    return false;
                }
            }
            return true;
        }

        private bool EnableScopeChannel(int channel, Imports.Range range, bool enable)
        {
            short enabled = 0;

            if(enable)
            {
                enabled = 1;
            }
            //Try to set the channel based on the given settings
            if (Imports.SetChannel(handle, Imports.Channel.CHANNEL_A + channel, enabled, 1, range, 0) != 0)
            {
                return false;
            }

            return true;
        }

        private void InitScopeBuffer(int channel, int size)
        {
            short[] channelBuffer = new short[size];
            dataBuffers[channel] = new PinnedArray<short>(channelBuffer);
            Imports.SetDataBuffer(handle, Imports.Channel.CHANNEL_A + channel, channelBuffer, size, 0, Imports.DownSamplingMode.None);
        }

        public short[] GetScopeBuffer(int channel)
        {
            if(channel > Imports.OCTO_SCOPE)
            {
                return null;
            }
            return dataBuffers[channel].Target;
        }

        public double GetScopeQuickValue(int channel, Imports.Range range)
        {
            double avg = 0;
            int numReadings = 500;
            uint sampleCount = (uint)numReadings;
            int timeoutCount = 0;
            int maxTimeoutCount = 100;
            int timeIndisposed;
            short overflow;

            if (range > MaxRange)
            {
                range = MaxRange;
            }

            //Enable scope with given range settings
            EnableScopeChannel(channel, range, true);

            //Disable trigger
            Imports.SetSimpleTrigger(handle, 0, Imports.Channel.CHANNEL_A, 0, Imports.ThresholdDirection.Rising, 0, 0);

            //Initialize the scope buffer
            InitScopeBuffer(channel, numReadings);

            //Start block read
            uint timebase = (MaxSampleRate / 1000000)-1;    //Sample at 1 MHz
            Imports.ps4000aBlockReady _callbackDelegate = BlockCallback;
            blockReady = false;
            status = Imports.RunBlock(handle, 0, numReadings, timebase, out timeIndisposed, 0, _callbackDelegate, IntPtr.Zero);

            //Wait for read to finish or timeout
            while (!blockReady && (timeoutCount < maxTimeoutCount))
            {
                Thread.Sleep(1);
                timeoutCount++;
            }

            //Ensure we did not timeout
            if(timeoutCount >= maxTimeoutCount)
            {
                return -1;
            }

            //Try to get readings
            status = Imports.GetValues(handle, 0, ref sampleCount, 1, Imports.DownSamplingMode.None, 0, out overflow);

            //Check if reading was successful
            if(sampleCount != numReadings || status != 0)
            {
                return -1;
            }

            //Get average value
            for (int i = 0; i < numReadings; i++)
            {
                avg += dataBuffers[channel].Target[i];
            }

            EnableScopeChannel(channel, range, false);
            dataBuffers[channel] = null;

            //Conversion from adc value to volts
            avg = (avg / numReadings);
            double scaling = inputRanges[(int)range] / Imports.MaxValue;
            return ((double)avg * scaling);
        }

        public bool CaptureScopeSignal(int[] channels, Imports.Range range, int sampleTimeMs)
        {
            short overflow;
            int timeIndisposed;

            //Set timeout values
            int timeoutCount = 0;
            int maxTimeoutCount = sampleTimeMs + 500;

            //Set number of samples and calculate time base
            int numReadings = 1000000;   //Get 1M readings
            uint sampleCount = (uint)numReadings;
            double frequency = 1000 * (numReadings / sampleTimeMs);
            uint timebase = (uint)(MaxSampleRate / frequency) - 1;    //Sample at 1 MHz

            if(channels.Length < 1 || channels.Length > Imports.OCTO_SCOPE)
            {
                return false;
            }

            //Disable trigger
            Imports.SetSimpleTrigger(handle, 0, Imports.Channel.CHANNEL_A+channels[0], 0, Imports.ThresholdDirection.Rising, 0, 0);

            foreach(int channel in channels)
            {
                //Enable scope with given range settings
                EnableScopeChannel(channel, range, true);

                InitScopeBuffer(channel, numReadings);
            }

            //Start block read
            Imports.ps4000aBlockReady _callbackDelegate = BlockCallback;
            blockReady = false;
            status = Imports.RunBlock(handle, 0, numReadings, timebase, out timeIndisposed, 0, _callbackDelegate, IntPtr.Zero);

            //Wait for data to be ready or timeout to occur
            while (!blockReady && (timeoutCount < maxTimeoutCount))
            {
                Thread.Sleep(1);
                timeoutCount++;
            }

            //Ensure we did not timeout
            if (timeoutCount >= maxTimeoutCount)
            {
                return false;
            }

            //Import values
            status = Imports.GetValues(handle, 0, ref sampleCount, 1, Imports.DownSamplingMode.None, 0, out overflow);

            //Check if reading was successful
            if(sampleCount != numReadings || status != 0)
            {
                return false;
            }

            return true;
        }

        public int GetScopeCrossingCounts(int channel, double expectedVoltage, int sampleTimeMs)
        {
            crossingIdx = new List<int>();
            int count = 0;
            double hysteresisFactor = 0.05;
            double crossThreshold = expectedVoltage / 2;
            Imports.Range range = MaxRange;
            bool rising = true;
            double voltage;


            //Get the voltage range using the expected voltage of the signal
            for (int i = 0; i < inputRanges.Length; i++)
            {
                if (expectedVoltage < inputRanges[i])
                {
                    range = (Imports.Range)(MinxRange + i);
                    break;
                }
            }
            double scaling = inputRanges[(int)range] / Imports.MaxValue;
            int[] channels = { channel };

            //Capture PWM waveform
            if (!CaptureScopeSignal(channels, range, sampleTimeMs))
            {
                return -1;
            }

            //Set rising or falling based on initial sample level
            if((double)dataBuffers[channel].Target[0] * scaling < crossThreshold)
            {
                rising = true;
            }
            else
            {
                rising = false;
            }
            //System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\MainBd_TestStand_2\Desktop\waveform.txt");

            //Iterate over results and get crossing counts
            for (
                int i = 0; i < dataBuffers[channel].Target.Length; i++)
            {
                
                double adc = (double)dataBuffers[channel].Target[i];
                //if(i<1000000)
                //    file.WriteLine(adc);
                //Convert voltage
                voltage = (adc * scaling);
                //If rising, check for voltage above threshold + hysteresis
                if (rising)
                {
                    if (voltage > (crossThreshold * (1 + hysteresisFactor)))
                    {
                        crossingIdx.Add(i);
                        count++;
                        rising = false;
                    }
                }
                //If falling, check for voltage below threshold + hysteresis
                else
                {
                    if (voltage < (crossThreshold * (1 - hysteresisFactor)))
                    {
                        crossingIdx.Add(i);
                        count++;
                        rising = true;
                    }
                }
            }

            return count;
        }

        public double GetScopePwmFrequency(int channel, double expectedVoltage, int sampleTimeMs)
        {
            int crossCount = GetScopeCrossingCounts(channel, expectedVoltage, sampleTimeMs);

            if(crossCount < 2)
            {
                return -1;
            }

            return CalculateSignalPwmFrequency(channel, sampleTimeMs);
        }

        public double CalculateSignalPwmFrequency(int channel, int sampleTimeMs)
        {
            int count = 0;
            int firstCross;
            int lastCross;

            if(crossingIdx.Count < 2)
            {
                return -1;
            }

            firstCross = crossingIdx[0];
            lastCross = crossingIdx[crossingIdx.Count - 1];

            //Calculate the total crossing time in seconds
            double crossingTime = lastCross - firstCross;
            crossingTime /= dataBuffers[channel].Target.Length;
            crossingTime *= sampleTimeMs;
            crossingTime /= 1000;

            //Frequency is half the number of crosses (since we count both edges) over the total crossing time
            return count / (crossingTime * 2);
        }

        public double CalculateSignalMaxVoltage(int channel)
        {
            short max = 0;
            foreach(short val in dataBuffers[channel].Target)
            {
                if(val > max)
                {
                    max = val;
                }
            }
            return max;
        }

        void BlockCallback(short handle, short status, IntPtr pVoid)
        {
            blockReady = true;
        }
    }
}
