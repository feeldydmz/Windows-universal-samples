using System.Collections.Generic;
using Megazone.HyperSubtitleEditor.Domain.Subtitle;

namespace Megazone.HyperSubtitleEditor.Service.Subtitle
{
    public interface IWebVttInterpreter
    {
        string ConvertITextsToInsideString(IList<IText> texts);
        string ConvertITextsToOutsideString(IList<IText> texts);
        IList<IText> ConvertToStringToITexts(string inputText);
    }
}