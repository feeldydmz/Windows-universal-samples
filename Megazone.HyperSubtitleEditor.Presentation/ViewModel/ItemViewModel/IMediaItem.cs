using System.Windows.Media.Imaging;
using Megazone.Core.Windows.Control.VideoPlayer;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    internal interface IMediaItem
    {
        string Url { get; }
        void SetMediaHeaderData(MediaHeaderData mediaData);
        void SetThumbnail(BitmapSource thumbnail);
    }
}