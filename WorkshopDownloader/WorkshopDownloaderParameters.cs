using System;
using System.Collections.Generic;
using System.Text;

namespace WorkshopDownloader
{
    class WorkshopDownloaderParameters
    {
        public long publishedFileId;
        public long? collectionId;
        public bool hidden;
        public string downloadFormat;
        public bool autodownload;
    }
}
