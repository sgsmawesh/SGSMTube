using SGSMTube_Lib.Views;
using System.Diagnostics;

namespace SGSMTube_Lib.Models
{
    public class MusicTagEditor
    {
        public async Task UpdateMusicTag(string musicFilePath, MusicTagModel tagModel)
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var tFile = TagLib.File.Create(musicFilePath);
                    if (tFile == null)
                        return;
                    tFile.Tag.Album = tagModel.Album;
                    tFile.Tag.AlbumArtists = [tagModel.Artist];
                    if (!string.IsNullOrEmpty(tagModel.Picture))
                        tFile.Tag.Pictures = [new TagLib.Picture(tagModel.Picture)];
                    tFile.Save();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            });
        }
    }


}
