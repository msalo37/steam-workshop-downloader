using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using WorkshopDownloader.Core.Parsers;
using HtmlAgilityPack;
using System.IO;

namespace WorkshopDownloader.Core.Downloader
{
    /// <summary>
    /// Downloads addons from steamworkshop.download
    /// It may not work, because I did not fully understand how it works.
    /// It may return an error instead of a link to download the mod, for example, that there is no free space on the server. Thats is big problem in this downloader
    /// </summary>
    public class Vova1234AddonDownloader : AddonDownloader
    {
        public Vova1234AddonDownloader(string modPath, AddonInfoParser addonInfoParser = null, HttpClient httpClient = null, Action<string> logHandler = null) : base(modPath, httpClient, logHandler)
        {
            this.addonInfoParser = addonInfoParser != null ? new AddonInfoParser() : addonInfoParser;
        }

        private AddonInfoParser addonInfoParser;

        public async override Task<bool> DownloadAddonAsync(ulong id)
        {
            string downloaderResponse = await RequestModAsync(id);

            Console.WriteLine("Vova1234's Addon downloader response:");
            Console.WriteLine(downloaderResponse);

            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(downloaderResponse);

                var anchor = doc.DocumentNode.SelectSingleNode("//a");
                if (anchor == null) return false;

                string link = anchor.Attributes["href"].Value;
                await DownloadFileAsync(link, id);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Vova1234's Addon downloader error:");
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private async Task<string> RequestModAsync(ulong id)
        {
            var workshopResponse = await addonInfoParser.RequestAddonInfo(id);
            if (workshopResponse == null) return string.Empty;
            if (workshopResponse.Response.Result != 1) return string.Empty;
            if (workshopResponse.Response.PublishedFileDetails.Length == 0) return string.Empty;

            ulong gameId = workshopResponse.Response.PublishedFileDetails[0].CreatorAppId;

            Uri downloadRequestUri = new Uri("http://steamworkshop.download/online/steamonline.php");

            var requestParameters = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("item", id.ToString()),
                new KeyValuePair<string, string>("app", gameId.ToString())
            };
            var data = new FormUrlEncodedContent(requestParameters);

            var response = await httpClient.PostAsync(downloadRequestUri, data);

            if (response.IsSuccessStatusCode == false) return string.Empty;

            string responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        private async Task DownloadFileAsync(string link, ulong itemId)
        {
            Uri downloadRequestUri = new Uri(link);
            HttpResponseMessage response = await httpClient.GetAsync(downloadRequestUri);
            string filePath = Path.Combine(modPath, $"{itemId}.zip");
            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                await response.Content.CopyToAsync(fs);
            }
        }
    }
}
