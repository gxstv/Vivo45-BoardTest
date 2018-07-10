using System.Collections.Generic;

namespace Breas.Device.Monitoring.Measurements
{
    public interface IUIMeasurement
    {
        void Update(IDictionary<DeviceMeasurement, int> measurementValues);

        void DumpMeasurements(List<DeviceMeasurement> measurements);
    }
}