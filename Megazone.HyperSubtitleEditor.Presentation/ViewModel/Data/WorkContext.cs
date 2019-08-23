using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal class WorkContext
    {
        public static Video Video { get; private set; }
        public static CaptionAsset Caption { get; private set; }
        public static IEnumerable<Caption> Captions { get; private set; }
        public static bool IsModified { get; private set; }
        public static void SetVideo(Video video)
        {
            Video = video;
        }

        public static void SetCaption(CaptionAsset captionAsset)
        {
            Caption = captionAsset;
        }

        public static void SetCaptions(IEnumerable<Caption> elements)
        {
            Captions = elements;
        }

        public static void Modify()
        {
            IsModified = true;
        }

        public static void Save()
        {
            IsModified = false;
        }
    }
}
