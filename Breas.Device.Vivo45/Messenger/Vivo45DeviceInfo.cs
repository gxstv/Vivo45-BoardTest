using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breas.Device.Vivo45
{

    public class Vivo45DeviceInfo
    {
        public UInt16 Id { get; set; }
        public bool ReadAccess { get; set; }
        public bool WriteAccess { get; set; }
        public string ExplanatoryText { get; set; }

        public Vivo45DeviceInfo()
        {
        }

        public Vivo45DeviceInfo(byte[] inData)
        {
            ParseArray(inData);
        }

        public int ParseArray(byte[] inData)
        {
            int i = 0;
            Id = BitConverter.ToUInt16(inData, i);
            i += 2;
            ReadAccess = Convert.ToBoolean(inData[i] & 0x01);
            WriteAccess = Convert.ToBoolean(inData[i] & 0x02);
            i++;
            ExplanatoryText = System.Text.Encoding.UTF8.GetString(inData, i, inData.Skip(i).TakeWhile(s => s != 0).ToArray().Length);             

            return i;
        }
    }
}