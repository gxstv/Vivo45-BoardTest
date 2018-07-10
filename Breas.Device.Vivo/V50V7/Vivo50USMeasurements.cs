using System.Collections.Generic;
using Breas.Device.Monitoring;
using Breas.Device.Monitoring.Measurements;

namespace Breas.Device.Vivo.V50V7
{
    /// <summary>
    /// Implementation of the Vivo50 US. It uses V7, but adds support for Simv Support Pressure
    /// </summary>
    public class Vivo50USMeasurements : Vivo50V7Measurements
    {
        protected static DeviceMeasurement _simvSupportPressureMeasurement = new SettingsDeviceMeasurement("T SIMV Supp Press", "STREAM_USAGE_SETTING_SIMV_SUPPORT_PRESSURE_V8", Unit.CmH2O, .5, 22);

        public override DeviceMeasurement SimvSupportPressure
        {
            get { return _simvSupportPressureMeasurement; }
        }

        protected static DeviceMeasurement[] pcvSimVV8 = new DeviceMeasurement[]
        {
            Vivo50V7Measurements.settingInspPress,
            Vivo50V7Measurements.settingPeep,
            Vivo50V7Measurements.settingBackupRate,
            Vivo50V7Measurements.settingInspTime,
            Vivo50V7Measurements.settingRiseTimePress,
            Vivo50V7Measurements.settingInspTrigger,
            Vivo50USMeasurements._simvSupportPressureMeasurement,
            Vivo50V7Measurements.settingExpTrigger,
            Vivo50V7Measurements.settingExpTrigger,
            Vivo50V7Measurements.alarmSoundLevel,
        };

        protected static DeviceMeasurement[] vcvSimVV8 = new DeviceMeasurement[]
        {
            Vivo50V7Measurements.settingSetVol,
            Vivo50V7Measurements.settingPeep,
            Vivo50V7Measurements.settingBackupRate,
            Vivo50V7Measurements.settingInspTime,
            Vivo50V7Measurements.settingRiseTimeVol,
            Vivo50V7Measurements.settingInspTrigger,
            Vivo50V7Measurements.settingFlowPattern,
            Vivo50USMeasurements._simvSupportPressureMeasurement,
            Vivo50V7Measurements.settingRiseTimePress,
            Vivo50V7Measurements.settingExpTrigger,
            Vivo50V7Measurements.alarmSoundLevel,
        };

        public override List<DeviceMeasurement> GetUsageSettingMeasurements(OpMode opmode)
        {
            if (opmode == OpMode.PCV_SIMV)
            {
                return new List<DeviceMeasurement>(pcvSimVV8);
            }
            else if (opmode == OpMode.VCV_SIMV)
            {
                return new List<DeviceMeasurement>(vcvSimVV8);
            }
            else {
                return base.GetUsageSettingMeasurements(opmode);
            }
        }
    }
}