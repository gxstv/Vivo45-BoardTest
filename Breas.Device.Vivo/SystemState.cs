namespace Breas.Device.Vivo
{
    public struct SystemState
    {
        public int Id { get; set; }

        public bool OffState { get; set; }

        public string NameKey { get; set; }

        public SystemState(int id, bool off, string nameKey)
            : this()
        {
            this.Id = id;
            this.OffState = off;
            this.NameKey = nameKey;
        }
    }
}