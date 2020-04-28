using System;
using System.Collections.Generic;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model
{
    public interface ISubtitleTabItemViewModel : IDisposable
    {
        string Id { get; }
        string Name { get; set; }
        bool IsDirty { get; }
        string LanguageCode { get; set; }
        string CountryCode { get; set; }
        CaptionKind Kind { get; set; }
        IList<ISubtitleListItemViewModel> Rows { get; }
        bool IsSelected { get; set; }

        bool IsDeployedOnce { get; }
        
        Caption Caption { get; }
        string VideoId { get; }
        string CaptionAssetId { get; }
        string FilePath { get; set; }
        SourceLocationKind SourceLocation { get; set; }
        void SetAsDeployed();
        bool CheckDirty();

        void Reset();
    }
}