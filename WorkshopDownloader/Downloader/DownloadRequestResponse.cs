namespace WorkshopDownloader.Downloader
{
    public class DownloadRequestResponse
    {
        public DownloadRequestResponse(bool available, string storageNode, string storagePath)
        {
            this.available = available;
            this.storageNode = storageNode;
            this.storagePath = storagePath;
        }

        public DownloadRequestResponse(bool available)
        {
            this.available = available;
        }

        public bool available;
        public string storageNode;
        public string storagePath;
    }
}
