using Breas.Device.Monitoring;
using Breas.Device.Monitoring.Measurements;
using System.Collections.Generic;
using System.Linq;

namespace Breas.Device.Vivo45
{

    public class Vivo45Measurements : IDeviceMeasurements
    {

        private static DeviceMeasurement triggerType = new DeviceMeasurement((ushort)TMpIds.InspirationTriggerType, Unit.None);
        private static DeviceMeasurement powerSource = new DeviceMeasurement((ushort)TMpIds.PowerSource, Unit.Percent);
        private static DeviceMeasurement systemState = new DeviceMeasurement((ushort)TMpIds.SystemState, Unit.Percent);
        private static DeviceMeasurement alarmMute = new DeviceMeasurement((ushort)TMpIds.MainBuzzerOn, Unit.None);

        private static DeviceMeasurement measurementPMean = new MonitorDeviceMeasurement((ushort)TMpIds.MonPmean, Unit.CmH2O, 0.1);
        private static DeviceMeasurement measurementPPeak = new MonitorDeviceMeasurement((ushort)TMpIds.MonPpeak, Unit.CmH2O, 0.1);//Tomas
        private static DeviceMeasurement measurementPeep = new MonitorDeviceMeasurement((ushort)TMpIds.MonPeep, Unit.CmH2O, 1.0);
        private static DeviceMeasurement measurementLeakage = new MonitorDeviceMeasurement((ushort)TMpIds.MonLeakage, Unit.LitrePerMinute, 1.0, 0xFF);
        private static DeviceMeasurement measurementMv = new MonitorDeviceMeasurement((ushort)TMpIds.MonMv, Unit.Litre, 0.1, 0xFFFF);
        private static DeviceMeasurement measurementVt = new MonitorDeviceMeasurement((ushort)TMpIds.MonVt, Unit.Millilitre, 2, 0xFFFF);
        private static DeviceMeasurement measurementFiO2 = new MonitorDeviceMeasurement((ushort)TMpIds.FiO2Perc, Unit.Percent, 1.0, 0xFFFF);
        private static DeviceMeasurement measurementIE = new MonitorDeviceMeasurement((ushort)TMpIds.MonIE, Unit.None, MeasurementFormatters.FormatIe, StorageType.Short, 1.0, 0);
        private static DeviceMeasurement measurementInspTime = new MonitorDeviceMeasurement((ushort)TMpIds.MonInspTime, Unit.Seconds, 0.1);
        private static DeviceMeasurement measurementRiseTime = new MonitorDeviceMeasurement((ushort)TMpIds.MonRiseTime, Unit.Seconds, 0.1);
        private static DeviceMeasurement measurementDV = new MonitorDeviceMeasurement((ushort)TMpIds.MonDeltaVol, Unit.Millilitre);
        private static DeviceMeasurement measurementPercentTV = new MonitorDeviceMeasurement((ushort)TMpIds.MonPercInTv, Unit.Percent, 1.0, 0xFFFF);
        private static DeviceMeasurement measurementTotalRate = new MonitorDeviceMeasurement((ushort)TMpIds.MonTotalRate, Unit.Bpm);
        private static DeviceMeasurement measurementSpontRate = new MonitorDeviceMeasurement((ushort)TMpIds.MonSpontRate, Unit.Bpm, 1.0, 0xFF);
        private static DeviceMeasurement measurementPercentSpont = new MonitorDeviceMeasurement((ushort)TMpIds.MonPercSpont, Unit.Percent, 1.0, 0xFFFF);
        private static DeviceMeasurement measurementSpO2 = new MonitorDeviceMeasurement((ushort)TMpIds.SpO2, Unit.Percent, 1.0, 0xFFFF);
        private static DeviceMeasurement measurementEtCO2 = new MonitorDeviceMeasurement((ushort)TMpIds.EtCO2Perc, Unit.Percent, 1.0, 0xFFFF);
        private static DeviceMeasurement measurementInspCO2 = new MonitorDeviceMeasurement((ushort)TMpIds.InspCO2Perc, Unit.Percent, 1.0, 0xFFFF);
        private static DeviceMeasurement measurementPulse = new MonitorDeviceMeasurement((ushort)TMpIds.Pulse, Unit.PulseBpm, 1.0, 0xFFFF);

