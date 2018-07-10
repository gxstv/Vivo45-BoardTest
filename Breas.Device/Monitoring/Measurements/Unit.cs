namespace Breas.Device.Monitoring.Measurements
{
    public struct Unit
    {
        public static readonly Unit Percent = new Unit("UNIT_PERCENT");
        public static readonly Unit Bpm = new Unit("UNIT_BPM");
        public static readonly Unit None = new Unit("");
        public static readonly Unit Litre = new Unit("UNIT_LITRE");
        public static readonly Unit Seconds = new Unit("UNIT_SECONDS");
        public static readonly Unit CmH2O = new Unit("UNIT_CMH2O");
        public static readonly Unit MmHg = new Unit("UNIT_MMHG");
        public static readonly Unit LitrePerMinute = new Unit("UNIT_LITRE_PER_MINUTE");
        public static readonly Unit Millilitre = new Unit("UNIT_MILLILITRE");
        public static readonly Unit Kpa = new Unit("UNIT_KPA");
        public static readonly Unit PulseBpm = new Unit("UNIT_PULSE_BPM");
        public static readonly Unit Hpa = new Unit("UNIT_HPA");
        public static readonly Unit MBar = new Unit("UNIT_MBAR");

        public Unit(string key)
            : this()
        {
            this.Key = key;
        }

        public string Key { get; set; }
    }
}