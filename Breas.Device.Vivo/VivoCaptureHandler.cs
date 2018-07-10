using Breas.Device.Monitoring;
using System;

namespace Breas.Device.Vivo
{
    public class VivoCaptureHandler : ICaptureHandler
    {
        private const long InvalidVolumeValue = 0xFFFF;
        private const long InvalidMomentaryC02Value = 0xFF;
        private const long InvalidAtmosphericValue = 0xFFFF;
    
        private VivoDevice device;

        private int eventTimeForPreviousEvent = 0;
        private long reportedTimeForPreviousEvent = 0;

        public Breas.Device.Device Device { get { return device; } }
        public int shift;
        public VivoCaptureHandler(VivoDevice device)
        {
            this.device = device;
        }

        /// <summary>
        /// Get a chunk of capture data from the device
        /// </summary>
        /// <returns>
        /// If the Capture Data has exceeded the event threshold, a list of capture data objects.
        /// If not, null is returned
        /// </returns>
        public ICaptureData GetCaptureData(long time, int shift)
        {
            byte[] capture = device.GetCaptureData();
            //for (int i = 0; i < capture.Length; i++)
            //{
            //    capture[i] = Rotate(capture[i], 5);
            //}
            if (capture == null)
                return null;//disconnection can make capture = null. 
            //shift = device.getshift(); ;
            var captureData = Decode(capture,shift);
            captureData.CalcEventTime = CalcTime(time, captureData);
            captureData.EventTime = time; //add the time it took to read from the comm to the time
            return captureData;
        }
        private byte Rotate(byte data, int x)
        {
            while (x > 0)
            {
                int tmp;
                tmp = (byte)(data >> 7 & 1);
                data = (byte)(data << 1);
                data = (byte)(data | tmp);
                x--;
            }
            return data;
        }
        private VivoCaptureData Decode(byte[] capture,int shift)
        {
            for (int i = 1; i < capture.Length; i++)
            {
                capture[i] = Rotate(capture[i], shift);
            }
            int fl = capture[1];
            int p = GetShort(capture, 2);
            int f = GetShort(capture, 4);
            int v = GetShort(capture, 6);
            // replace invalid volume with zero
            if (v == InvalidVolumeValue)
            {
                v = 0;
            }
            int last10BitsOfMs = (capture[14] & 0xFF) * 4;
            int mCo2 = capture[15];
            // replace invalid momentary CO2 with zero
            if (mCo2 == InvalidMomentaryC02Value)
            {
                mCo2 = 0;
            }
            int aP = GetShort(capture, 20);
            // replace invalid atmospheric pressure with zero
            if (aP == InvalidAtmosphericValue)
            {
                aP = 0;
            }
            int co2u = capture[22];
            return new VivoCaptureData(p, f, v, fl, last10BitsOfMs, mCo2, aP, co2u);
        }

        private long CalcTime(long ourTime, VivoCaptureData data)
        {
            long now = ourTime;
            long time = now;
            int eventTime = data.Last10BitsOfMs;
            long timeSinceLastCapture = time - reportedTimeForPreviousEvent;
            if (timeSinceLastCapture < data.EventTimeRange / 2)
            {
                // getting events in a punctual fashion, so instead of using our clock time,
                // we use the ventilator delta to add to the time we said the last event happend on
                int ventilatorDelta = eventTime - eventTimeForPreviousEvent;
                if (ventilatorDelta < 0)
                {
                    // eventTime is modulo event time range, so an underflow means we need to add the range
                    ventilatorDelta += data.EventTimeRange;
                }
                time = reportedTimeForPreviousEvent + ventilatorDelta;
            }
            // double check: if the time we are using are differing too much from our system clock, reset the calc
            if (now - time > data.EventTimeRange)
            {
                time = now;
            }
            eventTimeForPreviousEvent = eventTime;
            reportedTimeForPreviousEvent = time;
            return time;
        }

        private short GetShort(byte[] buf, int i)
        {
            int lsb = buf[i] & 0xFF;
            int msb = buf[i + 1] & 0xFF;
            return (short)(lsb + msb * 256);
        }
    }
}