        private static DeviceMeasurement measurementFiO2OnOff = new MonitorDeviceMeasurement((ushort)TMpIds.FiO2SensorExists, Unit.None, 1.0, 0xFFFF);
        private static DeviceMeasurement measurementEtCO2OnOff = new MonitorDeviceMeasurement((ushort)TMpIds.XCO2SensorExists, Unit.None, 1.0, 0xFFFF);
        private static DeviceMeasurement measurementSpO2OnOff = new MonitorDeviceMeasurement((ushort)TMpIds.SPO2SensorExists, Unit.None, 1.0, 0xFFFF);

        public static V45Setting settingProfile = new V45Setting(TSettingsId.SelectedProfile, "Profile", Unit.None);
        public static V45Setting settingCircuit = new V45Setting(TSettingsId.CurrentCircuit, "Circuit", Unit.None);
        public static V45Setting settingBreathMode = new V45Setting(TSettingsId.BreathMode, "BREATH_MODE", Unit.None);
        public static V45Setting settingVentMode = new V45Setting(TSettingsId.VentMode, "VENTILATORMODE", Unit.None);
        private static V45MMVSetting settingCpap = new V45MMVSetting(TSettingsId.CPAP, "STREAM_USAGE_SETTING_CPAP", Unit.CmH2O);
        private static V45Setting settingEpap = new V45Setting(TSettingsId.PEEP_EPAP, "EPAP", Unit.CmH2O);
        private static V45Setting settingIpap = new V45Setting(TSettingsId.IPAP, "IPAP", Unit.CmH2O);
        private static V45Setting settingRamp = new V45Setting(TSettingsId.Ramp, "RAMP", Unit.None);
        private static V45Setting settingRampStartPressure = new V45Setting(TSettingsId.RampStartPressure, "Ramp Start Pressure", Unit.CmH2O, MeasurementFormatters.FormatOffableInteger);
        private static V45MMVSetting settingBackupRate = new V45MMVSetting(TSettingsId.BackupRate, "STREAM_USAGE_SETTING_BACKUP_RATE", Unit.Bpm);
        private static V45MMVSetting settingInspTime = new V45MMVSetting(TSettingsId.InspiratoryTime, "Insp. Time", Unit.Seconds);
        private static V45Setting settingMinInspTime = new V45Setting(TSettingsId.InspiratoryTimeMin, "Min Insp. Time", Unit.Seconds);
        private static V45Setting settingMaxInspTime = new V45Setting(TSettingsId.InspiratoryTimeMax, "Max Insp. Time", Unit.Seconds);
        private static V45MMVSetting settingInspTrigger = new V45MMVSetting(TSettingsId.InspiratoryTrigger, "Insp. Trigger", Unit.None, MeasurementFormatters.FormatOffableInteger);
        private static V45MMVSetting settingExpirTrigger = new V45MMVSetting(TSettingsId.ExpiratoryTrigger, "STREAM_BREATH_EXP_TRIGGER", Unit.None);

