using Breas.Device.Monitoring.Alarms;
using Breas.Device.Vivo.V50V7;

namespace Breas.Device.Vivo.V60
{
    public class Vivo60Alarms
    {
        public static readonly Alarm[] Alarms;

        static Vivo60Alarms()
        {
            Alarms = (Alarm[])Vivo50V7Alarms.Alarms.Clone();

            Alarms.Add(new Alarm(67, "ALARM_TXT_LOW_MINUTE_VOLUME_INSP", AlarmType.High));
            Alarms.Add(new Alarm(68, "ALARM_TXT_LOW_TIDAL_VOLUME_INSP", AlarmType.High));

            Alarms.Add(new Alarm(84, "ALARM_TXT_LOW_MINUTE_VOLUME_EXP", AlarmType.High));
            Alarms.Add(new Alarm(85, "ALARM_TXT_LOW_TIDAL_VOLUME_EXP", AlarmType.High));
            Alarms.Add(new Alarm(86, "ALARM_TXT_EXH_VALVE_CTRL_ERROR", AlarmType.High));

            Alarms.Add(new Alarm(96, "ALARM_TXT_HIGH_MINUTE_VOLUME_INSP", AlarmType.Medium));
            Alarms.Add(new Alarm(97, "ALARM_TXT_HIGH_TIDAL_VOLUME_INSP", AlarmType.Medium));

            Alarms.Add(new Alarm(112, "ALARM_TXT_HIGH_MINUTE_VOLUME_EXP", AlarmType.Medium));
            Alarms.Add(new Alarm(113, "ALARM_TXT_HIGH_TIDAL_VOLUME_EXP", AlarmType.Medium));
            Alarms.Add(new Alarm(114, "ALARM_TXT_CIRCUIT_TYPE_ERROR", AlarmType.Medium));
            Alarms.Add(new Alarm(115, "ALARM_TXT_HUMIDITY", AlarmType.Medium));
            Alarms.Add(new Alarm(116, "ALARM_TXT_AMBIENT_AIR_TEMP", AlarmType.Medium));
            Alarms.Add(new Alarm(117, "ALARM_TXT_MODE_INSERT_MISMATCH", AlarmType.Medium));
        }
    }
}