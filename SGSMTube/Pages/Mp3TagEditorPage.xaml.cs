using SGSMTube_Lib.Models;
using SGSMTube_Lib.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SGSMTube.Pages
{
    /// <summary>
    /// Interaction logic for Mp3TagEditorPage.xaml
    /// </summary>
    public partial class Mp3TagEditorPage : Page
    {
        public Mp3TagEditorPage()
        {
            InitializeComponent();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string path = txtMusicPath.Text;
            if (string.IsNullOrEmpty(path))
                return;

            var musicFiles = System.IO.Directory.EnumerateFiles(path, "*.*", System.IO.SearchOption.AllDirectories).Where(x => x.ToLower().EndsWith(".mp3") || x.ToLower().EndsWith(".m4a"));

            if (musicFiles == null)
            {
                MessageBox.Show("No files found at this Path!");
                return;
            }
            btnSave.IsEnabled = false;
            int filesCount = musicFiles.ToList().Count;

            MusicTagEditor editor = new MusicTagEditor();
            MusicTagModel model = new MusicTagModel() { Album = txtAlbum.Text, Artist = txtArtist.Text, Picture = txtAlbumArt.Text };
            prgProgress.Maximum = filesCount;
            int counter = 1;
            foreach (var item in musicFiles)
            {
                await editor.UpdateMusicTag(item, model);
                prgProgress.Value = counter;
                counter++;
                await Task.Delay(100);
            }
            prgProgress.Value = 0;
            btnSave.IsEnabled = true;

        }
    }
}
