namespace Breas.Device.Monitoring.Measurements
{
    public class MeasurePointDefinitionShortKey : MeasurePointDefinition
    {
        public MeasurePointDefinitionShortKey(string mpName, double m_multiplier, ushort size, ushort id)
            : base(mpName, m_multiplier, size, id)
        {
            ShortKey = id;
        }

        public ushort ShortKey { get; set; }

        public override object Key
        {
            get { return ShortKey; }
        }
    }
}