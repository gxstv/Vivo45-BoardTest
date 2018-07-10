namespace Breas.Device.Monitoring
{
    public interface ICaptureData
    {
        /// <summary>
        /// The time this event was captured
        /// </summary>
        long EventTime { get; set; }

        /// <summary>
        /// The length of the encoded data.
        /// A value of 0 means variable length and the encoder will write the
        /// length of the array produced by Encode() before the packet
        /// </summary>
        byte EncodedLength { get; }

        /// <summary>
        /// Return the encoded data of this capture data
        /// </summary>
        byte[] Encode();

        /// <summary>
        /// Read this capture data from the stream
        /// Any properties defined in <see cref="Breas.LM.Device.ICaptureData"/>
        /// are read before this method is called
        /// </summary>
        /// <param name="offset">offset in the byte array</param>
        /// <param name="data">the data</param>
        void Decode(byte[] data);
    }
}