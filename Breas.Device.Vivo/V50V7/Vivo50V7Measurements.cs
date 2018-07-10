using Breas.Device.Monitoring;
using Breas.Device.Monitoring.Measurements;
using System.Collections.Generic;
using System.Linq;

namespace Breas.Device.Vivo.V50V7
{
    /// <summary>
    /// Measurements for Vivo50 V7. Vivo50 US inherits from this class and uses most measurements
    /// </summary>
    public class Vivo50V7Measurements : VivoMeasurements
    {
        #region Fields

        //not monitor values?
        //protected static DeviceMeasurement settingEtCO2HighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingEtCO2HighAlarm, Unit.Percent, MeasurementFormatters.FormatOffableInteger, .5, 33);
        //protected static DeviceMeasurement settingEtCO2LowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingEtCO2LowAlarm, Unit.Percent, MeasurementFormatters.FormatOffableInteger, .5, 34);

        //these measurements are static because Vivo 50 US uses them as well and we don't want to have the same measurements defined twice
        private static DeviceMeasurement ventMode = new SettingsDeviceMeasurement(VivoDeviceKeys.VentMode, "USAGE_SETTING_VENT_MODE_V7", Unit.None, MeasurementFormatters.FormatVentilatorMode, 54);

        private static DeviceMeasurement breathMode = new SettingsDeviceMeasurement(VivoDeviceKeys.BreathMode, "USAGE_SETTING_BREATH_MODE_V7", Unit.None, MeasurementFormatters.FormatBreathMode, 55);
        private static DeviceMeasurement patientMode = new SettingsDeviceMeasurement(VivoDeviceKeys.PatientMode, "USAGE_STATE_PATIENT_MODE_V7", Unit.None, MeasurementFormatters.FormatPatientMode, 71);
        private static DeviceMeasurement deviceMode = new SettingsDeviceMeasurement(VivoDeviceKeys.DeviceMode, "USAGE_STATE_DEVICE_MODE_V7", Unit.None, 72);
        private static DeviceMeasurement selectedProfile = new SettingsDeviceMeasurement(VivoDeviceKeys.SelectedProfile, "USAGE_STATE_SELECTED_PROFILE_V7", Unit.None, 76);
        private static DeviceMeasurement adjustmentInHome = new SettingsDeviceMeasurement(VivoDeviceKeys.AdjustmentInHome, "USAGE_STATE_ADJUSTMENT_IN_HOME_V7", Unit.None, 77);
        private static DeviceMeasurement circuitRecognition = new SettingsDeviceMeasurement(VivoDeviceKeys.CircuitRecognition, "USAGE_STATE_CIRCUIT_RECOGNITION_V7", Unit.None, 62);
        private static DeviceMeasurement currentCircuit = new SettingsDeviceMeasurement(VivoDeviceKeys.CurrentCircuit, "USAGE_STATE_CURRENT_CIRCUIT_V7", Unit.None, MeasurementFormatters.FormatCircuitMode, 63);

        protected static DeviceMeasurement alarmSoundLevel = new SettingsDeviceMeasurement(VivoDeviceKeys.AlarmSoundLevel, "STREAM_USAGE_SETTING_ALARM_SOUND_LEVEL_V7", Unit.None, 60);
        
