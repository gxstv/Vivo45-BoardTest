﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breas.Device.Vivo45
{
    public class Vivo45ActiveAlarm
    {
        public enum TAlarmPriority
        {
            PrioNone = 0,
            PrioInfo = 1,
            PrioLow = 2,
            PrioMedium = 3,
            PrioHigh = 4,
            PrioFuncFail = 5
        }

        public enum TTaskId
        {
            MediatorId = 0x0001,
            SurveilId = 0x0002,
            CPUComId = 0x0004,
            LogId = 0x0010,
            DebugId = 0x0020,
            UIId = 0x0040,
            ParamServerId = 0x0080,
            TreatmentId = 0x0100,
            TaskAll = 0x01FF
        }

        public UInt16 AlarmId { get; set; }
        public TAlarmPriority Priority { get; set; }
        public UInt32 EpochTime { get; set; }
        public TTaskId TaskOrigin { get; set; }
        public string ExplanatoryText { get; set; }

        public DateTime TimeStamp
        {
            get
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddSeconds(EpochTime);
            }
        }
        public int size { get; set; }

        public Vivo45ActiveAlarm(byte[] inData)
        {
            int i = 0;

            this.TaskOrigin = (TTaskId)BitConverter.ToUInt16(inData, i);
            i += 2;
            this.AlarmId = inData[i++];
            this.Priority = (TAlarmPriority)inData[i++];
            this.EpochTime = BitConverter.ToUInt32(inData, i);
            i += 4;
            this.ExplanatoryText = System.Text.Encoding.UTF8.GetString(inData, i, inData.Skip(i).TakeWhile(s => s != 0).ToArray().Length);
            i += ExplanatoryText.Length + 1; //+1 for the null termination
            size = i;
        }
    }
}
