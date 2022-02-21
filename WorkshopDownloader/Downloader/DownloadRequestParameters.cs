using Newtonsoft.Json;

namespace WorkshopDownloader.Downloader
{
    public class DownloadRequestParameters
    {
        [JsonProperty("publishedFileId")]
        public ulong PublishedFileId { get; set; }

        [JsonProperty("collectionId")]
        public ulong? CollectionId { get; set; }

        [JsonProperty("hidden")]
        public bool IsHidden { get; set; }

        [JsonProperty("downloadFormat")]
        public string DownloadFormat { get; set; }

        [JsonProperty("autodownload")]
        public bool AutoDownload { get; set; }
    }
}
