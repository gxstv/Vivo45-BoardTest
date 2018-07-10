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
        private TestResult RunTestTermDDR()
        {
            double meter = 0;
            double targetVoltage = 0.758;
            double tolerance = 0.03;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.InternalVoltage,
                "Tests the DDR termination regulator voltage");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.TermRegDDR, Imports.Range.Range_1V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }

        private TestResult RunTest4V5C()
        {
            double meter = 0;
            double targetVoltage = 4.5;
            double tolerance = 0.005;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.InternalVoltage,
                "Tests the 4.5V communication reference voltage");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply4V5C, Imports.Range.Range_5V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }

        private TestResult RunTest4V5T()
        {
            double meter = 0;
            double targetVoltage = 4.5;
            double tolerance = 0.005;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.InternalVoltage,
                "Tests the 4.5V treatment reference voltage");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.Supply4V5T, Imports.Range.Range_5V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }

        private TestResult RunTestVDDI()
        {
            double meter = 0;
            double targetVoltage = 1.25;
            double tolerance = 0.1;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.InternalVoltage,
                "Tests the VDDI EMMC supply voltage");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - 0.15), targetVoltage * (1 + 0.05));

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.SupplyEmmcVdd, Imports.Range.Range_5V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }

        private TestResult RunTestVddSOC()
        {
            double meter = 0;
            double targetVoltage = 1.15;
            double tolerance = 0.08; //Larger tolerance as SOC LDO can be either 1.1 or 1.2 V
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.InternalVoltage,
                "Tests the Vdd SOC internal voltage");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.CapVddSOC, Imports.Range.Range_2V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }

        private TestResult RunTestVddPU()
        {
            double meter = 0;
            double targetVoltage = 1.15;
            double tolerance = 0.08; //Larger tolerance as PU LDO can be either 1.1 or 1.2 V
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.InternalVoltage,
                "Tests the Vdd PU internal voltage");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.CapVddPU, Imports.Range.Range_2V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }

        private TestResult RunTestVddARM()
        {
            double meter = 0;
            double targetVoltage = 1.15; //Larger tolerance as PU LDO can be either 1.1 or 1.2 V
            double tolerance = 0.08;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.InternalVoltage,
                "Tests the Vdd ARM internal voltage");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.CapVddARM, Imports.Range.Range_2V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }

        private TestResult RunTestVddHIGH()
        {
            double meter = 0;
            double targetVoltage = 2.5;
            double tolerance = 0.05;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.InternalVoltage,
                "Tests the Vdd HIGH internal voltage");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.CapVddHIGH, Imports.Range.Range_5V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }

        private TestResult RunTestVddSNVS()
        {
            double meter = 0;
            double targetVoltage = 1.05;
            double tolerance = 0.08; //Larger tolerance as SNVS LDO can be either 1.0 or 1.1 V
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.InternalVoltage,
                "Tests the Vdd SNVS internal voltage");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.CapVddSNVS, Imports.Range.Range_5V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }

        private TestResult RunTestVddSOCIN()
        {
            double meter = 0;
            double targetVoltage = 1.375;
            double tolerance = 0.05;
            //Create test result
            TestResult outputResult = new TestResult(resultList.Count, TestType.InternalVoltage,
                "Tests the Vdd SOC INPUT internal voltage");

            //Add voltage condition to result
            outputResult.AddCondition("Voltage", Operation.Between, targetVoltage * (1 - tolerance), targetVoltage * (1 + tolerance));

            //Measure using scope
            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.CapVddIN, Imports.Range.Range_2V);

            //Set the test outcome and return result
            outputResult.SetOutcome("Voltage", meter);
            return outputResult;
        }
    }
}
