using System.Collections.Generic;

namespace Megazone.HyperSubtitleEditor.Domain.Subtitle.Model
{
    public class Bold : ITag
    {
        public IList<IText> Children { get; set; }
    }
}