        private static V45Setting settingApneaAlarm = new V45Setting(TSettingsId.ApneaAlarm, "STREAM_USAGE_SETTING_ALARM_APNEA", Unit.Seconds, MeasurementFormatters.FormatOffableInteger);
        private static V45Setting settingLowPressureAlarm = new V45Setting(TSettingsId.PressLowAlarm, "USAGE_SETTING_ALARM_PRESS_LOW_V7", Unit.CmH2O);
        private static V45Setting settingHighPressureAlarm = new V45Setting(TSettingsId.PressHighAlarm, "USAGE_SETTING_ALARM_PRESS_HIGH_V7", Unit.CmH2O);
        private static V45Setting settingLowVtiAlarm = new V45Setting(TSettingsId.TidalVolLowAlarm, "STREAM_USAGE_SETTING_ALARM_TIDAL_VOL_LOW", Unit.Millilitre);
        private static V45Setting settingHighVtiAlarm = new V45Setting(TSettingsId.TidalVolHighAlarm, "STREAM_USAGE_SETTING_ALARM_TIDAL_VOL_HIGH", Unit.Millilitre);
        private static V45Setting settingLowMinuteVolAlarm = new V45Setting(TSettingsId.MinuteVolLowAlarm, "STREAM_USAGE_SETTING_ALARM_MINUTE_VOL_LOW_", Unit.Litre, MeasurementFormatters.FormatOffableInteger);
        private static V45Setting settingHighMinuteVolAlarm = new V45Setting(TSettingsId.MinuteVolHighAlarm, "STREAM_USAGE_SETTING_ALARM_MINUTE_VOL_HIGH_", Unit.Litre, MeasurementFormatters.FormatOffableInteger);
        private static V45Setting settingLowBreathRateAlarm = new V45Setting(TSettingsId.BreathRateLowAlarm, "USAGE_SETTING_ALARM_BREATH_RATE_LOW_V7", Unit.Bpm, MeasurementFormatters.FormatOffableInteger);
        private static V45Setting settingHighBreathRateAlarm = new V45Setting(TSettingsId.BreathRateHighAlarm, "USAGE_SETTING_ALARM_BREATH_RATE_HIGH_V7", Unit.Bpm, MeasurementFormatters.FormatOffableInteger);
        private static V45Setting settingLowPulseRateAlarm = new V45Setting(TSettingsId.PulseRateLowAlarm, "STREAM_USAGE_SETTING_ALARM_PULSE_RATE_LOW", Unit.None, MeasurementFormatters.FormatOffableInteger);
        private static V45Setting settingHighPulseRateAlarm = new V45Setting(TSettingsId.PulseRateHighAlarm, "STREAM_USAGE_SETTING_ALARM_PULSE_RATE_HIGH", Unit.None, MeasurementFormatters.FormatOffableInteger);
        private static V45Setting settingDisconnectionAlarm = new V45Setting(TSettingsId.DisconnectionAlarm, "USAGE_SETTING_ALARM_LEAKAGE_HIGH_V7", Unit.None, MeasurementFormatters.FormatOnOff);
        private static V45Setting settingRebreathingAlarm = new V45Setting(TSettingsId.RebreathingAlarm, "USAGE_SETTING_ALARM_LEAKAGE_LOW_V7", Unit.None, MeasurementFormatters.FormatOnOff);
        private static V45Setting settingLowFiO2Alarm = new V45Setting(TSettingsId.FIO2LowAlarm, "STREAM_USAGE_SETTING_ALARM_FIO2_LOW", Unit.Percent, MeasurementFormatters.FormatOffableInteger);
        private static V45Setting settingHighFiO2Alarm = new V45Setting(TSettingsId.FIO2HighAlarm, "STREAM_USAGE_SETTING_ALARM_FIO2_HIGH", Unit.Percent, MeasurementFormatters.FormatOffableInteger);
        private static V45Setting settingLowSPO2Alarm = new V45Setting(TSettingsId.SPO2LowAlarm, "STREAM_USAGE_SETTING_ALARM_SPO2_LOW", Unit.Percent);
        private static V45Setting settingHighSPO2Alarm = new V45Setting(TSettingsId.SPO2HighAlarm, "STREAM_USAGE_SETTING_ALARM_SPO2_HIGH", Unit.Percent, MeasurementFormatters.FormatOffableInteger);

        private static V45Setting settingEtCO2Unit = new V45Setting(TSettingsId.EtCO2Unit, "USAGE_SETTING_ETCO2_UNIT_V7", Unit.None);
        private static V45Setting settingInCO2Unit = new V45Setting(TSettingsId.PressureUnit, "Pressure unit", Unit.None);

