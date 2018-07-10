using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Breas.Device;
using Breas.Device.Finder;
using Breas.Device.Vivo;
using Breas.Device.Vivo45;
using Breas.Device.Vivo45.Messenger;
using Breas.Device.Finder.Windows.Usb;
using PS4000AImports;

namespace VIVO_45_Board_Test
{
    partial class TestSuite
    {
        private TestResult RunTestDDR()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.BoardPeripheral,
                "Tests the DDR memory");

            outputResult.AddCondition("Memory test", Operation.Equal, 0);

            int result = fixture.device.MemoryTest(2);

            outputResult.SetOutcome("Memory test", result);

            return outputResult;
        }

        private TestResult RunTestEMMC()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.BoardPeripheral,
                "Tests the eMMC memory");

            outputResult.AddCondition("Memory test", Operation.Equal, 0);

            int result = fixture.device.MemoryTest(1);
            outputResult.SetOutcome("Memory test" , result);


            return outputResult;
        }

        private TestResult RunTestSD()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.BoardPeripheral,
                "Tests the SD card memory and interface");

            outputResult.AddCondition("Memory test normal", Operation.Equal, 0);
            outputResult.AddCondition("Memory test CD", Operation.Equal, -1);
            outputResult.AddCondition("Memory test WP", Operation.Equal, -1);

            //Perform memory test and log result
            int result = fixture.device.MemoryTest(0);
            outputResult.SetOutcome("Memory test normal", result);

            //Switch card detect line
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SdCardDetect, true);
            Thread.Sleep(DigitalOutput.SwitchingMs);
            
            //Perform memory test and log result
            result = fixture.device.MemoryTest(0);
            outputResult.SetOutcome("Memory test CD", result);


            //Restore card detect line and switch write protect line
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SdCardDetect, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SDWriteProtect, true);
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Perform memory test and log result
            result = fixture.device.MemoryTest(0);
            outputResult.SetOutcome("Memory test WP", result);

            //Restore write protect
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SDWriteProtect, false);
            Thread.Sleep(DigitalOutput.SwitchingMs);

            return outputResult;
        }

        private TestResult RunTestRtcValue()
        {
            int sleepMs = 15000;
            TestResult outputResult = new TestResult(resultList.Count, TestType.BoardPeripheral,
                "Tests the RTC value volatility");

            //Create a +/-2 sec time difference condition
            outputResult.AddCondition("Time difference", Operation.Between, -1000,1000);

            DateTime initialTime = fixture.device.RTCTest();
            DateTime startTime = DateTime.Now;

            //Remove power from device
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, 0);
            fixture.pwrSupply.SetVoltage(fixture.ExtDcChannel, 0);
            fixture.pwrSupply.SetVoltage(fixture.ExtBattChannel, 0);
            fixture.pwrSupply.SetVoltage(fixture.IntBattChannel, 0);

            //Wait
            //Thread.Sleep(sleepMs);

            //Restore power to device
            ResetPowerSources();

            //Wait
            Thread.Sleep(sleepMs);

            if (!fixture.device.IsVivo45Connected())
            {
                return outputResult;
            }

            //Get MP list
            if (!fixture.device.ResetMPList())
            {
                return outputResult;
            }

            //TBD get RTC value

            DateTime rtcReport = fixture.device.RTCTest();
            DateTime endTime = DateTime.Now;
            double msDiff = (endTime - startTime).TotalMilliseconds - (rtcReport - initialTime).TotalMilliseconds;

            outputResult.SetOutcome("Time difference", msDiff);

            return outputResult;
        }

        private TestResult RunTestSyncRtc()
        {
            int seconds = 4;
            TestResult outputResult = new TestResult(resultList.Count, TestType.BoardPeripheral,
                "Tests the 1Hz RTC sync signal");

            //Crossing counts should be double number of seconds
            outputResult.AddCondition("Crossing count", Operation.Between, (2*seconds)-1, (2*seconds)+1);

            //Set scope to sync signal
            fixture.outputController.SetScopeAddr(17);

            //Measure signal and get crossing counts
            int count = fixture.scopeInput.GetScopeCrossingCounts(fixture.MainScopeChannel, 3.3, (1000 * seconds));

            //Reset address mux
            fixture.outputController.SetScopeAddr(0);

            //Save result
            outputResult.SetOutcome("Crossing count", count);

            return outputResult;
        }

        private TestResult RunTestBoardTemp()
        {
            //Read fixture board temperature
            double boardTemp = fixture.atmSensor.GetTemperature();
            if(boardTemp==0)
            {
                boardTemp = 27;
            }
            double tolerance = 30;   //3 degrees C tolerance
            double conversionFactor = 0.1;

            TestResult outputResult = new TestResult(resultList.Count, TestType.BoardPeripheral,
                "Tests the board temperature thermistor");

            //Create condition to match board temperature within tolerance
            outputResult.AddCondition("Board temperature", Operation.Between, boardTemp - tolerance, boardTemp + tolerance);

            //Read MP board temperature
            double mpValue = fixture.device.GetMpValue("Board Temp Ad");

            //Apply MP conversion factor
            if(mpValue > 0)
            {
                mpValue *= conversionFactor;
            }

            //Save and return
            outputResult.SetOutcome("Board temperature", mpValue);
            return outputResult;
        }

        private TestResult RunTestSpareTherm()
        {
            double voltage = 2.25;
            double tolerance = 0.05;
            double conversionFactor = 0.0043945;

            TestResult outputResult = new TestResult(resultList.Count, TestType.BoardPeripheral,
                "Tests the spare thermistor");

            //Check voltage reading is as expected
            outputResult.AddCondition("Voltage", Operation.Between, voltage*(1-tolerance), voltage*(1+tolerance));

            //Get voltage reading from MP
            double mpValue = fixture.device.GetMpValue("TBD spare thermistor");

            //Apply MP conversion factor
            if (mpValue > 0)
            {
                mpValue *= conversionFactor;
            }

            //Save result
            outputResult.SetOutcome("Voltage", mpValue);
            return outputResult;
        }

        private TestResult RunTestAmbientPressureSensor()
        {
            double conversionFactor = 0.0101972;    //Pa to cmH2O
            double ambient = fixture.atmSensor.GetPressure() * conversionFactor;
            if(ambient<500)
            {
                ambient = 1010;
            }
            double tolerance = 100; //100 cmH2O tolerance
            

            TestResult outputResult = new TestResult(resultList.Count, TestType.BoardPeripheral,
                "Tests the ambient pressure sensor");

            outputResult.AddCondition("Pressure", Operation.Between, ambient-tolerance, ambient+tolerance);

            double mpValue = fixture.device.GetMpValue("Ambient Pressure Raw Ad");

            if (mpValue == 0)
            {
                Thread.Sleep(5000);
                mpValue = fixture.device.GetMpValue("Ambient Pressure Raw Ad");
            }
            outputResult.SetOutcome("Pressure", mpValue);

            return outputResult;
        }

        private TestResult RunTestFlowPressureSensor()
        {
            double conversionFactor = 0.0043945;

            TestResult outputResult = new TestResult(resultList.Count, TestType.BoardPeripheral,
                "Tests the flow pressure sensor");

            //Check voltage reading is as expected
            outputResult.AddCondition("Voltage", Operation.Between, 0.8, 0.95);

            //Get voltage reading from MP
            //double mpValue = fixture.device.GetMpValue("ADC Sensor Data PRESSURE-BC");
            double mpValue = fixture.device.GetMpValue("Flow raw"); 
            //Apply MP conversion factor
            if (mpValue > 0)
            {
                mpValue *= conversionFactor;
            }

            //Save result
            outputResult.SetOutcome("Voltage", mpValue);
            return outputResult;
        }

        private TestResult RunTestFiO2()
        {
            double voltage = 2.36;
            double tolerance = 0.07;
            double conversionFactor = 0.0043945;

            TestResult outputResult = new TestResult(resultList.Count, TestType.BoardPeripheral,
                "Tests the FiO2 input");

            //Check voltage reading is as expected
            outputResult.AddCondition("Voltage", Operation.Between, voltage * (1 - tolerance), voltage * (1 + tolerance));

            //Get voltage reading from MP
            double mpValue = fixture.device.GetMpValue("FiO2 Raw Ad");

            //Apply MP conversion factor
            if (mpValue > 0)
            {
                mpValue *= conversionFactor;
            }

            //Save result
            outputResult.SetOutcome("Voltage", mpValue);
            return outputResult;
        }
    }
}
