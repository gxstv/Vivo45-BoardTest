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
        private TestResult RunTestHeaterPlateOutput()
        {
            double target = 18;
            double tolerance = 0.01;
            double meter;
            int feedback;
            TestResult outputResult = new TestResult(resultList.Count, TestType.Heater,
                "Tests the heater plate output");

            //Set expected conditions
            //First readings with no humidifier, next with full output, last with no output
            outputResult.AddCondition("No humidifier", Operation.Between, -0.1, 0.1);
            outputResult.AddCondition("No humidifier feedback", Operation.Equal, 0);
            outputResult.AddCondition("Output", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            outputResult.AddCondition("Output feedback", Operation.Equal, 1);
            outputResult.AddCondition("No output", Operation.Between, -0.1, 0.1);
            outputResult.AddCondition("No output feedback", Operation.Equal, 0);

            //Set known output voltage to mains DC
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, target);

            //Enable heater load
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResHeaterPlate, true);

            //Wait for relay switching time
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Enable heater output via MP
            //fixture.device.SetMP("Humidifier Level", 5);
            //fixture.device.SetMpSetting("Humidifier On/Off", 1);   //Changed per Breas request
            //Enable output
            fixture.device.SetMP("BoardTestMp", 2);
            fixture.device.SetMP("Surveil Test 1", 13);
            //Read heater output using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.HeaterPlateOutput, Imports.Range.Range_20V);
            outputResult.SetOutcome("No humidifier", meter);

            //Read the heater feedback using MP
            feedback = fixture.device.GetMpValue("Humidifier Installed");
            outputResult.SetOutcome("No humidifier feedback", feedback);

            //Enable humidifier present simulation
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.HumidifierSim, true);

            //Wait for relay switching time
            Thread.Sleep(DigitalOutput.SwitchingMs);


            //feedback = fixture.device.GetMpValue("Heater Plate Check");
            //Read heater output using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.HeaterPlateOutput, Imports.Range.Range_20V);
            outputResult.SetOutcome("Output", meter);

            //Read the heater feedback using MP
            feedback = fixture.device.GetMpValue("Humidifier Installed");
            outputResult.SetOutcome("Output feedback", feedback);

            //Disable heater output via MP
            //fixture.device.SetMP("Humidifier Level", 1);
            fixture.device.SetMP("BoardTestMp", 3);   //Changed per Breas request
            fixture.device.SetMP("Surveil Test 1", 14);
            //Read heater output using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.HeaterPlateOutput, Imports.Range.Range_20V);
            outputResult.SetOutcome("No output", meter);

            //Disable humidifier present simulation and remove heater load
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.HumidifierSim, false);

            //Read the heater feedback using MP
            feedback = fixture.device.GetMpValue("Humidifier Installed");
            outputResult.SetOutcome("No output feedback", feedback);

            
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResHeaterPlate, false);

            //Wait for relay switching time
            Thread.Sleep(DigitalOutput.SwitchingMs);

            ResetPowerSources();
            return outputResult;
        }

        private TestResult RunTestHeaterPlateTemp()
        {
            double target = 2.5;
            double tolerance = 0.05;
            double conversionFactor = 0.0043945;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Heater,
                "Tests the heater plate temperature feedback");

            //Add condition looking for simulated temperature voltage
            outputResult.AddCondition("Temp voltage", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));

            double mpValue = fixture.device.GetMpValue("Heaterplate Temp Raw Ad");

            if (mpValue > 0)
            {
                mpValue *= conversionFactor;
            }

            outputResult.SetOutcome("Temp voltage", mpValue);

            return outputResult;
        }

        private TestResult RunTestFan()
        {
            double lowSpeedMin = 10;
            double mediumSpeedMin = 750;
            double mediumSpeedMax = 2000;
            double highSpeedMin = 3500;

            int fanSpinWaitMs = 3000;
            int mpValue;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Heater,
                "Tests the cooling fan functionality and feedback");

            //Set conditions, little RPM with fan off, medium RPM with fan at medium, high RPM at high speed
            outputResult.AddCondition("Fan off", Operation.LessThan, lowSpeedMin);
            outputResult.AddCondition("Fan medium speed", Operation.Between, mediumSpeedMin, mediumSpeedMax);
            outputResult.AddCondition("Fan high speed", Operation.GreaterThan, highSpeedMin);

            //Turn off fan
            fixture.device.SetMP("Cooling Fan Control", 1);
            Thread.Sleep(fanSpinWaitMs);

            //Read and save RPM feedback
            mpValue = fixture.device.GetMpValue("Cooling Fan RPM");
            outputResult.SetOutcome("Fan off", mpValue);

            //Turn fan on medium speed
            fixture.device.SetMP("Cooling Fan Control", 30);
            Thread.Sleep(fanSpinWaitMs);

            //Read and save RPM feedback
            mpValue = fixture.device.GetMpValue("Cooling Fan RPM");
            outputResult.SetOutcome("Fan medium speed", mpValue);

            //Turn fan on high speed
            fixture.device.SetMP("Cooling Fan Control", 100);
            Thread.Sleep(fanSpinWaitMs);

            //Read and save RPM feedback
            mpValue = fixture.device.GetMpValue("Cooling Fan RPM");
            outputResult.SetOutcome("Fan high speed", mpValue);

            //Revert fan back to automatic control
            fixture.device.SetMP("Cooling Fan Control", 0);

            return outputResult;
        }

        private TestResult RunTestHeatedWireControl()
        {
            double target = 5;
            double tolerance = 0.05;
            double meter;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Heater,
                "Tests the heated wire control functionality");

            //Create on and off conditions
            outputResult.AddCondition("Control on", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            outputResult.AddCondition("Control off", Operation.Between, -0.1, 0.1 );

            //Enable heated wire output using MP
            //fixture.device.SetMP("Heated wire level", 1);
            //fixture.device.SetMpSetting("Heater Wire On/Off", 1);   //Changed per Breas request
            fixture.device.SetMP("Heated wire power", 3);
            //Read heated wire control using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.HeatedWireControl, Imports.Range.Range_10V);

            //Save result
            outputResult.SetOutcome("Control on", meter);

            //Disable heated wire output using MP
            //fixture.device.SetMP("Heated wire level", 0);
            //fixture.device.SetMpSetting("Heated Wire On/Off", 0);   //Changed per Breas request
            fixture.device.SetMP("Heated wire power", 2);
            //Read heated wire control using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.HeatedWireControl, Imports.Range.Range_10V);

            //Save result
            outputResult.SetOutcome("Control off", meter);

            return outputResult;
        }

        private TestResult RunTestHeatedWirePower()
        {
            double target = 18;
            double tolerance = 0.05;
            double meter;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Heater,
                "Tests the power to the heated wire");

            outputResult.AddCondition("Voltage", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));

            //Set mains DC to known voltage
            fixture.pwrSupply.SetVoltage(fixture.MainsDcChannel, target);

            Thread.Sleep(PowerSettleMs);

            //Enable heated wire load
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResHeatedWire, true);

            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Read power voltage and save
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.HeatedWireSupply, Imports.Range.Range_20V);
            outputResult.SetOutcome("Voltage", meter);

            //Remove heated wire load
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.LoadResHeatedWire, false);

            Thread.Sleep(DigitalOutput.SwitchingMs);

            ResetPowerSources();

            return outputResult;
        }

        private TestResult RunTestHeatedWireFeedback()
        {
            int mpValue;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Heater,
                "Tests the heated wire feedback signal");

            outputResult.AddCondition("On feedback", Operation.Equal, 0);
            outputResult.AddCondition("Off feedback", Operation.Equal, 1);

            //Enable heated wire pull down
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.HeatedWireSim, true);

            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Read and save heated wire state
            mpValue = fixture.device.GetMpValue("Heated Wire Installed");
            outputResult.SetOutcome("On feedback", mpValue);

            //Remove heated wire pull down
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.HeatedWireSim, false);

            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Read and save heated wire state
            mpValue = fixture.device.GetMpValue("Heated Wire Installed");
            outputResult.SetOutcome("Off feedback", mpValue);

            return outputResult;
        }

        private TestResult RunTestHeatedWireTemp()
        {
            double target = 2.5;
            double tolerance = 0.03;
            double conversionFactor = 0.0043945;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Heater,
                "Tests the heated wire temperature feedback");

            //Add condition looking for simulated temperature voltage
            outputResult.AddCondition("Temp voltage", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));

            //Enable PCB power via MP
            fixture.device.SetMP("Heated wire power", 1);

            //double mpValue = fixture.device.GetMpValue("Heated Wire Temp Raw Ad");
            double mpValue = fixture.device.GetMpValue("Heated wire power");
            if (mpValue > 0)
            {
                mpValue *= conversionFactor;
            }

            outputResult.SetOutcome("Temp voltage", mpValue);

            return outputResult;
        }

        private TestResult RunTestHeatedWirePcbPower()
        {
            double target = 5;
            double tolerance = 0.05;
            double meter;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Heater,
                "Tests the heated wire pcb power");

            //Add conditions looking for PCB power on and off
            outputResult.AddCondition("Off voltage", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            outputResult.AddCondition("On voltage", Operation.Between, -0.1, 0.1);

            //Disable PCB power via MP
            fixture.device.SetMP("Heated wire power", 0);

            //Read PCB power using scope and save result
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.HeatedWirePcbPower, Imports.Range.Range_10V);
            outputResult.SetOutcome("Off voltage", meter);

            //Enable PCB power via MP
            fixture.device.SetMP("Heated wire power", 1);

            //Read PCB power using scope and save result
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.HeatedWirePcbPower, Imports.Range.Range_10V);
            outputResult.SetOutcome("On voltage", meter);

            return outputResult;
        }
    }
}
