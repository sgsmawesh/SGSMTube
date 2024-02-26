using SGSMTube_Lib.Views;
using System.Diagnostics;

namespace SGSMTube_Lib.Models
{
    public class MusicTagEditor
    {
        public async Task<string> UpdateMusicTag(string musicFilePath, MusicTagModel tagModel)
        {
            return await Task.Run(() =>
             {
                 try
                 {
                     var tFile = TagLib.File.Create(musicFilePath);
                     if (tFile == null)
                         return "FAILED";
                     tFile.Tag.Album = tagModel.Album;
                     tFile.Tag.AlbumArtists = [tagModel.Artist];
                     if (!string.IsNullOrEmpty(tagModel.Picture))
                         tFile.Tag.Pictures = [new TagLib.Picture(tagModel.Picture)];
                     tFile.Save();
                     return "Success";
                 }
                 catch (Exception ex)
                 {
                     Debug.WriteLine(ex);
                     return $"Error - {ex.Message}";
                 }
             });
        }
    }


}
