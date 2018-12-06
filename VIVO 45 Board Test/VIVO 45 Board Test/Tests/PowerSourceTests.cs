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
using PicoStatus;
using PicoPinnedArray;

namespace VIVO_45_Board_Test
{
    partial class TestSuite
    {
        const double MainsVoltage = 19.0;
        const double VoltageStep = 0.5;
        const int PowerSettleMs = 500;

        private void ResetPowerSources()
        {
            //Set all power sources to default
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, MainsVoltage);
            fixture.pwrSupply.SetVoltage(fixture.ExtDcChannel, MainsVoltage);
            fixture.pwrSupply.SetVoltage(fixture.IntBattChannel, MainsVoltage);
            fixture.pwrSupply.SetVoltage(fixture.ExtBattChannel, MainsVoltage);

            //Enable all power sources using MP
            fixture.device.SetMP("Power Source", 11);

            //Delay for power sources to settle
            Thread.Sleep(PowerSettleMs);
        }

        private TestResult RunTestMainsOverride()
        {
            double meter = 0;
            double target = 17.5;
            double tolerance = 0.01;

            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSource,
                "Test the mains DC override functionality");

            //Add condition to look for mux voltage value
            outputResult.AddCondition("MUX Voltage", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));

            //Power device from MAINS DC but set other sources > MAINS DC
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, target);
            fixture.pwrSupply.SetVoltage(fixture.ExtDcChannel, target + (3 * VoltageStep));
            fixture.pwrSupply.SetVoltage(fixture.ExtBattChannel, target + (2 * VoltageStep));
            fixture.pwrSupply.SetVoltage(fixture.IntBattChannel, target + VoltageStep);

            //Wait for power supplies to settle
            Thread.Sleep(PowerSettleMs);

            //Load MUXED POWER
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMuxPower, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Enable all power sources using MP
            fixture.device.SetMP("Power Source", 0);  //TBD value as well, filler for now

            //Measure MUX voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MuxedPwr, Imports.Range.Range_20V);

            //Save value to result
            outputResult.SetOutcome("MUX Voltage", meter);

            //Diable load on MUXED POWER
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMuxPower, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestExtDcOverride()
        {
            double meter = 0;
            double target = 15;
            double tolerance = 0.05;
            double lowVoltage = 8;
            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSource,
                "Test the EXT DC override functionality");

            //Add condition to look for mux voltage value
            outputResult.AddCondition("MUX Voltage", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            
            //Enable auto switch
            fixture.device.SetMP("Power Source", 0);
            //Power device from EXT DC, set battery sources > EXT DC and mains DC < EXT DC
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, lowVoltage);// (target - VoltageStep));
            fixture.pwrSupply.SetVoltage(fixture.ExtDcChannel, target);
            fixture.pwrSupply.SetVoltage(fixture.ExtBattChannel, lowVoltage);//target + (2 * VoltageStep));
            fixture.pwrSupply.SetVoltage(fixture.IntBattChannel, lowVoltage);// target + VoltageStep);

            //Wait for power supplies to settle
            Thread.Sleep(PowerSettleMs);

            //Load MUXED POWER
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMuxPower, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Enable all power sources using MP
            //fixture.device.SetMP("TBD power source enable", 0x0F);  //TBD value as well, filler for now
            //fixture.device.SetMP("PowerSource", 11);
            //fixture.device.SetMP("PowerSource", 7);
            //fixture.device.SetMP("PowerSource", 9);

            //Measure MUX voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MuxedPwr, Imports.Range.Range_20V);

            //Save value to result
            outputResult.SetOutcome("MUX Voltage", meter);

            //Diable load on MUXED POWER
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMuxPower, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestExtBattOverride()
        {
            double meter = 0;
            double target = 15;
            double tolerance = 0.05;
            double lowVoltage = 8;

            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSource,
                "Test the external battery override functionality");

            //Add condition to look for mux voltage value
            outputResult.AddCondition("MUX Voltage", Operation.Between, target * (1 - 2 * tolerance), target * (1 + tolerance));
            //Enable auto switch
            fixture.device.SetMP("Power Source", 0);
            //Power device from external battery, set internal battery above, set EXT DC and mains low
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, lowVoltage);
            fixture.pwrSupply.SetVoltage(fixture.ExtDcChannel, lowVoltage);
            fixture.pwrSupply.SetVoltage(fixture.ExtBattChannel, target);
            fixture.pwrSupply.SetVoltage(fixture.IntBattChannel, target + (4 * VoltageStep));

            //Wait for power supplies to settle
            Thread.Sleep(PowerSettleMs);

            //Load MUXED POWER
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMuxPower, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Enable all power sources using MP
            //fixture.device.SetMP("TBD power source enable", 0x0F);  //TBD value as well, filler for now

            //Measure MUX voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MuxedPwr, Imports.Range.Range_20V);

            //Save value to result
            outputResult.SetOutcome("MUX Voltage", meter);

            //Diable load on MUXED POWER
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMuxPower, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestIntBattSource()
        {
            double meter = 0;
            double target = 15;
            double tolerance = 0.01;
            double lowVoltage = 8;

            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSource,
                "Test the internal battery source functionality");

            //Add condition to look for mux voltage value
            outputResult.AddCondition("MUX Voltage", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            //Enable auto switch
            fixture.device.SetMP("Power Source", 0);
            //Power device from external battery, set internal battery above, set EXT DC and mains low
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, lowVoltage);
            fixture.pwrSupply.SetVoltage(fixture.ExtDcChannel, lowVoltage);
            fixture.pwrSupply.SetVoltage(fixture.ExtBattChannel, lowVoltage);
            fixture.pwrSupply.SetVoltage(fixture.IntBattChannel, target);

            //Wait for power supplies to settle
            Thread.Sleep(PowerSettleMs);

            //Load MUXED POWER
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMuxPower, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Enable all power sources using MP
            //fixture.device.SetMP("TBD power source enable", 0x0F);  //TBD value as well, filler for now

            //Measure MUX voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MuxedPwr, Imports.Range.Range_20V);

            //Save value to result
            outputResult.SetOutcome("MUX Voltage", meter);

            //Diable load on MUXED POWER
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMuxPower, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestMainsDisable()
        {
            double meter = 0;
            double target = 19;
            double nextTarget = target - VoltageStep;
            double tolerance = 0.01;

            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSource,
                "Test the mains DC disable functionality");

            //Add condition to look for mux voltage value
            outputResult.AddCondition("MUX Voltage first", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            outputResult.AddCondition("MUX Voltage second", Operation.Between, nextTarget * (1 - tolerance), nextTarget * (1 + tolerance));

            //Power device from mains DC with each other voltage source decreasing by one voltage step
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, target);
            fixture.pwrSupply.SetVoltage(fixture.ExtDcChannel, nextTarget);
            fixture.pwrSupply.SetVoltage(fixture.ExtBattChannel, target - (2 * VoltageStep));
            fixture.pwrSupply.SetVoltage(fixture.IntBattChannel, target - (3 * VoltageStep));

            //Wait for power supplies to settle
            Thread.Sleep(PowerSettleMs);

            //Load MUXED POWER
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMuxPower, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Enable all power sources using MP
            fixture.device.SetMP("Power Source", 0);

            //Measure MUX voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MuxedPwr, Imports.Range.Range_20V);

            //Save value to result
            outputResult.SetOutcome("MUX Voltage first", meter);

            //Enable only the backup sources using MP
            fixture.device.SetMP("Power Source", 2);

            //Measure MUX voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MuxedPwr, Imports.Range.Range_20V);

            //Save value to result
            outputResult.SetOutcome("MUX Voltage second", meter);

            //Diable load on MUXED POWER
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMuxPower, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestExtDcDisable()
        {
            double meter = 0;
            double target = 19;
            double low = 0;
            double nextTarget = target - 4 * VoltageStep;
            double tolerance = 0.05;

            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSource,
                "Test the EXT DC disable functionality");
            //fixture.device.SetMP("Power Source", 0); //TBD value, disable battery
            //Thread.Sleep(100);

            //fixture.pwrSupply.DisableOutput();
            ////Disable super cap
            //fixture.device.SetMP("Surveil Test 1", 2);
            //Thread.Sleep(15000);
            //Add condition to look for mux voltage value
            outputResult.AddCondition("MUX Voltage first", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            outputResult.AddCondition("MUX Voltage second", Operation.Between, nextTarget * (1 - 2 * tolerance), nextTarget * (1 + tolerance));
            outputResult.AddCondition("MUX Voltage third", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            
            
            //Enable only the battery sources using MP
            fixture.device.SetMP("Power Source", 9);
            //fixture.device.SetMP("Power source", 9);
            fixture.device.SetMP("Power Source", 7);
            fixture.device.SetMP("Power Source", 5);
            Thread.Sleep(PowerSettleMs * 10);
            //Set Mains DC low. Power device from EXT DC with each other voltage source decreasing by one voltage step
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, low);
            fixture.pwrSupply.SetVoltage(fixture.ExtDcChannel, target);
            fixture.pwrSupply.SetVoltage(fixture.ExtBattChannel, nextTarget);
            fixture.pwrSupply.SetVoltage(fixture.IntBattChannel, target - (4 * VoltageStep));

            //Wait for power supplies to settle
            Thread.Sleep(PowerSettleMs * 10);

            //Load MUXED POWER
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMuxPower, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs * 10);

            //Enable all power sources using MP
            //fixture.device.SetMP("Power Source", 0);

            //Measure MUX voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MuxedPwr, Imports.Range.Range_20V);

            //Save value to result
            outputResult.SetOutcome("MUX Voltage first", meter);

            //Enable only the battery sources using MP
            fixture.device.SetMP("Power Source", 10);
            //fixture.device.SetMP("Power source", 9);
            //fixture.device.SetMP("Power Source", 7);
            //fixture.device.SetMP("Power Source", 5);
            Thread.Sleep(DigitalOutput.SwitchingMs * 10);
            //Measure MUX voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MuxedPwr, Imports.Range.Range_20V);

            //Save value to result
            outputResult.SetOutcome("MUX Voltage second", meter);

            //Simulate on/start button press
            //fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.ButtonStartSim, true);

            fixture.device.SetMP("Power Source", 9);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs * 10);

            //Measure MUX voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MuxedPwr, Imports.Range.Range_20V);

            //Save value to result
            outputResult.SetOutcome("MUX Voltage third", meter);

            //Diable load on MUXED POWER
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMuxPower, false);

            //Disable on/start button press
            //fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.ButtonStartSim, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs * 10);
            fixture.device.SetMP("Power Source", 0);
            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestExtBattDisable()
        {
            double meter = 0;
            double target = 19;
            double low = 8;
            double nextTarget = target - VoltageStep;
            double tolerance = 0.01;

            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSource,
                "Test the external battery disable functionality");

            //Add condition to look for mux voltage value
            outputResult.AddCondition("MUX Voltage first", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            outputResult.AddCondition("MUX Voltage second", Operation.Between, nextTarget * (1 - tolerance), nextTarget * (1 + tolerance));
            outputResult.AddCondition("MUX Voltage third", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));

            //Set Mains and EXT DC low. Power device from external battery with internal battery down one step
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, low);
            fixture.pwrSupply.SetVoltage(fixture.ExtDcChannel, low);
            fixture.pwrSupply.SetVoltage(fixture.ExtBattChannel, target);
            fixture.pwrSupply.SetVoltage(fixture.IntBattChannel, nextTarget);

            //Wait for power supplies to settle
            Thread.Sleep(PowerSettleMs * 10);

            //Load MUXED POWER
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMuxPower, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs * 10);

            //Enable all power sources using MP
            fixture.device.SetMP("Power Source", 0);

            //Measure MUX voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MuxedPwr, Imports.Range.Range_20V);

            //Save value to result
            outputResult.SetOutcome("MUX Voltage first", meter);

            //Enable only the internal battery source
            fixture.device.SetMP("Power Source", 2);

            //Measure MUX voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MuxedPwr, Imports.Range.Range_20V);

            //Save value to result
            outputResult.SetOutcome("MUX Voltage second", meter);

            //Simulate on/start button press
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.ButtonStartSim, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs * 10);

            //Measure MUX voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MuxedPwr, Imports.Range.Range_20V);

            //Save value to result
            outputResult.SetOutcome("MUX Voltage third", meter);

            //Diable load on MUXED POWER
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMuxPower, false);

            //Disable on/start button press
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.ButtonStartSim, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs * 10);

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestIntBattDisable()
        {
            double meter = 0;
            double target = 19;
            double low = 8;
            double nextTarget = low * 2;
            double tolerance = 0.01;

            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSource,
                "Test the internal battery disable functionality");

            //Add condition to look for mux voltage value
            outputResult.AddCondition("MUX Voltage first", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            outputResult.AddCondition("MUX Voltage second", Operation.Between, nextTarget * (1 - tolerance), nextTarget * (1 + tolerance));
            outputResult.AddCondition("MUX Voltage third", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));

            //Set Mains and EXT DC low. Power device from external battery with internal battery down one step
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, nextTarget);
            fixture.pwrSupply.SetVoltage(fixture.ExtDcChannel, low);
            fixture.pwrSupply.SetVoltage(fixture.ExtBattChannel, low);
            fixture.pwrSupply.SetVoltage(fixture.IntBattChannel, target);

            //Wait for power supplies to settle
            Thread.Sleep(PowerSettleMs);

            //Load MUXED POWER
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMuxPower, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Enable all power sources using MP
            fixture.device.SetMP("Power Source", 0);

            //Measure MUX voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MuxedPwr, Imports.Range.Range_20V);

            //Save value to result
            outputResult.SetOutcome("MUX Voltage first", meter);

            //Enable only mains DC
            fixture.device.SetMP("Power Source", 0);

            //Measure MUX voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MuxedPwr, Imports.Range.Range_20V);

            //Save value to result
            outputResult.SetOutcome("MUX Voltage second", meter);

            //Simulate on/start button press
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.ButtonStartSim, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure MUX voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MuxedPwr, Imports.Range.Range_20V);

            //Save value to result
            outputResult.SetOutcome("MUX Voltage third", meter);

            //Diable load on MUXED POWER
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMuxPower, false);

            //Disable on/start button press
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.ButtonStartSim, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestExtDcInputRectifier()
        {
            double target = 18;
            double meter = 0;
            double tolerance = 0.01;

            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSource,
                "Test the EXT DC input rectifier");

            //Add condition to look for voltage values
            outputResult.AddCondition("Voltage first", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            outputResult.AddCondition("Voltage second", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));

            //Power device from Mains DC, set EXT DC to be lower
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, target + (2 * VoltageStep));
            fixture.pwrSupply.SetVoltage(fixture.ExtDcChannel, target);

            //Wait for power supplies to settle
            Thread.Sleep(PowerSettleMs);

            //Switch in mains DC to EXT DC bridge
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.PassMainstoExtDC, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            double psVoltage = fixture.pwrSupply.GetVoltageMeasurement(fixture.ExtDcChannel);

            outputResult.SetOutcome("Voltage first", psVoltage);

            //Disable mains DC to EXT DC bridge
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.PassMainstoExtDC, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Disable mains DC
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, 0);

            //Wait for power supplies to settle
            Thread.Sleep(PowerSettleMs);

            //Measure MUX voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.PowerMuxExtDc, Imports.Range.Range_20V);

            //Save value to result
            outputResult.SetOutcome("Voltage second", meter);

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestMainsRectifier()
        {
            double meter = 0;
            double tolerance = 0.01;
            double target = 15;

            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSource,
                "Test the mains DC MUX recitifer");

            //Add condition to look for voltage value
            //outputResult.AddCondition("Voltage", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            //Fixture HW bug, SW workaround to lower low end tolerance
            outputResult.AddCondition("Voltage", Operation.Between, (target-0.2) * (1 - tolerance), target * (1 + tolerance));

            //Set mains DC to target voltage, set rest of power sources to default descending
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, target);
            fixture.pwrSupply.SetVoltage(fixture.ExtDcChannel, MainsVoltage);
            fixture.pwrSupply.SetVoltage(fixture.IntBattChannel, MainsVoltage - VoltageStep);
            fixture.pwrSupply.SetVoltage(fixture.ExtBattChannel, MainsVoltage - (2 * VoltageStep));

            //Wait for power supplies to settle
            Thread.Sleep(PowerSettleMs);

            //Load Mains DC
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMainsDC, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure mains DC voltage using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.PowerMuxMainsDc, Imports.Range.Range_20V);

            //Disable mains DC load
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResMainsDC, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            ResetPowerSources();

            //Save and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }

        private TestResult RunTestBackupRectifiers()
        {
            double meter = 0;
            double tolerance = 0.01;
            double target = MainsVoltage;

            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSource,
                "Test the backup power MUX recitifers");

            //Set mains DC to default voltage, set rest of power sources to descending
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, target);

            //Add conditions to look for descending voltage values
            target -= VoltageStep;
            //Fixture HW bug, SW workaround to lower low end tolerance
            outputResult.AddCondition("Voltage EXT DC", Operation.Between, (target - 0.2) * (1 - tolerance), target * (1 + tolerance));
            //outputResult.AddCondition("Voltage EXT DC", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            fixture.pwrSupply.SetVoltage(fixture.ExtDcChannel, target);

            target -= VoltageStep;
            //Fixture HW bug, SW workaround to lower low end tolerance
            outputResult.AddCondition("Voltage ext batt", Operation.Between, (target - 0.2) * (1 - tolerance), target * (1 + tolerance));
            //outputResult.AddCondition("Voltage ext batt", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            fixture.pwrSupply.SetVoltage(fixture.ExtBattChannel, target);

            target -= VoltageStep;
            //Fixture HW bug, SW workaround to lower low end tolerance
            outputResult.AddCondition("Voltage int batt", Operation.Between, (target - 0.2) * (1 - tolerance), target * (1 + tolerance));
            //outputResult.AddCondition("Voltage int batt", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            fixture.pwrSupply.SetVoltage(fixture.IntBattChannel, target);

            //Wait for power supplies to settle
            Thread.Sleep(PowerSettleMs);

            //Load backup power supplies
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResExtDC, true);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResExtBatt, true);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResIntBatt, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Enable on/start button press
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.ButtonStartSim, true);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Measure voltage using scope and save to result
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.PowerMuxExtDc, Imports.Range.Range_20V);
            outputResult.SetOutcome("Voltage EXT DC", meter);

            //Measure voltage using scope and save to result
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.PowerMuxExtBatt, Imports.Range.Range_20V);
            outputResult.SetOutcome("Voltage ext batt", meter);

            //Measure voltage using scope and save to result
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.PowerMuxIntBatt, Imports.Range.Range_20V);
            outputResult.SetOutcome("Voltage int batt", meter);

            //Disable load on backup power supplies and release on/start button
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.ButtonStartSim, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResExtDC, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResExtBatt, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResIntBatt, false);

            //Delay for relay to switch
            Thread.Sleep(DigitalOutput.SwitchingMs);

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestMainsMeasure()
        {
            double conversionFactor = 0.03423;
            double tolerance = 0.03;
            double target = 18.5;

            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSource,
                "Test the mains DC internal measurement");

            //Add condition to look for voltage value
            outputResult.AddCondition("Voltage", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));

            //Set mains DC voltage
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, target);

            //Wait for power supplies to settle
            Thread.Sleep(PowerSettleMs);

            //Get MP value
            double mpResult = (double)fixture.device.GetMpValue("Internal DC Voltage Raw Ad");

            //Convert if valid value received
            if (mpResult != -1)
            {
                mpResult *= conversionFactor;
            }

            //Save value to test result
            outputResult.SetOutcome("Voltage", mpResult);

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestExtDcMeasure()
        {
            double conversionFactor = 0.03423;
            double tolerance = 0.03;
            double target = 18.5;

            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSource,
                "Test the EXT DC internal measurement");

            //Add condition to look for voltage value
            outputResult.AddCondition("Voltage", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            //outputResult.AddCondition("Voltage disabled", Operation.LessThan, 0.5);

            //Set EXT DC voltage
            fixture.pwrSupply.SetVoltage(fixture.ExtDcChannel, target);

            //Wait for power supplies to settle
            Thread.Sleep(PowerSettleMs);

            //Get MP value
            double mpResult = (double)fixture.device.GetMpValue("External DC Voltage Raw Ad");

            //Convert if valid value received
            if (mpResult != -1)
            {
                mpResult *= conversionFactor;
            }

            //Save value to test result
            outputResult.SetOutcome("Voltage", mpResult);

            /*
            //Get surpressed MP value
            mpResult = (double)fixture.device.GetMpValue("TBD surpressed EXT DC measure");

            //Convert if valid value received
            if (mpResult != -1)
            {
                mpResult *= conversionFactor;
            }

            //Save value to test result
            outputResult.SetOutcome("Voltage disabled", mpResult);*/

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestExtBattMeasure()
        {
            double conversionFactor = 0.03423;
            double tolerance = 0.03;
            double target = 18.5;

            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSource,
                "Test the external battery internal measurement");

            //Add condition to look for voltage value
            outputResult.AddCondition("Voltage", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            //outputResult.AddCondition("Voltage disabled", Operation.LessThan, 0.5);

            //Set external battery voltage
            fixture.pwrSupply.SetVoltage(fixture.ExtBattChannel, target);

            //Wait for power supplies to settle
            Thread.Sleep(PowerSettleMs);

            //Get MP value
            double mpResult = (double)fixture.device.GetMpValue("External Battery Voltage Raw Ad");

            //Convert if valid value received
            if (mpResult != -1)
            {
                mpResult *= conversionFactor;
            }

            //Save value to test result
            outputResult.SetOutcome("Voltage", mpResult);

            /*
            //Get surpressed MP value
            mpResult = (double)fixture.device.GetMpValue("TBD surpressed ext batt measure");

            //Convert if valid value received
            if (mpResult != -1)
            {
                mpResult *= conversionFactor;
            }

            //Save value to test result
            outputResult.SetOutcome("Voltage disabled", mpResult);*/

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestIntBattMeasure()
        {
            double conversionFactor = 0.03423;
            double tolerance = 0.03;
            double target = 18.5;

            TestResult outputResult = new TestResult(resultList.Count, TestType.PowerSource,
                "Test the internal battery internal measurement");

            //Add condition to look for voltage value
            outputResult.AddCondition("Voltage", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            //outputResult.AddCondition("Voltage disabled", Operation.LessThan, 0.5);

            //Set internal battery voltage
            fixture.pwrSupply.SetVoltage(fixture.IntBattChannel, target);

            //Wait for power supplies to settle
            Thread.Sleep(PowerSettleMs);

            //Get MP value
            double mpResult = (double)fixture.device.GetMpValue("Internal Battery Voltage Raw Ad");

            //Convert if valid value received
            if (mpResult != -1)
            {
                mpResult *= conversionFactor;
            }

            //Save value to test result
            outputResult.SetOutcome("Voltage", mpResult);

            /*
            //Get surpressed MP value
            mpResult = (double)fixture.device.GetMpValue("TBD surpressed int batt measure");

            //Convert if valid value received
            if (mpResult != -1)
            {
                mpResult *= conversionFactor;
            }

            //Save value to test result
            outputResult.SetOutcome("Voltage disabled", mpResult);*/

            ResetPowerSources();

            return outputResult;
        }

    }
}
