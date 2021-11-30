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
        private TestResult RunTest12VT()
        {
            double conversionFactor = 0.0138655;
            double targetVoltage = 12;
            double tolerance = 0.05;

            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSupply,
                "Tests the 12V treatment power supply");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Switch in loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes12VT, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Get MP value
            double mpResult = (double)fixture.device.GetMpValue("12 Volt CPU Raw Ad");

            //Convert if valid value received
            if (mpResult != -1)
            {
                mpResult *= conversionFactor;
            }

            //Save value to test result
            outputResult.SetOutcome("Voltage", mpResult);

            //Switch out loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes12VT, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            return outputResult;
        }

        private TestResult RunTest12VC()
        {
            double meter = 0;
            double targetVoltage = 12;
            double tolerance = 0.05;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSupply,
                "Tests the 12V communication power supply");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Switch in loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes12VC, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply12VC, Imports.Range.Range_20V);

            //Switch out loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes12VC, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }

        private TestResult RunTest5VT()
        {
            double conversionFactor = 0.004394545;
            double targetVoltage = 5;
            double offset = 0.1;
            double tolerance = 0.05;
            double mpTolerance = 0.05;

            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSupply,
                "Tests the 5V treatment power supply");

            //Add voltage conditions to result
            outputResult.AddCondition("Voltage (scope)", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));
            //outputResult.AddCondition("Voltage (scope + divider)", Operation.Between, 2.35, 2.62);

            //Switch in loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes5VT, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure voltage using scope
            double meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VT, Imports.Range.Range_10V);

            //Save value to test result
            outputResult.SetOutcome("Voltage (scope)", meter);

            // Disable Voltage (scope + divider) test. 
            //Measure divided value using scope
            //meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VTDiv, Imports.Range.Range_5V) + offset;

            //Save value to test result
            //outputResult.SetOutcome("Voltage (scope + divider)", meter);

            //Get MP value of divided value
            double mpResult = (double)fixture.device.GetMpValue("5 Volt CPU Raw Ad");

            //Convert if valid value received
            if (mpResult != -1)
            {
                mpResult *= conversionFactor;
            }

            //Create new condition, checking that MP value is within tolerance of scope measurement
            //outputResult.AddCondition("Voltage (MP)", Operation.Between, meter * (1 - mpTolerance), meter * (1 + mpTolerance));
            // Use fix limits
            outputResult.AddCondition("Voltage (MP)", Operation.Between, 2.356, 2.616);

            //Save value to test result
            outputResult.SetOutcome("Voltage (MP)", mpResult);

            //Switch out loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes5VT, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            return outputResult;
        }

        private TestResult RunTest5VC()
        {
            double conversionFactor = 0.004394545;
            double targetVoltage = 5;
            double tolerance = 0.05;
            double mpTolerance = 0.05;
            
            // Disable Voltage (scope + divider) test. 
            // Use fix limits for Voltage (MP) test.
            
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSupply,
                "Tests the 5V communication power supply");

            //Add voltage conditions to result
            outputResult.AddCondition("Voltage (scope)", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));
            //Disable Voltage (scope + divider) test. 
            //outputResult.AddCondition("Voltage (scope + divider)", Operation.Between, 2.35, 2.62); 

            //Switch in loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes5VC, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure voltage using scope
            double meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VC, Imports.Range.Range_10V);

            //Save value to test result
            outputResult.SetOutcome("Voltage (scope)", meter);

            //Measure divided value using scope
            //Disable Voltage (scope + divider) test.
            //meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VCDiv, Imports.Range.Range_5V);

            //Save value to test result
            //Disable Voltage (scope + divider) test. 
            //outputResult.SetOutcome("Voltage (scope + divider)", meter);

            //Get MP value of divided value
            double mpResult = (double)fixture.device.GetMpValue("ADC Sensor Data 5VC");

            //Convert if valid value received
            if (mpResult != -1)
            {
                mpResult *= conversionFactor;
            }

            //Create new condition, checking that MP value is within tolerance of scope measurement
            //Use fix limits for Voltage(MP) test.
            //outputResult.AddCondition("Voltage (MP)", Operation.Between, meter * (1 - mpTolerance), meter * (1 + mpTolerance));
            outputResult.AddCondition("Voltage (MP)", Operation.Between, 2.377, 2.595);

            //Save value to test result
            outputResult.SetOutcome("Voltage (MP)", mpResult);

            //Switch out loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes5VC, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            return outputResult;
        }

        private TestResult RunTest3V3C()
        {
            double conversionFactor = 0.004394545;
            double targetVoltage = 3.3;
            double tolerance = 0.05;

            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSupply,
                "Tests the 3.3V communication power supply");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Switch in loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes3V3C, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Get MP value
            double mpResult = (double)fixture.device.GetMpValue("ADC Sensor Data 3V3C");

            //Convert if valid value received
            if (mpResult != -1)
            {
                mpResult *= conversionFactor;
            }

            //Save value to test result
            outputResult.SetOutcome("Voltage", mpResult);

            //Switch out loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes3V3C, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            return outputResult;
        }

        private TestResult RunTest3V3T()
        {
            double meter = 0;
            double targetVoltage = 3.3;
            double tolerance = 0.03;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSupply,
                "Tests the 3.3V treatment power supply");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply3V3T, Imports.Range.Range_5V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }

        private TestResult RunTestVSNVS()
        {
            double conversionFactor = 0.004394545;
            double targetVoltage = 3;
            double tolerance = 0.05;

            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSupply,
                "Tests the VSNVS (3V) power supply");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Switch in loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResVSNVS, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Get MP value
            double mpResult = (double)fixture.device.GetMpValue("ADC Sensor Data VSNVS");

            //Convert if valid value received
            if (mpResult != -1)
            {
                mpResult *= conversionFactor;
            }

            //Save value to test result
            outputResult.SetOutcome("Voltage", mpResult);

            //Switch out loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResVSNVS, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            return outputResult;
        }

        private TestResult RunTest2V5C()
        {
            double meter = 0;
            double targetVoltage = 2.5;
            double tolerance = 0.03;

            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSupply,
                "Tests the 2.5V communication power supply");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Switch in loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes2V5, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply2V5, Imports.Range.Range_5V);

            //Switch out loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes2V5, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }

        private TestResult RunTest1V8C()
        {
            double meter = 0;
            double targetVoltage = 1.8;
            double tolerance = 0.03;

            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSupply,
                "Tests the 1.8V communication power supply");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Switch in loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes1V8, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply1V8, Imports.Range.Range_5V);

            //Switch out loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes1V8, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }

        private TestResult RunTest1V5DDR()
        {
            double conversionFactor = 0.004394545;
            double targetVoltage = 1.5;
            double tolerance = 0.05;

            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSupply,
                "Tests the 1.5V DDR power supply");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Switch in loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes1V5, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Get MP value
            double mpResult = (double)fixture.device.GetMpValue("ADC Sensor Data 1V5DDR");

            //Convert if valid value received
            if (mpResult != -1)
            {
                mpResult *= conversionFactor;
            }

            //Save value to test result
            outputResult.SetOutcome("Voltage", mpResult);

            //Switch out loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes1V5, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            return outputResult;
        }

        private TestResult RunTest1V375C()
        {
            double conversionFactor = 0.004394545;
            double targetVoltage = 1.375;
            double tolerance = 0.05;

            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSupply,
                "Tests the 1.375V communication power supply");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Switch in loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes1V375, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Get MP value
            double mpResult = (double)fixture.device.GetMpValue("ADC Sensor Data 1V375C");

            //Convert if valid value received
            if (mpResult != -1)
            {
                mpResult *= conversionFactor;
            }

            //Save value to test result
            outputResult.SetOutcome("Voltage", mpResult);

            //Switch out loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes1V375, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            return outputResult;
        }

        private TestResult RunTest1V2C()
        {
            double meter = 0;
            double targetVoltage = 1.2;
            double tolerance = 0.03;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSupply,
                "Tests the 1.2V communication power supply");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Switch in loading resistor
            //fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes1V2, true); TBD load if resistor change

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply1V2, Imports.Range.Range_2V);

            //Switch out loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes1V2, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }

        private TestResult RunTestMotorPower()
        {
            double meter = 0;
            double targetVoltage = 36.6;
            double tolerance = 0.05;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSupply,
                "Tests the motor power supply");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Start motor
            fixture.device.StartCalibration();
            Thread.Sleep(waitMotorMs);

            fixture.device.SetMP("Motor ctrl", 10000);
            Thread.Sleep(waitMotorMs);

            //Switch in loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.MotorSim, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MotorPower, Imports.Range.Range_50V);

            //Switch out loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.MotorSim, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Turn off motor and exit calibration mode
            fixture.device.SetMP("Motor ctrl", motorOffPwm);
            fixture.device.EndCalibration();

            Thread.Sleep(waitMotorMs);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }

        private TestResult RunTestRtcBatt()
        {
            double meter = 0;
            double targetVoltage = 3;
            double tolerance = 0.10;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSupply,
                "Tests the RTC battery supply");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Switch in loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResRtcBatt, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.RtcBatt, Imports.Range.Range_5V);

            //Switch out loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResRtcBatt, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }
    }
}
