using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Diagnostics;

namespace Installer
{
    /// <summary>
    /// Interaction logic for InstallLoader.xaml
    /// </summary>
    public partial class InstallMod: Page
    {
        private bool Ultra;
        private string GamePath;
        private Mod CurrentMod;

        public string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            if (File.Exists(tempDirectory))
            {
                return GetTemporaryDirectory();
            }
            else
            {
                Directory.CreateDirectory(tempDirectory);
                return tempDirectory;
            }
        }

        public InstallMod(bool ultra, string gamePath, Mod mod)
        {
            InitializeComponent();
            Ultra = ultra;
            GamePath = gamePath;
            CurrentMod = mod;
            // Update UI to show the process has started
            DataText.Text = $"{DataText.Text}\nStarting installation...";

            // Run the installation process in a background thread
            Task.Run(() =>
            {

                // Download Mod
                Task<string> CrystalMarblePathTask = GithubAPI.DownloadLatestRelease(CurrentMod.Repo);
                Debug.WriteLine("Download Release");
                CrystalMarblePathTask.Wait();
                string ModPath = CrystalMarblePathTask.Result;
                Debug.WriteLine("Written updated file to " + ModPath);
                Directory.CreateDirectory(Path.Combine([ModPath, CurrentMod.Id]));
                System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine([ModPath, "Download.zip"]), Path.Combine([ModPath, CurrentMod.Id]));
                Debug.WriteLine("Extracted Repo");

                // Update the UI after the download is complete
                Dispatcher.Invoke(new Action(() =>
                {
                    DataText.Text = $"{DataText.Text}\nDownloaded " + CurrentMod.Name;
                }));
                Debug.WriteLine(ModPath);
                // Copy Loader.dll
                if (!Directory.Exists(Path.Combine(GamePath, "Mods")))
                {
                    Directory.CreateDirectory(Path.Combine(GamePath, "Mods"));
                }
                if (!Directory.Exists(Path.Combine(GamePath, "Mods", CurrentMod.Id)))
                {
                    Directory.CreateDirectory(Path.Combine(GamePath, "Mods", CurrentMod.Id));
                }
                File.Copy(Path.Combine(ModPath, CurrentMod.Id, CurrentMod.MainFile), Path.Combine(GamePath, "Mods", CurrentMod.Id, CurrentMod.MainFile), true);
                Debug.WriteLine("Copied mod");
                Dispatcher.Invoke(new Action(() =>
                {
                    DataText.Text = $"{DataText.Text}\nInstalled Mod " + CurrentMod.Name;
                }));
            });
        }

    }
}

