using Breas.Device.Monitoring.Alarms;
using Breas.Device.Monitoring.Measurements;
using Breas.Device.Vivo.V50;
using Breas.Device.Vivo.V50V7;
using Breas.Device.Vivo.V60;
using System.Collections;
using System.Collections.Generic;

namespace Breas.Device.Vivo
{
    public class VivoAlarmMonitor : IAlarmMonitor
    {
        private DeviceMeasurement[] alarmMeasurements = new DeviceMeasurement[5];

        public event AlarmActivated AlarmActivated;

        public event AlarmDeactivated AlarmDeactivated;

        private Product product;

        private Alarm[] alarms;

        private BitArray currentBits;

        public VivoAlarmMonitor(Product product,
                                DeviceMeasurement alarmMask0_31,
                                DeviceMeasurement alarmMask32_63,
                                DeviceMeasurement alarmMask64_95,
                                DeviceMeasurement alarmMask96_127,
                                DeviceMeasurement alarmMask128_159)
        {
            this.product = product;
            this.alarms = VivoAlarms.Alarms[product];
            this.alarmMeasurements[0] = alarmMask0_31;
            this.alarmMeasurements[1] = alarmMask32_63;
            this.alarmMeasurements[2] = alarmMask64_95;
            this.alarmMeasurements[3] = alarmMask96_127;
            this.alarmMeasurements[4] = alarmMask128_159;
            int bitIndex = 0;
            //we could just hardcode the bitarrays size, but this way
            //is more maintainable
            for (int i = 0; i < alarmMeasurements.Length; i++)
            {
                bitIndex += alarmMeasurements[i].StorageType.size * 8;
            }
            currentBits = new BitArray(bitIndex);
        }

        public void Update(IDictionary<DeviceMeasurement, int> measurementValues)
        {
            int bitIndex = 0;
            for (int i = 0; i < alarmMeasurements.Length; i++)
            {
                var measurement = alarmMeasurements[i];
                int measurementVal = measurementValues[measurement];
                for (int a = 0; a < measurement.StorageType.size * 8; a++)
                {
                    bitIndex = i * 32 + a;
                    if (bitIndex == 0)
                        continue; //first alarm bit is not an alarm
                    bool active = (measurementVal & (1 << a)) != 0;
                    if (currentBits.Get(bitIndex) != active)
                    {
                        RaiseEvent(bitIndex, active);
                        currentBits.Set(bitIndex, active);
                    }
                }
            }
        }

        private void RaiseEvent(int bitIndex, bool active)
        {
            Alarm alarm;
            if (bitIndex < 0 || bitIndex >= alarms.Length || alarms[bitIndex] == null)
                alarm = Alarm.UnknownAlarm;
            else
                alarm = alarms[bitIndex];
            if (active)
                RaiseAlarmActivated(new AlarmActivatedEventArgs(alarm));
            else
                RaiseAlarmDeactived(new AlarmDeactivatedEventArgs(alarm));
        }

        private void RaiseAlarmDeactived(AlarmDeactivatedEventArgs alarmDeactivatedEventArgs)
        {
            if (AlarmDeactivated != null)
                AlarmDeactivated(this, alarmDeactivatedEventArgs);
        }

        private void RaiseAlarmActivated(AlarmActivatedEventArgs alarmActivatedEventArgs)
        {
            if (AlarmActivated != null)
                AlarmActivated(this, alarmActivatedEventArgs);
        }

        public void DumpMeasurements(List<DeviceMeasurement> measurements)
        {
            foreach (var m in alarmMeasurements)
                measurements.Add(m);
        }
    }

    internal static class VivoAlarms
    {
        public static readonly Dictionary<Product, Alarm[]> Alarms = new Dictionary<Product, Alarm[]>();

        static VivoAlarms()
        {
            Alarms[VivoProducts.Vivo50] = Vivo50Alarms.Alarms;
            Alarms[VivoProducts.Vivo50V7] = Vivo50V7Alarms.Alarms;
            Alarms[VivoProducts.Vivo50US] = Vivo50V7Alarms.Alarms;
            Alarms[VivoProducts.Vivo60] = Vivo60Alarms.Alarms;
        }
    }
}