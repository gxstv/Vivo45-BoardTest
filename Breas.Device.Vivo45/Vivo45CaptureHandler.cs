using Breas.Device.Monitoring;
using System;

namespace Breas.Device.Vivo45
{
    public class Vivo45CaptureHandler : ICaptureHandler
    {
        private Vivo45Device _device;

        public Device Device
        {
            get
            {
                return _device;
            }
        }

        public Vivo45CaptureHandler(Vivo45Device device)
        {
            this._device = device;
        }

        public ICaptureData GetCaptureData(long time,int shift)
        {
            try
            {
                var start = Environment.TickCount;
                byte[] capture = _device.GetCaptureData();
                if (capture == null)
                    return null;
                var captureData = new Vivo45CaptureData(capture, _device.MeasurePointDefinitions);
                captureData.ReadLatency = (int)Environment.TickCount - start;
                captureData.CalcEventTime = captureData.epochTime;
                captureData.EventTime = time + Environment.TickCount - start;                  
                return captureData;
            }
            catch
            {
                return null;
            }
        }
        
        private long CalcTime(uint time)
        {
            return time;
        }

        private short getShort(byte[] buf, int i)
        {
            int lsb = buf[i] & 0xFF;
            int msb = buf[i + 1] & 0xFF;
            return (short)(lsb + msb * 256);
        }
    }
}