using Newtonsoft.Json;

namespace WorkshopDownloader.Parser
{
    // Generated with https://app.quicktype.io/?l=csharp
    public partial class WorkshopCollectionRequestMessage
    {
        [JsonProperty("response")]
        public WorkshopCollectionRequestResponse Response { get; set; }
    }

    public partial class WorkshopCollectionRequestResponse
    {
        [JsonProperty("result")]
        public long Result { get; set; }

        [JsonProperty("resultcount")]
        public long Count { get; set; }

        [JsonProperty("collectiondetails")]
        public CollectionDetails[] CollectionDetails { get; set; }
    }

    public partial class CollectionDetails
    {
        [JsonProperty("publishedfileid")]
        public string PublishedFileId { get; set; }

        [JsonProperty("result")]
        public long Result { get; set; }

        [JsonProperty("children")]
        public CollectionAddonDetails[] CollectionAddonDetails { get; set; }
    }

    public partial class CollectionAddonDetails
    {
        [JsonProperty("publishedfileid")]
        public string PublishedFileId { get; set; }

        [JsonProperty("sortorder")]
        public long SortOrder { get; set; }

        [JsonProperty("filetype")]
        public long FileType { get; set; }
    }
}
