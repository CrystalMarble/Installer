using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Installer
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page, INotifyPropertyChanged
    {
        private string _gamePath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Marble it Up!";

        public string GamePath
        {
            get => _gamePath;
            set
            {
                if (_gamePath != value)
                {
                    _gamePath = value;
                    OnPropertyChanged(nameof(GamePath));  // Notify UI of change
                }
            }
        }

        public MainPage()
        {
            InitializeComponent();
            DataContext = this; // Set DataContext to the current instance
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select the game installation folder",
                InitialDirectory = GamePath
            };

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                GamePath = folderDialog.FileName;  // Update the GamePath property
            }
        }

        private bool IsUltraPicked()
        {
            if (UltraRadio.IsPressed && !ClassicRadio.IsPressed)
            {
                return true;
            }
            return false;
        }
        private void InstallMods_Click(object sender, RoutedEventArgs e)
        {
            bool ultra = IsUltraPicked();
            NavigationService.Navigate(new ModsList(ultra, GamePath));
        }

        private void InstallLoader_Click(object sender, RoutedEventArgs e)
        {

            bool ultra = IsUltraPicked();
            NavigationService.Navigate(new InstallLoader(ultra, GamePath));
        }
    }
}
