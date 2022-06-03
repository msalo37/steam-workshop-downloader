using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkshopDownloader.Core.ZipUtils
{
    public static class Unzipper
    {
        public static async void UnzipFileAsync(string filePath, bool createFolder = true)
        {
            string extractPath = filePath.Replace(".zip", "");

            if (createFolder)
            {
                Directory.CreateDirectory(extractPath);
                if (Directory.Exists(extractPath)) Directory.Delete(extractPath, true);
            }
            else
            {
                string[] t = extractPath.Split('\\');
                extractPath = string.Empty;
                for (int i = 0; i < t.Length; i++)
                {
                    if (i == t.Length - 1) break;
                    extractPath += t[i] + "\\";
                }
            }

            await Task.Run(() => ZipFile.ExtractToDirectory(filePath, extractPath));
            
            File.Delete(filePath);
        }
    }
}
