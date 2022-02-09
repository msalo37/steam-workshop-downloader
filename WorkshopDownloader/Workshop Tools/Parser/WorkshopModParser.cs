using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WorkshopTools.Parser
{
    public class WorkshopModParser
    {
        public WorkshopModParser(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public WorkshopModParser()
        {
            httpClient = new HttpClient();
        }

        private const string url = "https://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1/";
        private HttpClient httpClient;

        public async Task<string> GetTitle(ulong itemId)
        {
            JObject json = (await RequestInfo(itemId)).Content;
            return (string)json["response"]["publishedfiledetails"][0]["title"];
        }

        private async Task<WorkshopRequestMessage> RequestInfo(ulong itemId)
        {
            var requestParameters = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("itemcount", "1"),
                new KeyValuePair<string, string>("publishedfileids[0]", itemId.ToString())
            };
            var data = new FormUrlEncodedContent(requestParameters);

            var response = await httpClient.PostAsync(url, data);

            string jsonStr = await response.Content.ReadAsStringAsync();
            return new WorkshopRequestMessage(jsonStr);
        }
    }
}
