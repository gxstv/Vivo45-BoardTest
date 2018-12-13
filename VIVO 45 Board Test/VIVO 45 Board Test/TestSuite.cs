#define version1

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using Breas.Device;
using Breas.Device.Finder;
using Breas.Device.Vivo;
using Breas.Device.Vivo45;
using Breas.Device.Vivo45.Messenger;
using Breas.Device.Finder.Windows.Usb;
using Breas.Device.Monitoring.Measurements;
using System.Diagnostics;

namespace VIVO_45_Board_Test
{

#if version1
    public enum TestNames {
        //User interface tests -- always have LCD tests first out of all tests
        TestLCD = 0,
        //DebugTest,
        TestBacklight,
        TestExtDcDisable,
        TestExtBattControl,  
        TestSpeakerAudio, TestSpeakerCurrent,
        TestTransducerAudio,
        TestButtons, TestOverlayLEDs, TestMainsLED, TestButtonLED,

        //Power supply tests
        Test12VT, Test12VC, Test5VT, Test5VC, Test3V3C, Test3V3T,
        TestVSNVS, Test2V5, Test1V8, Test1V5DDR, Test1V375, Test1V2,
        TestMotorPower, TestRtcBatt,

        //Internal and reference voltage tests
        TestDDRTerm, Test4V5C, Test4V5T, TestVDDI,
        TestVddSOCCap, TestVddPU, TestVddARM, TestVddHIGH, TestVddSNVSCap, TestVddSOCIN,

        //Current limiter tests
        //TestLimiterRA, TestLimiter5VT, TestLimiter5VC, TestLimiter3V3C,

        //Power source tests
        TestMainsOverride, TestExtDcOverride, TestExtBattOverride,
        //TestIntBattSource,
        //TestMainsDisable, 
        //TestExtDcDisable, 
        //TestExtBattDisable, 
        //TestIntBattDisable,
        TestExtDcInputRectifier, TestMainsRectifier,
        //TestBackupRectifiers,
        TestMainsMeasure, TestExtDcMeasure, TestExtBattMeasure,
        //TestIntBattMeasure,

        //Heater and fan tests
        //TestHeaterPlateOutput,
        TestHeaterPlateTemp,
        //TestFan,
        TestHeatedWireControl, TestHeatedWirePower, TestHeatedWireFeedback,
        //TestHeatedWireTemp, 
        TestHeatedWirePcbPower,

        //Onboard peripheral tests
        TestDDR, TestEMMC,
        //TestSD,
        TestSyncRtc,
        TestBoardTemp,
        //TestSpareTherm,
        TestAmbientPressureSensor,
        //TestFlowPressureSensor,
        TestFiO2,
        
        //Communication interface tests
        //TestEthernet, TestWifi, TestBT,
        //TestExtBattComm, TestIntBattComm,
        TestXCO2, TestSpO2, TestEffortBelt,
        //TestFPBoardComm, 
        TestPatientBoardComm,
        //TestNurseCall, 
        //TestRAComm,

        //Battery tests
        TestExtBattTherm, TestIntBattTherm,
        TestIntBattControl,
        TestExtBattCharging,
        TestIntBattCharging,
        //TestExtBattControl,
        
        TestExtBattChargeRectifier, TestIntBattChargeRectifier,

        //Motor tests
        TestMotorOutput, TestMotorError, TestMotorDisable,
        //TestMotorOvervoltage,

        //Supercap tests
        //TestSupercapCharging, TestSupercapTherm,

        //Power control tests
        //TestRAPowerControl,
        TestRtcBackup,
        //Test5VCControl, Test5VTControl,

        //Tests requiring device resets
        Test5VTReset, Test5VCReset, //Power control
        TestRtcValue                //Board peripheral
    }
#else
    public enum TestNames {
        //User interface tests -- always have LCD tests first out of all tests
        TestLCD = 0, DebugTest, TestBacklight,
        TestSpeakerAudio, TestSpeakerCurrent, TestTransducerAudio,
        TestButtons, TestOverlayLEDs, TestMainsLED, TestButtonLED,

        //Power supply tests
        Test12VT, Test12VC, Test5VT, Test5VC, Test3V3C, Test3V3T,
        TestVSNVS, Test2V5, Test1V8, Test1V5DDR, Test1V375, Test1V2,
        TestMotorPower, TestRtcBatt,

        //Internal and reference voltage tests
        TestDDRTerm, Test4V5C, Test4V5T, TestVDDI,
        TestVddSOCCap, TestVddPU, TestVddARM, TestVddHIGH, TestVddSNVSCap, TestVddSOCIN,

        //Current limiter tests
        TestLimiterRA, TestLimiter5VT, TestLimiter5VC, TestLimiter3V3C,

        //Power source tests
        TestMainsOverride, TestExtDcOverride, TestExtBattOverride, TestIntBattSource,
        TestMainsDisable, TestExtDcDisable, TestExtBattDisable, TestIntBattDisable,
        TestExtDcInputRectifier, TestMainsRectifier, TestBackupRectifiers,
        TestMainsMeasure, TestExtDcMeasure, TestExtBattMeasure, TestIntBattMeasure,

        //Heater and fan tests
        TestHeaterPlateOutput, TestHeaterPlateTemp, TestFan,
        TestHeatedWireControl, TestHeatedWirePower, TestHeatedWireFeedback,
        TestHeatedWireTemp, TestHeatedWirePcbPower,

