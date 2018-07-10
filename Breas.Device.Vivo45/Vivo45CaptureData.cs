using Breas.Device.Monitoring;
using Breas.Device.Monitoring.Measurements;
using System;
using System.Collections.Generic;

namespace Breas.Device.Vivo45
{
    public class Vivo45CaptureData : ICaptureData
    {
        public struct measurePoint
        {
            public UInt16 measurePointId { get; set; }

            public Int32 MeasurePointValue { get; set; }

            public double ScaledValue { get; set; }
        }

        public UInt32 epochTime { get; set; }

        public byte packetNumber { get; set; }

        public byte subscriptionCount { get; set; } //the number och measurepoints that are subscribed to

        public List<measurePoint> measurepoints { get; set; }

        public Vivo45CaptureData(byte[] inBytes, Dictionary<object, MeasurePointDefinition> definitions)
        {
            int i = 0;

            measurepoints = new List<measurePoint>();

            this.epochTime = BitConverter.ToUInt32(inBytes, 0);
            this.packetNumber = inBytes[4];
            this.subscriptionCount = inBytes[5];
            for (i = 0; i < this.subscriptionCount; i++)
            {
                measurePoint mp = new measurePoint();
                mp.measurePointId = BitConverter.ToUInt16(inBytes, 6 + (i * 6));
                mp.MeasurePointValue = BitConverter.ToInt32(inBytes, 8 + (i * 6));
                mp.ScaledValue = mp.MeasurePointValue * definitions[mp.measurePointId].CorrectionFactor;
                measurepoints.Add(mp);
            }

            if (measurepoints.Count == 4)
            {
                Pressure = measurepoints[0].ScaledValue;
                Flow = measurepoints[1].ScaledValue;
                Volume = measurepoints[2].ScaledValue;
                IsInhaling = ((measurepoints[3].MeasurePointValue & 0x02) != 0);
            } else
            {
                this.Flow = 0;
                this.Pressure = 0;
                this.Volume = 0;
                this.IsInhaling = false;
            }
            this.CalcEventTime = 0;
            this.EventTime = 0;
            this.ReadLatency = 0;
            this.ReadPosition = 0;
        }

        public Vivo45CaptureData()
        {
            this.epochTime = 0;
            this.packetNumber = 0;
            this.subscriptionCount = 0;
            this.measurepoints = null;
            this.Flow = 0;
            this.Pressure = 0;
            this.Volume = 0;
            this.IsInhaling = false;
            this.CalcEventTime = 0;
            this.EventTime = 0;
            this.ReadLatency = 0;
            this.ReadPosition = 0;
        }

        public long CalcEventTime { get; set; }

        public double Flow { get; internal set; }

        public bool IsInhaling { get; set; }

        public int ReadLatency { get; set; }

        public long ReadPosition { get; set; }

        public long EventTime { get; set; }

        public byte EncodedLength
        {
            get { return 0; }
        }

        public void Decode(byte[] data)
        {
            Flow = BitConverter.ToDouble(data, 0);
            Pressure = BitConverter.ToDouble(data, 8);
            Volume = BitConverter.ToDouble(data, 16);
            CalcEventTime = EventTime;
        }

        public byte[] Encode()
        {
            byte[] data = new byte[24];
            int offset = 0;
            data.Add(ref offset, BitConverter.GetBytes(Flow));
            data.Add(ref offset, BitConverter.GetBytes(Pressure));
            data.Add(ref offset, BitConverter.GetBytes(Volume));
            return data;
        }

        public double Pressure { get; set; }

        public double Volume { get; set; }
    }
}