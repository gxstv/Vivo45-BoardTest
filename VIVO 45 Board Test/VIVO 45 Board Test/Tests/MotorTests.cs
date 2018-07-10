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
        int waitMotorMs = 3000;
        int motorOffPwm = 8000;

        private TestResult RunTestMotorOutput()
        {
            int motorLowPwm = 13000;
            int motorHighPwm = 24000;
            bool calib = false;

            double mCurrent;
            double mRPM;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Motor,
                "Tests normal motor output functionality");
            //Increse current limit for high speed
            fixture.pwrSupply.SetCurrent(1,5.0);

            outputResult.AddCondition("Stopped current", Operation.LessThan, 10);   //Value TBD
            outputResult.AddCondition("Stopped RPM", Operation.LessThan, 30);
            outputResult.AddCondition("Low speed current", Operation.Between, 1, 50);  //Values TBD
            outputResult.AddCondition("Low speed RPM", Operation.Between, 75, 200); //Values TBD
            outputResult.AddCondition("High speed current", Operation.GreaterThan, 200);  //Value TBD
            outputResult.AddCondition("High speed RPM", Operation.GreaterThan, 200); //Value TBD

            //If we are not in standby, cannot enter calibration, so NA test result
            int mpMode = fixture.device.GetMpValue("System State");
            if(mpMode != 2)
            {
                Thread.Sleep(5000);
                mpMode = fixture.device.GetMpValue("System State");
                if (mpMode != 2)
                {
                    return outputResult;
                }     
            }

            //Enter calibration mode
            calib = fixture.device.StartCalibration();
            Thread.Sleep(waitMotorMs);

            //Turn off motor
            fixture.device.SetMP("Motor ctrl", motorOffPwm);
            Thread.Sleep(waitMotorMs);

            //Measure current and RPM
            mCurrent = fixture.device.GetMpValue("Blower Motor Current Raw Ad");
            mRPM = fixture.device.GetMpValue("Motor RPM");

            //Save results
            outputResult.SetOutcome("Stopped current", mCurrent);
            outputResult.SetOutcome("Stopped RPM", mRPM);

            //Set motor to low speed
            fixture.device.SetMP("Motor ctrl", motorLowPwm);
            Thread.Sleep(waitMotorMs);

            //Measure current and RPM
            mCurrent = fixture.device.GetMpValue("Blower Motor Current Raw Ad");
            mRPM = fixture.device.GetMpValue("Motor RPM");

            //Save results
            outputResult.SetOutcome("Low speed current", mCurrent);
            outputResult.SetOutcome("Low speed RPM", mRPM);

            //Set motor to high speed
            fixture.device.SetMP("Motor ctrl", motorHighPwm);
            Thread.Sleep(waitMotorMs);

            //Measure current and RPM
            mCurrent = fixture.device.GetMpValue("Blower Motor Current Raw Ad");
            mRPM = fixture.device.GetMpValue("Motor RPM");

            //Save results
            outputResult.SetOutcome("High speed current", mCurrent);
            outputResult.SetOutcome("High speed RPM", mRPM);

            //Turn off motor and exit calibration mode
            fixture.device.SetMP("Motor ctrl", motorOffPwm);
            fixture.device.EndCalibration();

            fixture.pwrSupply.SetCurrent(1, 2.5);
            Thread.Sleep(waitMotorMs);
            return outputResult;
        }

        private TestResult RunTestMotorError()
        {
            int motorOnPwm = 12000;
            int motorErrorPwm = 30000;
            double mCurrent;
            double mRPM;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Motor,
                "Tests motor error functionality");

            outputResult.AddCondition("On current", Operation.Between, 1, 20);  //Values TBD
            outputResult.AddCondition("On RPM", Operation.Between, 100, 200); //Values TBD
            outputResult.AddCondition("Off current", Operation.LessThan, 10);   //Value TBD
            outputResult.AddCondition("Off RPM", Operation.LessThan, 20);

            //Increse current limit for high speed
            fixture.pwrSupply.SetCurrent(1, 5.0);
            //If we are not in standby, cannot enter calibration, so NA test result
            int mpMode = fixture.device.GetMpValue("System State");
            if (mpMode != 2)
            {
                Thread.Sleep(5000);
                mpMode = fixture.device.GetMpValue("System State");
                if (mpMode != 2)
                {
                    return outputResult;
                }
            }

            //Enter calibration mode
            fixture.device.StartCalibration();
            Thread.Sleep(waitMotorMs);

            //Turn on motor
            fixture.device.SetMP("Motor ctrl", motorOnPwm);
            Thread.Sleep(waitMotorMs);

            //Measure current and RPM
            mCurrent = fixture.device.GetMpValue("Blower Motor Current Raw Ad");
            mRPM = fixture.device.GetMpValue("Motor RPM");

            //Save results
            outputResult.SetOutcome("On current", mCurrent);
            outputResult.SetOutcome("On RPM", mRPM);

            //Create motor error
            fixture.device.SetMP("Motor ctrl", motorErrorPwm);
            Thread.Sleep(waitMotorMs);

            //Measure current and RPM
            mCurrent = fixture.device.GetMpValue("Blower Motor Current Raw Ad");
            mRPM = fixture.device.GetMpValue("Motor RPM");

            //Save results
            outputResult.SetOutcome("Off current", mCurrent);
            outputResult.SetOutcome("Off RPM", mRPM);

            //Turn off motor and exit calibration mode
            fixture.device.SetMP("Motor ctrl", motorOffPwm);
            fixture.device.EndCalibration();
            fixture.pwrSupply.SetCurrent(1, 2.5);
            Thread.Sleep(waitMotorMs);
            return outputResult;
        }

        private TestResult RunTestMotorDisable()
        {
            int motorOnPwm = 13000;
            int motorDisablePwm = 0;
            double mCurrent;
            double mRPM;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Motor,
                "Tests motor disable functionality");

            outputResult.AddCondition("On current", Operation.Between, 1, 30);  
            outputResult.AddCondition("On RPM", Operation.Between, 75, 200); 
            outputResult.AddCondition("Off current", Operation.LessThan, 10);   
            outputResult.AddCondition("Off RPM", Operation.LessThan, 20);


            //Increse current limit for high speed
            fixture.pwrSupply.SetCurrent(1, 5.0);
            //If we are not in standby, cannot enter calibration, so NA test result
            int mpMode = fixture.device.GetMpValue("System State");
            if (mpMode != 2)
            {
                Thread.Sleep(5000);
                mpMode = fixture.device.GetMpValue("System State");
                if (mpMode != 2)
                {
                    return outputResult;
                }
            }


            //Enter calibration mode
            fixture.device.StartCalibration();
            Thread.Sleep(waitMotorMs);

            //Turn on motor
            fixture.device.SetMP("Motor ctrl", motorOnPwm);
            Thread.Sleep(waitMotorMs);

            //Measure current and RPM
            mCurrent = fixture.device.GetMpValue("Blower Motor Current Raw Ad");
            mRPM = fixture.device.GetMpValue("Motor RPM");

            //Save results
            outputResult.SetOutcome("On current", mCurrent);
            outputResult.SetOutcome("On RPM", mRPM);

            //Create motor error
            fixture.device.SetMP("Motor ctrl", motorDisablePwm);
            Thread.Sleep(waitMotorMs);

            //Measure current and RPM
            mCurrent = fixture.device.GetMpValue("Blower Motor Current Raw Ad");
            mRPM = fixture.device.GetMpValue("Motor RPM");

            //Save results
            outputResult.SetOutcome("Off current", mCurrent);
            outputResult.SetOutcome("Off RPM", mRPM);

            //Turn off motor and exit calibration mode
            fixture.device.SetMP("Motor ctrl", motorOffPwm);
            fixture.device.EndCalibration();
            fixture.pwrSupply.SetCurrent(1,2.5);
            Thread.Sleep(waitMotorMs);
            return outputResult;
        }

        private TestResult RunTestMotorOvervoltage()
        {
            double mPower;
            double target = 32;
            double tolerance = 0.05;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Motor,
                "Tests the motor overvoltage circuitry");

            outputResult.AddCondition("Normal voltage", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            outputResult.AddCondition("Overvoltage", Operation.Between, -0.5, 0.5);
            //Read motor power with no overvoltage simulation
            mPower = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MotorPowerDump, Imports.Range.Range_50V);
            outputResult.SetOutcome("Normal voltage", mPower);

            //Switch in pull resistor to simulate overvoltage
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.MotorPowerDumpComp, true);
            Thread.Sleep(DigitalOutput.SwitchingMs);

            //Read motor power with overvoltage simulation
            mPower = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.MotorPowerDump, Imports.Range.Range_50V);
            outputResult.SetOutcome("Overvoltage", mPower);

            //Switch out overvoltage simulation
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.MotorPowerDumpComp, false);
            Thread.Sleep(DigitalOutput.SwitchingMs);

            return outputResult;
        }
    }
}
