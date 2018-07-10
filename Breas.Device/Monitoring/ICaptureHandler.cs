namespace Breas.Device.Monitoring
{
    /// <summary>
    /// The CaptureHandler captures the momentary data from the ventilator(Pressure, Flow, Volume etc..)
    /// </summary>
    public interface ICaptureHandler
    {
        /// <summary>
        /// The device this capture handler is attached to
        /// </summary>
        Device Device { get; }

        /// <summary>
        /// Reads capture data (synchronously) from the device
        /// </summary>
        /// <param name="time">What the current time is</param>
        ICaptureData GetCaptureData(long time,int shift);

    }
}