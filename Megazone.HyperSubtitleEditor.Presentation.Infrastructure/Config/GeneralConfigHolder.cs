using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config.JsonData;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config
{
    public class GeneralConfigHolder : ViewModelBase
    {
        private int _connectionTimeout;
        private bool _isAutoLogin;
        private bool _isRetry;
        private bool _isShowOnlyMediaFile;
        private int _retryCount;
        private int _retryInterval;

        public GeneralConfigHolder(GeneralJsonData jsonData)
        {
            _isAutoLogin = jsonData.IsAutoLogin;
            _connectionTimeout = jsonData.ConnectionTimeout;
            _retryCount = jsonData.RetryCount;
            _retryInterval = jsonData.RetryInterval;
            _isRetry = jsonData.IsRetry;
            _isShowOnlyMediaFile = jsonData.IsShowOnlyMediaFile;
        }

        public bool IsAutoLogin
        {
            get => _isAutoLogin;
            set
            {
                Set(ref _isAutoLogin, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public int ConnectionTimeout
        {
            get => _connectionTimeout;
            set
            {
                Set(ref _connectionTimeout, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public int RetryCount
        {
            get => _retryCount;
            set
            {
                Set(ref _retryCount, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public int RetryInterval
        {
            get => _retryInterval;
            set
            {
                Set(ref _retryInterval, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsRetry
        {
            get => _isRetry;
            set
            {
                Set(ref _isRetry, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsShowOnlyMediaFile
        {
            get => _isShowOnlyMediaFile;
            set
            {
                Set(ref _isShowOnlyMediaFile, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsDirty => !ConfigHolder.Current.General.IsAutoLogin.Equals(IsAutoLogin) ||
                               !ConfigHolder.Current.General.ConnectionTimeout.Equals(ConnectionTimeout) ||
                               !ConfigHolder.Current.General.RetryCount.Equals(RetryCount) ||
                               !ConfigHolder.Current.General.RetryInterval.Equals(RetryInterval) ||
                               !ConfigHolder.Current.General.IsRetry.Equals(IsRetry) ||
                               !ConfigHolder.Current.General.IsShowOnlyMediaFile.Equals(IsShowOnlyMediaFile);

        public GeneralJsonData GetJsonData()
        {
            return new GeneralJsonData
            {
                IsAutoLogin = _isAutoLogin,
                ConnectionTimeout = _connectionTimeout,
                RetryCount = _retryCount,
                RetryInterval = _retryInterval,
                IsRetry = _isRetry,
                IsShowOnlyMediaFile = _isShowOnlyMediaFile
            };
        }

        public bool Equal(GeneralConfigHolder config)
        {
            return IsAutoLogin == config.IsAutoLogin &&
                   ConnectionTimeout == config.ConnectionTimeout &&
                   RetryCount == config.RetryCount &&
                   RetryInterval == config.RetryInterval &&
                   IsRetry == config.IsRetry &&
                   IsShowOnlyMediaFile == config.IsShowOnlyMediaFile;
        }
    }
}