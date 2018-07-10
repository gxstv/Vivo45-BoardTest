using Breas.Device.Monitoring;
using Breas.Device.Monitoring.Measurements;
using Breas.Device.Vivo.V50;
using Breas.Device.Vivo.V50V7;
using Breas.Device.Vivo.V60;
using System.Collections.Generic;

namespace Breas.Device.Vivo
{
    /// <summary>
    /// Base class for Vivo50/Vivo60/Vivo50US measurements since most of them are shared.
    /// Each implementation of this class has the oppurtunity to provide it's own settings and alarms
    /// </summary>
    public abstract class VivoMeasurements : IDeviceMeasurements
    {
        public static readonly Dictionary<Product, VivoMeasurements> Measurements;

        static VivoMeasurements()
        {
            Measurements = new Dictionary<Product, VivoMeasurements>();
            Measurements.Add(VivoProducts.Vivo50, new Vivo50Measurements());
            Measurements.Add(VivoProducts.Vivo50V7, new Vivo50V7Measurements());
            Measurements.Add(VivoProducts.Vivo50US, new Vivo50USMeasurements());
            Measurements.Add(VivoProducts.Vivo60, new Vivo60Measurements());
        }

        #region Fields

        //empty measurements for v8 shared in vivo50/v7/us
        protected static DeviceMeasurement simvSupportPressure = new SettingsDeviceMeasurement(string.Empty, "Simv", Unit.None, -1);

        protected static DeviceMeasurement mviMeasurement = new MonitorDeviceMeasurement(string.Empty, Unit.Litre, 0.1, 0xFFFF);
        protected static DeviceMeasurement mveMeasurement = mviMeasurement;
        protected static DeviceMeasurement vtiMeasurement = new MonitorDeviceMeasurement(string.Empty, Unit.Millilitre, 1.0, 0xFFFF);
        protected static DeviceMeasurement vteMeasurement = vtiMeasurement;

        //make these static so theyre all shared between instances of UniversalMeasurements
        private static DeviceMeasurement triggerType = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementTriggerType, Unit.None);

