using System;
using System.Collections.Generic;
using System.Text;

namespace WorkshopDownloader
{
    class WorkshopItem
    {
        public WorkshopItem(string title, ulong id)
        {
            Title = title;
            Id = id;
        }

        public string Title { get; set; }
        public ulong Id { get; set; }
    }
}
