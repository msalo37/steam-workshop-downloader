using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WorkshopDownloader.Core.Downloader
{
    public abstract class AddonDownloader
    {
        public AddonDownloader(string modPath, HttpClient httpClient = null, Action<string> logHandler = null)
        {
            this.httpClient = httpClient != null ? httpClient : new HttpClient();
            this.modPath = modPath;
            if (logHandler != null) this.logHandler = logHandler;
            if (Directory.Exists(modPath) == false)
                Directory.CreateDirectory(modPath);
        }
        
        protected HttpClient httpClient;
        protected string modPath;

        protected Action<string> logHandler;

        public abstract Task<bool> DownloadAddonAsync(ulong id);
    }
}
