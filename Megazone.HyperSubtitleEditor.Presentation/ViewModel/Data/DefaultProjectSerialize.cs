using System;
using Newtonsoft.Json;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    [Serializable]
    internal class DefaultProjectSerialize
    {
        [JsonProperty] public string StageId { get; set; }

        [JsonProperty] public string ProjectId { get; set; }
    }
}