        private static V45Setting settingLowEtCO2KpaAlarm = new V45Setting(TSettingsId.EtCO2LowAlarmkPa, "STREAM_USAGE_SETTING_ALARM_END_TIDAL_CO2_LOW_KPA_V7", Unit.Kpa, MeasurementFormatters.FormatOffableInteger);
        private static V45Setting settingHighEtCO2KpaAlarm = new V45Setting(TSettingsId.EtCO2HighAlarmkPa, "STREAM_USAGE_SETTING_ALARM_END_TIDAL_CO2_HIGH_KPA_V7", Unit.Kpa, MeasurementFormatters.FormatOffableInteger);
        private static V45Setting settingLowEtCO2mmHgAlarm = new V45Setting(TSettingsId.EtCO2LowAlarmmmHg, "STREAM_USAGE_SETTING_ALARM_END_TIDAL_CO2_LOW_MMHG_V7", Unit.MmHg, MeasurementFormatters.FormatOffableInteger);
        private static V45Setting settingHighEtCO2mmHgAlarm = new V45Setting(TSettingsId.EtCO2HighAlarmmmHg, "STREAM_USAGE_SETTING_ALARM_END_TIDAL_CO2_HIGH_MMHG_V7", Unit.MmHg, MeasurementFormatters.FormatOffableInteger);
        private static V45Setting settingLowEtCO2PercentAlarm = new V45Setting(TSettingsId.EtCO2LowAlarmPercent, "STREAM_USAGE_SETTING_ALARM_END_TIDAL_CO2_LOW_PERC_V7", Unit.MmHg, MeasurementFormatters.FormatOffableInteger);
        private static V45Setting settingHighEtCO2PercentAlarm = new V45Setting(TSettingsId.EtCO2HighAlarmPercent, "STREAM_USAGE_SETTING_ALARM_END_TIDAL_CO2_HIGH_PERC_V7", Unit.MmHg, MeasurementFormatters.FormatOffableInteger);

        private static V45Setting settingHighInspCO2KpaAlarm = new V45Setting(TSettingsId.InCO2HighAlarmkPa, "STREAM_USAGE_SETTING_ALARM_INSPIRED_CO2_HIGH_KPA_V7", Unit.Kpa, MeasurementFormatters.FormatOffableInteger);
        private static V45Setting settingHighInspCO2mmHgAlarm = new V45Setting(TSettingsId.InCO2HighAlarmmmHg, "STREAM_USAGE_SETTING_ALARM_INSPIRED_CO2_HIGH_MMHG_V7", Unit.MmHg, MeasurementFormatters.FormatOffableInteger);
        private static V45Setting settingHighInspCO2PercentAlarm = new V45Setting(TSettingsId.InCO2HighAlarmPercent, "STREAM_USAGE_SETTING_ALARM_INSPIRED_CO2_HIGH_PERC_V7", Unit.Percent, MeasurementFormatters.FormatOffableInteger);

        private static DeviceMeasurement settingEtCO2LowAlarm = new MultiUnitDeviceMeasurement(settingEtCO2Unit,
            new Dictionary<int, DeviceMeasurement>()
            {
                //{ -1, settingEtCO2HighAlarm },
                { 0, settingLowEtCO2mmHgAlarm },
                { 1, settingLowEtCO2KpaAlarm },
                { 2, settingLowEtCO2PercentAlarm }
            });

        private static DeviceMeasurement settingEtCO2HighAlarm = new MultiUnitDeviceMeasurement(settingEtCO2Unit,
            new Dictionary<int, DeviceMeasurement>()
            {
                //{ -1, settingEtCO2HighAlarm },
                { 0, settingHighEtCO2mmHgAlarm },
                { 1, settingHighEtCO2KpaAlarm },
                { 2, settingHighEtCO2PercentAlarm }
            });

