﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breas.Device.Vivo45.Messenger
{
    public class MessageType
    {
        public enum EventMessage
        {
            SessionStart = 0x01,
            SessionStop = 0x02,
            Session24h = 0x03,
            SessionSync = 0x04,
            BreathData = 0x05,
            SnapshotData = 0x06,
            SettingsData = 0x07,
            SystemStateChanged = 0x08,
            PowerSourceStatus = 0x09,
            ClickInBatConnected = 0x0A,
            ClickInBatDisconnected = 0x0B,
            BatteryNearEndOfLife = 0x0C,
            BatteryStateChanged = 0x0D,
            BatteryNotCharging = 0x0E,
            BatteryTooLongChargeTime = 0x0F,
            BatteryError = 0x10,
            BatteryFccLow = 0x11,
            BatteryErrorRegChange = 0x12,
            BatteryVoltageError = 0x13,
            SensorCalibrationDone = 0x14,
            CircuitTestProgress = 0x15,
            CircuitTestResult = 0x16,
            SensorCO2Zeroed = 0x17,
            BatteryShutdown = 0x18,
            TreatmentSurvQ_Full = 0x19,
            TreatmentPwrQ_Full = 0x1A,
            TreatmentADC_Restart = 0x1B,
            MuteAlarm = 0x1C,
            ParameterChanged = 0x1D,
            ParameterGet = 0x1E,
            ProfileCopied = 0x1F,
            AllParameters = 0x20,
            GetProfileEvent = 0x21,
            ProfileNameChanged = 0x22,
            NewPatient = 0x23,
            ProfileChanged = 0x24,
            ParameterCheckOutOfLimits = 0x25,
            ParameterCheckPropertyDependencyError = 0x26,
            ParameterCheckDependencyError = 0x27,
            TrendDataEvent = 0x2C,
            UsageDataList = 0x2D,
            SensorChanged = 0x2E,
            SensorPtCO2Data = 0x2F,
            SensorCO2Data = 0x30,
            SensorSpO2Data = 0x31,
            SensorFiO2Data = 0x32,
            SensorFiO2TestProgress = 0x33,
            FiO2TestResult = 0x34,
            SensorRespEffortStatus = 0x35,
            RealtimeData = 0x36,
            RealtimeClock = 0x37,
            CalibrationProgress = 0x38,
            TempCompCalibStatus = 0x39,
            SaveMassFlowCalibData = 0x3A,
            RampStatus = 0x3B,
            PatientOpTimeChanged = 0x3C,
            RtcTimeChanged = 0x3D,
            RtcTimeRead = 0x3E,
            SoundTestResult = 0x3F,
            HumidifierStatusChanged = 0x40,
            SleepEntered = 0x41,
            PatientComplianceDataReset = 0x42,
            UsageSessionList = 0x43,
            LedTestResult = 0x44,
            SighBreath = 0x45,
            UsageEventLogRecord = 0x46,
            SessionRestart = 0x47,
            BatteryChargeCycleCount = 0x48,
            RemoteAlarmEvent = 0x49,
            StopTreatmentConfirmed = 0x4A,
            PowerFail = 0x4B
        }

        public enum Alarm
        {
            AlarmNonAlarm = 0,
            AlarmRam = 1,
            AlarmMainPressureSensor = 2,
            AlarmBackupPressureSensor = 3,
            AlarmFlowSensor = 4,
            AlarmFanShutdown = 5,
            AlarmUiFanShutdown = 6,
            AlarmSoundTestFail = 7,
            AlarmSpeakerVolumeFail = 8,
            AlarmCpuVoltFail = 9,
            AlarmAadcSpiComm = 10,
            AlarmPressSensorPg1Pg2Equal = 11,
            AlarmPressureSensorTempHigh = 12,
            AlarmPressureSensorTempLow = 13,
            AlarmBldcHighTemp = 14,
            AlarmBldcLowTemp = 15,
            AlarmFanMotorError = 16,
            AlarmFanMotorRPMError = 17,
            AlarmFanMotorBreakError = 18,
            AlarmFanMotorEarlyError = 19,
            AlarmSensorReadBackup = 20,
            AlarmParameterReadBackup = 21,
            AlarmRefTempOffset = 22,
            AlarmMotorCurrentOffset = 23,
            AlarmPressureMainCoef = 24,
            AlarmPressureBackupCoef = 25,
            AlarmPressureAmbientOffset = 26,
            AlarmMassFlowCoef = 27,
            AlarmPressureMainTempCoef = 28,
            AlarmMassFlowTempCoef = 29,
            AlarmOverPressure = 30,
            AlarmBoardTemptureHigh = 31,
            AlarmBoardTemptureLow = 32,
            AlarmCommCPUTemptureHigh = 33,
            AlarmPowerFail = 40,
            AlarmLowPressure = 41,
            AlarmHighPressure = 42,
            AlarmLowFlow = 43,
            AlarmHighFlow = 44,
            AlarmLowMV = 45,
            AlarmLowVt = 46,
            AlarmLowBreathRate = 47,
            AlarmApnea = 48,
            AlarmLowFiO2 = 49,
            AlarmLowPulseRate = 50,
            AlarmDisconnection = 51,
            AlarmLowSpO2 = 52,
            AlarmHighETCO2 = 53,
            AlarmHighInspCO2 = 54,
            AlarmDisconnectFiO2 = 55,
            AlarmDisconnectCO2 = 56,
            AlarmDisconnectSpO2 = 57,
            AlarmPerfusionArtifactSpO2 = 58,
            AlarmCO2CheckAdapter = 59,
            AlarmCO2UnspecifiedAcc = 60,
            AlarmCO2SensorError = 61,
            AlarmObstruction = 62,
            AlarmCriticalLowLastPowerSrcWarning = 63,
            AlarmCircuitTypeError = 64,
            AlarmExhValveCtrlError = 65,
            AlarmSpO2SignalLost = 66,
            AlarmHighPTCO2 = 67,
            AlarmRebreathing = 68,
            AlarmIntBatDischargingOverheat = 69,
            AlarmClikinBatDischargingOverheat = 70,
            AlarmPatientTempLow = 71,
            AlarmPatientTempHigh = 72,
            AlarmTaskDown = 73,
            AlarmPipeSendFail = 74,
            AlarmPipeReceiveFail = 75,
            AlarmMessageLost = 76,
            AlarmMediatorMainPipeSendFail = 77,
            AlarmMediatorMainPipeReceiveFail = 78,
            AlarmDatabaseIntegrityFail = 79,
            AlarmCPUComPrepareMsgFail = 80,
            Alarm_SemaphoreErr = 81,
            AlarRTCSyncFailed = 82,
            AlarmSerCommToTreat = 83,
            AlarmCoolingFan = 84,
            AlarmSerCommToUi = 85,
            AlarmRtcFail = 86,
            Alarm_Q_ToComCpuFull = 87,
            AlarmInternalTempHigh = 88,
            AlarmCPUTempHigh = 89,
            AlarmHighMV = 100,
            AlarmHighVt = 101,
            AlarmLowPEEP = 102,
            AlarmHighPEEP = 103,
            AlarmHighPulseRate = 104,
            AlarmHighFiO2 = 105,
            AlarmHighSpO2 = 106,
            AlarmLowPowerLastSrcWarn = 107,
            AlarmHighBreathRate = 108,
            AlarmRebreathingMed = 109,
            AlarmAmbientPressure = 110,
            AlarmLowEtCO2 = 111,
            AlarmAmbientTemperature = 112,
            AlarmHumidity = 113,
            AlarmLedFailure = 114,
            AlarmHeatedWireTemperatureFailure = 115,
            AlarmHumidifierTemp = 116,
            AlarmLowAlarmBat = 117,
            AlarmAlarmBatError = 118,
            AlarmHeatedWireMeasTempToleranceAlarm = 119,
            AlarmLostMainsPower = 120,
            AlarmHeatingSensorAlarm = 121,
            AlarmHumidifierTechnicalFailure = 122,
            AlarmHeatedWireTechnicalFailure = 123,
            AlarmAirChamberNotLatched = 124,
            InfoDefaultSettings = 130,
            InfoAlarmIntBatFail = 131,
            InfoAlarmClickonBatFail = 132,
            InfoRestartAfterPowerFail = 133,
            InfoSwitchToIntBat = 134,
            InfoSwitchToClickinBat = 135,
            InfoSwitchToExtDc = 136,
            InfoSwitchToMains = 137,
            InfoAlarmIntBatHalf = 138,
            InfoAlarmIntBatEmpty = 139,
            InfoAlarmRTC_Reset = 140,
            InfoDcSource = 141,
            InfoInconsistentSettingsDetected = 142,
            AlarmIntBatChargingOverheat = 143,
            AlarmClikinBatChargingOverheat = 144,
            AlarmIntBatChargingTooCold = 145,
            AlarmClikinBatChargingTooCold = 146,
            AlarmHumidifierRemoved = 147,
            InfoDisconnectFiO2 = 150,
            InfoDisconnectSpO2 = 151,
            InfoDisconnectCO2 = 152,
            InfoLogBackupFailed = 153,
            InfoDisconnectedRespEffort = 154,
            InfoSomeAlarmsDisabledOnRamp = 155,
            InfoHumidifierManualStartNeeded = 156,
            InfoCO2SensorError = 157,
            InfoLastActiveAlarm = 158,
            InfoCouldNotReadCal = 159,
            InfoCouldNotReadLog = 160,
            InfoCheckHomeAdjust = 161,
            InfoHumidifierSettingDisabled = 162,
            InfoSpO2Signal = 163,
            InfoCheckRamp = 164,
            InfoHeatedCircuitSettingDisabled = 165,
            InfoVersionMismatch = 166,
            AlarmCodeEnd = 167
        }
    }
}