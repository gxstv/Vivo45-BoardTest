using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breas.Device.Monitoring.Measurements
{
    public class DeviceMeasurementSet
    {

        /// <summary>
        /// Creates a new device measurement set
        /// </summary>
        /// <param name="measurement">the measurement to set</param>
        /// <param name="value">the raw value to set it to</param>
        public DeviceMeasurementSet(DeviceMeasurement measurement, int value)
        {
            this.Measurement = measurement;
            this.Value = value;
        }

        /// <summary>
        /// Creates a new device measurement set with a scaled value
        /// </summary>
        /// <param name="measurement">The measurement to set</param>
        /// <param name="value">The scaled value, which will be unscaled when sent</param>
        public DeviceMeasurementSet(DeviceMeasurement measurement, double value)
        {
            this.Measurement = measurement;
            this.Value = measurement.ReverseScale(value);
        }

        public DeviceMeasurement Measurement { get; private set; }
        public int Value { get; private set; }
    }
}
