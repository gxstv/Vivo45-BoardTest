using Breas.Device.Monitoring;
using Breas.Device.Monitoring.Measurements;
using System.Collections.Generic;
using System.Linq;

namespace Breas.Device.Vivo.V50
{
    public class Vivo50Measurements : VivoMeasurements
    {
        #region Fields

        //these measurements can be instance fields because there should only be one Vivo50Measurements class
        private static DeviceMeasurement ventMode = new SettingsDeviceMeasurement(VivoDeviceKeys.VentMode, "VENTILATORMODE", Unit.None, MeasurementFormatters.FormatVentilatorMode, 44);

        private static DeviceMeasurement breathMode = new SettingsDeviceMeasurement(VivoDeviceKeys.BreathMode, "BREATH_MODE", Unit.None, MeasurementFormatters.FormatBreathMode, 45);
        private static DeviceMeasurement patientMode = new SettingsDeviceMeasurement(VivoDeviceKeys.PatientMode, "PATIENT_MODE", Unit.None, MeasurementFormatters.FormatPatientMode, 61);
        private static DeviceMeasurement deviceMode = new SettingsDeviceMeasurement(VivoDeviceKeys.DeviceMode, "STREAM_USAGE_STATE_DEVICE_MODE", Unit.None, 62);
        private static DeviceMeasurement selectedProfile = new SettingsDeviceMeasurement(VivoDeviceKeys.SelectedProfile, "STREAM_USAGE_STATE_SELECTED_PROFILE", Unit.None, 66);
        private static DeviceMeasurement adjustmentInHome = new SettingsDeviceMeasurement(VivoDeviceKeys.AdjustmentInHome, "STREAM_USAGE_STATE_ADJUSTMENT_IN_HOME_V7", Unit.None, 67);
        private static DeviceMeasurement circuitRecognition = new SettingsDeviceMeasurement(VivoDeviceKeys.CircuitRecognition, "USAGE_STATE_CIRCUIT_RECOGNITION_V7", Unit.None, 52);
        private static DeviceMeasurement currentCircuit = new SettingsDeviceMeasurement(VivoDeviceKeys.CurrentCircuit, "STREAM_USAGE_STATE_CURRENT_CIRCUIT", Unit.None, MeasurementFormatters.FormatCircuitMode, 53);

        #endregion Fields

        private static DeviceMeasurement etCO2Measurement = new MonitorDeviceMeasurement(string.Empty, Unit.MmHg, 0.1, 0xFFFF);
        private static DeviceMeasurement inspCO2Measurement = new MonitorDeviceMeasurement(string.Empty, Unit.MmHg, 0.1, 0xFFFF);
        private static DeviceMeasurement co2AtmPressMeasurement = new MonitorDeviceMeasurement(string.Empty, Unit.Percent, 0.1);

        private static DeviceMeasurement alarmSoundLevel = new SettingsDeviceMeasurement(VivoDeviceKeys.AlarmSoundLevel, "USAGE_SETTING_ALARM_SOUND_LEVEL_V7", Unit.None, 60);

        //alarm settings
        private static DeviceMeasurement settingPressHighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingPressHighAlarm, "USAGE_SETTING_ALARM_PRESS_HIGH_V7", Unit.CmH2O, .5, 19);