        private static DeviceMeasurement settingInCO2HighAlarm = new MultiUnitDeviceMeasurement(settingInCO2Unit,
            new Dictionary<int, DeviceMeasurement>()
            {
                //{ -1, settingEtCO2HighAlarm },
                { 0, settingHighInspCO2mmHgAlarm },
                { 1, settingHighInspCO2KpaAlarm },
                { 2, settingHighInspCO2PercentAlarm }
            });

        //private static V45SettingMeasurement settingPeep = new V45SettingMeasurement(TSettingsId.Peep, Unit.None); Tidal Volume Pressure
        //private static V45SettingMeasurement settingBreathRate = new V45SettingMeasurement(TSettingsId.Rate, Unit.Bpm); FIND OUT WHAT THIS IS

        public static DeviceMeasurement[] pcvSettings = new DeviceMeasurement[]
        {
            settingEpap,
            settingIpap,
            settingRamp,
            settingRampStartPressure,
            settingBackupRate,
            //settingBreathRate,
            settingInspTime,
            //settingMinInspTime,
            //settingMaxInspTime,
            settingInspTrigger,
            //settingExpirTrigger
        };

        public static DeviceMeasurement[] alarmSettings = new DeviceMeasurement[]
        {
            settingLowPressureAlarm,
            settingHighPressureAlarm,
            settingLowVtiAlarm,
            settingHighVtiAlarm,
            settingLowMinuteVolAlarm,
            settingHighMinuteVolAlarm,
            settingLowBreathRateAlarm,
            settingHighBreathRateAlarm,
            settingLowPulseRateAlarm,
            settingHighPulseRateAlarm,
            settingDisconnectionAlarm,
            settingRebreathingAlarm,
            settingLowFiO2Alarm,
            settingHighFiO2Alarm,
            settingLowSPO2Alarm,
            settingHighSPO2Alarm,
            settingEtCO2LowAlarm,
            settingEtCO2HighAlarm,
            settingInCO2HighAlarm
        };

        private static Dictionary<OpMode, DeviceMeasurement[]> usageSettingsMap = new Dictionary<OpMode, DeviceMeasurement[]>();
        private static Dictionary<OpMode, DeviceMeasurement[]> alarmSettingsMap = new Dictionary<OpMode, DeviceMeasurement[]>();

        static Vivo45Measurements()
        {
            usageSettingsMap[OpMode.PCV] = pcvSettings;
            alarmSettingsMap[OpMode.PCV] = alarmSettings;
        }

        public DeviceMeasurement VentMode
        {
            get { return settingVentMode; }
        }

        public DeviceMeasurement BreathMode
        {
            get { return settingBreathMode; }
        }

        public virtual DeviceMeasurement TriggerType
        {
            get
            {
                return triggerType;
            }
        }

        public virtual DeviceMeasurement PowerSource
        {
            get
            {
                return powerSource;
            }
        }

        public virtual DeviceMeasurement AlarmMute
        {
            get
            {
                return alarmMute;
            }
        }

        public virtual DeviceMeasurement MeasurementTotalRate { get { return measurementTotalRate; } }
        public virtual DeviceMeasurement MeasurementIE { get { return measurementIE; } }
        public virtual DeviceMeasurement MeasurementPPeak { get { return measurementPPeak; } }
        public virtual DeviceMeasurement MeasurementVt { get { return measurementVt; } }
        public virtual DeviceMeasurement MeasurementPeep { get { return measurementPeep; } }
        public virtual DeviceMeasurement MeasurementInspTime { get { return measurementInspTime; } }
        public virtual DeviceMeasurement MeasurementSpontRate { get { return measurementSpontRate; } }
        public virtual DeviceMeasurement MeasurementMv { get { return measurementMv; } }
        public virtual DeviceMeasurement MeasurementPercentSpont { get { return measurementPercentSpont; } }
        public virtual DeviceMeasurement MeasurementRiseTime { get { return measurementRiseTime; } }
        public virtual DeviceMeasurement MeasurementPMean { get { return measurementPMean; } }
        public virtual DeviceMeasurement MeasurementFiO2 { get { return measurementFiO2; } }
        public virtual DeviceMeasurement MeasurementSpO2 { get { return measurementSpO2; } }
        public virtual DeviceMeasurement MeasurementEtCO2 { get { return measurementEtCO2; } }
        public virtual DeviceMeasurement MeasurementLeakage { get { return measurementLeakage; } }
        public virtual DeviceMeasurement MeasurementPercentTV { get { return measurementPercentTV; } }
        public virtual DeviceMeasurement MeasurementPulse { get { return measurementPulse; } }
        public virtual DeviceMeasurement MeasurementInspCO2 { get { return measurementInspCO2; } }

