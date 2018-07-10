using Breas.Device.DeviceClass;
using Breas.Device.Monitoring;
using Breas.Device.Monitoring.Alarms;
using Breas.Device.Monitoring.Measurements;
using System.Collections.Generic;

namespace Breas.Device.DeviceTypes
{
    /// <summary>
    /// Provides functionality for a device that is able to be monitored
    /// </summary>
    public interface IMonitorDevice : IDeviceClass
    {
        Dictionary<object, MeasurePointDefinition> MeasurePointDefinitions { get; }

        /// <summary>
        /// Capture Handler for this device
        /// </summary>
        ICaptureHandler CaptureHandler { get; }

        /// <summary>
        /// Contains all of the devicemeasurements available for this device
        /// </summary>
        IDeviceMeasurements Measurements { get; }

        /// <summary>
        /// The alarm monitor for this device
        /// </summary>
        IAlarmMonitor AlarmMonitor { get; }

        bool Monitoring { get; set; }

        bool StartMonitoring();

        bool StopMonitoring();

        int getshift();

        Dictionary<object, MeasurePointDefinition> MapMeasurepointDefinitions(Dictionary<object, MeasurePointDefinition> definitions = null);

        bool GetMeasurementPointValues(Dictionary<DeviceMeasurement, int> measurementValues);

        byte[] GetCaptureData();

        ICaptureData CreateEmptyCaptureData();
    }
}