﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    public class VideoResolutionInfo
    {
        public VideoResolutionInfo(int width, int height, string codec)
        {
            Width = width;
            Height = height;
            Codec = codec;
        }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Codec { get; set; }

        public string DisplayString => $"{Codec}, {Width}x{Height}";
    }
}