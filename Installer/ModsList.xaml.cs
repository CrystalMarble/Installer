using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Installer
{
    public class Mod
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("repo")]
        public string Repo { get; set; }

        [JsonPropertyName("owner")]
        public string Owner { get; set; }

        [JsonPropertyName("main")]
        public string MainFile { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }

        public ICommand OpenGithubLinkCommand { get; set; }
    }

    public partial class ModsList : Page, INotifyPropertyChanged
    {
        private string _gamePath;
        private bool _ultra;
        private ObservableCollection<Mod> _mods;

        public ICommand OpenGithubLinkCommand { get; }

        public ObservableCollection<Mod> Mods
        {
            get => _mods;
            set
            {
                _mods = value;
                OnPropertyChanged(nameof(Mods));
            }
        }

        public ModsList(bool ultra, string gamePath)
        {
            InitializeComponent();
            _gamePath = gamePath;
            _ultra = ultra;
            Mods = new ObservableCollection<Mod>();
            OpenGithubLinkCommand = new RelayCommand<string>(GithubAPI.OpenGithubLink);
            LoadModsAsync();
            DataContext = this;
        }

        private async void LoadModsAsync()
        {
            try
            {
                string fileData = await GithubAPI.DownloadFileFromRepo("CrystalMarble/ModsRepo", "ultra.json");
                var mods = JsonSerializer.Deserialize<Mod[]>(fileData);

                // Update the collection on the UI thread.
                await Dispatcher.InvokeAsync(() =>
                {
                    foreach (var mod in mods)
                    {
                        Debug.WriteLine(mod.Name);
                        mod.OpenGithubLinkCommand = new RelayCommand<string>(GithubAPI.OpenGithubLink);
                        Mods.Add(mod);
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load mods: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button s = sender as Button;
            if (s == null)
            {
                Debug.WriteLine("No button?");
                return;
            }
            Mod dataContext = s.DataContext as Mod;
            if (dataContext == null)
            {
                Debug.WriteLine("No DataContext?");
                return;
            }
            NavigationService.Navigate(new InstallMod(_ultra, _gamePath, dataContext));
        }
    }
}
