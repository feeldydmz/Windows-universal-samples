using System.Collections.Generic;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal class WorkContext
    {
        public static Video Video { get; private set; }
        public static CaptionAsset Caption { get; private set; }
        public static IEnumerable<CaptionContext> Captions { get; private set; }
        public static bool IsModified { get; private set; }

        public static void SetVideo(Video video)
        {
            Video = video;
        }

        public static void SetCaption(CaptionAsset captionAsset)
        {
            Caption = captionAsset;
        }

        public static void SetCaptions(IEnumerable<CaptionContext> elements)
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

        public class CaptionContext : Caption
        {
            public CaptionContext(string id, bool isDraft, bool isPreferred, string language, string country,
                string kind, string label, string url) : base(id, isDraft, isPreferred, language, country, kind, label,
                url)
            {
            }

            public CaptionContext(Caption caption) : base(caption.Id, caption.IsDraft, caption.IsPreferred,
                caption.Language, caption.Country, caption.Kind, caption.Label, caption.Url)
            {
            }

            public string Text { get; set; }
        }
    }
}