        //Onboard peripheral tests
        TestDDR, TestEMMC, TestSD,
        TestSyncRtc,
        TestBoardTemp, TestSpareTherm,
        TestAmbientPressureSensor, TestFlowPressureSensor,
        TestFiO2,
        
        //Communication interface tests
        TestEthernet, TestWifi, TestBT,
        TestExtBattComm, TestIntBattComm,
        TestXCO2, TestSpO2, TestEffortBelt, TestFPBoardComm, TestPatientBoardComm,
        TestNurseCall, TestRAComm,

        //Battery tests
        TestExtBattTherm, TestIntBattTherm,
        TestExtBattCharging, TestIntBattCharging,
        TestExtBattControl, TestIntBattControl,
        TestExtBattChargeRectifier, TestIntBattChargeRectifier,

        //Motor tests
        TestMotorOutput, TestMotorError, TestMotorDisable,
        TestMotorOvervoltage,

        //Supercap tests
        TestSupercapCharging, TestSupercapTherm,

        //Power control tests
        TestRAPowerControl, TestRtcBackup, Test5VCControl, Test5VTControl,

        //Tests requiring device resets
        Test5VCReset, Test5VTReset, //Power control
        TestRtcValue                //Board peripheral
    }
#endif
    partial class TestSuite
    {
        const int baseHwRev = 5;
        const double baseFwRev = 0.1;

        List<TestResult> resultList;
        TestFunction[] testFunctions;
        int allTestCount = Enum.GetNames(typeof(TestNames)).Length;
        TestNames currentTest;
        string pcbId;
        Version treatmentFwRev;
        Version commFwRev;
        Version hwRev;
        FixtureHardware fixture;
        bool useSensor;

        public TestSuite()
        {
            InitializeTestFunctions();
            ResetTestSuite("");
        }

