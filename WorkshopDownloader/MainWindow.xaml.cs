using System;
using System.Collections.Generic;
using System.Web;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Media;

namespace WorkshopDownloader
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TextBoxModsFolder.Text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");
            workshopItems = new List<WorkshopItem>();
            WorkshopListView.ItemsSource = workshopItems;
        }

        private HttpClient httpClient = new HttpClient();
        private List<WorkshopItem> workshopItems;

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            string tx = TextBoxContentAdd.Text;
            
            if (long.TryParse(tx, out long resultId))
            {
                AddWorkshopItem(resultId);
                TextBoxContentAdd.Clear();
            }

            if (Uri.TryCreate(tx, UriKind.Absolute, out Uri uriResult))
            {
                string finalId = HttpUtility.ParseQueryString(uriResult.Query).Get("id");
                AddWorkshopItem(finalId);
                TextBoxContentAdd.Clear();
            }
        }

        private void AddWorkshopItem(string id)
        {
            if (long.TryParse(id, out long finalId))
                AddWorkshopItem(finalId);
        }

        private void AddWorkshopItem(long id)
        {
            var data = new WorkshopItem()
            {
                Title = "Mod name",
                Id = id
            };

            if (workshopItems.Find(x => x.Id.Equals(data.Id)) == null)
            {
                workshopItems.Add(data);
                WorkshopListView.Items.Refresh();
                StatusInfo.Content = $"Mod added {data.Id}";
            }
        }

        private void ChooseModsFolder()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();
                var result = openFileDlg.ShowDialog();
                if (result.ToString() != string.Empty)
                {
                    TextBoxModsFolder.Text = openFileDlg.SelectedPath;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChooseModsFolder();
        }

        private async void DownloadAllMods()
        {
            Uri downloadRequestUri = new Uri(TextBoxServerUrl.Text + "prod/api/download/request");

            foreach (WorkshopItem item in workshopItems)
            {
                StatusInfo.Content = "Requesting mod with id " + item.Id;

                var body = new WorkshopDownloaderParameters()
                {
                    publishedFileId = item.Id,
                    collectionId = null,
                    hidden = true,
                    downloadFormat = "raw",
                    autodownload = true
                };

                HttpResponseMessage response = await httpClient.PostAsync(downloadRequestUri, new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode == false) continue;

                string responseString = await response.Content.ReadAsStringAsync();
                string uuid = (string)JObject.Parse(responseString)["uuid"];

                StatusInfo.Content = $"Mod ({item.Id}) got uuid - {uuid}";
                await CheckDownloadStatus(item.Id, uuid);
            }

            SystemSounds.Beep.Play();
            StatusInfo.Content = "All mods downloaded!";
        }

        private async Task CheckDownloadStatus(long itemId, string uuid)
        {
            Uri downloadRequestUri = new Uri(TextBoxServerUrl.Text + "prod/api/download/status");

            // Try 10 times for 2 seconds of waiting
            for (int i = 0; i < 10; i++)
            {
                StatusInfo.Content = $"{i+1} - Checking mod ({uuid})";

                HttpResponseMessage response = await httpClient.PostAsync(downloadRequestUri, new StringContent("{\"uuids\": [\"" + uuid + "\"]}", Encoding.UTF8, "application/json"));
                string responseString = await response.Content.ReadAsStringAsync();

                if (responseString.Contains("prepared"))
                {
                    JObject json = JObject.Parse(responseString);
                    string storageNode = (string)json[uuid]["storageNode"];
                    string storagePath = (string)json[uuid]["storagePath"];

                    StatusInfo.Content = $"Ready to start download! {storageNode}/prod/storage/{storagePath}";
                    await DownloadFile(itemId, uuid, storageNode, storagePath);
                    break;
                }

                await Task.Delay(2 * 1000);
            }
        }

        private async Task DownloadFile(long itemId, string uuid, string storageNode, string storagePath)
        {
            StatusInfo.Content = $"Mod is downloading from server ({itemId})";
            Uri downloadRequestUri = new Uri($"https://{storageNode}/prod/storage/{storagePath}?uuid={uuid}");
            HttpResponseMessage response = await httpClient.GetAsync(downloadRequestUri);
            string filePath = Path.Combine(TextBoxModsFolder.Text, $"{itemId}.zip");
            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                await response.Content.CopyToAsync(fs);
                StatusInfo.Content = $"Mod ({itemId}) downloaded!";
            }

            UnzipFile(filePath);
        }

        private async void UnzipFile(string filePath)
        {
            string extractPath = filePath.Replace(".zip", "");

            if (Directory.Exists(extractPath))
                Directory.Delete(extractPath, true);

            Directory.CreateDirectory(extractPath);
            await Task.Run(() => ZipFile.ExtractToDirectory(filePath, extractPath));

            File.Delete(filePath);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DownloadAllMods();
        }
    }
}
