using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    [Serializable]
    class DefaultProjectItem
    {
        public DefaultProjectItem()
        {
        }

        [JsonProperty]
        public string StageId { get; set; }

        [JsonProperty]
        public string ProjectId { get; set; }
    }
}
