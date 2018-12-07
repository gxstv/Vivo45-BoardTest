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
        private TestResult RunTestEthernet()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.CommunicationInterface,
                "Tests the Ethernet communication interface");

            outputResult.AddCondition("Result", Operation.Equal, 1);

            int mpValue = fixture.device.GetMpValue("TBD ethernet test");

            outputResult.SetOutcome("Result", mpValue);

            return outputResult;
        }

        private TestResult RunTestWifi()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.CommunicationInterface,
                "Tests the wifi communication interface");

            outputResult.AddCondition("Result", Operation.Equal, 1);
            fixture.device.SetMpSetting("Wifi enable",1);
            Thread.Sleep(3000);
            bool mpValue = fixture.device.WifiTest();
            if(mpValue)
                outputResult.SetOutcome("Result", 1);
            else
                outputResult.SetOutcome("Result", 0);
            fixture.device.SetMpSetting("Wifi enable", 0);
            return outputResult;
        }

        private TestResult RunTestBT()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.CommunicationInterface,
                "Tests the Bluetooth communication interface");

            outputResult.AddCondition("Result", Operation.Equal, 1);

            int mpValue = fixture.device.GetMpValue("TBD BT test");

            outputResult.SetOutcome("Result", mpValue);

            return outputResult;
        }

        private TestResult RunTestExtBattComm()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.CommunicationInterface,
                "Tests the external battery communication interface");

            //Test changed per BRE request, no more CRC verification, just read value
            outputResult.AddCondition("Result", Operation.GreaterThan, 1);

            int mpValue = fixture.device.GetMpValue("Click-in Bat Remaining Capacity mWh");

            outputResult.SetOutcome("Result", mpValue);

            return outputResult;
        }

        private TestResult RunTestIntBattComm()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.CommunicationInterface,
                "Tests the internal battery communication interface");

            //Test changed per BRE request, no more CRC verification, just read value
            outputResult.AddCondition("Result", Operation.GreaterThan, 1);

            int mpValue = fixture.device.GetMpValue("Int Bat Remaining Capacity mWh");

            outputResult.SetOutcome("Result", mpValue);

            return outputResult;
        }

        private TestResult RunTestXCO2()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.CommunicationInterface,
                "Tests the xCO2 communication interface");

            //Test changed per BRE request, no more CRC verification, just read value
            outputResult.AddCondition("Result", Operation.GreaterThan, 1);

            int mpValue = fixture.device.GetMpValue("EtCO2");

            outputResult.SetOutcome("Result", mpValue);

            return outputResult;
        }

        private TestResult RunTestSpO2()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.CommunicationInterface,
                "Tests the SpO2 communication interface");

            //Test changed per BRE request, no more CRC verification, just read value
            outputResult.AddCondition("Result", Operation.GreaterThan, 1);

            int mpValue = fixture.device.GetMpValue("SpO2");

            outputResult.SetOutcome("Result", mpValue);

            return outputResult;
        }

        private TestResult RunTestEffortBelt()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.CommunicationInterface,
                "Tests the effort belt communication interface");

            outputResult.AddCondition("Thoracic Effort", Operation.NotEqual, 0);
            int mpValue = fixture.device.GetMpValue("Thoracic Effort");
            if(mpValue== 0)
            {
                Thread.Sleep(5000);
                mpValue = fixture.device.GetMpValue("Thoracic Effort");
            }
            outputResult.SetOutcome("Thoracic Effort", mpValue);

            outputResult.AddCondition("Abdominal Effort", Operation.NotEqual, 0);

            mpValue = fixture.device.GetMpValue("Abdominal Effort");

            outputResult.SetOutcome("Abdominal Effort", mpValue);

            return outputResult;
        }

        private TestResult RunTestFPBoardComm()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.CommunicationInterface,
                "Tests the flow and pressure communication interface");

            //Test changed per BRE request, no more CRC verification, just read value
            outputResult.AddCondition("Result", Operation.Between, 0, 1);

            int mpValue = fixture.device.GetMpValue("Pressure Ad");

            outputResult.SetOutcome("Result", mpValue);

            return outputResult;
        }

        private TestResult RunTestPatientBoardComm()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.CommunicationInterface,
                "Tests the patient pressure communication interface");

            outputResult.AddCondition("Result", Operation.Between, 0, 300);

            int mpValue = fixture.device.GetMpValue("ADC Sensor Data PRESSURE-BC");

            outputResult.SetOutcome("Result", mpValue);

            return outputResult;
        }

        private TestResult RunTestNurseCall()
        {
            double target = 12;
            double tolerance = 0.05;
            double meter;
            int off = 1;
            int on = 0;
            int comoff = 11;
            int comon = 12;
            TestResult outputResult = new TestResult(resultList.Count, TestType.CommunicationInterface,
                "Tests the nurse call interface");

            outputResult.AddCondition("Comm only", Operation.Between, target*(1-tolerance), target*(1+tolerance));
            outputResult.AddCondition("Treatment only", Operation.Between, target*(1-tolerance), target*(1+tolerance));
            outputResult.AddCondition("BothOn", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));
            outputResult.AddCondition("BothOff", Operation.Between, -0.1, 0.1);

            //Use MP to enable comm only nurse call
            fixture.device.SetMP("Surveil Test 1", comon);       //Enables NC comm
            fixture.device.SetMP("Nurse Call Active", off);   //Disables NC treatment

            //Read nurse call and save result
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.NurseCall, Imports.Range.Range_20V);
            outputResult.SetOutcome("Comm only", meter);

            //Use MP to clear comm nurse call
            fixture.device.SetMP("Surveil Test 1", comoff);       //Disables NC comm
            //Use MP to enable treatment only nurse call
            fixture.device.SetMP("Nurse Call Active", on);   //Enable NC treatment

            //Read nurse call and save result
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.NurseCall, Imports.Range.Range_20V);
            outputResult.SetOutcome("Treatment only", meter);

            //Use MP to enable both nurse call
            fixture.device.SetMP("Surveil Test 1", comon);       //Enables NC comm
            fixture.device.SetMP("Nurse Call Active", on);   //Enables NC treatment

            //Read nurse call and save result
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.NurseCall, Imports.Range.Range_20V);
            outputResult.SetOutcome("BothOn", meter);

            //Use MP to enable both nurse call
            fixture.device.SetMP("Surveil Test 1", comoff);       //Disables NC comm
            fixture.device.SetMP("Nurse Call Active", off);   //Disables NC treatment

            //Read nurse call and save result
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.NurseCall, Imports.Range.Range_20V);
            outputResult.SetOutcome("BothOff", meter);

            //Use MP to restore nurse call functionality
            fixture.device.SetMP("Surveil Test 1", comoff);       //Disables NC comm
            fixture.device.SetMP("Nurse Call Active", 1);   //Disables NC treatment

            return outputResult;
        }

        private TestResult RunTestRAComm()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.CommunicationInterface,
                "Tests the remote alarm communication interface");

            outputResult.AddCondition("Remote alarm connected", Operation.Equal,1);

            //Start RA comm output and capture crossing counts
            //fixture.device.SetMP("TBD RA comm interface", 0xAAAA);   //TBD value
            //int counts = fixture.scopeInput.GetScopeCrossingCounts(fixture.MainScopeChannel, 5, 1000);
            int result = fixture.device.GetMpValue("Remote alarm connected");
            outputResult.SetOutcome("Remote alarm connected", result);

            return outputResult;
        }
    }
}
