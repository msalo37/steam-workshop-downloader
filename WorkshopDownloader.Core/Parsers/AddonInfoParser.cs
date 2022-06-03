using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WorkshopDownloader.Core.Parsers.RequestMessages;

namespace WorkshopDownloader.Core.Parsers
{
    public class AddonInfoParser : BaseParser
    {
        private const string url = "https://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1/";


        public async override Task<string[]> RequestInfo(ulong itemId)
        {
            var requestParameters = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("itemcount", "1"),
                new KeyValuePair<string, string>("publishedfileids[0]", itemId.ToString())
            };
            var data = new FormUrlEncodedContent(requestParameters);

            var response = await httpClient.PostAsync(url, data);
            string jsonStr = await response.Content.ReadAsStringAsync();

            var workshopRequestMessage = JsonConvert.DeserializeObject<WorkshopResponse<WorkshopResponseAddonDelails>>(jsonStr);
            if (workshopRequestMessage.Response.Result != 1) return null;

            string[] result = new string[workshopRequestMessage.Response.Count];
            for (int i = 0; i < result.Length; i++)
                result[i] = workshopRequestMessage.Response.PublishedFileDetails[i].Title;

            return result;
        }
    }
}
