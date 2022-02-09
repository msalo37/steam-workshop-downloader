using System;
using System.Collections.Generic;
using System.Web;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Net.Http;
using WorkshopTools.Parser;

namespace WorkshopDownloader
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TextBox_ModsFolderPath.Text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");

            workshopItems = new List<WorkshopItem>();
            WorkshopListView.ItemsSource = workshopItems;

            workshop = new WorkshopModParser();
        }

        private HttpClient httpClient = new HttpClient();
        private List<WorkshopItem> workshopItems;

        private WorkshopModParser workshop;

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
            var downloader = new WorkshopTools.Downloader.WorkshopDownloader(TextBox_ServerURL.Text, TextBox_ModsFolderPath.Text, httpClient, Notify);
            foreach (WorkshopItem workshopItem in workshopItems)
            {
                bool status = await downloader.DownloadModAsync(workshopItem.Id);
                if (status == true)
                    Unzipper.UnzipFileAsync(Path.Combine(TextBox_ModsFolderPath.Text, workshopItem.Id + ".zip"));
            }
            Notify("All modes downloaded!");
        }

        private void RemoveWorkshopItem(WorkshopItem workshopItem)
        {
            workshopItems.Remove(workshopItem);
            WorkshopListView.Items.Refresh();
        }

        // Window functions \\

        private void AddMod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            string typedText = TextBox_AddMod.Text;

            if (ulong.TryParse(typedText, out ulong resultId))
            {
                AddWorkshopItemAsync(resultId);
                TextBox_AddMod.Clear();
            }

            if (Uri.TryCreate(typedText, UriKind.Absolute, out Uri uriResult))
            {
                string finalId = HttpUtility.ParseQueryString(uriResult.Query).Get("id");
                AddWorkshopItem(finalId);
                TextBox_AddMod.Clear();
            }
        }

        private void AddWorkshopItem(string id)
        {
            if (ulong.TryParse(id, out ulong finalId))
                AddWorkshopItemAsync(finalId);
        }

        private async void AddWorkshopItemAsync(ulong id)
        {
            string addonName = (bool)CheckBox_RequestRealNames.IsChecked ? await workshop.GetTitle(id) : "Mod Name";

            var data = new WorkshopItem(addonName, id);

            if (workshopItems.Find(x => x.Id.Equals(data.Id)) == null)
            {
                workshopItems.Add(data);
                WorkshopListView.Items.Refresh();
            }
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
            var selected = (WorkshopItem)WorkshopListView.SelectedItem;
            RemoveWorkshopItem(selected);
        }
    }
}