        private void InitializeTestFunctions()
        {
            testFunctions = new TestFunction[allTestCount];

            //Populate the array of test functions with uninitialized tests
            //Can potentially delete later, but useful for debugging new tests added / tests removed
            for (int i = 0; i < allTestCount; i++)
            {
                testFunctions[i] = new TestFunction(((TestNames)i).ToString());
            }
#if version1
            //Initialize tests with specific descriptions, and if needed required revisions
            testFunctions[(int)TestNames.Test12VT] = new TestFunction(RunTest12VT, "12V treatment supply test");
            testFunctions[(int)TestNames.Test12VC] = new TestFunction(RunTest12VC, "12V communication supply test");
            testFunctions[(int)TestNames.Test5VT] = new TestFunction(RunTest5VT, "5V treatment supply test");
            testFunctions[(int)TestNames.Test5VC] = new TestFunction(RunTest5VC, "5V communication supply test");
            testFunctions[(int)TestNames.Test3V3C] = new TestFunction(RunTest3V3C, "3.3V communication supply test");

            testFunctions[(int)TestNames.Test3V3T] = new TestFunction(RunTest3V3T, "3.3V treatment supply test");
            testFunctions[(int)TestNames.Test3V3T].MinHwRev = new Version(6, 0);    //Only run 3V3T testing on rev 6 HW or higher

            testFunctions[(int)TestNames.TestVSNVS] = new TestFunction(RunTestVSNVS, "VSNVS (3V) supply test");
            testFunctions[(int)TestNames.Test2V5] = new TestFunction(RunTest2V5C, "2.5V communication supply test");
            testFunctions[(int)TestNames.Test1V8] = new TestFunction(RunTest1V8C, "1.8V communication supply test");
            testFunctions[(int)TestNames.Test1V5DDR] = new TestFunction(RunTest1V5DDR, "1.5V DDR supply test");
            testFunctions[(int)TestNames.Test1V375] = new TestFunction(RunTest1V375C, "1.375V communication supply test");
            testFunctions[(int)TestNames.Test1V2] = new TestFunction(RunTest1V2C, "1.2V communication supply test");
            testFunctions[(int)TestNames.TestMotorPower] = new TestFunction(RunTestMotorPower, "Motor power supply test");
            testFunctions[(int)TestNames.TestRtcBatt] = new TestFunction(RunTestRtcBatt, "RTC battery supply test");

            testFunctions[(int)TestNames.TestDDRTerm] = new TestFunction(RunTestTermDDR, "DDR termination regulator test");
            testFunctions[(int)TestNames.Test4V5C] = new TestFunction(RunTest4V5C, "4.5V communication reference test");
            testFunctions[(int)TestNames.Test4V5T] = new TestFunction(RunTest4V5T, "4.5V treatment reference test");
            testFunctions[(int)TestNames.TestVDDI] = new TestFunction(RunTestVDDI, "VDDI EMMC supply voltage test");
            testFunctions[(int)TestNames.TestVddSOCCap] = new TestFunction(RunTestVddSOC, "Vdd SOC internal voltage test");
            testFunctions[(int)TestNames.TestVddPU] = new TestFunction(RunTestVddPU, "Vdd PU internal voltage test");
            testFunctions[(int)TestNames.TestVddARM] = new TestFunction(RunTestVddARM, "Vdd ARM internal voltage test");
            testFunctions[(int)TestNames.TestVddHIGH] = new TestFunction(RunTestVddHIGH, "Vdd HIGH internal voltage test");
            testFunctions[(int)TestNames.TestVddSNVSCap] = new TestFunction(RunTestVddSNVS, "Vdd SNVS internal voltage test");
            testFunctions[(int)TestNames.TestVddSOCIN] = new TestFunction(RunTestVddSOCIN, "Vdd SOCIN internal voltage test");

            //testFunctions[(int)TestNames.TestLimiterRA] = new TestFunction(RunTestLimiterRA, "RA power (12VC) current limiter test");
            //testFunctions[(int)TestNames.TestLimiter5VT] = new TestFunction(RunTestLimiter5VT, "5V treatment current limiter test");
            //testFunctions[(int)TestNames.TestLimiter5VC] = new TestFunction(RunTestLimiter5VC, "5V communication current limiter test");
            //testFunctions[(int)TestNames.TestLimiter5VC].MinHwRev = new Version(6, 0);    //Only run 5VC limiter testing on rev 6 HW or higher
            //testFunctions[(int)TestNames.TestLimiter3V3C] = new TestFunction(RunTestLimiter3V3C, "3.3V communication (SD power) current limiter test");

            //testFunctions[(int)TestNames.TestRAPowerControl] = new TestFunction(RunTestRAPowerControl, "RA power (12VC) control test");
            testFunctions[(int)TestNames.TestRtcBackup] = new TestFunction(RunTestRtcBackup, "RTC backup power test");
            //testFunctions[(int)TestNames.Test5VCControl] = new TestFunction(RunTest5VCControl, "5V communication supply control test");
            //testFunctions[(int)TestNames.Test5VTControl] = new TestFunction(RunTest5VTControl, "5V treatment supply control test");
            testFunctions[(int)TestNames.Test5VCReset] = new TestFunction(RunTest5VCReset, "5V communication supply reset test");
            testFunctions[(int)TestNames.Test5VTReset] = new TestFunction(RunTest5VTReset, "5V treatment supply reset test");

            testFunctions[(int)TestNames.TestMainsOverride] = new TestFunction(RunTestMainsOverride, "Mains DC override test");
            testFunctions[(int)TestNames.TestExtDcOverride] = new TestFunction(RunTestExtDcOverride, "EXT DC override test");
            testFunctions[(int)TestNames.TestExtBattOverride] = new TestFunction(RunTestExtBattOverride, "External battery override test");
            //testFunctions[(int)TestNames.TestIntBattSource] = new TestFunction(RunTestIntBattSource, "Internal battery source test");
            //testFunctions[(int)TestNames.TestMainsDisable] = new TestFunction(RunTestMainsDisable, "Mains DC disable test");
            testFunctions[(int)TestNames.TestExtDcDisable] = new TestFunction(RunTestExtDcDisable, "Ext DC disable test");
            //testFunctions[(int)TestNames.TestExtBattDisable] = new TestFunction(RunTestExtBattDisable, "External battery disable test");
            //testFunctions[(int)TestNames.TestIntBattDisable] = new TestFunction(RunTestIntBattDisable, "Internal battery disable test");
            testFunctions[(int)TestNames.TestExtDcInputRectifier] = new TestFunction(RunTestExtDcInputRectifier, "External DC input rectifier test");
            testFunctions[(int)TestNames.TestMainsRectifier] = new TestFunction(RunTestMainsRectifier, "Mains DC MUX rectifier test");
            //testFunctions[(int)TestNames.TestBackupRectifiers] = new TestFunction(RunTestBackupRectifiers, "Backup power MUX rectifier test");
            testFunctions[(int)TestNames.TestMainsMeasure] = new TestFunction(RunTestMainsMeasure, "Mains DC internal measurement test");
            testFunctions[(int)TestNames.TestExtDcMeasure] = new TestFunction(RunTestExtDcMeasure, "EXT DC internal measurement test");
            testFunctions[(int)TestNames.TestExtBattMeasure] = new TestFunction(RunTestExtBattMeasure, "External battery internal measurement test");
            //testFunctions[(int)TestNames.TestIntBattMeasure] = new TestFunction(RunTestIntBattMeasure, "Internal battery internal measurement test");

            testFunctions[(int)TestNames.TestLCD] = new TestFunction(RunTestLCD, "LCD display test");
            testFunctions[(int)TestNames.TestBacklight] = new TestFunction(RunTestBacklight, "LCD backlight test");
            testFunctions[(int)TestNames.TestSpeakerAudio] = new TestFunction(RunTestSpeakerAudio, "Speaker audio test");
            testFunctions[(int)TestNames.TestSpeakerCurrent] = new TestFunction(RunTestSpeakerCurrent, "Speaker current test");
            testFunctions[(int)TestNames.TestTransducerAudio] = new TestFunction(RunTestTransducerAudio, "Transducer audio test");
            testFunctions[(int)TestNames.TestButtons] = new TestFunction(RunTestButtons, "Button test");
            testFunctions[(int)TestNames.TestOverlayLEDs] = new TestFunction(RunTestOverlayLEDs, "Overlay LEDs test");
            testFunctions[(int)TestNames.TestMainsLED] = new TestFunction(RunTestMainsLED, "Mains LED test");
            testFunctions[(int)TestNames.TestButtonLED] = new TestFunction(RunTestButtonLED, "Top button LED test");

           // testFunctions[(int)TestNames.TestHeaterPlateOutput] = new TestFunction(RunTestHeaterPlateOutput, "Heater plate output test");
            testFunctions[(int)TestNames.TestHeaterPlateTemp] = new TestFunction(RunTestHeaterPlateTemp, "Heater plate temperature test");
            //testFunctions[(int)TestNames.TestFan] = new TestFunction(RunTestFan, "Cooling fan test");
            testFunctions[(int)TestNames.TestHeatedWireControl] = new TestFunction(RunTestHeatedWireControl, "Heater wire control test");
            testFunctions[(int)TestNames.TestHeatedWirePower] = new TestFunction(RunTestHeatedWirePower, "Heated wire power test");
            testFunctions[(int)TestNames.TestHeatedWireFeedback] = new TestFunction(RunTestHeatedWireFeedback, "Heated wire feedback test");
            //testFunctions[(int)TestNames.TestHeatedWireTemp] = new TestFunction(RunTestHeatedWireTemp, "Heated wire temperature test");
            testFunctions[(int)TestNames.TestHeatedWirePcbPower] = new TestFunction(RunTestHeatedWirePcbPower, "Heated wire PCB power test");

            testFunctions[(int)TestNames.TestDDR] = new TestFunction(RunTestDDR, "DDR memory test");
            testFunctions[(int)TestNames.TestEMMC] = new TestFunction(RunTestEMMC, "eMMC memory test");
            //testFunctions[(int)TestNames.TestSD] = new TestFunction(RunTestSD, "SD card memory and interface test");
            testFunctions[(int)TestNames.TestRtcValue] = new TestFunction(RunTestRtcValue, "RTC value test");
            testFunctions[(int)TestNames.TestSyncRtc] = new TestFunction(RunTestSyncRtc, "RTC sync signal test");
            testFunctions[(int)TestNames.TestBoardTemp] = new TestFunction(RunTestBoardTemp, "Board temperature test");
           // testFunctions[(int)TestNames.TestSpareTherm] = new TestFunction(RunTestSpareTherm, "Spare thermistor test");
            testFunctions[(int)TestNames.TestAmbientPressureSensor] = new TestFunction(RunTestAmbientPressureSensor, "Ambient pressure sensor test");
            //testFunctions[(int)TestNames.TestFlowPressureSensor] = new TestFunction(RunTestFlowPressureSensor, "Flow pressure sensor test");
            testFunctions[(int)TestNames.TestFiO2] = new TestFunction(RunTestFiO2, "FiO2 sensor input test");

            testFunctions[(int)TestNames.TestExtBattTherm] = new TestFunction(RunTestExtBattTherm, "External battery thermistor test");
            testFunctions[(int)TestNames.TestIntBattTherm] = new TestFunction(RunTestIntBattTherm, "Internal battery thermistor test");
            testFunctions[(int)TestNames.TestExtBattCharging] = new TestFunction(RunTestExtBattCharging, "External battery charging test");
            testFunctions[(int)TestNames.TestIntBattCharging] = new TestFunction(RunTestIntBattCharging, "Internal battery charging test");
            testFunctions[(int)TestNames.TestExtBattControl] = new TestFunction(RunTestExtBattControl, "External battery control test");
            testFunctions[(int)TestNames.TestIntBattControl] = new TestFunction(RunTestIntBattControl, "Internal battery control test");
            testFunctions[(int)TestNames.TestExtBattChargeRectifier] = new TestFunction(RunTestExtBattChargeRectifier, "External battery charging rectifier test");
            testFunctions[(int)TestNames.TestIntBattChargeRectifier] = new TestFunction(RunTestIntBattChargeRectifier, "Internal battery charging rectifier test");

            //testFunctions[(int)TestNames.TestEthernet] = new TestFunction(RunTestEthernet, "Ethernet test");
            //testFunctions[(int)TestNames.TestWifi] = new TestFunction(RunTestWifi, "Wifi test");
            //testFunctions[(int)TestNames.TestBT] = new TestFunction(RunTestBT, "Bluetooth test");
            //testFunctions[(int)TestNames.TestExtBattComm] = new TestFunction(RunTestExtBattComm, "External battery communication test");
            //testFunctions[(int)TestNames.TestIntBattComm] = new TestFunction(RunTestIntBattComm, "Internal battery communication test");
            testFunctions[(int)TestNames.TestXCO2] = new TestFunction(RunTestXCO2, "xCO2 communication test");
            testFunctions[(int)TestNames.TestSpO2] = new TestFunction(RunTestSpO2, "SpO2 communication test");
            testFunctions[(int)TestNames.TestEffortBelt] = new TestFunction(RunTestEffortBelt, "Effort belt communication test");
            //testFunctions[(int)TestNames.TestFPBoardComm] = new TestFunction(RunTestFPBoardComm, "Flow and pressure commmunication test");
            testFunctions[(int)TestNames.TestPatientBoardComm] = new TestFunction(RunTestPatientBoardComm, "Patient board communication test");
            //testFunctions[(int)TestNames.TestNurseCall] = new TestFunction(RunTestNurseCall, "Nurse call interface test");
            //testFunctions[(int)TestNames.TestRAComm] = new TestFunction(RunTestRAComm, "Remote alarm interface test");

            testFunctions[(int)TestNames.TestMotorOutput] = new TestFunction(RunTestMotorOutput, "Motor normal output test");
            testFunctions[(int)TestNames.TestMotorError] = new TestFunction(RunTestMotorError, "Motor output error test");
            testFunctions[(int)TestNames.TestMotorDisable] = new TestFunction(RunTestMotorDisable, "Motor disable test");
           // testFunctions[(int)TestNames.TestMotorOvervoltage] = new TestFunction(RunTestMotorOvervoltage, "Motor overvoltage test");

            //testFunctions[(int)TestNames.TestSupercapCharging] = new TestFunction(RunTestSupercapCharging, "Supercap charging test");
            //testFunctions[(int)TestNames.TestSupercapTherm] = new TestFunction(RunTestSupercapTherm, "Supercap thermistor test");

            //testFunctions[(int)TestNames.DebugTest] = new TestFunction(RunDebugTest, "Debug Test");
#else
            //Initialize tests with specific descriptions, and if needed required revisions
            testFunctions[(int)TestNames.Test12VT] = new TestFunction(RunTest12VT, "12V treatment supply test");
            testFunctions[(int)TestNames.Test12VC] = new TestFunction(RunTest12VC, "12V communication supply test");
            testFunctions[(int)TestNames.Test5VT] = new TestFunction(RunTest5VT, "5V treatment supply test");
            testFunctions[(int)TestNames.Test5VC] = new TestFunction(RunTest5VC, "5V communication supply test");
            testFunctions[(int)TestNames.Test3V3C] = new TestFunction(RunTest3V3C, "3.3V communication supply test");

            testFunctions[(int)TestNames.Test3V3T] = new TestFunction(RunTest3V3T, "3.3V treatment supply test");
            testFunctions[(int)TestNames.Test3V3T].MinHwRev = new Version(6, 0);    //Only run 3V3T testing on rev 6 HW or higher

            testFunctions[(int)TestNames.TestVSNVS] = new TestFunction(RunTestVSNVS, "VSNVS (3V) supply test");
            testFunctions[(int)TestNames.Test2V5] = new TestFunction(RunTest2V5C, "2.5V communication supply test");
            testFunctions[(int)TestNames.Test1V8] = new TestFunction(RunTest1V8C, "1.8V communication supply test");
            testFunctions[(int)TestNames.Test1V5DDR] = new TestFunction(RunTest1V5DDR, "1.5V DDR supply test");
            testFunctions[(int)TestNames.Test1V375] = new TestFunction(RunTest1V375C, "1.375V communication supply test");
            testFunctions[(int)TestNames.Test1V2] = new TestFunction(RunTest1V2C, "1.2V communication supply test");
            testFunctions[(int)TestNames.TestMotorPower] = new TestFunction(RunTestMotorPower, "Motor power supply test");
            testFunctions[(int)TestNames.TestRtcBatt] = new TestFunction(RunTestRtcBatt, "RTC battery supply test");

            testFunctions[(int)TestNames.TestDDRTerm] = new TestFunction(RunTestTermDDR, "DDR termination regulator test");
            testFunctions[(int)TestNames.Test4V5C] = new TestFunction(RunTest4V5C, "4.5V communication reference test");
            testFunctions[(int)TestNames.Test4V5T] = new TestFunction(RunTest4V5T, "4.5V treatment reference test");
            testFunctions[(int)TestNames.TestVDDI] = new TestFunction(RunTestVDDI, "VDDI EMMC supply voltage test");
            testFunctions[(int)TestNames.TestVddSOCCap] = new TestFunction(RunTestVddSOC, "Vdd SOC internal voltage test");
            testFunctions[(int)TestNames.TestVddPU] = new TestFunction(RunTestVddPU, "Vdd PU internal voltage test");
            testFunctions[(int)TestNames.TestVddARM] = new TestFunction(RunTestVddARM, "Vdd ARM internal voltage test");
            testFunctions[(int)TestNames.TestVddHIGH] = new TestFunction(RunTestVddHIGH, "Vdd HIGH internal voltage test");
            testFunctions[(int)TestNames.TestVddSNVSCap] = new TestFunction(RunTestVddSNVS, "Vdd SNVS internal voltage test");
            testFunctions[(int)TestNames.TestVddSOCIN] = new TestFunction(RunTestVddSOCIN, "Vdd SOCIN internal voltage test");

            testFunctions[(int)TestNames.TestLimiterRA] = new TestFunction(RunTestLimiterRA, "RA power (12VC) current limiter test");
            testFunctions[(int)TestNames.TestLimiter5VT] = new TestFunction(RunTestLimiter5VT, "5V treatment current limiter test");
            testFunctions[(int)TestNames.TestLimiter5VC] = new TestFunction(RunTestLimiter5VC, "5V communication current limiter test");
            testFunctions[(int)TestNames.TestLimiter5VC].MinHwRev = new Version(6, 0);    //Only run 5VC limiter testing on rev 6 HW or higher
            testFunctions[(int)TestNames.TestLimiter3V3C] = new TestFunction(RunTestLimiter3V3C, "3.3V communication (SD power) current limiter test");

            testFunctions[(int)TestNames.TestRAPowerControl] = new TestFunction(RunTestRAPowerControl, "RA power (12VC) control test");
            testFunctions[(int)TestNames.TestRtcBackup] = new TestFunction(RunTestRtcBackup, "RTC backup power test");
            testFunctions[(int)TestNames.Test5VCControl] = new TestFunction(RunTest5VCControl, "5V communication supply control test");
            testFunctions[(int)TestNames.Test5VTControl] = new TestFunction(RunTest5VTControl, "5V treatment supply control test");
            testFunctions[(int)TestNames.Test5VCReset] = new TestFunction(RunTest5VCReset, "5V communication supply reset test");
            testFunctions[(int)TestNames.Test5VTReset] = new TestFunction(RunTest5VTReset, "5V treatment supply reset test");

            testFunctions[(int)TestNames.TestMainsOverride] = new TestFunction(RunTestMainsOverride, "Mains DC override test");
            testFunctions[(int)TestNames.TestExtDcOverride] = new TestFunction(RunTestExtDcOverride, "EXT DC override test");
            testFunctions[(int)TestNames.TestExtBattOverride] = new TestFunction(RunTestExtBattOverride, "External battery override test");
            testFunctions[(int)TestNames.TestIntBattSource] = new TestFunction(RunTestIntBattSource, "Internal battery source test");
            testFunctions[(int)TestNames.TestMainsDisable] = new TestFunction(RunTestMainsDisable, "Mains DC disable test");
            testFunctions[(int)TestNames.TestExtDcDisable] = new TestFunction(RunTestExtDcDisable, "Ext DC disable test");
            testFunctions[(int)TestNames.TestExtBattDisable] = new TestFunction(RunTestExtBattDisable, "External battery disable test");
            testFunctions[(int)TestNames.TestIntBattDisable] = new TestFunction(RunTestIntBattDisable, "Internal battery disable test");
            testFunctions[(int)TestNames.TestExtDcInputRectifier] = new TestFunction(RunTestExtDcInputRectifier, "External DC input rectifier test");
            testFunctions[(int)TestNames.TestMainsRectifier] = new TestFunction(RunTestMainsRectifier, "Mains DC MUX rectifier test");
            testFunctions[(int)TestNames.TestBackupRectifiers] = new TestFunction(RunTestBackupRectifiers, "Backup power MUX rectifier test");
            testFunctions[(int)TestNames.TestMainsMeasure] = new TestFunction(RunTestMainsMeasure, "Mains DC internal measurement test");
            testFunctions[(int)TestNames.TestExtDcMeasure] = new TestFunction(RunTestExtDcMeasure, "EXT DC internal measurement test");
            testFunctions[(int)TestNames.TestExtBattMeasure] = new TestFunction(RunTestExtBattMeasure, "External battery internal measurement test");
            testFunctions[(int)TestNames.TestIntBattMeasure] = new TestFunction(RunTestIntBattMeasure, "Internal battery internal measurement test");

            testFunctions[(int)TestNames.TestLCD] = new TestFunction(RunTestLCD, "LCD display test");
            testFunctions[(int)TestNames.TestBacklight] = new TestFunction(RunTestBacklight, "LCD backlight test");
            testFunctions[(int)TestNames.TestSpeakerAudio] = new TestFunction(RunTestSpeakerAudio, "Speaker audio test");
            testFunctions[(int)TestNames.TestSpeakerCurrent] = new TestFunction(RunTestSpeakerCurrent, "Speaker current test");
            testFunctions[(int)TestNames.TestTransducerAudio] = new TestFunction(RunTestTransducerAudio, "Transducer audio test");
            testFunctions[(int)TestNames.TestButtons] = new TestFunction(RunTestButtons, "Button test");
            testFunctions[(int)TestNames.TestOverlayLEDs] = new TestFunction(RunTestOverlayLEDs, "Overlay LEDs test");
            testFunctions[(int)TestNames.TestMainsLED] = new TestFunction(RunTestMainsLED, "Mains LED test");
            testFunctions[(int)TestNames.TestButtonLED] = new TestFunction(RunTestButtonLED, "Top button LED test");

            testFunctions[(int)TestNames.TestHeaterPlateOutput] = new TestFunction(RunTestHeaterPlateOutput, "Heater plate output test");
            testFunctions[(int)TestNames.TestHeaterPlateTemp] = new TestFunction(RunTestHeaterPlateTemp, "Heater plate temperature test");
            testFunctions[(int)TestNames.TestFan] = new TestFunction(RunTestFan, "Cooling fan test");
            testFunctions[(int)TestNames.TestHeatedWireControl] = new TestFunction(RunTestHeatedWireControl, "Heater wire control test");
            testFunctions[(int)TestNames.TestHeatedWirePower] = new TestFunction(RunTestHeatedWirePower, "Heated wire power test");
            testFunctions[(int)TestNames.TestHeatedWireFeedback] = new TestFunction(RunTestHeatedWireFeedback, "Heated wire feedback test");
            testFunctions[(int)TestNames.TestHeatedWireTemp] = new TestFunction(RunTestHeatedWireTemp, "Heated wire temperature test");
            testFunctions[(int)TestNames.TestHeatedWirePcbPower] = new TestFunction(RunTestHeatedWirePcbPower, "Heated wire PCB power test");

            testFunctions[(int)TestNames.TestDDR] = new TestFunction(RunTestDDR, "DDR memory test");
            testFunctions[(int)TestNames.TestEMMC] = new TestFunction(RunTestEMMC, "eMMC memory test");
            testFunctions[(int)TestNames.TestSD] = new TestFunction(RunTestSD, "SD card memory and interface test");
            testFunctions[(int)TestNames.TestRtcValue] = new TestFunction(RunTestRtcValue, "RTC value test");
            testFunctions[(int)TestNames.TestSyncRtc] = new TestFunction(RunTestSyncRtc, "RTC sync signal test");
            testFunctions[(int)TestNames.TestBoardTemp] = new TestFunction(RunTestBoardTemp, "Board temperature test");
            testFunctions[(int)TestNames.TestSpareTherm] = new TestFunction(RunTestSpareTherm, "Spare thermistor test");
            testFunctions[(int)TestNames.TestAmbientPressureSensor] = new TestFunction(RunTestAmbientPressureSensor, "Ambient pressure sensor test");
            testFunctions[(int)TestNames.TestFlowPressureSensor] = new TestFunction(RunTestFlowPressureSensor, "Flow pressure sensor test");
            testFunctions[(int)TestNames.TestFiO2] = new TestFunction(RunTestFiO2, "FiO2 sensor input test");

            testFunctions[(int)TestNames.TestExtBattTherm] = new TestFunction(RunTestExtBattTherm, "External battery thermistor test");
            testFunctions[(int)TestNames.TestIntBattTherm] = new TestFunction(RunTestIntBattTherm, "Internal battery thermistor test");
            testFunctions[(int)TestNames.TestExtBattCharging] = new TestFunction(RunTestExtBattCharging, "External battery charging test");
            testFunctions[(int)TestNames.TestIntBattCharging] = new TestFunction(RunTestIntBattCharging, "Internal battery charging test");
            testFunctions[(int)TestNames.TestExtBattControl] = new TestFunction(RunTestExtBattControl, "External battery control test");
            testFunctions[(int)TestNames.TestIntBattControl] = new TestFunction(RunTestIntBattControl, "Internal battery control test");
            testFunctions[(int)TestNames.TestExtBattChargeRectifier] = new TestFunction(RunTestExtBattChargeRectifier, "External battery charging rectifier test");
            testFunctions[(int)TestNames.TestIntBattChargeRectifier] = new TestFunction(RunTestIntBattChargeRectifier, "Internal battery charging rectifier test");

            testFunctions[(int)TestNames.TestEthernet] = new TestFunction(RunTestEthernet, "Ethernet test");
            testFunctions[(int)TestNames.TestWifi] = new TestFunction(RunTestWifi, "Wifi test");
            testFunctions[(int)TestNames.TestBT] = new TestFunction(RunTestBT, "Bluetooth test");
            testFunctions[(int)TestNames.TestExtBattComm] = new TestFunction(RunTestExtBattComm, "External battery communication test");
            testFunctions[(int)TestNames.TestIntBattComm] = new TestFunction(RunTestIntBattComm, "Internal battery communication test");
            testFunctions[(int)TestNames.TestXCO2] = new TestFunction(RunTestXCO2, "xCO2 communication test");
            testFunctions[(int)TestNames.TestSpO2] = new TestFunction(RunTestSpO2, "SpO2 communication test");
            testFunctions[(int)TestNames.TestEffortBelt] = new TestFunction(RunTestEffortBelt, "Effort belt communication test");
            testFunctions[(int)TestNames.TestFPBoardComm] = new TestFunction(RunTestFPBoardComm, "Flow and pressure commmunication test");
            testFunctions[(int)TestNames.TestPatientBoardComm] = new TestFunction(RunTestPatientBoardComm, "Patient board communication test");
            testFunctions[(int)TestNames.TestNurseCall] = new TestFunction(RunTestNurseCall, "Nurse call interface test");
            testFunctions[(int)TestNames.TestRAComm] = new TestFunction(RunTestRAComm, "Remote alarm interface test");

            testFunctions[(int)TestNames.TestMotorOutput] = new TestFunction(RunTestMotorOutput, "Motor normal output test");
            testFunctions[(int)TestNames.TestMotorError] = new TestFunction(RunTestMotorError, "Motor output error test");
            testFunctions[(int)TestNames.TestMotorDisable] = new TestFunction(RunTestMotorDisable, "Motor disable test");
            testFunctions[(int)TestNames.TestMotorOvervoltage] = new TestFunction(RunTestMotorOvervoltage, "Motor overvoltage test");

            testFunctions[(int)TestNames.TestSupercapCharging] = new TestFunction(RunTestSupercapCharging, "Supercap charging test");
            testFunctions[(int)TestNames.TestSupercapTherm] = new TestFunction(RunTestSupercapTherm, "Supercap thermistor test");

            testFunctions[(int)TestNames.DebugTest] = new TestFunction(RunDebugTest, "Debug Test");
#endif
        }

