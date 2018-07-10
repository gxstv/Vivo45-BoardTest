using Breas.Device.Monitoring.Measurements;

namespace Breas.Device.Monitoring.Alarms
{
    public delegate void AlarmActivated(object sender, AlarmActivatedEventArgs args);

    public delegate void AlarmDeactivated(object sender, AlarmDeactivatedEventArgs args);

    /// <summary>
    /// Alarm monitors just implement the IUIMeasurement so they can be used in the
    /// measurement system.
    /// </summary>
    public interface IAlarmMonitor : IUIMeasurement
    {
        event AlarmActivated AlarmActivated;

        event AlarmDeactivated AlarmDeactivated;
    }

    public class AlarmActivatedEventArgs
    {
        public Alarm Alarm { get; set; }

        public AlarmActivatedEventArgs(Alarm alarm)
        {
            this.Alarm = alarm;
        }
    }

    public class AlarmDeactivatedEventArgs
    {
        public Alarm Alarm { get; set; }

        public AlarmDeactivatedEventArgs(Alarm alarm)
        {
            this.Alarm = alarm;
        }
    }
}