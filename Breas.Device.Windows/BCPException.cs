namespace Breas.Device.Finder.Windows
{
    public class BCPException : Communication.CommunicationException
    {
        public BCPException(string message)
            : base(message)
        {
        }
    }
}