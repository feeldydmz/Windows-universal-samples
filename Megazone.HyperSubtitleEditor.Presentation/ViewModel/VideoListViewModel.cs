using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    class VideoListViewModel
    {
        private readonly ICloudMediaService _cloudMediaService;
        public VideoListViewModel(ICloudMediaService cloudMediaService)
        {
            _cloudMediaService = cloudMediaService;
        }
    }
}