        protected static DeviceMeasurement settingSigh = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingSigh, "STREAM_USAGE_SETTING_SIGH_V7", null, MeasurementFormatters.FormatOnOff, 19);
        protected static DeviceMeasurement settingSighRate = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingSighRate, "STREAM_USAGE_SETTING_SIGH_RATE_V7", null, 20);
        protected static DeviceMeasurement settingSighPerc = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingSighPerc, "STREAM_USAGE_SETTING_SIGH_PERC_V7", null, 21);

        protected static DeviceMeasurement settingPressHighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingPressHighAlarm, "STREAM_USAGE_SETTING_ALARM_PRESS_HIGH_V7", Unit.CmH2O, .5, 22);
        protected static DeviceMeasurement settingPressLowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingPressLowAlarm, "STREAM_USAGE_SETTING_ALARM_PRESS_LOW_V7", Unit.CmH2O, .5, 23);

        protected static DeviceMeasurement settingTidalVolHighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingTidalVolHighAlarm, "STREAM_USAGE_SETTING_ALARM_TIDAL_VOL_HIGH_V7", Unit.Millilitre, MeasurementFormatters.FormatOffableInteger, StorageType.UShort, 10d, 26);
        protected static DeviceMeasurement settingTidalVolLowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingTidalVolLowAlarm, "STREAM_USAGE_SETTING_ALARM_TIDAL_VOL_LOW_V7", Unit.Millilitre, null, StorageType.UShort, 10d, 27);

        protected static DeviceMeasurement settingBreathRateHighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingBreathRateHighAlarm, "STREAM_USAGE_SETTING_ALARM_BREATH_RATE_HIGH_V7", Unit.Bpm, MeasurementFormatters.FormatOffableInteger, 1.0, 32);
        protected static DeviceMeasurement settingBreathRateLowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingBreathRateLowAlarm, "STREAM_USAGE_SETTING_ALARM_BREATH_RATE_LOW_V7", Unit.Bpm, MeasurementFormatters.FormatOffableInteger, 1.0, 33);

        protected static DeviceMeasurement settingMinuteVolHighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingMinuteVolHighAlarm, "STREAM_USAGE_SETTING_ALARM_MINUTE_VOL_HIGH_V7", Unit.Litre, MeasurementFormatters.FormatOffableDecimal3_1, StorageType.UShort, .1, 28);
        protected static DeviceMeasurement settingMinuteVolLowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingMinuteVolLowAlarm, "STREAM_USAGE_SETTING_ALARM_MINUTE_VOL_LOW_V7", Unit.Litre, MeasurementFormatters.FormatOffableDecimal3_1, StorageType.UShort, .1, 29);

        protected static DeviceMeasurement settingApneaAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingApneaAlarm, "STREAM_USAGE_SETTING_ALARM_APNEA_V7", Unit.Seconds, MeasurementFormatters.FormatOffableInteger, 1.0, 52);

        protected static DeviceMeasurement settingLeakageHighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingLeakageHighAlarm, "STREAM_USAGE_SETTING_ALARM_LEAKAGE_HIGH_V7", Unit.None, MeasurementFormatters.FormatOnOff, 1.0, 49);
        protected static DeviceMeasurement settingLeakageLowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingLeakageLowAlarm, "STREAM_USAGE_SETTING_ALARM_LEAKAGE_LOW_V7", Unit.None, MeasurementFormatters.FormatOnOff, 1.0, 50);

        protected static DeviceMeasurement settingPeepHighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingPeepHighAlarm, "STREAM_USAGE_SETTING_ALARM_PEEP_HIGH_V7", Unit.None, MeasurementFormatters.FormatOnOff, 1.0, 24);
        protected static DeviceMeasurement settingPeepLowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingPeepLowAlarm, "STREAM_USAGE_SETTING_ALARM_PEEP_LOW_V7", Unit.None, MeasurementFormatters.FormatOnOff, 1.0, 25);

        protected static DeviceMeasurement settingSpO2HighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingSpO2HighAlarm, "STREAM_USAGE_SETTING_ALARM_SPO2_HIGH_V7", Unit.Percent, MeasurementFormatters.FormatOffableInteger, .5, 45);
        protected static DeviceMeasurement settingSpO2LowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingSpO2LowAlarm, "STREAM_USAGE_SETTING_ALARM_SPO2_LOW_V7", Unit.Percent, MeasurementFormatters.FormatOffableInteger, .5, 46);

        protected static DeviceMeasurement settingPulseRateHighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingPulseRateHighAlarm, "STREAM_USAGE_SETTING_ALARM_PULSE_RATE_HIGH_V7", Unit.PulseBpm, MeasurementFormatters.FormatOffableInteger, 1.0, 47);
        protected static DeviceMeasurement settingPulseRateLowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingPulseRateLowAlarm, "STREAM_USAGE_SETTING_ALARM_PULSE_RATE_LOW_V7", Unit.PulseBpm, MeasurementFormatters.FormatOffableInteger, 1.0, 48);

        //not modeled
        //private static DeviceMeasurement _settingO2LowAlarm = new AlarmSettingsDeviceMeasurement(Vivo50DeviceKeys.SettingO2LowAlarm, Unit.None, 51));
        //private static DeviceMeasurement _settingFlowHighAlarm = new AlarmSettingsDeviceMeasurement(Vivo50DeviceKeys.SettingFlowHighAlarm, Unit.None, 30));
        //private static DeviceMeasurement _settingFlowLowAlarm = new AlarmSettingsDeviceMeasurement(Vivo50DeviceKeys.SettingFlowLowAlarm, Unit.None, 31));

        protected static DeviceMeasurement settingFiO2HighAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingFiO2HighAlarm, "STREAM_USAGE_SETTING_ALARM_FIO2_HIGH_V7", Unit.Percent, MeasurementFormatters.FormatOffableInteger, .5, 34);
        protected static DeviceMeasurement settingFiO2LowAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingFiO2LowAlarm, "STREAM_USAGE_SETTING_ALARM_FIO2_LOW_V7", Unit.Percent, MeasurementFormatters.FormatOffableInteger, .5, 35);

        protected static DeviceMeasurement settingTidalCO2HighAlarmPerc = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingEtCO2HighAlarmPerc, "STREAM_USAGE_SETTING_ALARM_END_TIDAL_CO2_HIGH_PERC_V7", Unit.Percent, MeasurementFormatters.FormatOffableDecimal3_1, .1, 36);
        protected static DeviceMeasurement settingTidalCO2LowAlarmPerc = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingEtCO2LowAlarmPerc, "STREAM_USAGE_SETTING_ALARM_END_TIDAL_CO2_LOW_PERC_V7", Unit.Percent, MeasurementFormatters.FormatOffableDecimal3_1, .1, 39);
        protected static DeviceMeasurement settingTidalCO2HighAlarmMmhg = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingEtCO2HighAlarmMmhg, "STREAM_USAGE_SETTING_ALARM_END_TIDAL_CO2_HIGH_MMHG_V7", Unit.MmHg, MeasurementFormatters.FormatOffableDecimal3_1, 1.0, 37);
        protected static DeviceMeasurement settingTidalCO2LowAlarmMmhg = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingEtCO2LowAlarmMmhg, "STREAM_USAGE_SETTING_ALARM_END_TIDAL_CO2_LOW_MMHG_V7", Unit.MmHg, MeasurementFormatters.FormatOffableDecimal3_1, 1.0, 40);
        protected static DeviceMeasurement settingTidalCO2HighAlarmKpa = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingEtCO2HighAlarmKpa, "STREAM_USAGE_SETTING_ALARM_END_TIDAL_CO2_HIGH_KPA_V7", Unit.Kpa, MeasurementFormatters.FormatOffableDecimal3_1, .1, 38);
        protected static DeviceMeasurement settingTidalCO2LowAlarmKpa = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingEtCO2LowAlarmKpa, "STREAM_USAGE_SETTING_ALARM_END_TIDAL_CO2_LOW_KPA_V7", Unit.Kpa, MeasurementFormatters.FormatOffableDecimal3_1, .1, 41);

        protected static DeviceMeasurement settingInspCO2HighAlarmPerc = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingInspCO2HighAlarmPerc, "STREAM_USAGE_SETTING_ALARM_INSPIRED_CO2_HIGH_PERC_V7", Unit.Percent, MeasurementFormatters.FormatOffableDecimal3_1, .1, 42);
        protected static DeviceMeasurement settingInspCO2HighAlarmKpa = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingInspCO2HighAlarmKpa, "STREAM_USAGE_SETTING_ALARM_INSPIRED_CO2_HIGH_KPA_V7", Unit.Kpa, MeasurementFormatters.FormatOffableDecimal3_1, 1.0, 44);
        protected static DeviceMeasurement settingInspCO2HighAlarmMmhg = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingInspCO2HighAlarmMmhg, "STREAM_USAGE_SETTING_ALARM_INSPIRED_CO2_HIGH_MMHG_V7", Unit.MmHg, MeasurementFormatters.FormatOffableDecimal3_1, .1, 43);

        protected static DeviceMeasurement settingCO2Unit = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingEtCO2Unit, "STREAM_USAGE_SETTING_ETCO2_UNIT_V7", Unit.None, 79);
        protected static DeviceMeasurement settingPressureUnit = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.PressureUnit, "STREAM_USAGE_SETTING_PRESSURE_UNIT", Unit.None, -1);

        private static DeviceMeasurement ppeakCmh20 = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementPPeak, Unit.CmH2O, 0.1);
        private static DeviceMeasurement ppeakHpa = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementPPeak, Unit.Hpa, 0.1, 0xFFFF);
        private static DeviceMeasurement ppeakmBar = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementPPeak, Unit.MBar, 0.1, 0xFFFF);

        private static DeviceMeasurement peepCmh20 = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementPeep, Unit.CmH2O, 0.1);
        private static DeviceMeasurement peepHpa = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementPeep, Unit.Hpa, 0.1, 0xFFFF);
        private static DeviceMeasurement peepmBar = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementPeep, Unit.MBar, 0.1, 0xFFFF);

        private static DeviceMeasurement pmeanCmh20 = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementPMean, Unit.CmH2O, 0.1);
        private static DeviceMeasurement pmeanHpa = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementPMean, Unit.Hpa, 0.1, 0xFFFF);
        private static DeviceMeasurement pmeanmBar = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementPMean, Unit.MBar, 0.1, 0xFFFF);

        private static DeviceMeasurement etCO2MeasurementPerc = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementEtCO2, Unit.Percent, 0.1, 0xFFFF);
        private static DeviceMeasurement etCO2MeasurementKpa = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementEtCO2, Unit.Kpa, 1.0, 0xFFFF);
        private static DeviceMeasurement etCO2MeasurementMmhg = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementEtCO2, Unit.MmHg, 0.1, 0xFFFF);

        private static DeviceMeasurement inspCO2MeasurementPerc = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementInspCO2, Unit.Percent, 0.1, 0xFFFF);
        private static DeviceMeasurement inspCO2MeasurementKpa = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementInspCO2, Unit.Kpa, 1.0, 0xFFFF);
        private static DeviceMeasurement inspCO2MeasurementMmhg = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementInspCO2, Unit.MmHg, 0.1, 0xFFFF);

        private static DeviceMeasurement co2AtmPressMeasurementPerc = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementCO2AtmPress, Unit.Percent, 0.1, 0xFFFF);
        private static DeviceMeasurement co2AtmPressMeasurementKpa = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementCO2AtmPress, Unit.Kpa, 1.0, 0xFFFF);
        private static DeviceMeasurement co2AtmPressMeasurementMmhg = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementCO2AtmPress, Unit.MmHg, 0.1, 0xFFFF);

        protected static DeviceMeasurement pPeakMeasurement = new MultiUnitDeviceMeasurement(settingPressureUnit,
            new Dictionary<int, DeviceMeasurement>()
            {
                //{ -1, settingEtCO2HighAlarm },
                { 0, ppeakCmh20 },
                { 1, ppeakHpa },
                { 2, ppeakmBar }
            });

        protected static DeviceMeasurement peepMeasurement = new MultiUnitDeviceMeasurement(settingPressureUnit,
            new Dictionary<int, DeviceMeasurement>()
            {
                //{ -1, settingEtCO2HighAlarm },
                { 0, peepCmh20 },
                { 1, peepHpa },
                { 2, peepmBar }
            });
        
        protected static DeviceMeasurement pmeanMeasurement = new MultiUnitDeviceMeasurement(settingPressureUnit,
            new Dictionary<int, DeviceMeasurement>()
            {
                //{ -1, settingEtCO2HighAlarm },
                { 0, pmeanCmh20 },
                { 1, pmeanHpa },
                { 2, pmeanmBar }
            });

        protected static DeviceMeasurement etCO2Measurement = new MultiUnitDeviceMeasurement(settingCO2Unit,
            new Dictionary<int, DeviceMeasurement>()
            {
                //{ -1, settingEtCO2HighAlarm },
                { 0, etCO2MeasurementMmhg },
                { 1, etCO2MeasurementKpa },
                { 2, etCO2MeasurementPerc }
            });

        protected static DeviceMeasurement inspCO2Measurement = new MultiUnitDeviceMeasurement(settingCO2Unit,
            new Dictionary<int, DeviceMeasurement>()
            {
                //{ -1, settingEtCO2HighAlarm },
                { 0, inspCO2MeasurementMmhg },
                { 1, inspCO2MeasurementKpa },
                { 2, inspCO2MeasurementPerc }
            });
        
        protected static DeviceMeasurement co2AtmPressMeasurement = new MultiUnitDeviceMeasurement(settingCO2Unit,
            new Dictionary<int, DeviceMeasurement>()
            {
                //{ -1, settingEtCO2HighAlarm },
                { 0, co2AtmPressMeasurementMmhg },
                { 1, co2AtmPressMeasurementKpa },
                { 2, co2AtmPressMeasurementPerc }
            });

        protected static DeviceMeasurement settingEtCO2HighAlarm = new MultiUnitDeviceMeasurement(settingCO2Unit,
            new Dictionary<int, DeviceMeasurement>()
            {
                //{ -1, settingEtCO2HighAlarm },
                { 0, settingTidalCO2HighAlarmMmhg },
                { 1, settingTidalCO2HighAlarmKpa },
                { 2, settingTidalCO2HighAlarmPerc }
            });

        protected static DeviceMeasurement settingEtCO2LowAlarm = new MultiUnitDeviceMeasurement(settingCO2Unit,
            new Dictionary<int, DeviceMeasurement>()
            {
               //{ -1, settingEtCO2LowAlarm },
                { 0, settingTidalCO2LowAlarmMmhg },
                { 1, settingTidalCO2LowAlarmKpa },
                { 2, settingTidalCO2LowAlarmPerc }
            });

        protected static DeviceMeasurement settingInspCO2HighAlarm = new MultiUnitDeviceMeasurement(settingCO2Unit,
            new Dictionary<int, DeviceMeasurement>()
            {
                { 0, settingInspCO2HighAlarmMmhg },
                { 1, settingInspCO2HighAlarmKpa },
                { 2, settingInspCO2HighAlarmPerc }
            });

        #endregion Fields

        #region Usage settings

        public override DeviceMeasurement VentMode
        {
            get { return Vivo50V7Measurements.ventMode; }
        }

        public override DeviceMeasurement BreathMode
        {
            get { return Vivo50V7Measurements.breathMode; }
        }

        public override DeviceMeasurement PatientMode
        {
            get { return Vivo50V7Measurements.patientMode; }
        }

        public override DeviceMeasurement DeviceMode
        {
            get { return Vivo50V7Measurements.deviceMode; }
        }

        public override DeviceMeasurement SelectedProfile
        {
            get { return Vivo50V7Measurements.selectedProfile; }
        }

        public override DeviceMeasurement AdjustmentInHome
        {
            get { return Vivo50V7Measurements.adjustmentInHome; }
        }

        public override DeviceMeasurement CircuitRecognition
        {
            get { return Vivo50V7Measurements.circuitRecognition; }
        }

        public override DeviceMeasurement CurrentCircuit
        {
            get { return Vivo50V7Measurements.currentCircuit; }
        }

        public override DeviceMeasurement SimvSupportPressure
        {
            get { return VivoMeasurements.simvSupportPressure; }
        }

        #endregion Usage settings

        #region Measurements

        public override DeviceMeasurement MeasurementPPeak
        {
            get
            {
                return Vivo50V7Measurements.pPeakMeasurement;
            }
        }

        public override DeviceMeasurement MeasurementPeep
        {
            get
            {
                return Vivo50V7Measurements.peepMeasurement;
            }
        }

        public override DeviceMeasurement MeasurementPMean
        {
            get
            {
                return Vivo50V7Measurements.pmeanMeasurement;
            }
        }

        public override DeviceMeasurement CO2Unit
        {
            get { return Vivo50V7Measurements.settingCO2Unit; }
        }

        public override DeviceMeasurement PressureUnit
        {
            get
            {
                return Vivo50V7Measurements.settingPressureUnit;
            }
        }

        public override DeviceMeasurement MeasurementEtCO2
        {
            get { return Vivo50V7Measurements.etCO2Measurement; }
        }

        public override DeviceMeasurement MeasurementInspCO2
        {
            get { return Vivo50V7Measurements.inspCO2Measurement; }
        }

        public override DeviceMeasurement MeasurementCO2AtmPress
        {
            get { return Vivo50V7Measurements.co2AtmPressMeasurement; }
        }

        public override DeviceMeasurement MeasurementMvI
        {
            get { return VivoMeasurements.mviMeasurement; }
        }

        public override DeviceMeasurement MeasurementMve
        {
            get { return VivoMeasurements.mveMeasurement; }
        }

        public override DeviceMeasurement MeasurementVti
        {
            get { return VivoMeasurements.vtiMeasurement; }
        }

        public override DeviceMeasurement MeasurementVte
        {
            get { return VivoMeasurements.vteMeasurement; }
        }

        #endregion Measurements

        #region Target properties

        public override DeviceMeasurement SettingPressHighAlarm
        {
            get { return Vivo50V7Measurements.settingPressHighAlarm; }
        }

        public override DeviceMeasurement SettingPressLowAlarm
        {
            get { return Vivo50V7Measurements.settingPressLowAlarm; }
        }

        public override DeviceMeasurement SettingTidalVolHighAlarm
        {
            get { return Vivo50V7Measurements.settingTidalVolHighAlarm; }
        }

        public override DeviceMeasurement SettingTidalVolLowAlarm
        {
            get { return Vivo50V7Measurements.settingTidalVolLowAlarm; }
        }

        #endregion Target properties

        #region Misc measurements

        public override DeviceMeasurement AlarmSoundLevel
        {
            get { return Vivo50V7Measurements.alarmSoundLevel; }
        }

        #endregion Misc measurements

        protected static DeviceMeasurement[] sigh = new DeviceMeasurement[]
        {
            Vivo50V7Measurements.settingSigh,
            Vivo50V7Measurements.settingSighRate,
            Vivo50V7Measurements.settingSighPerc
        };

        protected static DeviceMeasurement[] pcvSimV = new DeviceMeasurement[]
        {
            Vivo50V7Measurements.settingInspPress,
            Vivo50V7Measurements.settingPeep,
            Vivo50V7Measurements.settingBackupRate,
            Vivo50V7Measurements.settingInspTime,
            Vivo50V7Measurements.settingRiseTimePress,
            Vivo50V7Measurements.settingInspTrigger,
            Vivo50V7Measurements.simvSupportPressure,
            Vivo50V7Measurements.settingExpTrigger,
            Vivo50V7Measurements.settingExpTrigger,
            Vivo50V7Measurements.alarmSoundLevel,
        };

        protected static DeviceMeasurement[] vcvSimV = new DeviceMeasurement[]
        {
            Vivo50V7Measurements.settingSetVol,
            Vivo50V7Measurements.settingPeep,
            Vivo50V7Measurements.settingBackupRate,
            Vivo50V7Measurements.settingInspTime,
            Vivo50V7Measurements.settingRiseTimeVol,
            Vivo50V7Measurements.settingInspTrigger,
            Vivo50V7Measurements.settingFlowPattern,
            Vivo50V7Measurements.simvSupportPressure,
            Vivo50V7Measurements.settingRiseTimePress,
            Vivo50V7Measurements.settingExpTrigger,
            Vivo50V7Measurements.alarmSoundLevel,
        };

        protected static Dictionary<OpMode, DeviceMeasurement[]> usageSettingsMap = new Dictionary<OpMode, DeviceMeasurement[]>();
        protected static DeviceMeasurement[] alarmSettings;

        static Vivo50V7Measurements()
        {
            usageSettingsMap[OpMode.PSV] = psvMeasurements.Extend(sigh)
                                                          .Extend(alarmSoundLevel);

            usageSettingsMap[OpMode.PSV_T] = psvMeasurements.Extend(targetMeasurements)
                                                            .Extend(sigh)
                                                            .Extend(alarmSoundLevel);

            usageSettingsMap[OpMode.PCV] = pcvMeasurements.Extend(sigh)
                                                          .Extend(alarmSoundLevel);

            usageSettingsMap[OpMode.PCV_T] = pcvMeasurements.Extend(settingTargetVolume)
                                                            .Extend(targetMeasurements)
                                                            .Extend(sigh)
                                                            .Extend(alarmSoundLevel);

            usageSettingsMap[OpMode.PCV_A] = pcvMeasurements.Extend(aMeasurements)
                                                            .Extend(sigh)
                                                            .Extend(alarmSoundLevel);

            usageSettingsMap[OpMode.PCV_A_T] = pcvMeasurements.Extend(settingTargetVolume)
                                                              .Extend(aMeasurements)
                                                              .Extend(targetMeasurements)
                                                              .Extend(sigh)
                                                              .Extend(alarmSoundLevel);

            usageSettingsMap[OpMode.VCV] = vcvMeasurements.Extend(sigh)
                                                          .Extend(alarmSoundLevel);

            usageSettingsMap[OpMode.VCV_A] = vcvMeasurements.Extend(sigh)
                                                            .Extend(alarmSoundLevel);

            usageSettingsMap[OpMode.CPAP] = cpapMeasurements.Extend(alarmSoundLevel);

            usageSettingsMap[OpMode.PCV_SIMV] = pcvSimV;
            usageSettingsMap[OpMode.VCV_SIMV] = vcvSimV;

            //alarm settings are used in all modes
            alarmSettings = new DeviceMeasurement[]
            {
                Vivo50V7Measurements.settingPressHighAlarm,
                Vivo50V7Measurements.settingPressLowAlarm,
                Vivo50V7Measurements.settingTidalVolHighAlarm,
                Vivo50V7Measurements.settingTidalVolLowAlarm,
                Vivo50V7Measurements.settingBreathRateHighAlarm,
                Vivo50V7Measurements.settingBreathRateLowAlarm,
                Vivo50V7Measurements.settingMinuteVolHighAlarm,
                Vivo50V7Measurements.settingMinuteVolLowAlarm,
                Vivo50V7Measurements.settingApneaAlarm,
                Vivo50V7Measurements.settingLeakageHighAlarm,
                Vivo50V7Measurements.settingLeakageLowAlarm,
                Vivo50V7Measurements.settingPeepHighAlarm,
                Vivo50V7Measurements.settingPeepLowAlarm,
                Vivo50V7Measurements.settingSpO2HighAlarm,
                Vivo50V7Measurements.settingSpO2LowAlarm,
                Vivo50V7Measurements.settingPulseRateHighAlarm,
                Vivo50V7Measurements.settingPulseRateLowAlarm,
                Vivo50V7Measurements.settingFiO2HighAlarm,
                Vivo50V7Measurements.settingFiO2LowAlarm,
                Vivo50V7Measurements.settingEtCO2HighAlarm,
                Vivo50V7Measurements.settingEtCO2LowAlarm,
                Vivo50V7Measurements.settingInspCO2HighAlarm
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