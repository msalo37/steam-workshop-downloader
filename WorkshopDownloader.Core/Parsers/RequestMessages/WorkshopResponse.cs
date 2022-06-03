using Newtonsoft.Json;

namespace WorkshopDownloader.Core.Parsers.RequestMessages
{
    public partial class WorkshopResponse<T>
    {
        [JsonProperty("response")]
        public T Response { get; set; }
    }

    public partial class WorkshopResponseCollectionDelails
    {
        [JsonProperty("result")]
        public long Result { get; set; }

        [JsonProperty("resultcount")]
        public long Count { get; set; }

        [JsonProperty("collectiondetails")]
        public CollectionDetails[] CollectionDetails { get; set; }
    }

    public partial class WorkshopResponseAddonDelails
    {
        [JsonProperty("result")]
        public long Result { get; set; }

        [JsonProperty("resultcount")]
        public long Count { get; set; }

        [JsonProperty("publishedfiledetails")]
        public PublishedFileDetails[] PublishedFileDetails { get; set; }
    }
}
