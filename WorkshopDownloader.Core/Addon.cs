namespace WorkshopDownloader.Core.Addons
{
    public class Addon
    {
        public Addon(string title, ulong id)
        {
            Title = title;
            Id = id;
        }

        public string Title { get; private set; }
        public ulong Id { get; private set; }
    }
}
