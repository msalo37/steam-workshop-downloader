using System;
using System.Collections.Generic;
using System.Text;

namespace WorkshopTools.Downloader
{
    public class WorkshopDownloaderParameters
    {
        public ulong publishedFileId;
        public ulong? collectionId;
        public bool hidden;
        public string downloadFormat;
        public bool autodownload;
    }
}
