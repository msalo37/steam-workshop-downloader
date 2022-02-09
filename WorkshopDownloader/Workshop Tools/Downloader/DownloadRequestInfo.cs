using System;
using System.Collections.Generic;
using System.Text;

namespace WorkshopTools.Downloader
{
    public class DownloadRequestInfo
    {
        public DownloadRequestInfo(bool available, string storageNode, string storagePath)
        {
            this.available = available;
            this.storageNode = storageNode;
            this.storagePath = storagePath;
        }

        public DownloadRequestInfo(bool available)
        {
            this.available = available;
        }

        public bool available;
        public string storageNode;
        public string storagePath;
    }
}
