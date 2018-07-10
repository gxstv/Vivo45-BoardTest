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
        private TestResult RunTestSupercapCharging()
        {
            double capThreshold = 2.6;
            double capLevel = 0;

            TestResult outputResult = new TestResult(resultList.Count, TestType.Supercap,
                " Tests the supercap charging");

            outputResult.AddCondition("Cap voltage charge", Operation.GreaterThan, 0.05);

            //Drain the supercaps
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SupercapDisconnect, true);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SupercapDump1, true);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SupercapDump2, true);

            Thread.Sleep(10000);

            capLevel = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.SupercapCapStack, Imports.Range.Range_10V);
            if (capLevel < capThreshold)
            {
                return outputResult;
            }

            //Connect supercaps to charge
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SupercapDump1, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SupercapDump2, false);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SupercapDisconnect, true);

            Thread.Sleep(10000);

            //Disconnect supercaps
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SupercapDisconnect, true);

            double newCapLevel = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.SupercapCapStack, Imports.Range.Range_10V);

            outputResult.SetOutcome("Cap voltage charge", newCapLevel - capLevel);

            return outputResult;
        }

        private TestResult RunTestSupercapTherm()
        {
            TestResult outputResult = new TestResult(resultList.Count, TestType.Supercap,
                "Tests the supercap thermistor");

            double target = 25; //TBD placeholder value
            double tolerance = 0.05;
            double scalingFactor = 10;  //TBD verify scaling

            outputResult.AddCondition("Supercap temp", Operation.Between, target * (1 - tolerance), target * (1 + tolerance));

            int mpTemp = fixture.device.GetMpValue("Super capacitors temperature");

            double capTemp = (double)mpTemp * scalingFactor;

            outputResult.SetOutcome("Supercap temp", capTemp);

            return outputResult;
        }
    }
}
