using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopDownloader.Parser
{
    class WorkshopCollectionParser
    {
        public WorkshopCollectionParser(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public WorkshopCollectionParser()
        {
            httpClient = new HttpClient();
        }

        private const string url = "https://api.steampowered.com/ISteamRemoteStorage/GetCollectionDetails/v1/";
        private HttpClient httpClient;

        public async Task<CollectionAddonDetails[]> RequestCollectionDetails(ulong itemId)
        {
            var requestParameters = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("collectioncount", "1"),
                new KeyValuePair<string, string>("publishedfileids[0]", itemId.ToString())
            };
            var data = new FormUrlEncodedContent(requestParameters);

            var response = await httpClient.PostAsync(url, data);
            string jsonStr = await response.Content.ReadAsStringAsync();

            var workshopRequestMessage = JsonConvert.DeserializeObject<WorkshopCollectionRequestMessage>(jsonStr);

            if (workshopRequestMessage.Response.Result != 1) return null;
            if (workshopRequestMessage.Response.CollectionDetails[0].Result != 1) return null;

            return workshopRequestMessage.Response.CollectionDetails[0].CollectionAddonDetails;
        }
    }
}
