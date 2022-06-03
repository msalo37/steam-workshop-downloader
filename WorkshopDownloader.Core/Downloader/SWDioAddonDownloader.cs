using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WorkshopDownloader.Core.Downloader.RequestMessages;

namespace WorkshopDownloader.Core.Downloader
{
    /// <summary>
    /// Downloads addons from steamworkshopdownloader.io
    /// </summary>
    public class SWDioAddonDownloader : AddonDownloader
    {
        public SWDioAddonDownloader(string modPath, string serverUrl, HttpClient httpClient = null, Action<string> logHandler = null) : base (modPath, httpClient, logHandler)
        {
            this.serverUrl = serverUrl;
            statusHandler = logHandler;

            
        }

        private string serverUrl;
        private Action<string> statusHandler;

        public override async Task<bool> DownloadAddonAsync(ulong id)
        {
            statusHandler?.Invoke($"Requesting mod - {id}");
            string uuid = await RequestModAsync(id);

            if (uuid == string.Empty) return false;
            statusHandler?.Invoke($"{id} now has uuid - {uuid}");

            DownloadRequestResponse requestInfo = new DownloadRequestResponse(false);
            for (int _ = 0; _ < 10; _++)
            {
                statusHandler?.Invoke($"Attempt {_+1} for mod request status of {id}");
                requestInfo = await CheckRequestStatusAsync(uuid);
                if (requestInfo.available) break;
                await Task.Delay(1000 * 2);
            }
            if (requestInfo.available == false) return false;
            statusHandler?.Invoke($"{id} is ready to download!");

            statusHandler?.Invoke($"{id} is downloading...");
            await DownloadFileAsync(id, uuid, requestInfo.storageNode, requestInfo.storagePath);

            statusHandler?.Invoke($"{id} downloaded!");
            return true;
        }

        private async Task<string> RequestModAsync(ulong id)
        {
            Uri downloadRequestUri = new Uri(serverUrl + "prod/api/download/request");

            var body = new DownloadRequestParameters()
            {
                PublishedFileId = id,
                CollectionId = null,
                IsHidden = true,
                DownloadFormat = "raw",
                AutoDownload = true
            };

            HttpResponseMessage response = await httpClient.PostAsync(downloadRequestUri, new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode == false) return string.Empty;

            string responseString = await response.Content.ReadAsStringAsync();
            string uuid = (string)JObject.Parse(responseString)["uuid"];

            return uuid;
        }

        private async Task<DownloadRequestResponse> CheckRequestStatusAsync(string uuid)
        {
            Uri downloadRequestUri = new Uri(serverUrl + "prod/api/download/status");

            HttpResponseMessage response = await httpClient.PostAsync(downloadRequestUri, new StringContent("{\"uuids\": [\"" + uuid + "\"]}", Encoding.UTF8, "application/json"));
            string responseString = await response.Content.ReadAsStringAsync();

            if (responseString.Contains("prepared"))
            {
                JObject json = JObject.Parse(responseString);
                string storageNode = (string)json[uuid]["storageNode"];
                string storagePath = (string)json[uuid]["storagePath"];

                return new DownloadRequestResponse(true, storageNode, storagePath);
            }

            return new DownloadRequestResponse(false);
        }

        private async Task DownloadFileAsync(ulong itemId, string uuid, string storageNode, string storagePath)
        {
            Uri downloadRequestUri = new Uri($"https://{storageNode}/prod/storage/{storagePath}?uuid={uuid}");
            HttpResponseMessage response = await httpClient.GetAsync(downloadRequestUri);
            string filePath = Path.Combine(modPath, $"{itemId}.zip");
            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                await response.Content.CopyToAsync(fs);
            }
        }
    }
}
