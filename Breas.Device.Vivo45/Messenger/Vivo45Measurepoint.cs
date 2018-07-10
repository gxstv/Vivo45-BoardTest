using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breas.Device.Monitoring.Measurements;


namespace Breas.Device.Vivo45.Messenger
{
    public class Vivo45Measurepoint : MeasurePointDefinition
    {

        public Vivo45Measurepoint(string mpName, double multiplier, byte access, ushort size, ushort id)
            : base(mpName, multiplier, size, id)
        {
            Access = access;
            ShortKey = id;
        }

        public ushort ShortKey { get; set; }

        public override object Key
        {
            get { return ShortKey; }
        }
        /// <summary>
        /// The byte that holds the accessbits
        /// </summary>
        public byte Access { get; set; }

        /// <summary>
        /// True if read access
        /// </summary>
        public Boolean ReadAccess
        {
            get
            {
                return Convert.ToBoolean(Access & 0x01);
            }
        }

        /// <summary>
        /// True if write access
        /// </summary>c
        public Boolean WriteAccess 
        { 
            get 
            {
                return Convert.ToBoolean(Access & 0x02);
            }
        }

       public string toReadableFormat()
        {
            string strout = "";
            strout += "Id: " + this.Key.ToString() + "\r\n";
            strout += "Correction: " + this.CorrectionFactor.ToString() + "\r\n";
            strout += "Read Access: " + this.ReadAccess.ToString() + "\r\n";
            strout += "Write Access: " + this.WriteAccess.ToString() + "\r\n";
            strout += "Name: " + this.Name.ToString() + "\r\n";     
            
            return strout;
        }
    
    }
}
