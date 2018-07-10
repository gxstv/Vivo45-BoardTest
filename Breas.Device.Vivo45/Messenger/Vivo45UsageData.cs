using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breas.Device.Vivo45
{
    public class Vivo45UsageData
    {
        public enum TRecordType
        {
            Event,
            Alarm
        }
        public TRecordType RecordType { get; set; }
        public UInt16 RecordId { get; set; }
        public UInt32 EpochTime { get; set; }
        public UInt16 MilliSeconds { get; set; }
        public List<UInt16> UsageData { get; set; }

        private DateTime _TimeStamp;

        public DateTime TimeStamp
        {
            get 
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                epoch = epoch.AddSeconds(EpochTime);
                return epoch.AddMilliseconds(MilliSeconds);                  
            }
        }
        

      public int size { get; set; }

      public Vivo45UsageData(byte[] inData)
      {
          int i = 0;
          this.RecordType = (TRecordType)inData[i++];
          this.RecordId = BitConverter.ToUInt16(inData, i);
          i += 2;
          this.EpochTime = BitConverter.ToUInt32(inData, i);
          i += 4;
          this.MilliSeconds = BitConverter.ToUInt16(inData, i); ;
          i += 2;
          UsageData = new List<ushort>();          
          for (int j = 0; j < 8; j++ )
          {
              UsageData.Add(BitConverter.ToUInt16(inData, i));
              i += 2;
          }
          size = i;
      }
    }
}
