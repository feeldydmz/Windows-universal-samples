﻿using System.Collections.Generic;
using Megazone.Api.Transcoder.Domain;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;

namespace Megazone.HyperSubtitleEditor.Presentation.Message.Parameter
{
    internal class CopyTabMessageParameter
    {
        public string Label { get; set; }
        public IList<ISubtitleListItemViewModel> Rows { get; set; }
        public string LanguageCode { get; set; }
        public TrackKind Kind { get; set; }
    }
}