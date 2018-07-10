namespace Breas.Device.Monitoring.Measurements
{
    public class MeasurePointDefinitionStringKey : MeasurePointDefinition
    {
        public MeasurePointDefinitionStringKey(string name, float correctionFactor, ushort size, ushort nativeId)
            : base(name, correctionFactor, size, nativeId)
        {
            StringKey = name;
        }

        public string StringKey { get; set; }

        public override object Key
        {
            get { return StringKey; }
        }
    }
}