        private TestResult RunDebugTest()
        {
            TestResult outputResult = new TestResult(1000, TestType.NA, "Debug test");

            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SupercapDump1, true);
            fixture.outputController.EnableOutput(DigitalOutput.OutputSignals.SupercapDump2, true);

            outputResult.AddCondition("ID check 38", Operation.Between, 0.2, 0.3);
            outputResult.AddCondition("ID check 39", Operation.Between, 0.2, 0.3);

            double meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.FixtureRevision, PS4000AImports.Imports.Range.Range_10V);
            outputResult.SetOutcome("ID check 38", meter);

            meter = fixture.GetScopeMeterReading(ScopeInput.MuxedInputSignal.ControllerRevision, PS4000AImports.Imports.Range.Range_5V);
            outputResult.SetOutcome("ID check 39", meter);
            

            return outputResult;
        }

        public void ResetTestSuite(string pcbSN)
        {
            fixture = new FixtureHardware();
            resultList = new List<TestResult>();
            for (int i = 0; i < allTestCount; i++)
            {
                testFunctions[i].Enabled = false;
            }
            //mpADCrefOK = true;
            pcbId = pcbSN;
            treatmentFwRev = new Version(0, 0);
            commFwRev = new Version(0, 0);
            hwRev = new Version(0, 0);
            useSensor = false;
        }

        public void EnableAllTests()
        {
            for(int i = 0; i < allTestCount; i++)
            {
                testFunctions[i].Enabled = true;
            }
            useSensor = true;
        }

        public void EnableTest(TestNames test)
        {
            testFunctions[(int)test].Enabled = true;
            if(test == TestNames.TestAmbientPressureSensor || test == TestNames.TestBoardTemp)
            {
                useSensor = true;
            }
        }

        public bool InitializeHardware(BackgroundWorker bw, int[] pwrChannels, int[] scopeChannels, int[] outputChannels)
        {

            //Try to connect to the fixture HW
            if (!fixture.ConnectToFixtureHw(bw, pwrChannels, scopeChannels, outputChannels, useSensor))
            {
                return false;
            }
            
            //Check HW revision
            string hwRevStr = fixture.GetFixtureHwRev();
            hwRev = new Version(hwRevStr);

            //Check FW revisions
            //TODO get device info strings for FW revs
            string treatmentRevStr = fixture.device.GetMpDeviceInfo("Treatment SW version");
            string commRevStr = fixture.device.GetMpDeviceInfo("Communication SW version");
            try
            {
                treatmentFwRev = new Version(treatmentRevStr);
                commFwRev = new Version(commRevStr);
            }
            catch
            {
                //Invalid FW revision
                return true;
                MessageBox.Show("Invalid unit firmware revision");
                return false;
            }

            return true;
        }

        public void DisconnectHardware()
        {
            fixture.DisconnectFixtureHW();
        }

        public void RunTest(int idx)
        {
            //Ensure test index is valid
            if (idx >= allTestCount) return;
            
            //Ensure test is enabled
            if (!testFunctions[idx].Enabled) return;

            //Ensure test can run on current treatment FW revision
            if(treatmentFwRev < testFunctions[idx].MinTreatmentFwRev)
            {
                return;
            }
            if(testFunctions[idx].MaxTreatmentFwRev != null)
            {
                if(treatmentFwRev > testFunctions[idx].MaxTreatmentFwRev)
                {
                    return;
                }
            }

            //Ensure test can run on current comm FW revision
            if (commFwRev < testFunctions[idx].MinCommFwRev)
            {
                return;
            }
            if (testFunctions[idx].MaxCommFwRev != null)
            {
                if (commFwRev > testFunctions[idx].MaxCommFwRev)
                {
                    return;
                }
            }

            //Ensure test can run on current HW revision
            if (hwRev < testFunctions[idx].MinHwRev)
            {
                return;
            }
            if (testFunctions[idx].MaxHwRev != null)
            {
                if (hwRev > testFunctions[idx].MaxHwRev)
                {
                    return;
                }
            }

            //Run test
            currentTest = (TestNames)idx;
            
            Thread.Sleep(100);  //TODO remove, this is for development purposes only
            resultList.Add(testFunctions[idx].RunTest());
            
            try
            {
                if (!fixture.device.IsVivo45Connected())
                {

                    //MessageBox.Show("Lost connection at test: " + currentTest.ToString());
                    return;
                }
                else
                {
                    //List<Vivo45Alarm> result = fixture.device.GetActiveAlarm(Breas.Device.Monitoring.Alarms.AlarmType.High);
                    //foreach (Vivo45Alarm a in result)
                    //{
                    //    if (a.Id == 40)
                    //    {
                    //        MessageBox.Show("Power fail happened at: " + currentTest.ToString());
                    //        return;
                    //    }
                    //}
                }
            }
            catch
            {
                return;
            }
        }

        public string[] GetTestDescriptions()
        {
            string[] testListing = new string[allTestCount];

            for (int i = 0; i < allTestCount; i++)
            {
                testListing[i] = testFunctions[i].Description;
            }

            return testListing;
        }

        public string PrintTestResults()
        {
            string output = "";
            foreach (TestResult t in resultList)
            {
                output += t.PrintToString();
            }
            return output;
        }

        public string PrintTestResultsJSON()
        {
            string output = "";
            output += "{\"PCB\": \"" + pcbId + "\",\r\n";

            output += "\"Hardware version\": \"" + hwRev.ToString() + "\",\r\n";

            output += "\"Treatment firmware version\": \"" + treatmentFwRev.ToString() + "\",\r\n";

            output += "\"Comm firmware version\": \"" + commFwRev.ToString() + "\",\r\n";

            output += "\"Test suite completion\": \"" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "\",\r\n";

            output += "\"Results\":[\r\n";

            //Print all but last result
            for (int i = 0; i < resultList.Count; i++)
            {
                if(i < resultList.Count - 1)
                {
                    output += resultList[i].PrintToJSON(1, true);
                }
                else
                {
                    output += resultList[i].PrintToJSON(1, false);
                }
            }
            output += "]\r\n}";

            return output;
        }

        public string PrintTestResultsXML()
        {
            string output = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";

            output += "<TestSuite>\r\n";
            output += "\t<PCB>" + pcbId + "</PCB>\r\n";
            output += "\t<HardwareRev>" + hwRev.ToString() + "</HardwareRev>\r\n";
            output += "\t<TreatmentFirmwareRev>" + treatmentFwRev.ToString() + "</TreatmentFirmwareRev>\r\n";
            output += "\t<CommFirmwareRev>" + commFwRev.ToString() + "</CommFirmwareRev>\r\n";
            output += "\t<TestSuiteCompletion>" + DateTime.Now.ToString("yy-MM-dd HH:mm:ss") + "</TestSuiteCompletion>\r\n";

            for (int i = 0; i < resultList.Count; i++)
            {
                output += resultList[i].PrintToXML(1);
            }

            output += "</TestSuite>";

            return output;
        }

        public List<TestResult> getResultList()
        {
           return resultList;
        }
    }
}
