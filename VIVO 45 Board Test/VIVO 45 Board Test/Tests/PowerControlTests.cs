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
        private TestResult RunTestRAPowerControl()
        {
            double meter = 0;
            double targetVoltage = 12;
            double disableTargetVoltage = 1.3;
            double tolerance = 0.05;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerControl,
                "Tests the RA Power (12VC) output control");

            //Add normal voltage and disabled voltage conditions to result
            outputResult.AddCondition("Normal Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));
            outputResult.AddCondition("Disabled Voltage", Operation.Between, disableTargetVoltage * (1 - tolerance), disableTargetVoltage * (1 + tolerance));

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.SupplyRaPower, Imports.Range.Range_20V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Normal Voltage", meter);

            //Switch relay to change RA Power control
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.RAPwrDisable, true);

            //Delay for relay to switch and power supply to settle
            Thread.Sleep(3000);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.SupplyRaPower, Imports.Range.Range_20V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Disabled Voltage", meter);

            //Restore default RA Power control
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.RAPwrDisable, false);

            //Delay for relay to switch and power supply to settle
            Thread.Sleep(200);

            return outputResult;
        }

        private TestResult RunTestRtcBackup()
        {
            double meter = 0;
            double targetVoltage = 3.0;
            double tolerance = 0.10;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerControl,
                "Tests the RTC backup power");

            //Add normal voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));
            //Shut down the device to use backup power
            fixture.pwrSupply.DisableOutput();
            //Switch relay to change RA Power control
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResRtc3V3C, true);
            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);
            fixture.pwrSupply.DisableOutput();
            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.RtcSupply, Imports.Range.Range_5V);
            // Compensate Resistance and relay
            meter = meter * 10.0 + 0.7;
            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);

            //Restore default RA Power control
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResRtc3V3C, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);
            //Wait device to reboot
            fixture.pwrSupply.EnableOutput();
            Thread.Sleep(10000);
            return outputResult;
        }

        private TestResult RunTest5VCControl()
        {
            double meter = 0;
            double minMicroControlVoltage = 1.8;    //TBD change to real value
            double minGateThreshold = 1.5;

            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerControl,
                "Tests the control of the 5V communication supply");

            //Add normal voltage condition to result
            outputResult.AddCondition("Treatment Control Voltage", Operation.GreaterThan, minMicroControlVoltage);
            outputResult.AddCondition("Comm Control Voltage", Operation.GreaterThan, minMicroControlVoltage);
            outputResult.AddCondition("Mux Control Voltage", Operation.Between, minGateThreshold, minMicroControlVoltage);

            //Switch relay to load control line
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResCtrl5VC10K, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Turn off POWERON-TC control via MP
            fixture.device.SetMP("TBD POWERON-TC control", 0);

            //Measure control voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VCEnable, Imports.Range.Range_5V);

            //Set the test condition outcome
            outputResult.SetOutcome("Comm Control Voltage", meter);

            //Turn on POWERON-TC control via MP
            fixture.device.SetMP("TBD POWERON-TC control", 1);

            //Turn off POWERON-CC control via MP
            fixture.device.SetMP("TBD POWERON-CC control", 0);

            //Measure control voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VCEnable, Imports.Range.Range_5V);

            //Set the test condition outcome
            outputResult.SetOutcome("Treatment Control Voltage", meter);

            //Turn off POWERON-TC control via MP
            fixture.device.SetMP("TBD POWERON-TC control", 0);

            //Measure control voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VCEnable, Imports.Range.Range_5V);

            //Set the test condition outcome
            outputResult.SetOutcome("Mux Control Voltage", meter);

            //Turn on POWERON-TC and POWERON-CC control via MP
            fixture.device.SetMP("TBD POWERON-TC control", 1);
            fixture.device.SetMP("TBD POWERON-CC control", 1);

            //Remove load from control line
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResCtrl5VC10K, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Return result
            return outputResult;
        }

        private TestResult RunTest5VTControl()
        {
            double meter = 0;
            double minMicroControlVoltage = 1.8;    //TBD change to real value
            double minGateThreshold = 1.5;

            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerControl,
                "Tests the control of the 5V treatment supply");

            //Add normal voltage condition to result
            outputResult.AddCondition("Comm Control Voltage", Operation.GreaterThan, minMicroControlVoltage);
            outputResult.AddCondition("Treatment Control Voltage", Operation.GreaterThan, minMicroControlVoltage);
            outputResult.AddCondition("Mux Control Voltage", Operation.Between, minGateThreshold, minMicroControlVoltage);

            //Switch relay to load control line
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResCtrl5VT10K, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Turn off POWERON-TC control via MP
            fixture.device.SetMP("Surveil Test 1", 1);

            //Measure control voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VTEnable, Imports.Range.Range_5V);

            //Set the test condition outcome
            outputResult.SetOutcome("Comm Control Voltage", meter);

            //Turn on POWERON-T control via MP
            fixture.device.SetMP("Surveil Test 1", 1);

            //Turn off POWERON-C control via MP
            fixture.device.SetMP("BoardTestMp", 1);

            //Measure control voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VTEnable, Imports.Range.Range_5V);

            //Set the test condition outcome
            outputResult.SetOutcome("Treatment Control Voltage", meter);

            //Turn off POWERON-T control via MP
            fixture.device.SetMP("BoardTestMp", 1);

            //Measure control voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VTEnable, Imports.Range.Range_5V);

            //Set the test condition outcome
            outputResult.SetOutcome("Mux Control Voltage", meter);

            //Turn on POWERON-T and POWERON-C control via MP
            fixture.device.SetMP("Surveil Test 1", 1);
            fixture.device.SetMP("BoardTestMp", 1);

            //Remove load from control line
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResCtrl5VT10K, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Return result
            return outputResult;
        }

        private TestResult RunTest5VCReset()
        {
            double meter = 0;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerControl,
                "Tests reset ability of the 5V communication supply");

            //Add disabled voltage conditions to result
            outputResult.AddCondition("Disabled Voltage", Operation.Between, -0.1, 0.1);

            //Load control line to enable reset
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResCtrl5VC100, true);

            fixture.pwrSupply.DisableOutput();
            //Disable super cap
            fixture.device.SetMP("Surveil Test 1", 2);

            //Delay for relay to switch and power supply to settle
            Thread.Sleep(1000);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VC, Imports.Range.Range_10V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Disabled Voltage", meter);

            //Restore normal operation of control line
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResCtrl5VC100, false);
            fixture.pwrSupply.EnableOutput();
            //Delay for device restart
            Thread.Sleep(15000);  

            //Add condition checking for restored communication
            outputResult.AddCondition("Restored communication", Operation.Equal, 1);

            bool ReConnected = fixture.device.IsVivo45Connected();
            //Save result of communication check
            if(ReConnected)
                outputResult.SetOutcome("Restored communication", 1.0);
            else
                outputResult.SetOutcome("Restored communication", 0);

            return outputResult;
        }

        private TestResult RunTest5VTReset()
        {
            double meter = 0;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerControl,
                "Tests reset ability of the 5V treatment supply");

            //Add disabled voltage conditions to result
            outputResult.AddCondition("Disabled Voltage", Operation.Between, -0.1, 0.1);

            //Load control line to enable reset
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResCtrl5VT100, true);
            fixture.pwrSupply.DisableOutput();
            //Delay for relay to switch and power supply to settle
            Thread.Sleep(1000);

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply5VT, Imports.Range.Range_10V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Disabled Voltage", meter);

            //Restore normal operation of control line
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResCtrl5VT100, false);
            fixture.pwrSupply.EnableOutput();
            //Delay for device restart
            Thread.Sleep(15000);  

            //Add condition checking for restored communication
            outputResult.AddCondition("Restored communication", Operation.Equal, 1);

            bool ReConnected = fixture.device.IsVivo45Connected();
            //Save result of communication check
            if (ReConnected)
                outputResult.SetOutcome("Restored communication", 1.0);
            else
                outputResult.SetOutcome("Restored communication", 0);

            return outputResult;

        }

    }
}
