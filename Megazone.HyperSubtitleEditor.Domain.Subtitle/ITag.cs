using System.Collections.Generic;

namespace Megazone.HyperSubtitleEditor.Domain.Subtitle
{
    public interface ITag : IText
    {
        IList<IText> Children { get; set; }
    }
}