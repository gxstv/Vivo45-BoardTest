using System;
using System.Collections.Generic;

namespace Breas.Device.Vivo45.Messenger
{
    public class SubscriptionResult
    {
        public enum TAction
        {
            Subscribe = 1,
            UnSubscribe = 3
        }

        public struct TSubscribedMeasurePoint
        {
            public bool Result;
            public UInt16 MeasurepointId;
        }

        public TAction Action { get; set; }

        public int Count { get; set; }

        public List<TSubscribedMeasurePoint> Measurepoints { get; set; }

        public SubscriptionResult()
        {
            this.Measurepoints = new List<TSubscribedMeasurePoint>();
        }

        public SubscriptionResult(byte[] Inbytes)
        {
            int i;
            this.Measurepoints = new List<TSubscribedMeasurePoint>();

            this.Action = (TAction)Inbytes[0];
            this.Count = Inbytes[1];
            for (i = 0; i < this.Count; i++)
            {
                TSubscribedMeasurePoint tmp;
                tmp.MeasurepointId = BitConverter.ToUInt16(Inbytes, 2 + (3 * i));
                tmp.Result = Convert.ToBoolean(Inbytes[4 + (3 * i)]);
                Measurepoints.Add(tmp);
            }
        }
    }
}