using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;

namespace Megazone.HyperSubtitleEditor.Presentation.Recently
{
    public class RecentlyItem
    {
        public bool IsOffline { get; set; }
        public Video Video { get; set; }
        public CaptionAsset CaptionAsset { get; set; }
        public IEnumerable<Caption> Captions { get; set; }
    }
}