        public virtual DeviceMeasurement MeasurementFiO2OnOff { get { return measurementFiO2OnOff; } }
        public virtual DeviceMeasurement MeasurementSpO2OnOff { get { return measurementSpO2OnOff; } }
        public virtual DeviceMeasurement MeasurementEtCO2OnOff { get { return measurementEtCO2OnOff; } }

        public virtual DeviceMeasurement CurrentCircuit { get { return settingCircuit; } }

        public virtual DeviceMeasurement Profile { get { return settingProfile; } }

        public virtual List<DeviceMeasurement> GetUsageSettingMeasurements(OpMode opmode)
        {
            return usageSettingsMap[opmode].ToList();
        }

        public virtual List<DeviceMeasurement> GetAlarmSettingMeasurements(OpMode opmode)
        {
            return alarmSettingsMap[opmode].ToList();
        }
    }

    public enum TSettingsId : ushort
    {
        IPAP = 0,
        PEEP_EPAP = 1,
        InspiratoryTrigger = 2,
        ExpiratoryTrigger = 3,
        RisetimeVolume = 4,
        RisetimePressure = 5,
        FallTime = 6,
        TargetVolume = 7,
        MaxInspiratoryPressure = 8,
        MinInspiratoryPressure = 9,
        BackupRate = 12,
        InspiratoryTimeMin = 11,
        InspiratoryTimeMax = 10,
        InspiratoryTime = 13,
        SetVolume = 14,
        CPAP = 15,
        Ramp = 16,
        RampStartPressure = 17,
        Humidifier = 18,
        PressHighAlarm = 19,
        PressLowAlarm = 20,
        PEEPHighAlarm = 21,
        PEEPLowAlarm = 22,
        TidalVolHighAlarm = 23,
        TidalVolLowAlarm = 24,
        MinuteVolHighAlarm = 25,
        MinuteVolLowAlarm = 26,
        BreathRateHighAlarm = 27,
        BreathRateLowAlarm = 28,
        FIO2HighAlarm = 29,
        FIO2LowAlarm = 30,
        EtCO2HighAlarmPercent = 31,
        EtCO2HighAlarmmmHg = 32,
        EtCO2HighAlarmkPa = 33,
        EtCO2LowAlarmPercent = 34,
        EtCO2LowAlarmmmHg = 35,
        EtCO2LowAlarmkPa = 36,
        InCO2HighAlarmPercent = 37,
        InCO2HighAlarmmmHg = 38,
        InCO2HighAlarmkPa = 39,
        SPO2HighAlarm = 40,
        SPO2LowAlarm = 41,
        PulseRateHighAlarm = 42,
        PulseRateLowAlarm = 43,
        DisconnectionAlarm = 44,
        RebreathingAlarm = 45,
        ApneaAlarm = 46,
        ProfileName = 47,
        VentMode = 48,
        BreathMode = 49,
        TimeFormat = 50,
        DateFormat = 51,
        Language = 52,
        DisplayLight = 53,
        LightIntensity = 54,
        KeypadLock = 55,
        CircuitRecognition = 56,
        CurrentCircuit = 57,
        TempCircuit = 58,
        CircuitResistance = 59,
        CircuitCompliance = 60,
        ExhValveFactor = 61,
        CurrentVentInsert = 62,
        FiO2Factor = 63,
        AlarmSoundLevel = 64,
        PatientMode = 65,
        DeviceMode = 66,
        Profile1Activated = 67,
        Profile2Activated = 68,
        Profile3Activated = 69,
        SelectedProfile = 70,
        AdjustmentInHome = 71,
        PressureUnit = 72,
        EtCO2Unit = 73,
        CurvePressureScale = 74,
        CurveFlowScale = 75,
        CurveVolumeScle = 76,
        CurveCO2Scale = 77,
        CurveTimeScale = 78,
        TrendPressScale = 79,
        TrendRateScale = 80,
        TrendLeakScale = 81,
        TrendETCO2Scale = 82,
        TrendVolScale = 83,
        TrendTimeBase = 84
    }

