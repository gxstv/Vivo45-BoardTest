using Breas.Device.Monitoring;
using Breas.Device.Monitoring.Measurements;
using Breas.Device.Vivo.V50V7;
using System.Collections.Generic;
using System.Linq;

namespace Breas.Device.Vivo.V60
{
    /// <summary>
    /// Measurements for the Vivo60. A lot of the measurements are shared with V7 + simv support pressure so we inherit from the US version
    /// </summary>
    public class Vivo60Measurements : Vivo50USMeasurements
    {
        protected static DeviceMeasurement settingMinuteVolHighInspAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingMinuteVolHighInspAlarm, "STREAM_USAGE_SETTING_ALARM_MINUTE_VOL_HIGH_INSP_V8", Unit.Litre, MeasurementFormatters.FormatOffableDecimal3_1, StorageType.UShort, .1, 29);
        protected static DeviceMeasurement settingMinuteVolHighExpAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingMinuteVolHighExpAlarm, "STREAM_USAGE_SETTING_ALARM_MINUTE_VOL_HIGH_EXP_V8", Unit.Litre, MeasurementFormatters.FormatOffableDecimal3_1, StorageType.UShort, .1, 33);
        protected static DeviceMeasurement settingMinuteVolLowInspAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingMinuteVolLowInspAlarm, "STREAM_USAGE_SETTING_ALARM_MINUTE_VOL_LOW_INSP_V8", Unit.Litre, MeasurementFormatters.FormatOffableDecimal3_1, StorageType.UShort, .1, 30);
        protected static DeviceMeasurement settingMinuteVolLowExpAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingMinuteVolLowExpAlarm, "STREAM_USAGE_SETTING_ALARM_MINUTE_VOL_LOW_EXP_V8", Unit.Litre, MeasurementFormatters.FormatOffableDecimal3_1, StorageType.UShort, .1, 34);
        protected static DeviceMeasurement settingTidalVolHighInspAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingTidalVolHighInspAlarm, "STREAM_USAGE_SETTING_ALARM_TIDAL_VOL_HIGH_INSP_V8", Unit.Millilitre, MeasurementFormatters.FormatOffableInteger, StorageType.UShort, 10, 27);
        protected static DeviceMeasurement settingTidalVolHighExpAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingTidalVolHighExpAlarm, "STREAM_USAGE_SETTING_ALARM_TIDAL_VOL_HIGH_EXP_V8", Unit.Millilitre, MeasurementFormatters.FormatOffableInteger, StorageType.UShort, 10, 31);
        protected static DeviceMeasurement settingTidalVolLowInspAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingTidalVolLowInspAlarm, "STREAM_USAGE_SETTING_ALARM_TIDAL_VOL_LOW_INSP_V8", Unit.Millilitre, MeasurementFormatters.FormatOffableInteger, StorageType.UShort, 10, 28);
        protected static DeviceMeasurement settingTidalVolLowExpAlarm = new AlarmSettingsDeviceMeasurement(VivoDeviceKeys.SettingTidalVolLowExpAlarm, "STREAM_USAGE_SETTING_ALARM_TIDAL_VOL_LOW_EXP_V8", Unit.Millilitre, MeasurementFormatters.FormatOffableInteger, StorageType.UShort, 10, 32);

        protected static DeviceMeasurement mviMeasurementv8 = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementMvI, Unit.Litre, 0.1, 0xFFFF);
        protected static DeviceMeasurement mveMeasurementv8 = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementMvE, Unit.Litre, 0.1, 0xFFFF);
        protected static DeviceMeasurement vtiMeasurementv8 = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementVtI, Unit.Millilitre, 1.0, 0xFFFF);
        protected static DeviceMeasurement vteMeasurementv8 = new MonitorDeviceMeasurement(VivoDeviceKeys.MeasurementVtE, Unit.Millilitre, 1.0, 0xFFFF);

        public override DeviceMeasurement MeasurementMvI
        {
            get { return Vivo60Measurements.mviMeasurementv8; }
        }

        public override DeviceMeasurement MeasurementMve
        {
            get { return Vivo60Measurements.mveMeasurementv8; }
        }

        public override DeviceMeasurement MeasurementVti
        {
            get { return Vivo60Measurements.vtiMeasurementv8; }
        }

        public override DeviceMeasurement MeasurementVte
        {
            get { return Vivo60Measurements.vteMeasurementv8; }
        }

        public override DeviceMeasurement SettingTidalVolHighAlarm
        {
            get
            {
                return Vivo60Measurements.settingTidalVolHighInspAlarm;
            }
        }

        public override DeviceMeasurement SettingTidalVolLowAlarm
        {
            get
            {
                return Vivo60Measurements.settingTidalVolLowInspAlarm;
            }
        }

        protected static DeviceMeasurement[] alarmSettingsv60;

        static Vivo60Measurements()
        {
            alarmSettingsv60 = new DeviceMeasurement[]
            {
                Vivo50V7Measurements.settingPressHighAlarm,
                Vivo50V7Measurements.settingPressLowAlarm,

                Vivo60Measurements.settingTidalVolHighInspAlarm,
                Vivo60Measurements.settingTidalVolHighExpAlarm,
                Vivo60Measurements.settingTidalVolLowInspAlarm,
                Vivo60Measurements.settingTidalVolLowExpAlarm,

                Vivo50V7Measurements.settingBreathRateHighAlarm,
                Vivo50V7Measurements.settingBreathRateLowAlarm,

                Vivo60Measurements.settingMinuteVolHighInspAlarm,
                Vivo60Measurements.settingMinuteVolHighExpAlarm,
                Vivo60Measurements.settingMinuteVolLowInspAlarm,
                Vivo60Measurements.settingMinuteVolLowExpAlarm,

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

        public override List<DeviceMeasurement> GetAlarmSettingMeasurements(OpMode opmode)
        {
            return alarmSettingsv60.ToList();
        }
    }
}