        private static DeviceMeasurement powerSource = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementPowerSource, Unit.Percent);
        private static DeviceMeasurement systemState = new MonitorDeviceMeasurement(VivoDeviceKeys.SystemState, Unit.Percent);
        private static DeviceMeasurement alarmMute = new MonitorDeviceMeasurement(VivoDeviceKeys.AudioMute, Unit.None);

        private static DeviceMeasurement alarmMask0_31 = new MonitorDeviceMeasurement(VivoDeviceKeys.AlarmMask0_31, Unit.None, StorageType.Int);
        private static DeviceMeasurement alarmMask32_63 = new MonitorDeviceMeasurement(VivoDeviceKeys.AlarmMask32_63, Unit.None, StorageType.Int);
        private static DeviceMeasurement alarmMask64_95 = new MonitorDeviceMeasurement(VivoDeviceKeys.AlarmMask64_95, Unit.None, StorageType.Int);
        private static DeviceMeasurement alarmMask96_127 = new MonitorDeviceMeasurement(VivoDeviceKeys.AlarmMask96_127, Unit.None, StorageType.Int);
        private static DeviceMeasurement alarmMask128_159 = new MonitorDeviceMeasurement(VivoDeviceKeys.AlarmMask128_159, Unit.None, StorageType.Int);

        private static DeviceMeasurement internalBatteryStatus = new MonitorDeviceMeasurement(VivoDeviceKeys.IntBatStatus, Unit.None);
        private static DeviceMeasurement internalBatteryCapacity = new MonitorDeviceMeasurement(VivoDeviceKeys.IntBatCapacity, Unit.None);
        private static DeviceMeasurement clickOnBatteryStatus = new MonitorDeviceMeasurement(VivoDeviceKeys.ClickonBatStatus, Unit.None);
        private static DeviceMeasurement clickOnBatteryCapacity = new MonitorDeviceMeasurement(VivoDeviceKeys.ClickonBatCapacity, Unit.None);

        private static DeviceMeasurement measurementSpO2OnOff = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementSpO2OnOff, Unit.None);
        private static DeviceMeasurement measurementFiO2OnOff = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementFiO2OnOff, Unit.None);
        private static DeviceMeasurement measurementEtCO2OnOff = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementEtCO2OnOff, Unit.None);

        private static DeviceMeasurement measurementPMean = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementPMean, Unit.CmH2O, 0.1);
        private static DeviceMeasurement measurementPPeak = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementPPeak, Unit.CmH2O, 0.1);
        private static DeviceMeasurement measurementPeep = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementPeep, Unit.CmH2O, 0.1);
        private static DeviceMeasurement measurementLeakage = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementLeakage, Unit.LitrePerMinute, 1.0, 0xFF);
        private static DeviceMeasurement measurementMv = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementMV, Unit.Litre, .1, 0xFFFF);
        private static DeviceMeasurement measurementVt = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementVt, Unit.Millilitre, 1.0, 0xFFFF);
        private static DeviceMeasurement measurementFiO2 = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementFiO2, Unit.Percent, 1.0, 0xFFFF);
        private static DeviceMeasurement measurementPulse = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementPulse, Unit.Bpm, 1.0, 0xFFFF);
        private static DeviceMeasurement measurementIE = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementIE, Unit.None, MeasurementFormatters.FormatIe, StorageType.Short, 1.0, 0);
        private static DeviceMeasurement measurementInspTime = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementInspTime, Unit.Seconds, 0.1);
        private static DeviceMeasurement measurementRiseTime = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementRiseTime, Unit.Seconds, 0.1);
        private static DeviceMeasurement measurementDV = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementDV, Unit.Millilitre);
        private static DeviceMeasurement measurementPercentTV = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementPercentTV, Unit.Percent, 1.0, 0xFFFF);
        private static DeviceMeasurement measurementTotalRate = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementTotalRate, Unit.Bpm);
        private static DeviceMeasurement measurementSpontRate = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementSpontRate, Unit.Bpm, 1.0, 0xFF);
        private static DeviceMeasurement measurementPercentSpont = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementPercentSpont, Unit.Percent, 1.0, 0xFFFF);
        private static DeviceMeasurement measurementSpO2 = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementSpO2, Unit.Percent, 1.0, 0xFFFF);

        //common usage settings
        protected static DeviceMeasurement settingInspPress = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingInspPress, "STREAM_USAGE_SETTING_INSP_PRESS", Unit.CmH2O, .5, 0);

        protected static DeviceMeasurement settingPeep = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingPeep, "STREAM_SHORT_USAGE_SETTING_PEEP", Unit.CmH2O, .5, 1);
        protected static DeviceMeasurement settingInspTrigger = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingInspTrigger, "STREAM_USAGE_SETTING_INSP_TRIG", Unit.None, 2);
        protected static DeviceMeasurement settingExpTrigger = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingExpTrigger, "STREAM_BREATH_EXP_TRIGGER", Unit.None, 3);
        protected static DeviceMeasurement settingRiseTimeVol = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingRiseTimeVol, "STREAM_USAGE_SETTING_RISE_TIME_VOL", Unit.Percent, MeasurementFormatters.FormatOffableInteger, 4);
        protected static DeviceMeasurement settingRiseTimePress = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingRiseTimePress, "STREAM_USAGE_SETTING_RISE_TIME_PRESS", Unit.None, 5);
        protected static DeviceMeasurement settingTargetVolume = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingTargetVolume, "STREAM_USAGE_SETTING_TARGET_VOL", Unit.Millilitre, MeasurementFormatters.FormatOffableInteger, 10d, 7);
        protected static DeviceMeasurement settingInspPressMax = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingInspPressMax, "STREAM_USAGE_SETTING_INSP_PRESS_MAX", Unit.CmH2O, .5, 8);
        protected static DeviceMeasurement settingInspPressMin = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingInspPressMin, "STREAM_USAGE_SETTING_INSP_PRESS_MIN", Unit.CmH2O, .5, 9);
        protected static DeviceMeasurement settingBackupRate = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingBackupRate, "STREAM_USAGE_SETTING_BACKUP_RATE", Unit.Bpm, 10);
        protected static DeviceMeasurement settingInspTimeMin = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingInspTimeMin, "STREAM_USAGE_SETTING_INSP_TIME_MIN", Unit.Seconds, MeasurementFormatters.FormatOffableDecimal3_1, .1, 12);
        protected static DeviceMeasurement settingInspTimeMax = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingInspTimeMax, "STREAM_USAGE_SETTING_INSP_TIME_MAX", Unit.Seconds, MeasurementFormatters.FormatOffableDecimal3_1, .1, 13);
        protected static DeviceMeasurement settingInspTime = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingInspTime, "STREAM_USAGE_SETTING_INSP_TIME", Unit.Seconds, .1, 14);
        protected static DeviceMeasurement settingSetVol = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingSetVol, "STREAM_USAGE_SETTING_SET_VOL", Unit.Millilitre, 10, 15);
        protected static DeviceMeasurement settingTargetPress = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingTargetPress, "STREAM_USAGE_SETTING_TARGET_PRESS", Unit.CmH2O, .5, 16);
        protected static DeviceMeasurement settingFlowPattern = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingFlowPattern, "STREAM_USAGE_SETTING_FLOW_PATTERN", Unit.None, MeasurementFormatters.FormatFlowPattern, 17);
        protected static DeviceMeasurement settingCpap = new SettingsDeviceMeasurement(VivoDeviceKeys.SettingCpap, "STREAM_USAGE_SETTING_CPAP", Unit.CmH2O, .5, 18);

        //shared arrays for settings
        protected static DeviceMeasurement[] psvMeasurements = new DeviceMeasurement[]{
            settingInspPress,
            settingPeep,
            settingRiseTimePress,
            settingInspTrigger,
            settingExpTrigger,
            settingInspTimeMin,
            settingInspTimeMax,
            settingBackupRate,
            settingBackupRate, //supposed to be backupinsptime but theyre the same
            settingTargetVolume,
        };

        protected static DeviceMeasurement[] pcvMeasurements = new DeviceMeasurement[]{
            settingInspPress,
            settingPeep,
            settingBackupRate,
            settingInspTime,
            settingRiseTimePress
        };

        protected static DeviceMeasurement[] vcvMeasurements = new DeviceMeasurement[]{
            settingSetVol,
            settingPeep,
            settingBackupRate,
            settingInspTime,
            settingRiseTimeVol,
            settingInspTrigger,
            settingFlowPattern
        };

        protected static DeviceMeasurement[] cpapMeasurements = new DeviceMeasurement[]{
            settingCpap
        };

        protected static DeviceMeasurement[] targetMeasurements = new DeviceMeasurement[]{
            settingInspPressMax,
            settingInspPressMin
        };

        //TODO find out what a means. (Assisted?)
        protected static DeviceMeasurement[] aMeasurements = new DeviceMeasurement[]
        {
            settingInspTrigger,
            settingTargetVolume
        };

        #endregion Fields

        #region Alarm Mask Properties

        public virtual DeviceMeasurement AlarmMask0_31 { get { return VivoMeasurements.alarmMask0_31; } }

        public virtual DeviceMeasurement AlarmMask32_63 { get { return VivoMeasurements.alarmMask32_63; } }

        public virtual DeviceMeasurement AlarmMask64_95 { get { return VivoMeasurements.alarmMask64_95; } }

        public virtual DeviceMeasurement AlarmMask96_127 { get { return VivoMeasurements.alarmMask96_127; } }

        public virtual DeviceMeasurement AlarmMask128_159 { get { return VivoMeasurements.alarmMask128_159; } }

        #endregion Alarm Mask Properties

        #region Ventilator Status Properties

        /// <summary>
        /// Measurement for the inspiration trigger type. Can be overriden if the device
        /// implements a different key
        /// </summary>
        public virtual DeviceMeasurement TriggerType { get { return VivoMeasurements.triggerType; } }

        /// <summary>
        /// Measurement for the power source. Can be overriden if the device
        /// implements a different key
        /// </summary>
        public virtual DeviceMeasurement PowerSource { get { return VivoMeasurements.powerSource; } }

        /// <summary>
        /// Measurement for the system state. Can be overriden if the device
        /// implements a different key
        /// </summary>
        public virtual DeviceMeasurement SystemState { get { return VivoMeasurements.systemState; } }

        /// <summary>
        /// Measurement for checking if the alarm is muted. Can be overriden if the device
        /// implements a different key
        /// </summary>
        public virtual DeviceMeasurement AlarmMute { get { return VivoMeasurements.alarmMute; } }

        public virtual DeviceMeasurement SettingInspTrigger { get { return VivoMeasurements.settingInspTrigger; } }

        public virtual DeviceMeasurement SettingTargetVolume { get { return VivoMeasurements.settingTargetVolume; } }

        #endregion Ventilator Status Properties

        #region Device Settings Properties

        public abstract DeviceMeasurement VentMode { get; }

        public abstract DeviceMeasurement BreathMode { get; }

        public abstract DeviceMeasurement PatientMode { get; }

        public abstract DeviceMeasurement DeviceMode { get; }

        public abstract DeviceMeasurement SelectedProfile { get; }

        public abstract DeviceMeasurement AdjustmentInHome { get; }

        public abstract DeviceMeasurement CircuitRecognition { get; }

        public abstract DeviceMeasurement CurrentCircuit { get; }

        public abstract DeviceMeasurement SimvSupportPressure { get; }

        public abstract DeviceMeasurement AlarmSoundLevel { get; }

        #endregion Device Settings Properties

        #region Universal Monitor Measurements

        public virtual DeviceMeasurement InternalBatteryStatus { get { return VivoMeasurements.internalBatteryStatus; } }

        public virtual DeviceMeasurement InternalBatteryCapacity { get { return VivoMeasurements.internalBatteryCapacity; } }

        public virtual DeviceMeasurement ClickOnBatteryStatus { get { return VivoMeasurements.clickOnBatteryStatus; } }

        public virtual DeviceMeasurement ClickOnBatteryCapacity { get { return VivoMeasurements.clickOnBatteryCapacity; } }

        public virtual DeviceMeasurement MeasurementSpO2OnOff { get { return VivoMeasurements.measurementSpO2OnOff; } }

        public virtual DeviceMeasurement MeasurementFiO2OnOff { get { return VivoMeasurements.measurementFiO2OnOff; } }

        public virtual DeviceMeasurement MeasurementEtCO2OnOff { get { return VivoMeasurements.measurementEtCO2OnOff; } }

        public virtual DeviceMeasurement MeasurementPMean { get { return VivoMeasurements.measurementPMean; } }

        public virtual DeviceMeasurement MeasurementPPeak { get { return VivoMeasurements.measurementPPeak; } }

        public virtual DeviceMeasurement MeasurementPeep { get { return VivoMeasurements.measurementPeep; } }

        public virtual DeviceMeasurement MeasurementLeakage { get { return VivoMeasurements.measurementLeakage; } }

        //V8 should override this!
        public virtual DeviceMeasurement MeasurementMv { get { return VivoMeasurements.measurementMv; } }

        //V8 should override this!
        public virtual DeviceMeasurement MeasurementVt { get { return VivoMeasurements.measurementVt; } }

        public virtual DeviceMeasurement MeasurementFiO2 { get { return VivoMeasurements.measurementFiO2; } }

        public virtual DeviceMeasurement MeasurementPulse { get { return VivoMeasurements.measurementPulse; } }

        public virtual DeviceMeasurement MeasurementIE { get { return VivoMeasurements.measurementIE; } }

        public virtual DeviceMeasurement MeasurementInspTime { get { return VivoMeasurements.measurementInspTime; } }

        public virtual DeviceMeasurement MeasurementRiseTime { get { return VivoMeasurements.measurementRiseTime; } }

        public virtual DeviceMeasurement MeasurementDV { get { return VivoMeasurements.measurementDV; } }

        public virtual DeviceMeasurement MeasurementPercentTV { get { return VivoMeasurements.measurementPercentTV; } }

        public virtual DeviceMeasurement MeasurementTotalRate { get { return VivoMeasurements.measurementTotalRate; } }

        public virtual DeviceMeasurement MeasurementSpontRate { get { return VivoMeasurements.measurementSpontRate; } }

        public virtual DeviceMeasurement MeasurementPercentSpont { get { return VivoMeasurements.measurementPercentSpont; } }

        public virtual DeviceMeasurement MeasurementSpO2 { get { return VivoMeasurements.measurementSpO2; } }

        public virtual DeviceMeasurement CO2Unit { get { return null; } }
        public virtual DeviceMeasurement PressureUnit { get { return null; } }

        #endregion Universal Monitor Measurements

        #region Specific Monitor Measurements

        public abstract DeviceMeasurement MeasurementEtCO2 { get; }

        public abstract DeviceMeasurement MeasurementInspCO2 { get; }

        public abstract DeviceMeasurement MeasurementCO2AtmPress { get; }

        public abstract DeviceMeasurement MeasurementMvI { get; }

        public abstract DeviceMeasurement MeasurementMve { get; }

        public abstract DeviceMeasurement MeasurementVti { get; }

        public abstract DeviceMeasurement MeasurementVte { get; }

        #endregion Specific Monitor Measurements

        #region Alarm Settings Measurements

        //These measurements are needed across all devices because of the volume and pressure gauges
        public abstract DeviceMeasurement SettingPressHighAlarm { get; }

        public abstract DeviceMeasurement SettingPressLowAlarm { get; }

        public abstract DeviceMeasurement SettingTidalVolHighAlarm { get; }

        public abstract DeviceMeasurement SettingTidalVolLowAlarm { get; }
    

        #endregion Alarm Settings Measurements

        /// <summary>
        /// Gets a list of the usage settings for the opmode.
        /// </summary>
        /// <param name="opmode">The opmode to get the settings for</param>
        /// <returns>
        /// A list of measurements for the opmode. They are just rendered as a list so the measurements returned
        /// don't need any properties to expose them
        /// </returns>
        public abstract List<DeviceMeasurement> GetUsageSettingMeasurements(OpMode opmode);

        public abstract List<DeviceMeasurement> GetAlarmSettingMeasurements(OpMode opmode);
    }
}