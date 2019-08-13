using System.Collections.Generic;

namespace Megazone.HyperSubtitleEditor.Domain.Subtitle.Model
{
    public class Italic : ITag
    {
        public IList<IText> Children { get; set; }
    }
}