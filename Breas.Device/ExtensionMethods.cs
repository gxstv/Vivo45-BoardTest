using Breas.Device.Monitoring.Alarms;
using Breas.Device.Monitoring.Measurements;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Breas.Device
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Method for getting a value from a dictionary even if the key isn't present
        /// </summary>
        /// <returns>null if the key isn't present, or the value if it is</returns>
        public static TValue GetValueSafe<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            return defaultValue;
        }

        public static void AddRange<TValue>(this ObservableCollection<TValue> collection, IEnumerable<TValue> other)
        {
            foreach (TValue val in other)
            {
                collection.Add(val);
            }
        }

        public static DeviceMeasurement[] Extend(this DeviceMeasurement[] measurements, params DeviceMeasurement[] additionalMeasurements)
        {
            List<DeviceMeasurement> measurementList = new List<DeviceMeasurement>(measurements.Length + additionalMeasurements.Length);
            measurementList.AddRange(measurements);
            measurementList.AddRange(additionalMeasurements);
            return measurementList.ToArray();
        }

        public static void Add(this Alarm[] alarms, Alarm alarm)
        {
            alarms[alarm.Id] = alarm;
        }

        public static byte[] Add(this byte[] bytes, ref int offset, byte b)
        {
            bytes[offset++] = b;
            return bytes;
        }

        public static byte[] Add(this byte[] bytes, ref int offset, byte[] addedBytes)
        {
            for (int i = 0; i < addedBytes.Length; i++)
                bytes[offset++] = addedBytes[i];
            return bytes;
        }
    }
}