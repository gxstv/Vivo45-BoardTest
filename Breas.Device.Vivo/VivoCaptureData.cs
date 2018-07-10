using Breas.Device.Monitoring;
using System;

namespace Breas.Device.Vivo
{
    public class VivoCaptureData : ICaptureData
    {
        private const int VivoCaptureDataLength = 20;

        public int Pressure { get; set; }

        public int Flow { get; set; }

        public int Volume { get; set; }

        /// <summary>
        /// Contains misc information about this capture, like inhaling
        /// <seealso cref="VivoCaptureData.IsInhaling"/>
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// Last 10 bits of ms data from the ventilator
        /// </summary>
        public int Last10BitsOfMs { get; set; }

        // V7+ data
        public int MomentaryCO2 { get; set; }

        public int AtmosphericPressure { get; set; }

        public int CO2Unit { get; set; }

        public int EventTimeRange { get { return 1024; } }

        public long CalcEventTime { get; set; }

        /// <summary>
        /// Determines if the ventilator inhaling in this capture data
        /// </summary>
        public bool IsInhaling { get { return (Flags & 0x02) != 0; } }

        /// <summary>
        /// The time of this event during recording
        /// </summary>
        public long EventTime { get; set; }

        #region calculations

        public float CalculatedPressure { get; set; }

        /// <summary>
        /// Returns flow in cmH2O/min
        /// </summary>
        public float CalculatedFlow { get; set; }

        /// <summary>
        /// Returns CalcuatedMCO2 based on the CO2 unit in this capture
        /// </summary>
        public float CalculatedMCO2
        {
            get;
            set;
        }

        public long ReadPosition { get; set; }

        #endregion calculations

        public byte EncodedLength
        {
            get
            {
                return VivoCaptureDataLength;
            }
        }
        
        public VivoCaptureData(int pressure, int flow, int volume, int flags, int last10BitsOfMs, int momentaryCo2, int atmosphericPressure, int co2Unit)
        {
            this.Pressure = pressure;
            this.Flow = flow;
            this.Volume = volume;
            this.Flags = flags;
            this.Last10BitsOfMs = last10BitsOfMs;
            this.MomentaryCO2 = momentaryCo2;
            this.AtmosphericPressure = atmosphericPressure;
            this.CO2Unit = co2Unit;
            this.CalcEventTime = 0;
            this.EventTime = 0;
            this.ReadPosition = 0;
            this.CalculatedMCO2 = 0;
            this.CalculatedFlow = 0;
            this.CalculatedPressure = 0;
            this.data = new byte[VivoCaptureDataLength];
            SetCalculated();
        }

        private void SetCalculated()
        {
            float scale = 0f;
            switch (CO2Unit)
            {
                case 0:
                    scale = 1.0f / 100.0f * 75.0f / 100.0f * AtmosphericPressure;
                    break;

                case 1:
                    scale = 1.0f / 100.0f / 10.0f * AtmosphericPressure;
                    break;

                case 2:
                    scale = 1.0f;
                    break;
            }
            CalculatedMCO2 = (MomentaryCO2 / 10f) * scale;
            CalculatedFlow = Flow * 60f / 1000f;
            CalculatedPressure = Pressure / 10f;
        }

        //because this capturedata is fixed length we can reuse the encoded array
        private byte[] data;

        public byte[] Encode()
        {
            int offset = 0;
            data.Add(ref offset, BitConverter.GetBytes((short)Pressure));
            data.Add(ref offset, BitConverter.GetBytes((short)Flow));
            data.Add(ref offset, BitConverter.GetBytes((short)Volume));
            data.Add(ref offset, BitConverter.GetBytes((short)Flags));
            data.Add(ref offset, BitConverter.GetBytes(CalcEventTime));
            data.Add(ref offset, BitConverter.GetBytes((MomentaryCO2 & 0xFF) + ((CO2Unit & 0xFF) << 8) + ((AtmosphericPressure & 0xFFFF) << 16)));
            return data;
        }

        public void Decode(byte[] data)
        {


            Pressure = BitConverter.ToInt16(data, 0);
            Flow = BitConverter.ToInt16(data, 2);
            Volume = BitConverter.ToInt16(data, 4);
            Flags = BitConverter.ToInt16(data, 6);
            CalcEventTime = BitConverter.ToInt64(data, 8);
            int thirdVal = BitConverter.ToInt32(data, 16);
            MomentaryCO2 = thirdVal & 0xFF;
            AtmosphericPressure = thirdVal >> 16 & 0xFF;
            CO2Unit = thirdVal >> 8 & 0xFF;
            SetCalculated();
        }

        public void Decrypt( int shift)
        {
            for (int i = 0; i < this.data.Length; i++)
            {
                this.data[i] = Rotate(this.data[i], shift);
            }
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

    }
}