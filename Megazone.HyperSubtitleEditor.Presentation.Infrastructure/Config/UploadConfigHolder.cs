using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config.JsonData;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config
{
    public class UploadConfigHolder : ViewModelBase
    {
        private long _chunkSize;
        private bool _isSecureProtocol;
        private bool _showSetting;
        private int _threadCount;
        private bool _useTransferAcceleration;

        public UploadConfigHolder(UploadJsonData jsonData)
        {
            _threadCount = jsonData.ThreadCount;
            _isSecureProtocol = jsonData.IsSecureProtocol;
            _chunkSize = jsonData.ChunkSize;
            _showSetting = jsonData.ShowSetting;
            _useTransferAcceleration = jsonData.UseTransferAcceleration;
        }

        public int ThreadCount
        {
            get => _threadCount;
            set
            {
                Set(ref _threadCount, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsSecureProtocol
        {
            get => _isSecureProtocol;
            set
            {
                Set(ref _isSecureProtocol, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public long ChunkSize
        {
            get => _chunkSize;
            set
            {
                Set(ref _chunkSize, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool ShowSetting
        {
            get => _showSetting;
            set
            {
                Set(ref _showSetting, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool UseTransferAcceleration
        {
            get => _useTransferAcceleration;
            set
            {
                Set(ref _useTransferAcceleration, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsDirty => !ConfigHolder.Current.Upload.ThreadCount.Equals(ThreadCount) ||
                               !ConfigHolder.Current.Upload.IsSecureProtocol.Equals(IsSecureProtocol) ||
                               !ConfigHolder.Current.Upload.ChunkSize.Equals(ChunkSize) ||
                               !ConfigHolder.Current.Upload.ShowSetting.Equals(ShowSetting) ||
                               !ConfigHolder.Current.Upload.UseTransferAcceleration.Equals(UseTransferAcceleration);

        public UploadJsonData GetJsonData()
        {
            return new UploadJsonData
            {
                ThreadCount = _threadCount,
                IsSecureProtocol = _isSecureProtocol,
                ChunkSize = _chunkSize,
                ShowSetting = _showSetting,
                UseTransferAcceleration = _useTransferAcceleration
            };
        }

        public bool Equal(UploadConfigHolder config)
        {
            return ThreadCount == config.ThreadCount &&
                   IsSecureProtocol == config.IsSecureProtocol &&
                   ChunkSize == config.ChunkSize &&
                   ShowSetting == config.ShowSetting &&
                   UseTransferAcceleration == config.UseTransferAcceleration;
        }
    }
}