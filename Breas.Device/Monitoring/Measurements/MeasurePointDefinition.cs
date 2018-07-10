namespace Breas.Device.Monitoring.Measurements
{
    /// <summary>
    /// Stores the definition for a measure point on the device
    /// </summary>
    public abstract class MeasurePointDefinition
    {
        public abstract object Key { get; }

        /// <summary>
        /// The friendly name of this measure point
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The native id of this measure point
        /// </summary>
        public ushort NativeId { get; set; }

        /// <summary>
        /// The size of this measure point. Normally, the device reports 0 for this.
        /// </summary>
        public ushort Size { get; set; }

        /// <summary>
        /// Correction factor to use on the value returned by <see cref="Breas.LM.Comm.IComm.GetMeasurePointValue"/>
        /// </summary>
        public double CorrectionFactor { get; set; }

        /// <summary>
        /// Initialize a new MeasurePointDefinition
        /// </summary>
        /// <param name="nativeName">The friendly name of this measure point</param>
        /// <param name="correctionFactor">Correction factor used to scale the value we get from <see cref="Breas.LM.Core.Comm.Device.IComm.GetMeasurePointValue"/></param>
        /// <param name="size">The size of this measure point</param>
        /// <param name="nativeId">The nativeid of this measure point</param>
        public MeasurePointDefinition(string nativeName, double correctionFactor, ushort size, ushort nativeId)
        {
            this.Name = nativeName;
            this.CorrectionFactor = correctionFactor;
            this.Size = size;
            this.NativeId = nativeId;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}