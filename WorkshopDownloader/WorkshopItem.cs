using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media.Imaging;

namespace WorkshopDownloader
{
    class WorkshopItem
    {
        public WorkshopItem(string title, ulong id)
        {
            Title = title;
            Id = id;
        }

        public string Title { get; private set; }
        public ulong Id { get; private set; }
    }
}
