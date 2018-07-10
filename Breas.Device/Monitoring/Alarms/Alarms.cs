namespace Breas.Device.Monitoring.Alarms
{
    public class Alarm
    {
        public static Alarm UnknownAlarm = new Alarm(-1, "UNKNOWN", AlarmType.None);

        public Alarm ()
        {

        }

        public Alarm(int id, string nameKey, AlarmType type)
        {
            this.Id = id;
            this.NameKey = nameKey;
            this.Type = type;
        }

        public int Id { get; set; }

        public string NameKey { get; set; }

        public AlarmType Type { get; set; }
    }

    public enum AlarmType
    {
        None,
        Info,
        Low,
        Medium,
        High,
        FunctionFail
    }
}