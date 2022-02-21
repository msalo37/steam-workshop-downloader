using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// Generated with https://app.quicktype.io/?l=csharp
namespace WorkshopDownloader.Parser
{
    public partial class WorkshopRequestMessage
    {
        [JsonProperty("response")]
        public WorkshopRequestResponse Response { get; set; }
    }

    public partial class WorkshopRequestResponse
    {
        [JsonProperty("result")]
        public long Result { get; set; }

        [JsonProperty("resultcount")]
        public long Resultcount { get; set; }

        [JsonProperty("publishedfiledetails")]
        public PublishedFileDetails[] PublishedFileDetails { get; set; }
    }

    public partial class PublishedFileDetails
    {
        [JsonProperty("publishedfileid")]
        public string PublishedFileId { get; set; }

        [JsonProperty("result")]
        public long Result { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("creator_app_id")]
        public ulong CreatorAppId { get; set; }

        [JsonProperty("consumer_app_id")]
        public ulong ConsumerAppId { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("file_size")]
        public ulong FileSize { get; set; }

        [JsonProperty("file_url")]
        public string FileUrl { get; set; }

        [JsonProperty("hcontent_file")]
        public string HContentFile { get; set; }

        [JsonProperty("preview_url")]
        public string PreviewUrl { get; set; }

        [JsonProperty("hcontent_preview")]
        public string HContentPreview { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("time_created")]
        public ulong TimeCreated { get; set; }

        [JsonProperty("time_updated")]
        public ulong TimeUpdated { get; set; }

        [JsonProperty("visibility")]
        public long Visibility { get; set; }

        [JsonProperty("banned")]
        public long Banned { get; set; }

        [JsonProperty("ban_reason")]
        public string BanReason { get; set; }

        [JsonProperty("subscriptions")]
        public ulong Subscriptions { get; set; }

        [JsonProperty("favorited")]
        public ulong Favorited { get; set; }

        [JsonProperty("lifetime_subscriptions")]
        public ulong LifetimeSubscriptions { get; set; }

        [JsonProperty("lifetime_favorited")]
        public ulong LifetimeFavorited { get; set; }

        [JsonProperty("views")]
        public ulong Views { get; set; }

        [JsonProperty("tags")]
        public Tag[] Tags { get; set; }
    }

    public partial class Tag
    {
        [JsonProperty("tag")]
        public string TagTag { get; set; }
    }
}
