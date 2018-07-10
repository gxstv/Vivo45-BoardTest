using System;
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
using PS4000AImports;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace VIVO_45_Board_Test
{
    partial class TestSuite
    {
        private TestResult RunTestLCD()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.UserInterface,
                "LCD display test");
            outputResult.AddCondition("LCD", Operation.Equal, 1);

            //Write to MP to enable test LCD output
            //fixture.device.SetMP("TBD LCD description", 1);

            //Ask user if LCD test image is displayed
            DialogResult lcdResult = MessageBox.Show("Is the LCD test image displayed?", "", MessageBoxButtons.YesNo);
            if (lcdResult == DialogResult.Yes)
            {
                outputResult.SetOutcome("LCD", 1);
            }
            else
            {
                outputResult.SetOutcome("LCD", 0);
            }

            //Restore default display functionality
            //fixture.device.SetMP("TBD LCD description", 0);

            return outputResult;
        }

        private TestResult RunTestBacklight()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.UserInterface,
                "LCD backlight test");
            outputResult.AddCondition("Backlight off", Operation.Equal, 1);
            outputResult.AddCondition("Backlight on", Operation.Equal, 1);
            outputResult.AddCondition("Backlight bright", Operation.Equal, 1);

            //Write MP to disable backlight
            fixture.device.SetMP("Display backlight", 1);

            //Ask user if the backlight is disabled
            DialogResult lcdResult = MessageBox.Show("Is the LCD backlight disabled?", "", MessageBoxButtons.YesNo);
            if (lcdResult == DialogResult.Yes)
            {
                outputResult.SetOutcome("Backlight off", 1);
            }
            else
            {
                outputResult.SetOutcome("Backlight off", 0);
            }

            //Write MP to set intermediate backlight level
            fixture.device.SetMP("Display backlight", 4);

            //Ask user if the backlight is disabled
            DialogResult lcdResult2 = MessageBox.Show("Is the LCD backlight on?", "", MessageBoxButtons.YesNo);
            if (lcdResult2 == DialogResult.Yes)
            {
                outputResult.SetOutcome("Backlight on", 1);
            }
            else
            {
                outputResult.SetOutcome("Backlight on", 0);
            }

            //Write MP to increase backlight
            fixture.device.SetMP("Display backlight", 7);

            //Ask user if the backlight is disabled
            DialogResult lcdResult3 = MessageBox.Show("Did the backlight intensity increase?", "", MessageBoxButtons.YesNo);
            if (lcdResult3 == DialogResult.Yes)
            {
                outputResult.SetOutcome("Backlight bright", 1);
            }
            else
            {
                outputResult.SetOutcome("Backlight bright", 0);
            }

            return outputResult;
        }
        
        private double[] GetFftSpectrum(double[] capture)
        {
            double[] spectrum = new double[capture.Length / 2];
            Complex[] buffer = new Complex[capture.Length];

            //Copy the audio capture into a complex buffer
            for (int i = 0; i < capture.Length; i++)
            {
                buffer[i] = new Complex(capture[i], 0);
            }

            Fourier.Forward(buffer);

            //Copy the FFT spectrum magnitude results for return
            for (int i = 0; i < spectrum.Length; i++)
            {
                spectrum[i] = buffer[i].Magnitude;
            }

            return spectrum;
        }
        

        private TestResult RunTestSpeakerAudio()
        {
            const double peakThreshold = 75000;
            const int hzPerBin = 5;

            TestResult outputResult = new TestResult(resultList.Count, TestType.UserInterface,
                "Speaker audio test");

            //Write MP to enable audio
            fixture.device.StartSpeaker();

            //Wait for speaker to enable
            Thread.Sleep(50);

            //Make sure there is only 1 peak frequency and it occurs at 700 Hz
            //outputResult.AddCondition("PeakCount", Operation.Equal, 1);
            outputResult.AddCondition("PeakIndex", Operation.Between, 130,150);
            outputResult.AddCondition("Frequency", Operation.Equal, 700);
            outputResult.AddCondition("Amplitude", Operation.GreaterThan, peakThreshold);

            //Get scope capture of audio at 5 MHz, 1M samples (200 ms)
            //Result will be 5 Hz per bin in FFT
            double[] audioCapture = fixture.GetAudioWaveform(200);

            if (audioCapture == null || audioCapture.Length == 0)
            {
                return outputResult;
            }

            //Get FFT
            double[] fft = GetFftSpectrum(audioCapture);

            //Check FFT for peaks, saving the max
            double maxPeakAmp = 0;
            double maxPeakFreq = 0;
            double peakCount = 0;
            int peekIndex = 0;
            for (int i = 1; i < fft.Length; i++)
            {
                if (fft[i] > peakThreshold)
                {
                    peakCount++;
                    if (fft[i] > maxPeakAmp)
                    {
                        peekIndex = i;
                        maxPeakAmp = fft[i];
                        maxPeakFreq = i * hzPerBin;
                    }
                }
            }

            //Save results
            outputResult.SetOutcome("PeakIndex", peekIndex);
            outputResult.SetOutcome("Frequency", maxPeakFreq);
            outputResult.SetOutcome("Amplitude", maxPeakAmp);
            return outputResult;
        }

        private TestResult RunTestSpeakerCurrent()
        {
            double conversionFactor = 0.1;  //TBD set real value
            double currentValue;
            int mpValue;

            TestResult outputResult = new TestResult(resultList.Count, TestType.UserInterface,
                "Speaker current test");

            outputResult.AddCondition("Off current", Operation.LessThan, 0.5);
            outputResult.AddCondition("On current", Operation.GreaterThan, 0.5);

            //Measure audio current and save result
            mpValue = fixture.device.GetMpValue("ADC Sensor Data SPKR");
            currentValue = (double)mpValue * conversionFactor;
            outputResult.SetOutcome("Off current", currentValue);

            //Enable audio output
            fixture.device.StartSpeaker();

            //Wait for speaker to enable
            Thread.Sleep(100);

            //Measure audio current and save result
            mpValue = fixture.device.GetMpValue("ADC Sensor Data SPKR");
            currentValue = (double)mpValue * conversionFactor;
            outputResult.SetOutcome("On current", currentValue);

            return outputResult;
        }

        private TestResult RunTestTransducerAudio()
        {
            //At 200 ms sample with 500Hz audio should produce 200 crossing counts
            double target = 200;
            double tolerance = 0.01;

            TestResult outputResult = new TestResult(resultList.Count, TestType.UserInterface,
                "Transducer audio test");

            outputResult.AddCondition("Crossing counts", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            outputResult.AddCondition("Max voltage", Operation.GreaterThan, 4);
            //Write MP to enable buzzer
            fixture.device.StartBuzzer();

            //Wait for buzzer to enable
            Thread.Sleep(100);

            //Set the scope address
            fixture.outputController.SetScopeAddr((int)ScopeInput.MuxedInputSignal.Transducer);

            //Get crossing counts for given time frame of 200 ms
            int counts = fixture.scopeInput.GetScopeCrossingCounts(fixture.MainScopeChannel, 5, 200);
            double maxVoltage = fixture.scopeInput.CalculateSignalMaxVoltage(fixture.MainScopeChannel);

            //Reset the scope address
            fixture.outputController.SetScopeAddr(0);

            //Save results
            outputResult.SetOutcome("Crossing counts", counts);
            outputResult.SetOutcome("Max voltage", maxVoltage);

            return outputResult;
        }

        private DigitalOutput.OutputSignals GetNextButton(int idx)
        {
            DigitalOutput.OutputSignals button = DigitalOutput.OutputSignals.ButtonDownSim;

            //Button order determined by firmware
            switch (idx)
            {
                case 0:
                    button = DigitalOutput.OutputSignals.Button1Sim;
                    break;
                case 1:
                    button = DigitalOutput.OutputSignals.Button2Sim;
                    break;
                case 2:
                    button = DigitalOutput.OutputSignals.Button3Sim;
                    break;
                case 3:
                    button = DigitalOutput.OutputSignals.Button4Sim;
                    break;
                case 4:
                    button = DigitalOutput.OutputSignals.Button5Sim;
                    break;
                case 5:
                    button = DigitalOutput.OutputSignals.ButtonISim;
                    break;
                case 6:
                    button = DigitalOutput.OutputSignals.ButtonDownSim;
                    break;
                case 7:
                    button = DigitalOutput.OutputSignals.ButtonPlusSim;
                    break;
                case 8:
                    button = DigitalOutput.OutputSignals.ButtonMinusSim; 
                    break;
                case 9:
                    button = DigitalOutput.OutputSignals.ButtonUpSim;
                    break;
                case 10:
                    button = DigitalOutput.OutputSignals.ButtonStartSim;
                    break;
                case 11:
                    button = DigitalOutput.OutputSignals.ButtonMuteSim;
                    break;
                default:
                    break;
            }

            return button;
        }

        private TestResult RunTestButtons()
        {
            int mpValue;
            int numKeys = 12;

            TestResult outputResult = new TestResult(resultList.Count, TestType.UserInterface,
                "Tests the input buttons");

            //Test each individual key can be pressed
            for (int i = 0; i < numKeys; i++)
            {
                outputResult.AddCondition("Button " + i.ToString(), Operation.Equal, Math.Pow(2, i));
            }

            //Iterate through key presses
            for (int i = 0; i < numKeys; i++)
            {
                DigitalOutput.OutputSignals nextButton = GetNextButton(i);

                //Simulate button press
                fixture.outputController.EnableOutput(nextButton, true);

                //Wait for signals to settle
                Thread.Sleep(DigitalOutput.SwitchingMs);

                //Check MP value
                mpValue = fixture.device.GetMpValue("Which keys are pressed");

                //Simulate button release
                fixture.outputController.EnableOutput(nextButton, false);

                //Save result
                outputResult.SetOutcome("Button " + i.ToString(), mpValue);
            }

            return outputResult;

        }

        private TestResult RunTestOverlayLEDs()
        {
            double conversionFactor = 0.0043945;
            double[] maxLedVoltage = { 1.13, 1.23, 1.34, 1.45, 1.54 };
            double[] minLedVoltage = { 0.97, 1.07, 1.17, 1.27, 1.39 };

            //Increased series resistance lowers expected voltages, resimulate at future date
            //Workaround for now
            for(int i = 0; i < minLedVoltage.Length; i++)
            {
                maxLedVoltage[i] *= 0.9;
                minLedVoltage[i] *= 0.9;
            }

            int numLeds = maxLedVoltage.Length;
            double ledVoltage = minLedVoltage[0];
            double minVoltage;

            TestResult outputResult = new TestResult(resultList.Count, TestType.UserInterface,
                "Overlay LEDs test");

            //Test Alarm LED (double diode, so tested seperately)
            //outputResult.AddCondition("Alarm LED", Operation.Between, 1.58, 1.81);
            //Increased series resistance lowers expected voltages, resimulate at future date
            //Workaround for now
            outputResult.AddCondition("Alarm LED", Operation.Between, 1.58*0.8, 1.81*0.8);


            //TODO get real MP description when implemented for alarm LED
            fixture.device.SetMP("Surveil Test 1", 3);  //Workaround MP provided by BRE

            //Check and save Alarm LED voltage
            ledVoltage = conversionFactor * (double)fixture.device.GetMpValue("ADC Sensor Data LED-CHECK");
            outputResult.SetOutcome("Alarm LED", ledVoltage);

            //Test other LEDs
            ledVoltage = minLedVoltage[0];
            for (int i = 0; i < numLeds; i++)
            {
                //Add expected range, ensuring that each LED voltage is at least as high as the previous
                minVoltage = (ledVoltage > minLedVoltage[i]) ? ledVoltage : minLedVoltage[i];
                outputResult.AddCondition("LED " + (i + 1).ToString(), Operation.Between, minVoltage, maxLedVoltage[i]);

                //Set each LED one at a time
                //TODO get real MP description when implemented
                fixture.device.SetMP("Surveil Test 1", i+4);    //Workaround MP provided by BRE

                //Get LED voltage value from MP
                ledVoltage = conversionFactor * (double)fixture.device.GetMpValue("ADC Sensor Data LED-CHECK");
                outputResult.SetOutcome("LED " + (i + 1).ToString(), ledVoltage);
            }

            return outputResult;
        }

        private TestResult RunTestMainsLED()
        {
            double voltage;

            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.UserInterface,
                "Mains LED test");

            //Expected on vs off voltage
            //outputResult.AddCondition("On voltage", Operation.Between, 2.3, 2.4);
            outputResult.AddCondition("On voltage", Operation.Between, 2.0, 2.4);   //Workaround for HW issues, lower tolerance
            outputResult.AddCondition("Off voltage", Operation.Between, -0.1, 0.1);

            //Set known power supply voltage
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, 18);

            //Wait for power supply to settle
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Turn on LED, read voltage and save value
            //fixture.device.SetMP("TBD mains LED", 1);   //No MP for now, on by default according to Breas
            voltage = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Led4Mains, Imports.Range.Range_5V);
            outputResult.SetOutcome("On voltage", voltage);

            //Turn off LED, read voltage and save value
            fixture.device.SetMP("Backlight brightness", 0);    //Workaround to set backlight to 0 to turn off LED
            voltage = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Led4Mains, Imports.Range.Range_5V);
            outputResult.SetOutcome("Off voltage", voltage);

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestButtonLED()
        {
            double voltage;
            double target = 2.5;
            double tolerance = 0.05;

            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.UserInterface,
                "Button LED test");

            //Expected on vs off voltage
            outputResult.AddCondition("On voltage", Operation.Between, -0.1, 0.1);
            outputResult.AddCondition("Off voltage", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));

            //Turn on LED, read voltage and save value
            fixture.device.SetMP("Backlight brightness", 100);
            voltage = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.ButtonLed, Imports.Range.Range_5V);
            outputResult.SetOutcome("On voltage", voltage);

            //Turn off LED, read voltage and save value
            fixture.device.SetMP("Backlight brightness", 0);
            voltage = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.ButtonLed, Imports.Range.Range_5V);
            outputResult.SetOutcome("Off voltage", voltage);

            return outputResult;
        }
    }
}
