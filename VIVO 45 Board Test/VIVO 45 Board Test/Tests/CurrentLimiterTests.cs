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
        private TestResult RunTestLimiterRA()
        {
            double meter = 0;
            double targetVoltage = 12;
            double tolerance = 0.05;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.CurrentLimiter,
                "Tests the RA Power (12VC) current limiter");

            //Add normal voltage and limited voltage conditions to result
            outputResult.AddCondition("Normal Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));
            outputResult.AddCondition("Limited Voltage", Operation.LessThan, targetVoltage * (1 - tolerance));
            outputResult.AddCondition("5VC supply", Operation.Between, 4.5, 5.5);
            outputResult.AddCondition("12VC supply", Operation.Between, 11, 13);

            //Switch in normal loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes12VCLim, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.SupplyRaPower, Imports.Range.Range_20V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Normal Voltage", meter);

            //TEST
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VC, Imports.Range.Range_10V);
            outputResult.SetOutcome("5VC supply", meter);
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply12VC, Imports.Range.Range_20V);
            outputResult.SetOutcome("12VC supply", meter);

            //Switch in loading resistor to overload limiter
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.OverloadRes12VCLim, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.SupplyRaPower, Imports.Range.Range_20V);

            //Switch out loading resistors
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.OverloadRes12VCLim, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes12VCLim, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Set the test outcome and return result
            outputResult.SetOutcome("Limited Voltage", meter);
            return outputResult;
        }

        private TestResult RunTestLimiter5VT()
        {
            double meter = 0;
            double targetVoltage = 5;
            double tolerance = 0.05;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.CurrentLimiter,
                "Tests the 5V treatment current limiter");

            //Add normal voltage and limited voltage conditions to result
            outputResult.AddCondition("Normal Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));
            outputResult.AddCondition("Limited Voltage", Operation.LessThan, targetVoltage * (1 - tolerance));

            //Switch in normal loading resistor
            //fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes5VTLim, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VTLimiter, Imports.Range.Range_10V);

            //Set the test outcome and return result

            outputResult.SetOutcome("Normal Voltage", meter);

            //Switch in loading resistor to overload limiter
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.OverloadRes5VTLim, true);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes5VTLim, true);
            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VTLimiter, Imports.Range.Range_10V);

            //Switch out loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.OverloadRes5VTLim, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes5VTLim, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Set the test outcome and return result
            outputResult.SetOutcome("Limited Voltage", meter);
            return outputResult;
        }

        private TestResult RunTestLimiter5VC()
        {
            double meter = 0;
            double targetVoltage = 5;
            double tolerance = 0.05;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.CurrentLimiter,
                "Tests the 5V communication current limiter");

            //Add normal voltage and limited voltage conditions to result
            outputResult.AddCondition("Normal Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));
            outputResult.AddCondition("Limited Voltage", Operation.LessThan, targetVoltage * (1 - tolerance));

            //Switch in normal loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes5VCLim, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VCLimiter, Imports.Range.Range_10V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Normal Voltage", meter);

            //Switch in loading resistor to overload limiter
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.OverloadRes5VCLim, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VCLimiter, Imports.Range.Range_10V);

            //Switch out loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.OverloadRes5VCLim, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes5VCLim, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Set the test outcome and return result
            outputResult.SetOutcome("Limited Voltage", meter);
            return outputResult;
        }

        private TestResult RunTestLimiter3V3C()
        {
            double meter = 0;
            double targetVoltage = 3.3;
            double tolerance = 0.05;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.CurrentLimiter,
                "Tests the (SD power) 3.3V communication current limiter");

            //Add normal voltage and limited voltage conditions to result
            outputResult.AddCondition("Normal Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));
            outputResult.AddCondition("Limited Voltage", Operation.LessThan, targetVoltage * (1 - tolerance));

            //Switch in normal loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes3V3CLim, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply3V3CLimiter, Imports.Range.Range_5V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Normal Voltage", meter);

            //Switch in loading resistor to overload limiter
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.OverloadRes3V3CLim, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply3V3CLimiter, Imports.Range.Range_5V);

            //Switch out loading resistor
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.OverloadRes3V3CLim, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadRes3V3CLim, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Set the test outcome and return result
            outputResult.SetOutcome("Limited Voltage", meter);
            return outputResult;
        }
    }
}
