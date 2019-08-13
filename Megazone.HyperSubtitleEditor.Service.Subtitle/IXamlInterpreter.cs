using System.Collections.Generic;
using Megazone.HyperSubtitleEditor.Domain.Subtitle;

namespace Megazone.HyperSubtitleEditor.Service.Subtitle
{
    public interface IXamlInterpreter
    {
        string ConvertITextsToString(IList<IText> texts);

        IList<IText> ConvertToStringToITexts(string inputText);
    }
}