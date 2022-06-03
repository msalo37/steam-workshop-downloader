using Newtonsoft.Json;

// Generated with https://app.quicktype.io/?l=csharp
namespace WorkshopDownloader.Core.Parsers.RequestMessages
{
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
