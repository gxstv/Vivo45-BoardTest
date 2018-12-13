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
            double voltageWithoutLoad = 16.4;
            double voltageWithLoad = 13.0;
            double toleranceLoad = 0.03;
            double toleranceNoLoad = 0.08;
            double currentLow = 2.0;


            TestResult outputResult = new TestResult(resultList.Count, TestType.Battery,
                "Tests the external battery charging functionality");

            //Check for charging vs not charging voltages
            outputResult.AddCondition("Voltage charging without load", Operation.Between, voltageWithoutLoad * (1- toleranceLoad), voltageWithoutLoad * (1 + toleranceLoad));
            outputResult.AddCondition("Voltage charging with load", Operation.Between , voltageWithLoad * (1 - toleranceNoLoad), voltageWithLoad * (1 + toleranceNoLoad));
            outputResult.AddCondition("Voltage uncharged", Operation.LessThan, currentLow);
           


            fixture.pwrSupply.DisableOutput();
            Thread.Sleep(1000);
            fixture.pwrSupply.EnableOutput();
            //Delay for device restart
            Thread.Sleep(15000);

            if (!fixture.device.IsVivo45Connected())
            {
                return outputResult;
            }

            //Get MP list
            if (!fixture.device.ResetMPList())
            {
                return outputResult;
            }

            fixture.pwrSupply.DisableChannel(fixture.ExtBattChannel);
            fixture.pwrSupply.DisableChannel(fixture.ExtDcChannel);
            fixture.pwrSupply.DisableChannel(fixture.IntBattChannel);
            fixture.device.SetMP("External Bat Charger Control", 0);
            double meter0 = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.ExtBatVolt, Imports.Range.Range_20V);
            outputResult.SetOutcome("Voltage charging without load", meter0);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan2, true);
            Thread.Sleep(50);
            double meter1 = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.ExtBatVolt, Imports.Range.Range_20V);
            outputResult.SetOutcome("Voltage charging with load", meter1);
            Thread.Sleep(500);
            fixture.device.SetMP("External Bat Charger Control", 1);
            double meter2 = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.ExtBatVolt, Imports.Range.Range_20V);
            outputResult.SetOutcome("Voltage uncharged", meter2);
            //Reset default charging
            fixture.device.SetMP("External Bat Charger Control", 2);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan2, false);
            ResetPowerSources();
            fixture.pwrSupply.EnableChannel(fixture.ExtBattChannel);
            fixture.pwrSupply.EnableChannel(fixture.ExtDcChannel);
            fixture.pwrSupply.EnableChannel(fixture.IntBattChannel);

            return outputResult;
        }

        private TestResult RunTestIntBattCharging()
        {
            double voltageWithoutLoad = 16.4;
            double voltageWithLoad = 9.5;
            double toleranceLoad = 0.03;
            double toleranceNoLoad = 0.09;
            double currentLow = 2.0;


            TestResult outputResult = new TestResult(resultList.Count, TestType.Battery,
                "Tests the Internal battery charging functionality");

            //Check for charging vs not charging voltages
            outputResult.AddCondition("Voltage charging without load", Operation.Between, voltageWithoutLoad * (1 - toleranceLoad), voltageWithoutLoad * (1 + toleranceLoad));
            outputResult.AddCondition("Voltage charging with load", Operation.Between, voltageWithLoad * (1 - toleranceNoLoad), voltageWithLoad * (1 + toleranceNoLoad));
            outputResult.AddCondition("Voltage uncharged", Operation.LessThan, currentLow);



            

            fixture.pwrSupply.DisableChannel(fixture.ExtBattChannel);
            fixture.pwrSupply.DisableChannel(fixture.ExtDcChannel);
            fixture.pwrSupply.DisableChannel(fixture.IntBattChannel);
            fixture.device.SetMP("Internal Bat Charger Control", 0);
            double meter0 = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.ExtBatVolt, Imports.Range.Range_20V);
            outputResult.SetOutcome("Voltage charging without load", meter0);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan2, true);
            Thread.Sleep(50);
            double meter1 = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.ExtBatVolt, Imports.Range.Range_20V);
            outputResult.SetOutcome("Voltage charging with load", meter1);
            Thread.Sleep(500);
            fixture.device.SetMP("Internal Bat Charger Control", 1);
            double meter2 = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.ExtBatVolt, Imports.Range.Range_20V);
            outputResult.SetOutcome("Voltage uncharged", meter2);
            //Reset default charging
            fixture.device.SetMP("Internal Bat Charger Control", 2);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan2, false);
            ResetPowerSources();

            if(outputResult.getStatus() != "Pass")
            {
                fixture.pwrSupply.DisableOutput();
                Thread.Sleep(1000);
                fixture.pwrSupply.EnableOutput();
                //Delay for device restart
                Thread.Sleep(15000);

                if (!fixture.device.IsVivo45Connected())
                {
                    return outputResult;
                }

                //Get MP list
                if (!fixture.device.ResetMPList())
                {
                    return outputResult;
                }

                fixture.pwrSupply.DisableChannel(fixture.ExtBattChannel);
                fixture.pwrSupply.DisableChannel(fixture.ExtDcChannel);
                fixture.pwrSupply.DisableChannel(fixture.IntBattChannel);
                fixture.device.SetMP("Internal Bat Charger Control", 0);
                meter0 = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.InBatVolt, Imports.Range.Range_20V);
                outputResult.SetOutcome("Voltage charging without load", meter0);
                fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan3, true);
                Thread.Sleep(50);
                meter1 = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.InBatVolt, Imports.Range.Range_20V);
                outputResult.SetOutcome("Voltage charging with load", meter1);
                Thread.Sleep(500);
                fixture.device.SetMP("Internal Bat Charger Control", 1);
                meter2 = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.InBatVolt, Imports.Range.Range_20V);
                outputResult.SetOutcome("Voltage uncharged", meter2);
                //Reset default charging
                fixture.device.SetMP("Internal Bat Charger Control", 2);
                fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SSLoadChan3, false);
                ResetPowerSources();
                fixture.pwrSupply.EnableChannel(fixture.ExtBattChannel);
                fixture.pwrSupply.EnableChannel(fixture.ExtDcChannel);
                fixture.pwrSupply.EnableChannel(fixture.IntBattChannel);
            }
            


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
