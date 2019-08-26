﻿using Megazone.Api.Transcoder.Domain;
using Megazone.Cloud.Media.Domain.Assets;

namespace Megazone.HyperSubtitleEditor.Presentation.Message.Parameter
{
    internal class FileOpenedMessageParameter
    {
        public string FilePath { get; set; }
        public string Label { get; set; }
        public string Text { get; set; }
        public string LanguageCode { get; set; }
        public TrackKind Kind { get; set; }
    }
}