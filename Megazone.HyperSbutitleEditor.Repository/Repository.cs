using System.Collections.Generic;
using Megazone.HyperSubtitleEditor.Domain;

namespace Megazone.HyperSbutitleEditor.Repository
{
    /// <summary>
    ///     Megazone Repository
    /// </summary>
    public class Repository : IHyperTranscoderRepository
    {
        public void GetSubtitles(string jobId)
        {
            throw new System.NotImplementedException();
        }

        public void DeploySubtitles(string jobId, IList<object> subtitles)
        {
            throw new System.NotImplementedException();
        }
    }
}