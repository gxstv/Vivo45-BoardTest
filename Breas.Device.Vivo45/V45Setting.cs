using Breas.Device.Monitoring.Measurements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breas.Device.Vivo45
{
    /// <summary>
    /// V45 does settings normally(ie, theyre not measurepoints).
    ///
    /// The entire DeviceMeasurement system was written before this,
    /// so to use the settings in the measurement system so we can share the recording
    /// code, we just wrap settings in a devicemeasurement and handle reading from
    /// the device in the V45Device class.
    /// </summary>
    public class V45Setting : SettingsDeviceMeasurement
    {
        /// <summary>
        /// We generate fake measurementkeys by doing
        /// 5000 + settingId
        /// </summary>
        public const ushort MeasurementKeyOffset = 5000;

        public ushort DeviceSettingId { get; private set; }

        public TreatmentSettingType TreatmentSettingType { get; set; }
        

        public V45Setting(TreatmentSettingType type, string nameKey, ushort settingId, Unit? unit, Func<double, string> formatter, double scale, int invalidValue)
            : base((ushort) (MeasurementKeyOffset + (settingId * 3) + (ushort)type), nameKey, unit, formatter, null, scale, invalidValue)
        {
            this.DeviceSettingId = settingId;
            this.TreatmentSettingType = type;
        }

        public V45Setting(ushort settingId, string nameKey, Unit? unit, Func<double, string> formatter, double scale, int invalidValue)
            : this(TreatmentSettingType.Value, nameKey, settingId, unit, formatter, scale, invalidValue)
        {
        }

        public V45Setting(TreatmentSettingType type, string nameKey, TSettingsId settingId, Unit? unit, Func<double, string> formatter)
            : this(type, nameKey, (ushort)settingId, unit, formatter, 1.0, int.MinValue)
        {
        }

        public V45Setting(TSettingsId settingId, string nameKey, Unit? unit, Func<double, string> formatter, double scale, int invalidValue)
            : this((ushort)settingId, nameKey, unit, formatter, scale, invalidValue)
        {
        }

        public V45Setting(TSettingsId settingId, string nameKey, Unit? unit, Func<double, string> formatter)
            : this((ushort)settingId, nameKey, unit, formatter, 1.0, int.MinValue)
        {
        }

        public V45Setting(ushort settingId, string nameKey, Unit? unit)
            : this(settingId, nameKey, unit, null, 1.0, int.MaxValue)
        {
        }

        public V45Setting(TSettingsId settingId, string nameKey, Unit? unit)
            : this((ushort)settingId, nameKey, unit)
        {
        }
    }
}