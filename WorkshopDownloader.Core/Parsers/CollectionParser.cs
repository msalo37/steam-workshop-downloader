using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WorkshopDownloader.Core.Parsers.RequestMessages;

namespace WorkshopDownloader.Core.Parsers
{
    public class CollectionParser : BaseParser
    {
        private const string url = "https://api.steampowered.com/ISteamRemoteStorage/GetCollectionDetails/v1/";

        public async override Task<string[]> RequestInfo(ulong itemId)
        {
            var requestParameters = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("collectioncount", "1"),
                new KeyValuePair<string, string>("publishedfileids[0]", itemId.ToString())
            };
            var data = new FormUrlEncodedContent(requestParameters);

            var response = await httpClient.PostAsync(url, data);
            string jsonStr = await response.Content.ReadAsStringAsync();

            var workshopRequestMessage = JsonConvert.DeserializeObject<WorkshopResponse<WorkshopResponseCollectionDelails>>(jsonStr);

            if (workshopRequestMessage.Response.Result != 1) return null;
            if (workshopRequestMessage.Response.Count == 0) return null;

            string[] result = new string[workshopRequestMessage.Response.CollectionDetails[0].CollectionAddonDetails.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = workshopRequestMessage.Response.CollectionDetails[0].CollectionAddonDetails[i].PublishedFileId;

            return result;
        }
    }
}
