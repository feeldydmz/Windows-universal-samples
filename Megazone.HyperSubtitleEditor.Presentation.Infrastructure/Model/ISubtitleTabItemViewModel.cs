using System.Collections.Generic;
using Megazone.Api.Transcoder.Domain;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model
{
    public interface ISubtitleTabItemViewModel
    {
        string Id { get; }
        string Name { get; set; }
        bool IsDirty { get; }
        string LanguageCode { get; set; }
        TrackKind Kind { get; set; }
        IList<ISubtitleListItemViewModel> Rows { get; }
        bool IsSelected { get; set; }

        bool IsDeployedOnce { get; }

        Track Track { get; }

        string FilePath { get; set; }

        void SetAsDeployed();

        bool CheckDirty();
    }
}