    public enum TMpIds : ushort
    {
        MeasurePointVersion = 0,
        ExtComTest2 = 1,
        MediatorTest1 = 2,
        MediatorTest2 = 3,
        SurveilTest1 = 4,
        SurveilTest2 = 5,
        ComNumMsgNotSent = 6,
        ComUtilPerc = 7,
        LogTest1 = 8,
        LogTest2 = 9,
        UITest1 = 10,
        UITest2 = 11,
        ActualPressure = 12,
        ActualPressBackup = 13,
        PatientPress = 14,
        FlowVolumeOutlet = 15,
        FlowVolumePatient = 16,
        Volume = 17,
        FlowLeakageLevel = 18,
        PatCompliance = 19,
        PatResistance = 20,
        MonPpeak = 21,
        MonPeep = 22,
        MonPmean = 23,
        MonLeakage = 24,
        MonMv = 25,
        MonVt = 26,
        MonPercInTv = 27,
        MonTotalRate = 28,
        MonSpontRate = 29,
        MonPercSpont = 30,
        MonIE = 31,
        MonInspTime = 32,
        MonRiseTime = 33,
        XCO2SensorExists = 34,
        EtCO2Perc = 35,
        InspCO2Perc = 36,
        AtmPressure = 37,
        MomentaryCO2Perc = 38,
        PtCO2 = 39,
        SPO2SensorExists = 40,
        SpO2 = 41,
        Pulse = 42,
        FiO2Perc = 43,
        FiO2SensorExists = 44,
        MonCO2sensorCalibBusy = 45,
        MonDeltaVol = 46,
        PeakFlow = 47,
        InspirationTriggerType = 48,
        MaskOn = 49,
        AlarmMask_0_31 = 50,
        AlarmMask_32_63 = 51,
        AlarmMask_64_95 = 52,
        AlarmMask_96_127 = 53,
        AlarmMask_128_159 = 54,
        SystemState = 55,
        PowerSource = 56,
        BackupSource = 57,
        ClickonBatStatus = 58,
        ClickonBatCapacity = 59,
        ClickonBatMode = 60,
        ClickonBatAtRate = 61,
        ClickonBatAtRateTimeToEmpty = 62,
        ClickonBatAtRateOk = 63,
        ClickonBatTemp = 64,
        ClickonBatCurrent = 65,
        ClickonBatAverageCurrent = 66,
        ClickonBatRelativeStateOfCharge = 67,
        ClickonBatTimeToEmpty = 68,
        ClickonBatChargingCurrent = 69,
        ClickonBatChargingVoltage = 70,
        ClickonBatChargingCycleCount = 71,
        ClickonBatChargingSerialNumber = 72,
        InternalBatStatus = 73,
        InternalBatCapacity = 74,
        InternalBatMode = 75,
        InternalBatAtRate = 76,
        InternalBatAtRateTimeToEmpty = 77,
        InternalBatAtRateOk = 78,
        InternalBatTemp = 79,
        InternalBatCurrent = 80,
        InternalBatAverageCurrent = 81,
        InternalBatRelativeStateOfCharge = 82,
        InternalBatTimeToEmpty = 83,
        InternalBatChargingCurrent = 84,
        InternalBatChargingVoltage = 85,
        InternalBatChargingCycleCount = 86,
        InternalBatChargingSerialNumber = 87,
        MainBuzzerOn = 88,
        PressureMainRawAd = 89,
        PressureBackupRawAd = 90,
        MassFlowRateRawAd = 91,
        FiO2RawAd = 92,
        PatientAirTempRawAd = 93,
        HeaterplateTempRawAd = 94,
        BlowerMotorVoltageRawAd = 95,
        BlowerMotorCurrentRawAd = 96,
        BlowerMotorTempRawAd = 97,
        InternalDcVoltageRawAd = 98,
        ExternalDcVoltageRawAd = 99,
        InternalBatteryVoltageRawAd = 100,
        InternalBatteryCurrentRawAd = 101,
        InternalBatteryTempRawAd = 102,
        ExternalBatteryVoltageRawAd = 103,
        ExternalBatteryCurrentRawAd = 104,
        ExternalBatteryTempRawAd = 105,
        CPU12VoltRawAd = 106,
        CPU5VoltRawAd = 107,
        CPUTempRawAd = 108,
        PressureMainFiltAd = 109,
        PressureBackupFiltAd = 110,
        MassFlowRateAd = 111,
        Pressure = 112,
        PressureBackup = 113,
        MassFlowRate = 114,
        FiO2 = 115,
        PatientAirTemp = 116,
        HeaterplateTemp = 117,
        BlowerMotorVoltage = 118,
        BlowerMotorCurrent = 119,
        BlowerMotorTemp = 120,
        InternalDcVoltage = 121,
        ExternalDcVoltage = 122,
        InternalBatteryVoltage = 123,
        InternalBatteryCurrent = 124,
        InternalBatteryTemp = 125,
        ExternalBatteryVoltage = 126,
        ExternalBatteryCurrent = 127,
        ExternalBatteryTemp = 128,
        CPU12Volt = 129,
        CPU5Volt = 130,
        CPUTemp = 131,
        PressureMainOffsetAD = 132,
        PressureBackupOffsetAD = 133,
        PressureExpOffsetAD = 134,
        FlowMassOffsetAD = 135,
        FlowExpOffsetAD = 136,
        OxygenPressureADOffset = 137,
        MotorPower = 138,
        MotorRPM = 139,
        MotorPWM = 140,
        MainBootVer = 141,
        MainCPuVer = 142,
        CoolingFanCtrl = 143,
        CoolingFanRPM = 144,
        AlarmAck1Voltage = 145,
        AlarmAck2Voltage = 146,
        ExhValve = 147,
        MotorCtrl = 148,
        SetCalibration = 149,
        SetCalibrationChannel = 150,
        SetCalibrationAddress = 151,
        CalibrationStatus = 152,
        CalibrationInProgress = 153,
        TurnOn = 154,
        ThreadOverrun = 155,
        ThreadOverrunReset = 156,
        ThreadStack = 157,
        TransferSensorOffset = 158,
        TestValue = 159,
        TreatTest1 = 160,
        TreatTest2 = 161,
        TreatTest3 = 162,
        TreatTest4 = 163,
        TreatTest5 = 164,
        TreatTest6 = 165,
        TreatTest7 = 166,
        TreatTest8 = 167,
        TreatTest9 = 168,
        TreatTest10 = 169,
        TotalTreatmentTime = 170,
        DebugTest1 = 171,
        DebugTest2 = 172,
        ParamServerTest1 = 173,
        ParamServerTest2 = 174

    }

    public enum TProfileNumber
    {
        P1 = 0,
        P2 = 1,
        P3 = 2,
        Pc = 4,
        ServicePc = 5
    }

    public enum TreatmentSettingType
    {
        Value = 0,          /**< Set treatment value >*/
        MaxLimit = 1,         /**< Treatment home adjust max limit >*/
        MinLimit = 2          /**< Treatment home adjust min limit >*/
    }

    public enum StepSettingDirection
    {
        Up = 0,
        Down = 1
    }
}