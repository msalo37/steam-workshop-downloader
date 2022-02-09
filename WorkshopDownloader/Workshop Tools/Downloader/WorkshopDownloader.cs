using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WorkshopTools.Downloader
{
    public class WorkshopDownloader
    {
        public WorkshopDownloader(string serverUrl, string modPath, HttpClient httpClient = null, Action<string> handler = null)
        {
            this.modsFolderPath = modPath;
            this.serverUrl = serverUrl;
            this.httpClient = httpClient != null ? httpClient : new HttpClient();
            statusHandler = handler;

            if (Directory.Exists(modPath) == false)
                Directory.CreateDirectory(modPath);
        }

        private string serverUrl;
        private HttpClient httpClient;
        private string modsFolderPath;
        private Action<string> statusHandler;

        public async Task<bool> DownloadModAsync(ulong id)
        {
            statusHandler?.Invoke($"Requesting mod - {id}");
            string uuid = await RequestModAsync(id);

            if (uuid == string.Empty) return false;
            statusHandler?.Invoke($"{id} now has uuid - {uuid}");

            DownloadRequestInfo requestInfo = new DownloadRequestInfo(false);
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

            var body = new WorkshopDownloaderParameters()
            {
                publishedFileId = id,
                collectionId = null,
                hidden = true,
                downloadFormat = "raw",
                autodownload = true
            };

            HttpResponseMessage response = await httpClient.PostAsync(downloadRequestUri, new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode == false) return string.Empty;

            string responseString = await response.Content.ReadAsStringAsync();
            string uuid = (string)JObject.Parse(responseString)["uuid"];

            return uuid;
        }

        private async Task<DownloadRequestInfo> CheckRequestStatusAsync(string uuid)
        {
            Uri downloadRequestUri = new Uri(serverUrl + "prod/api/download/status");

            HttpResponseMessage response = await httpClient.PostAsync(downloadRequestUri, new StringContent("{\"uuids\": [\"" + uuid + "\"]}", Encoding.UTF8, "application/json"));
            string responseString = await response.Content.ReadAsStringAsync();

            if (responseString.Contains("prepared"))
            {
                JObject json = JObject.Parse(responseString);
                string storageNode = (string)json[uuid]["storageNode"];
                string storagePath = (string)json[uuid]["storagePath"];

                return new DownloadRequestInfo(true, storageNode, storagePath);
            }

            return new DownloadRequestInfo(false);
        }

        private async Task DownloadFileAsync(ulong itemId, string uuid, string storageNode, string storagePath)
        {
            Uri downloadRequestUri = new Uri($"https://{storageNode}/prod/storage/{storagePath}?uuid={uuid}");
            HttpResponseMessage response = await httpClient.GetAsync(downloadRequestUri);
            string filePath = Path.Combine(modsFolderPath, $"{itemId}.zip");
            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                await response.Content.CopyToAsync(fs);
            }
        }
    }
}
