using SGSMTube.Pages;
using System.Windows;

namespace SGSMTube
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnYtMp3_Click(object sender, RoutedEventArgs e)
        {
            _mainFrame.NavigationService.Navigate(new YoutubeToMp3());
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _mainFrame.NavigationService.Navigate(new YoutubeToMp3());
        }
    }
}