        private static DeviceMeasurement settingPressLowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingPressLowAlarm, "USAGE_SETTING_ALARM_PRESS_LOW_V7", Unit.CmH2O, .5, 20);

        //Tidal vol group
        private static DeviceMeasurement settingTidalVolHighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingTidalVolHighAlarm, "STREAM_USAGE_SETTING_ALARM_TIDAL_VOL_HIGH", Unit.Millilitre, MeasurementFormatters.FormatOffableInteger, StorageType.UShort, 10d, 23);

        private static DeviceMeasurement settingTidalVolLowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingTidalVolLowAlarm, "STREAM_USAGE_SETTING_ALARM_TIDAL_VOL_LOW", Unit.Millilitre, null, StorageType.UShort, 10d, 24);

        //not modeled in current software
        //_alarmSettingsMeasurements.Add(SettingEndTidalCO2HighAlarm = new AlarmSettingsDeviceMeasurement(Vivo50DeviceKeys.SettingEndTidalCO2HighAlarm, Unit.Percent, MeasurementFormatters.FormatOffableInteger, .5, 33));
        //_alarmSettingsMeasurements.Add(SettingEndTidalCO2LowAlarm = new AlarmSettingsDeviceMeasurement(Vivo50DeviceKeys.SettingEndTidalCO2LowAlarm, Unit.Percent, MeasurementFormatters.FormatOffableInteger, .5, 34));

        //Breath rate group
        private static DeviceMeasurement settingBreathRateHighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingBreathRateHighAlarm, "USAGE_SETTING_ALARM_BREATH_RATE_HIGH_V7", Unit.Bpm, MeasurementFormatters.FormatOffableInteger, 1.0, 29);

        private static DeviceMeasurement settingBreathRateLowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingBreathRateLowAlarm, "USAGE_SETTING_ALARM_BREATH_RATE_LOW_V7", Unit.Bpm, MeasurementFormatters.FormatOffableInteger, 1.0, 30);

        //Minute vol group
        private static DeviceMeasurement settingMinuteVolHighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingMinuteVolHighAlarm, "STREAM_USAGE_SETTING_ALARM_MINUTE_VOL_HIGH_", Unit.Litre, MeasurementFormatters.FormatOffableDecimal3_1, StorageType.UShort, .1, 25);

        private static DeviceMeasurement settingMinuteVolLowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingMinuteVolLowAlarm, "STREAM_USAGE_SETTING_ALARM_MINUTE_VOL_LOW_", Unit.Litre, MeasurementFormatters.FormatOffableDecimal3_1, StorageType.UShort, .1, 24);

        //apnea
        private static DeviceMeasurement settingApneaAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingApneaAlarm, "STREAM_USAGE_SETTING_ALARM_APNEA", Unit.Seconds, MeasurementFormatters.FormatOffableInteger, 1.0, 42);

        //Leakage group
        private static DeviceMeasurement settingLeakageHighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingLeakageHighAlarm, "STREAM_USAGE_SETTING_ALARM_LEAKAGE_HIGH", Unit.None, MeasurementFormatters.FormatOnOff, 1.0, 39);

        private static DeviceMeasurement settingLeakageLowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingLeakageLowAlarm, "STREAM_USAGE_SETTING_ALARM_LEAKAGE_LOW", Unit.None, MeasurementFormatters.FormatOnOff, 1.0, 40);

        //Peep group
        private static DeviceMeasurement settingPeepHighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingPeepHighAlarm, "STREAM_USAGE_SETTING_ALARM_LEAKAGE_LOW", Unit.None, MeasurementFormatters.FormatOnOff, 1.0, 21);

        private static DeviceMeasurement settingPeepLowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingPeepLowAlarm, "STREAM_USAGE_SETTING_ALARM_PEEP_HIGH", Unit.None, MeasurementFormatters.FormatOnOff, 1.0, 22);

        //not modeled in current software
        //_alarmSettingsMeasurements.Add(SettingO2LowAlarm = new AlarmSettingsDeviceMeasurement(Vivo50DeviceKeys.SettingO2LowAlarm, Unit.None, 41));
        //_alarmSettingsMeasurements.Add(SettingFlowHighAlarm = new AlarmSettingsDeviceMeasurement(Vivo50DeviceKeys.SettingFlowHighAlarm, Unit.None, 27));
        //_alarmSettingsMeasurements.Add(SettingFlowLowAlarm = new AlarmSettingsDeviceMeasurement(Vivo50DeviceKeys.SettingFlowLowAlarm, Unit.None, 28));

        private static DeviceMeasurement settingSpO2HighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingSpO2HighAlarm, "STREAM_USAGE_SETTING_ALARM_SPO2_HIGH", Unit.Percent, MeasurementFormatters.FormatOffableInteger, .5, 35);
        private static DeviceMeasurement settingSpO2LowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingSpO2LowAlarm, "STREAM_USAGE_SETTING_ALARM_SPO2_LOW", Unit.Percent, MeasurementFormatters.FormatOffableInteger, .5, 36);
        private static DeviceMeasurement settingPulseRateHighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingPulseRateHighAlarm, "STREAM_USAGE_SETTING_ALARM_PULSE_RATE_HIGH", Unit.PulseBpm, MeasurementFormatters.FormatOffableInteger, 1.0, 37);
        private static DeviceMeasurement settingPulseRateLowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingPulseRateLowAlarm, "STREAM_USAGE_SETTING_ALARM_PULSE_RATE_LOW", Unit.PulseBpm, MeasurementFormatters.FormatOffableInteger, 1.0, 38);
        private static DeviceMeasurement settingFiO2HighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingFiO2HighAlarm, "STREAM_USAGE_SETTING_ALARM_FIO2_HIGH", Unit.Percent, MeasurementFormatters.FormatOffableInteger, .5, 31);
        private static DeviceMeasurement settingFiO2LowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingFiO2LowAlarm, "STREAM_USAGE_SETTING_ALARM_FIO2_LOW", Unit.Percent, MeasurementFormatters.FormatOffableInteger, .5, 32);

        #region Usage settings

        public override DeviceMeasurement VentMode
        {
            get { return ventMode; }
        }

        public override DeviceMeasurement BreathMode
        {
            get { return breathMode; }
        }

        public override DeviceMeasurement PatientMode
        {
            get { return patientMode; }
        }

        public override DeviceMeasurement DeviceMode
        {
            get { return deviceMode; }
        }

        public override DeviceMeasurement SelectedProfile
        {
            get { return selectedProfile; }
        }

        public override DeviceMeasurement AdjustmentInHome
        {
            get { return adjustmentInHome; }
        }

        public override DeviceMeasurement CircuitRecognition
        {
            get { return circuitRecognition; }
        }

        public override DeviceMeasurement CurrentCircuit
        {
            get { return currentCircuit; }
        }

        public override DeviceMeasurement SimvSupportPressure
        {
            get { return simvSupportPressure; }
        }

        #endregion Usage settings

        #region Measurements

        public override DeviceMeasurement MeasurementEtCO2
        {
            get { return Vivo50Measurements.etCO2Measurement; }
        }

        public override DeviceMeasurement MeasurementInspCO2
        {
            get { return Vivo50Measurements.inspCO2Measurement; }
        }

        public override DeviceMeasurement MeasurementCO2AtmPress
        {
            get { return Vivo50Measurements.co2AtmPressMeasurement; }
        }

        public override DeviceMeasurement MeasurementMvI
        {
            get { return Vivo50Measurements.mviMeasurement; }
        }

        public override DeviceMeasurement MeasurementMve
        {
            get { return Vivo50Measurements.mveMeasurement; }
        }

        public override DeviceMeasurement MeasurementVti
        {
            get { return Vivo50Measurements.vtiMeasurement; }
        }

        public override DeviceMeasurement MeasurementVte
        {
            get { return Vivo50Measurements.vteMeasurement; }
        }

        #endregion Measurements

        #region Target properties

        public override DeviceMeasurement SettingPressHighAlarm
        {
            get { return Vivo50Measurements.settingPressHighAlarm; }
        }

        public override DeviceMeasurement SettingPressLowAlarm
        {
            get { return Vivo50Measurements.settingPressLowAlarm; }
        }

        public override DeviceMeasurement SettingTidalVolHighAlarm
        {
            get { return Vivo50Measurements.settingTidalVolHighAlarm; }
        }

        public override DeviceMeasurement SettingTidalVolLowAlarm
        {
            get { return Vivo50Measurements.settingTidalVolLowAlarm; }
        }

        #endregion Target properties

        #region Misc measurements

        public override DeviceMeasurement AlarmSoundLevel
        {
            get { return Vivo50Measurements.alarmSoundLevel; }
        }

        #endregion Misc measurements

        protected static Dictionary<OpMode, DeviceMeasurement[]> usageSettingsMap = new Dictionary<OpMode, DeviceMeasurement[]>();
        protected static DeviceMeasurement[] alarmSettings;

        static Vivo50Measurements()
        {
            usageSettingsMap[OpMode.PSV] = psvMeasurements.Extend(alarmSoundLevel);

            usageSettingsMap[OpMode.PSV_T] = psvMeasurements.Extend(targetMeasurements)
                                                            .Extend(alarmSoundLevel);

            usageSettingsMap[OpMode.PCV] = pcvMeasurements.Extend(alarmSoundLevel);

            usageSettingsMap[OpMode.PCV_T] = pcvMeasurements.Extend(settingTargetVolume)
                                                            .Extend(targetMeasurements)
                                                            .Extend(alarmSoundLevel);

            usageSettingsMap[OpMode.PCV_A] = pcvMeasurements.Extend(aMeasurements)
                                                            .Extend(alarmSoundLevel);

            usageSettingsMap[OpMode.PCV_A_T] = pcvMeasurements.Extend(settingTargetVolume)
                                                              .Extend(aMeasurements)
                                                              .Extend(targetMeasurements)
                                                              .Extend(alarmSoundLevel);

            usageSettingsMap[OpMode.VCV] = vcvMeasurements.Extend(alarmSoundLevel);
            usageSettingsMap[OpMode.VCV_A] = vcvMeasurements.Extend(alarmSoundLevel);
            usageSettingsMap[OpMode.CPAP] = cpapMeasurements.Extend(alarmSoundLevel);

            //alarm settings are used in all modes
            alarmSettings = new DeviceMeasurement[]
            {
                Vivo50Measurements.settingPressHighAlarm,
                Vivo50Measurements.settingPressLowAlarm,
                Vivo50Measurements.settingTidalVolHighAlarm,
                Vivo50Measurements.settingTidalVolLowAlarm,
                Vivo50Measurements.settingBreathRateHighAlarm,
                Vivo50Measurements.settingBreathRateLowAlarm,
                Vivo50Measurements.settingMinuteVolHighAlarm,
                Vivo50Measurements.settingMinuteVolLowAlarm,
                Vivo50Measurements.settingApneaAlarm,
                Vivo50Measurements.settingLeakageHighAlarm,
                Vivo50Measurements.settingLeakageLowAlarm,
                Vivo50Measurements.settingPeepHighAlarm,
                Vivo50Measurements.settingPeepLowAlarm,
                Vivo50Measurements.settingSpO2HighAlarm,
                Vivo50Measurements.settingSpO2LowAlarm,
                Vivo50Measurements.settingPulseRateHighAlarm,
                Vivo50Measurements.settingPulseRateLowAlarm,
                Vivo50Measurements.settingFiO2HighAlarm,
                Vivo50Measurements.settingFiO2LowAlarm
            };
        }

        public override List<DeviceMeasurement> GetUsageSettingMeasurements(OpMode opmode)
        {
            if (usageSettingsMap.ContainsKey(opmode))
            {
                return usageSettingsMap[opmode].ToList();
            }
            return new List<DeviceMeasurement>();
        }

        public override List<DeviceMeasurement> GetAlarmSettingMeasurements(OpMode opmode)
        {
            return alarmSettings.ToList();
        }
    }
}