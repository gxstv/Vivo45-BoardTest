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
        private TestResult RunTestExtBattTherm()
        {
            double conversionFactor = 0.0043945;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Battery,
                "Tests the external battery thermistor");

            outputResult.AddCondition("Voltage", Operation.Between, 1.4, 1.48);

            double mpValue = fixture.device.GetMpValue("External Battery Temp Raw Ad");

            if(mpValue > 0)
            {
                mpValue *= conversionFactor;
            }

            outputResult.SetOutcome("Voltage", mpValue);

            return outputResult;
        }

        private TestResult RunTestIntBattTherm()
        {
            double conversionFactor = 0.0043945;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Battery,
                "Tests the internal battery thermistor");

            outputResult.AddCondition("Voltage", Operation.Between, 1.4, 1.48);

            double mpValue = fixture.device.GetMpValue("Internal Battery Temp Raw Ad");

            if (mpValue > 0)
            {
                mpValue *= conversionFactor;
            }

            outputResult.SetOutcome("Voltage", mpValue);

            return outputResult;
        }

        private TestResult RunTestExtBattCharging()
        {
            double lowVoltage = 12;
            double tolerance = 0.02;
            double powerRead;

            double currentLow = 0.2;
            double currentHigh = 0.5;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Battery,
                "Tests the external battery charging functionality");

            //Check for charging vs not charging voltages
            outputResult.AddCondition("Current uncharged", Operation.LessThan, currentLow);
            outputResult.AddCondition("Current charging", Operation.GreaterThan, currentHigh);
            //outputResult.AddCondition("Voltage uncharged", Operation.Between, lowVoltage*(1-tolerance), lowVoltage*(1+tolerance));
            //outputResult.AddCondition("Voltage charging", Operation.GreaterThan, lowVoltage*(1+tolerance));

            //Disable charging via MP
            fixture.device.SetMP("External Bat Charger Control", 0);

            //Set low voltage on battery
            fixture.pwrSupply.SetVoltage(fixture.ExtBattChannel, lowVoltage);

            Thread.Sleep(PowerSettleMs);

            //Enable sink-source loads on external power supply
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan2, true);

            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Read battery voltage and save result
            //powerRead = fixture.pwrSupply.GetVoltageMeasurement(fixture.ExtBattChannel);
            //outputResult.SetOutcome("Voltage uncharged", powerRead);

            //Read battery current and save result
            powerRead = fixture.pwrSupply.GetCurrentMeasurement(fixture.ExtBattChannel);
            outputResult.SetOutcome("Current uncharged", powerRead);

            //Re-enable charging via MP
            fixture.device.SetMP("External Bat Charger Control", 1);

            Thread.Sleep(PowerSettleMs);

            //Read battery voltage and save result
            //powerRead = fixture.pwrSupply.GetVoltageMeasurement(fixture.ExtBattChannel);
            //outputResult.SetOutcome("Voltage charging", powerRead);
            powerRead = fixture.pwrSupply.GetCurrentMeasurement(fixture.ExtBattChannel);
            outputResult.SetOutcome("Current charging", powerRead);

            //Remove sink source loads
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan1, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan2, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan3, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan4, false);

            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Reset default charging
            fixture.device.SetMP("External Bat Charger Control", 2);

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestIntBattCharging()
        {
            double lowVoltage = 12;
            double tolerance = 0.02;
            double powerRead;
            double currentLow = 0.1;
            double currentHigh = 0.5;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Battery,
                "Tests the internal battery charging functionality");

            //Check for charging vs not charging voltages
            outputResult.AddCondition("Current uncharged", Operation.LessThan, currentLow);
            outputResult.AddCondition("Current charging", Operation.GreaterThan, currentHigh);
            //outputResult.AddCondition("Voltage uncharged", Operation.Between, lowVoltage*(1-tolerance), lowVoltage*(1+tolerance));
            //outputResult.AddCondition("Voltage charging", Operation.GreaterThan, lowVoltage*(1+tolerance));

            //Disable charging via MP
            fixture.device.SetMP("Internal Bat Charger Control", 0);

            //Set low voltage on battery
            fixture.pwrSupply.SetVoltage(fixture.IntBattChannel, lowVoltage);

            Thread.Sleep(PowerSettleMs);

            //Enable sink-source loads on external power supply
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan3, true);

            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Read battery voltage and save result
            //powerRead = fixture.pwrSupply.GetVoltageMeasurement(fixture.IntBattChannel);
            //outputResult.SetOutcome("Voltage uncharged", powerRead);

            //Read battery current and save result
            powerRead = fixture.pwrSupply.GetCurrentMeasurement(fixture.IntBattChannel);
            outputResult.SetOutcome("Current uncharged", powerRead);


            //Re-enable charging via MP
            fixture.device.SetMP("Internal Bat Charger Control", 1);

            Thread.Sleep(PowerSettleMs);

            //Read battery voltage and save result
            //powerRead = fixture.pwrSupply.GetVoltageMeasurement(fixture.IntBattChannel);
            //outputResult.SetOutcome("Voltage charging", powerRead);
            powerRead = fixture.pwrSupply.GetCurrentMeasurement(fixture.IntBattChannel);
            outputResult.SetOutcome("Current charging", powerRead);

            //Remove sink source loads
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan1, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan2, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan3, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan4, false);

            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Reset default charging
            fixture.device.SetMP("Internal Bat Charger Control", 2);

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestExtBattControl()
        {
            double meter;
            double target = 1.9;

            double tolerance = 0.10;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Battery,
                "Tests the external battery control functionality");

            fixture.device.SetMP("Power Source", 0); //TBD value, disable battery
            Thread.Sleep(100);
            outputResult.AddCondition("Power On", Operation.Between, -0.1, 0.1);
            outputResult.AddCondition("Power Off", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));

            //Disable power on signal via MP
            fixture.device.SetMP("Power Source", 2); //TBD value, disable battery
            Thread.Sleep(50);
            //Read power on signal and save result
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.PowerOnExtBatt, Imports.Range.Range_10V);
            outputResult.SetOutcome("Power Off", meter);

            //Re-enable power on signal via MP
            fixture.device.SetMP("Power Source", 1); //TBD value, enable battery

            //Read power on signal and save result
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.PowerOnExtBatt, Imports.Range.Range_10V);
            outputResult.SetOutcome("Power On", meter);

            

            
            fixture.device.SetMP("Power Source", 0); //TBD value, enable battery
            return outputResult;
        }

        private TestResult RunTestIntBattControl()
        {
            double meter;
            double target = 1.9;
            double tolerance = 0.10;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Battery,
                "Tests the internal battery control functionality");

            outputResult.AddCondition("Power On", Operation.Between, -0.1, 0.1);
            outputResult.AddCondition("Power Off", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));

            //Disable power on signal via MP
            fixture.device.SetMP("Power Source", 4); //TBD value, disable battery
            Thread.Sleep(100);
            //Read power on signal and save result
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.PowerOnIntBatt, Imports.Range.Range_10V);
            outputResult.SetOutcome("Power Off", meter);

            //Re-enable power on signal via MP
            fixture.device.SetMP("Power Source", 3); //TBD value, enable battery

            //Read power on signal and save result
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.PowerOnIntBatt, Imports.Range.Range_10V);
            outputResult.SetOutcome("Power On", meter);

            return outputResult;
        }

        private TestResult RunTestExtBattChargeRectifier()
        {
            double mainsVoltage = 19;
            double batteryVoltage = 14;
            double loweredVoltage = 12;
            double tolerance = 0.01;
            double voltageRead;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Battery,
                "Tests the external battery charging rectifier functionality");

            outputResult.AddCondition("Charging voltage", Operation.GreaterThan, batteryVoltage * (1 + tolerance));
            outputResult.AddCondition("Uncharged voltage", Operation.Between, loweredVoltage * (1 + tolerance), batteryVoltage * (1 + tolerance));
            outputResult.AddCondition("Lowered mains voltage", Operation.Between, loweredVoltage * (1 - tolerance), loweredVoltage * (1 + tolerance));

            //Set power supply
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, mainsVoltage);
            fixture.pwrSupply.SetVoltage(fixture.ExtBattChannel, batteryVoltage);

            //Enable sink source loads
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan2, true);

            Thread.Sleep(PowerSettleMs);

            //Read and save battery voltage
            voltageRead = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.SupplyExtBattCharger, Imports.Range.Range_20V);
            outputResult.SetOutcome("Charging voltage", voltageRead);

            //Lower mains DC
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, loweredVoltage);
            Thread.Sleep(PowerSettleMs);

            //Read and save battery voltage
            voltageRead = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.SupplyExtBattCharger, Imports.Range.Range_20V);
            outputResult.SetOutcome("Uncharged voltage", voltageRead);

            //Read and save mains voltage
            voltageRead = fixture.pwrSupply.GetVoltageMeasurement(fixture.MainsDcChannel);
            outputResult.SetOutcome("Lowered mains voltage", voltageRead);

            //Disable sink source loads
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan1, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan2, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan3, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan4, false);

            Thread.Sleep(PowerSettleMs);

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestIntBattChargeRectifier()
        {
            double mainsVoltage = 19;
            double batteryVoltage = 14;
            double loweredVoltage = 12;
            double tolerance = 0.05;
            double voltageRead;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Battery,
                "Tests the internal battery charging rectifier functionality");

            outputResult.AddCondition("Charging voltage", Operation.GreaterThan, batteryVoltage * (1 + tolerance));
            outputResult.AddCondition("Uncharged voltage", Operation.Between, loweredVoltage * (1 - tolerance), batteryVoltage * (1 + tolerance));
            outputResult.AddCondition("Lowered mains voltage", Operation.Between, loweredVoltage * (1 - tolerance), loweredVoltage * (1 + tolerance));

            //Set power supply
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, mainsVoltage);
            fixture.pwrSupply.SetVoltage(fixture.IntBattChannel, batteryVoltage);

            //Enable sink source loads
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan3, true);

            Thread.Sleep(PowerSettleMs);

            //Read and save battery voltage
            voltageRead = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.SupplyIntBattCharger, Imports.Range.Range_20V);
            outputResult.SetOutcome("Charging voltage", voltageRead);

            //Lower mains DC
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, loweredVoltage);
            Thread.Sleep(PowerSettleMs);

            //Read and save battery voltage
            voltageRead = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.SupplyIntBattCharger, Imports.Range.Range_20V);
            outputResult.SetOutcome("Uncharged voltage", voltageRead);

            //Read and save mains voltage
            voltageRead = fixture.pwrSupply.GetVoltageMeasurement(fixture.MainsDcChannel);
            outputResult.SetOutcome("Lowered mains voltage", voltageRead);

            //Disable sink source loads
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan1, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan2, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan3, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan4, false);

            Thread.Sleep(PowerSettleMs);

            ResetPowerSources();

            return outputResult;
        }
    }
}
