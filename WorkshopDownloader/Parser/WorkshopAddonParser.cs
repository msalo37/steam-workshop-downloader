using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WorkshopDownloader.Parser
{
    public class WorkshopAddonParser
    {
        public WorkshopAddonParser(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public WorkshopAddonParser()
        {
            httpClient = new HttpClient();
        }

        private const string url = "https://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1/";
        private HttpClient httpClient;

        public async Task<PublishedFileDetails> RequestFileDetails(ulong itemId)
        {
            var requestParameters = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("itemcount", "1"),
                new KeyValuePair<string, string>("publishedfileids[0]", itemId.ToString())
            };
            var data = new FormUrlEncodedContent(requestParameters);

            var response = await httpClient.PostAsync(url, data);
            string jsonStr = await response.Content.ReadAsStringAsync();

            var workshopRequestMessage = JsonConvert.DeserializeObject<WorkshopRequestMessage>(jsonStr);

            return workshopRequestMessage.Response.PublishedFileDetails[0];
        }
    }
}
