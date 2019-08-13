using System;
using System.Collections.Generic;
using Megazone.Api.Transcoder.Domain;
using Megazone.Core.VideoTrack.Model;

namespace Megazone.HyperSubtitleEditor.Presentation.Serializable
{
    [Serializable]
    internal class SubtitleTabItem
    {
        public string Name { get; set; }
        public TrackKind Kind { get; set; }
        public string LanguageCode { get; set; }
        public IList<SubtitleItem> Rows { get; set; }
        public bool IsSelected { get; set; }
        public Track Track { get; set; }
        public string FilePath { get; set; }
    }
}