using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config.JsonData;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config
{
    public class DownloadConfigHolder : ViewModelBase
    {
        private bool _autoSuccessfullyCompletedQueueClear;
        private long _chunkSize;
        private bool _isSecureProtocol;
        private int _threadCount;
        private bool _useTransferAcceleration;

        public DownloadConfigHolder(DownloadJsonData jsonData)
        {
            _threadCount = jsonData.ThreadCount;
            _isSecureProtocol = jsonData.IsSecureProtocol;
            _chunkSize = jsonData.ChunkSize;
            _autoSuccessfullyCompletedQueueClear = jsonData.AutoSuccessfullyCompletedQueueClear;
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

        public bool AutoSuccessfullyCompletedQueueClear
        {
            get => _autoSuccessfullyCompletedQueueClear;
            set
            {
                Set(ref _autoSuccessfullyCompletedQueueClear, value);
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

        public bool IsDirty => !ConfigHolder.Current.Download.ThreadCount.Equals(ThreadCount) ||
                               !ConfigHolder.Current.Download.IsSecureProtocol.Equals(IsSecureProtocol) ||
                               !ConfigHolder.Current.Download.ChunkSize.Equals(ChunkSize) ||
                               !ConfigHolder.Current.Download.AutoSuccessfullyCompletedQueueClear.Equals(
                                   AutoSuccessfullyCompletedQueueClear) ||
                               !ConfigHolder.Current.Download.UseTransferAcceleration.Equals(UseTransferAcceleration);

        public DownloadJsonData GetJsonData()
        {
            return new DownloadJsonData
            {
                ThreadCount = _threadCount,
                IsSecureProtocol = _isSecureProtocol,
                ChunkSize = _chunkSize,
                AutoSuccessfullyCompletedQueueClear = _autoSuccessfullyCompletedQueueClear,
                UseTransferAcceleration = _useTransferAcceleration
            };
        }

        public bool Equal(DownloadConfigHolder config)
        {
            return ThreadCount == config.ThreadCount &&
                   IsSecureProtocol == config.IsSecureProtocol &&
                   ChunkSize == config.ChunkSize &&
                   AutoSuccessfullyCompletedQueueClear == config.AutoSuccessfullyCompletedQueueClear &&
                   UseTransferAcceleration == config.UseTransferAcceleration;
        }
    }
}