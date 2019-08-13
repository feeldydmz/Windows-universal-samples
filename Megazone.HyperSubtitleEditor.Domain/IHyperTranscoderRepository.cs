using System.Collections.Generic;

namespace Megazone.HyperSubtitleEditor.Domain
{
    public interface IHyperTranscoderRepository
    {
        void GetSubtitles(string jobId);

        void DeploySubtitles(string jobId, IList<object> subtitles);
    }
}