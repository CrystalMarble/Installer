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
    public partial class InstallLoader : Page
    {
        private bool Ultra;
        private string GamePath;

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

        private string DownloadDoorstop()
        {
            var client = new HttpClient();
            var responseTask = client.GetAsync("https://github.com/NeighTools/UnityDoorstop/releases/download/v4.3.0/doorstop_win_release_4.3.0.zip");
            Debug.WriteLine("Downloading Doorstop...");
            responseTask.Wait();
            string TmpDir = GetTemporaryDirectory();
            var contentTask = responseTask.Result.Content.ReadAsByteArrayAsync();
            contentTask.Wait();
            Debug.WriteLine("Writing Doorstop...");
            File.WriteAllBytes(Path.Combine(TmpDir, "_Doorstop.zip"), contentTask.Result);
            Directory.CreateDirectory(Path.Combine(TmpDir, "_Doorstop"));
            Debug.WriteLine("Opened zip");
            ZipFile.OpenRead(Path.Combine(TmpDir, "_Doorstop.zip")).ExtractToDirectory(Path.Combine(TmpDir, "_Doorstop"), true);
            Dispatcher.Invoke(new Action(() =>
            {
                DataText.Text = $"{DataText.Text}\nSuccessfully downloaded UnityDoorstop!";
            }));
            Debug.WriteLine("Downloaded doorstop (from fn)");
            return Path.Combine(TmpDir, "_Doorstop");
        }

        private string DownloadHarmony()
        {
            var client = new HttpClient();
            var responseTask = client.GetAsync("https://github.com/pardeike/Harmony/releases/download/v2.3.3.0/Harmony-Fat.2.3.3.0.zip");
            Debug.WriteLine("Downloading Harmony...");
            responseTask.Wait();
            string TmpDir = GetTemporaryDirectory();
            var contentTask = responseTask.Result.Content.ReadAsByteArrayAsync();
            contentTask.Wait();
            Debug.WriteLine("Writing Harmony...");
            File.WriteAllBytes(Path.Combine(TmpDir, "_Harmony.zip"), contentTask.Result);
            Directory.CreateDirectory(Path.Combine(TmpDir, "_Harmony"));
            Debug.WriteLine("Opened zip");
            ZipFile.OpenRead(Path.Combine(TmpDir, "_Harmony.zip")).ExtractToDirectory(Path.Combine(TmpDir, "_Harmony"), true);
            Dispatcher.Invoke(new Action(() =>
            {
                DataText.Text = $"{DataText.Text}\nSuccessfully downloaded 0Harmony!";
            }));
            Debug.WriteLine("Downloaded 0Harmony (from fn)");
            return Path.Combine(TmpDir, "_Harmony", "net48");
        }

        public InstallLoader(bool ultra, string gamePath)
        {
            InitializeComponent();
            Ultra = ultra;
            GamePath = gamePath;

            // Update UI to show the process has started
            DataText.Text = $"{DataText.Text}\nStarting installation...";

            // Run the installation process in a background thread
            Task.Run(() =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    DataText.Text = $"{DataText.Text}\nDownloading 0Harmony...";
                }));
                string HarmonyPath = DownloadHarmony();
                // Download Doorstop in a background thread
                string DoorstopPath = DownloadDoorstop();
                // Set up the configuration content
                var DoorstopConfigContent = """
        [General]
        enabled=true
        target_assembly=CrystalMarble.dll

        [UnityMono]
        debug_enabled=true
        debug_address=127.0.0.1:10000
        debug_suspend=false
        """;

                // Copy 0Harmony.dll
                File.Copy(Path.Combine(HarmonyPath, "0Harmony.dll"), Path.Combine(GamePath, "0Harmony.dll"), true);
                Debug.WriteLine("Copied 0Harmony");
                Dispatcher.Invoke(new Action(() =>
                {
                    DataText.Text = $"{DataText.Text}\nInstalled 0Harmony.dll";
                }));

                // Copy winhttp.dll
                File.Copy(Path.Combine(DoorstopPath, "x64", "winhttp.dll"), Path.Combine(GamePath, "winhttp.dll"), true);
                Debug.WriteLine("Copied winhttp");
                Dispatcher.Invoke(new Action(() =>
                {
                    DataText.Text = $"{DataText.Text}\nInstalled winhttp.dll";
                }));

                // Copy .doorstop_version
                File.Copy(Path.Combine(DoorstopPath, "x64", ".doorstop_version"), Path.Combine(GamePath, ".doorstop_version"), true);
                Debug.WriteLine("Copied version");
                Dispatcher.Invoke(new Action(() =>
                {
                    Debug.WriteLine("Copied version");
                    DataText.Text = $"{DataText.Text}\nInstalled .doorstop_version";
                }));

                // Write doorstop_config.ini
                File.WriteAllText(Path.Combine(GamePath, "doorstop_config.ini"), DoorstopConfigContent);
                Debug.WriteLine("Copied config");
                Dispatcher.Invoke(new Action(() =>
                {
                    DataText.Text = $"{DataText.Text}\nInstalled doorstop_config.ini";
                }));

                // Download CrystalMarble loader
                Task<string> CrystalMarblePathTask = GithubAPI.DownloadLatestRelease("CrystalMarble/Loader");
                Debug.WriteLine("Download Release");
                CrystalMarblePathTask.Wait();
                string CrystalMarblePath = CrystalMarblePathTask.Result;
                Debug.WriteLine("Written updated file to " + CrystalMarblePath);
                Directory.CreateDirectory(Path.Combine([CrystalMarblePath, "Loader"]));
                System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine([CrystalMarblePath, "Download.zip"]), Path.Combine([CrystalMarblePath, "Loader"]));
                Debug.WriteLine("Extracted Repo");

                // Update the UI after the download is complete
                Dispatcher.Invoke(new Action(() =>
                {
                    DataText.Text = $"{DataText.Text}\nDownloaded CrystalMarble loader";
                }));
                Debug.WriteLine(CrystalMarblePath);
                // Copy Loader.dll
                File.Copy(Path.Combine(CrystalMarblePath, "Loader", "Loader.dll"), Path.Combine(GamePath, "CrystalMarble.dll"), true);
                Debug.WriteLine("Copied loader");
                Dispatcher.Invoke(new Action(() =>
                {
                    DataText.Text = $"{DataText.Text}\nInstalled CrystalMarble loader";
                }));
            });
        }

    }
}

