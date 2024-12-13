using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Installer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            navFrame.Navigate(new Uri("MainPage.xaml", UriKind.Relative));
            DataContext = this; // Set DataContext to the current instance
        }

        
    }
}