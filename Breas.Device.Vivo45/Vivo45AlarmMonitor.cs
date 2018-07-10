using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breas.Device.Monitoring.Alarms;
using Breas.Device.Vivo45.Messenger;
using Breas.Device.Monitoring.Measurements;

namespace Breas.Device.Vivo45
{
    class Vivo45AlarmMonitor : IAlarmMonitor
    {
        private Vivo45Messenger _Vivo45Messenger;

        public event AlarmActivated AlarmActivated;

        public event AlarmDeactivated AlarmDeactivated;

        private List<Vivo45Alarm> ActiveAlarms;
        private bool updateNextFrame;

        public Vivo45AlarmMonitor(Vivo45Messenger V45)
        {
            _Vivo45Messenger = V45;
            _Vivo45Messenger.NewAlarmsAvailableNow += OnAlarmActivated;

            ActiveAlarms = new List<Vivo45Alarm>();
            //Update();
        }

        private void OnAlarmActivated()
        {
            updateNextFrame = true;
        }

        public void Update()
        {
            List<Vivo45Alarm> alarms;

            alarms = _Vivo45Messenger.GetActiveAlarms(AlarmType.FunctionFail);
            alarms.AddRange(  _Vivo45Messenger.GetActiveAlarms(AlarmType.High).ToArray());
            alarms.AddRange(_Vivo45Messenger.GetActiveAlarms(AlarmType.Medium).ToArray());

            var newalarms = alarms.Except(ActiveAlarms, new Vivo45AlarmComparer()).ToList();
            foreach (Vivo45Alarm alarm in newalarms)
            {
                RaiseAlarmActivated(new AlarmActivatedEventArgs(new Alarm(alarm.Id, alarm.ExplanatoryText, (AlarmType)alarm.Type)));
            }

            var removedalarms = ActiveAlarms.Except(alarms, new Vivo45AlarmComparer()).ToList();
            foreach (Vivo45Alarm alarm in removedalarms)
            {
                RaiseAlarmDeactived(new AlarmDeactivatedEventArgs(new Alarm(alarm.Id, alarm.ExplanatoryText,(AlarmType)alarm.Type)));
            }

            ActiveAlarms = alarms;
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

        public void Update(IDictionary<DeviceMeasurement, int> measurementValues)
        {
            //throw new NotImplementedException();
            if (updateNextFrame)
            {
                Update();
                updateNextFrame = false;
            }
        }

        public void DumpMeasurements(List<DeviceMeasurement> measurements)
        {
           // throw new NotImplementedException();
        }
    }
}
