using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Net.Http;
using System.IO;

namespace Installer
{


    class GithubAsset
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("browser_download_url")]
        public string DownloadUrl { get; set; }
    }

    class GitHubRelease
    {
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }

        [JsonPropertyName("assets")]
        public GithubAsset[] Assets { get; set; }
    }
    class GithubAPI
    {
        public static void OpenGithubLink(string link)
        {
            Debug.WriteLine($"https://github.com/{link}");
            // Logic to navigate to the repository URL
            Process.Start(new ProcessStartInfo
            {
                FileName = $"https://github.com/{link}", // Replace with actual repo URL
                UseShellExecute = true
            });
        }

        public static string GetTemporaryDirectory()
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

        public static async Task<string> DownloadLatestRelease(string Repo)
        {
            /// < summary>
            /// Downloads latest artifact and saves it to Download.zip in the returned temporary path
            /// </summary>
            string TempDir = GetTemporaryDirectory();

            string GithubApiUrl = $"https://api.github.com/repos/{Repo}/releases/latest";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("request");
            var response = await client.GetStringAsync(GithubApiUrl);
            var release = JsonSerializer.Deserialize<GitHubRelease>(response);

            HttpResponseMessage resp = await client.GetAsync(release.Assets[0].DownloadUrl);
            resp.EnsureSuccessStatusCode();
            File.WriteAllBytes(Path.Combine([TempDir, "Download.zip"]), await resp.Content.ReadAsByteArrayAsync());
            
            return TempDir;
        }

        public static async Task<string> DownloadFileFromRepo(string Repo, string Path, string Branch = "main")
        {
            string RepoUrl = $"https://raw.githubusercontent.com/{Repo}/refs/heads/{Branch}/{Path}";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("request");
            var response = await client.GetStringAsync(RepoUrl);
            return response;
        }
    }
}
