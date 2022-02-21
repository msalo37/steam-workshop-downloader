using System;
using System.Collections.Generic;
using System.Web;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Net.Http;
using WorkshopDownloader.Parser;
using WorkshopDownloader.Downloader;
using WorkshopDownloader.Zip;

namespace WorkshopDownloader
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TextBox_ModsFolderPath.Text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");

            addonList = new List<WorkshopAddon>();
            WorkshopListView.ItemsSource = addonList;

            workshopAddon = new WorkshopAddonParser();
            workshopCollection = new WorkshopCollectionParser();
        }

        private HttpClient httpClient = new HttpClient();
        private List<WorkshopAddon> addonList;

        private WorkshopAddonParser workshopAddon;
        private WorkshopCollectionParser workshopCollection;

        private void ChooseModsFolder()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();
                var result = openFileDlg.ShowDialog();
                if (result.ToString() != string.Empty)
                {
                    TextBox_ModsFolderPath.Text = openFileDlg.SelectedPath;
                }
            }
        }

        private void Notify(string message)
        {
            StatusInfo.Content = message;
        }

        private async void DownloadAllMods()
        {
            var downloader = new AddonDownloader(TextBox_ServerURL.Text, TextBox_ModsFolderPath.Text, httpClient, Notify);
            ProgressBar.Maximum = addonList.Count;
            foreach (WorkshopAddon workshopItem in addonList)
            {
                bool status = await downloader.DownloadModAsync(workshopItem.Id);
                if (status == true)
                    Unzipper.UnzipFileAsync(Path.Combine(TextBox_ModsFolderPath.Text, workshopItem.Id + ".zip"));
                ProgressBar.Value++;
            }
            Notify("All modes downloaded!");
        }

        private void RemoveAddon(WorkshopAddon workshopItem)
        {
            addonList.Remove(workshopItem);
            WorkshopListView.Items.Refresh();
        }

        private bool TryToGetAddonId(string text, out ulong id)
        {
            id = 0;

            if (ulong.TryParse(text, out ulong result))
            {
                id = result;
                return true;
            }

            if (Uri.TryCreate(text, UriKind.Absolute, out Uri uriResult))
            {
                string strId = HttpUtility.ParseQueryString(uriResult.Query).Get("id");
                if (ulong.TryParse(strId, out ulong finalId) == false) return false;

                id = finalId;
                return true;
            }

            return false;
        }

        private async void AddAddonAsync(ulong id)
        {
            string addonName;

            if (CheckBox_RequestRealNames.IsChecked == true)
            {
                PublishedFileDetails fileDetails = await workshopAddon.RequestFileDetails(id);
                addonName = fileDetails.Title;
            }
            else
            {
                addonName = "Addon name";
            }

            var data = new WorkshopAddon(addonName, id);

            if (addonList.Find(x => x.Id.Equals(data.Id)) == null)
            {
                addonList.Add(data);
                WorkshopListView.Items.Refresh();
            }
        }

        private async void AddCollectionAsync(ulong collectionId)
        {
            var addons = await workshopCollection.RequestCollectionDetails(collectionId);
            foreach (var addon in addons)
            {
                if (TryToGetAddonId(addon.PublishedFileId, out ulong id))
                {
                    AddAddonAsync(id);
                }
                else
                {
                    Notify("An error occurred while trying to get id from addon with id " + addon.PublishedFileId);
                }
            }
        }

        // Window functions \\

        private void AddAddonFromText()
        {
            if (TryToGetAddonId(TextBox_AddMod.Text, out ulong id))
            {
                AddAddonAsync(id);
                TextBox_AddMod.Clear();
            }
            else
            {
                Notify("An error occurred while trying to get id from addon");
            }
        }

        private void AddCollectionFromText()
        {
            if (TryToGetAddonId(TextBox_AddMod.Text, out ulong id))
            {
                AddCollectionAsync(id);
                TextBox_AddMod.Clear();
            }
            else
            {
                Notify("An error occurred while trying to get id from collection");
            }
        }

        private void AddMod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            AddAddonFromText();
        }

        private void Button_Folder_Click(object sender, RoutedEventArgs e)
        {
            ChooseModsFolder();
        }

        private void Button_DownloadAll_Click(object sender, RoutedEventArgs e)
        {
            DownloadAllMods();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selected = (WorkshopAddon)WorkshopListView.SelectedItem;
            RemoveAddon(selected);
        }

        private void Button_AddMod(object sender, RoutedEventArgs e)
        {
            AddAddonFromText();
        }

        private void Button_AddCollection(object sender, RoutedEventArgs e)
        {
            AddCollectionFromText();
        }
    }
}
