using System;
using System.Collections.Generic;

namespace Breas.Device.Monitoring.Measurements
{
    public struct StorageType
    {
        public static readonly StorageType None = new StorageType(0, false);
        public static readonly StorageType UByte = new StorageType(1, false);
        public static readonly StorageType Byte = new StorageType(1, true);
        public static readonly StorageType UShort = new StorageType(2, false);
        public static readonly StorageType Short = new StorageType(2, true);
        public static readonly StorageType Int = new StorageType(4, true);
        public static readonly StorageType Long = new StorageType(8, true);

        public static readonly StorageType Default = Short;

        public bool signed;
        public int size;

        public StorageType(int size, bool signed)
        {
            this.size = size;
            this.signed = signed;
        }
    }

    public class DeviceMeasurement : IEquatable<DeviceMeasurement>
    {
        public static readonly DeviceMeasurement EmptyMeasurement = new DeviceMeasurement(string.Empty, Unit.None);

        public Func<double, string> Formatter { get; set; }

        public object Key { get; set; }

        public Unit Unit { get; set; }

        public int InvalidValue { get; set; }

        public StorageType StorageType { get; set; }

        public double Scale { get; set; }

        public DeviceMeasurement(object key, Measurements.Unit? unit, Func<double, string> formatter, StorageType? type, double scale, int invalidValue)
        {
            this.Key = key == null ? string.Empty : key;
            this.Unit = unit == null ? Unit.None : unit.Value;
            this.StorageType = type == null ? StorageType.Default : type.Value;
            this.InvalidValue = invalidValue;
            this.Scale = scale;
            if (formatter == null)
            {
                // automatically default the formatter to integer/decimal/double decimal depending on the scale
                double fraction = scale - (int)scale;
                if (fraction == 0)
                {
                    formatter = MeasurementFormatters.FormatInteger4;
                }
                else if (fraction * 10 - (int)(fraction * 10) == 0)
                {
                    formatter = MeasurementFormatters.FormatDecimal3_1;
                }
                else
                {
                    formatter = MeasurementFormatters.FormatDoubleDecimal3_2;
                }
            }
            Formatter = formatter;
        }

        public DeviceMeasurement(object key, Measurements.Unit? unit, double scale, int invalidValue)
            : this(key, unit, null, StorageType.Default, scale, invalidValue)
        {
        }

        public DeviceMeasurement(object key, Measurements.Unit? unit, double scale)
            : this(key, unit, null, StorageType.Default, scale, int.MinValue)
        {
        }

        public DeviceMeasurement(object key, Measurements.Unit? unit)
            : this(key, unit, 1.0, int.MinValue)
        {
        }

        public virtual double ScaleValue(int rawValue)
        {
            //if this is an empty measurement return the invalid value so the UI just
            //disables the field
            if (Key.ToString() == string.Empty)
                return InvalidValue;
            return rawValue * Scale;
        }

        public virtual int ReverseScale(double value)
        {
            return (int) (value / Scale);
        }

        public virtual string FormatValue(int rawValue)
        {
            return Formatter(ScaleValue(rawValue));
        }

        public bool Equals(DeviceMeasurement other)
        {
            return other.Key == Key;
        }
    }

    public class MonitorDeviceMeasurement : DeviceMeasurement
    {
        public MonitorDeviceMeasurement(object key, Measurements.Unit? unit, Func<double, string> formatter, StorageType type, double scale, int invalidValue)
            : base(key, unit, formatter, type, scale, invalidValue)
        {
        }

        public MonitorDeviceMeasurement(object key, Measurements.Unit? unit, StorageType type, double scale, int invalidValue)
            : this(key, unit, null, type, scale, invalidValue)
        {
        }

        public MonitorDeviceMeasurement(object key, Measurements.Unit? unit, double scale, int invalidValue)
            : this(key, unit, null, StorageType.Short, scale, invalidValue)
        {
        }

        public MonitorDeviceMeasurement(object key, Measurements.Unit? unit, StorageType type)
            : this(key, unit, type, 1.0, int.MinValue)
        {
        }

        public MonitorDeviceMeasurement(object key, Measurements.Unit? unit, double scale)
            : this(key, unit, scale, int.MinValue)
        {
        }

        public MonitorDeviceMeasurement(object key, Measurements.Unit? unit)
            : this(key, unit, 1.0, int.MinValue)
        {
        }
    }

    public class SettingsDeviceMeasurement : DeviceMeasurement
    {
        private int _settingId;

        public int SettingId { get { return _settingId; } }

        public string NameKey { get; private set; }

        public SettingsDeviceMeasurement(object key, string nameKey, Unit? unit, Func<double, string> formatter, StorageType? type, double scale, int settingId)
            : base(key, unit, formatter, type, scale, int.MinValue)
        {
            this._settingId = settingId;
            this.NameKey = nameKey;
        }

        public SettingsDeviceMeasurement(object key, string nameKey, Unit? unit, Func<double, string> formatter, double scale, int settingId)
            : this(key, nameKey, unit, formatter, null, scale, settingId)
        {
        }

        public SettingsDeviceMeasurement(object key, string nameKey, Unit? unit, Func<double, string> formatter, int settingId)
            : this(key, nameKey, unit, formatter, 1.0, settingId)
        {
        }

        public SettingsDeviceMeasurement(object key, string nameKey, Unit? unit, double scale, int settingId)
            : this(key, nameKey, unit, null, scale, settingId)
        {
        }

        public SettingsDeviceMeasurement(object key, string nameKey, Unit? unit, int settingId)
            : this(key, nameKey, unit, 1.0, settingId)
        {
        }
    }

    public class AlarmSettingsDeviceMeasurement : SettingsDeviceMeasurement
    {
        public AlarmSettingsDeviceMeasurement(object key, string nameKey, Unit? unit, Func<double, string> formatter, StorageType? type, double scale, int settingsId)
            : base(key, nameKey, unit, formatter, type, scale, settingsId)
        {
        }

        public AlarmSettingsDeviceMeasurement(object key, string nameKey, Unit? unit, Func<double, string> formatter, double scale, int settingsId)
            : this(key, nameKey, unit, formatter, null, scale, settingsId)
        {
        }

        public AlarmSettingsDeviceMeasurement(object key, string nameKey, Unit? unit, double scale, int settingsId)
            : this(key, nameKey, unit, null, scale, settingsId)
        {
        }

        public AlarmSettingsDeviceMeasurement(object key, string nameKey, Unit? unit, int settingsId)
            : this(key, nameKey, unit, 1.0, settingsId)
        {
        }
    }

    /// <summary>
    /// Provides data on a measurement that has multiple units(CO2)
    /// </summary>
    public class MultiUnitDeviceMeasurement : DeviceMeasurement
    {
        public DeviceMeasurement UnitMeasurement { get; set; }

        public IDictionary<int, DeviceMeasurement> UnitMeasurementMap { get; set; }

        public MultiUnitDeviceMeasurement(DeviceMeasurement unitMeasurement,
                                          IDictionary<int, DeviceMeasurement> unitMeasurementMap)
            : base(string.Empty, null)
        {
            this.UnitMeasurement = unitMeasurement;
            this.UnitMeasurementMap = unitMeasurementMap;
        }
    }
}