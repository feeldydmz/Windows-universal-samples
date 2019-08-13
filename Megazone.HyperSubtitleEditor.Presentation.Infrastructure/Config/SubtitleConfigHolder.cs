using System;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config.JsonData;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config
{
    public class SubtitleConfigHolder : ViewModelBase
    {
        private int _maxCharactersPerSecond;
        private TimeSpan _maxDuration;
        private int _maxLines;
        private int _mediaBufferingSeconds;
        private TimeSpan _minDuration;
        private TimeSpan _minGap;
        private int _singleLineMaxBytes;
        private TimeSpan _startEndTimeTick;

        public SubtitleConfigHolder(SubtitleJsonData jsonData)
        {
            SingleLineMaxBytes = jsonData.SingleLineMaxBytes;
            MaxLines = jsonData.MaxLines;
            MaxCharactersPerSecond = jsonData.MaxCharactersPerSecond;
            StartEndTimeTick = TimeSpan.FromMilliseconds(jsonData.StartEndTimeTickMilliseconds);
            MinDuration = TimeSpan.FromMilliseconds(jsonData.MinDurationMilliseconds);
            MaxDuration = TimeSpan.FromMilliseconds(jsonData.MaxDurationMilliseconds);
            MinGap = TimeSpan.FromMilliseconds(jsonData.MinGapMilliseconds);
            MediaBufferingSeconds = jsonData.MediaBufferingSeconds;
        }

        public int MediaBufferingSeconds
        {
            get => _mediaBufferingSeconds;
            set
            {
                Set(ref _mediaBufferingSeconds, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public TimeSpan MinDuration
        {
            get => _minDuration;
            set
            {
                Set(ref _minDuration, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public TimeSpan MaxDuration
        {
            get => _maxDuration;
            set
            {
                Set(ref _maxDuration, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public TimeSpan MinGap
        {
            get => _minGap;
            set
            {
                Set(ref _minGap, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public int SingleLineMaxBytes
        {
            get => _singleLineMaxBytes;
            set
            {
                Set(ref _singleLineMaxBytes, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }


        public int MaxCharactersPerSecond
        {
            get => _maxCharactersPerSecond;
            set
            {
                Set(ref _maxCharactersPerSecond, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public int MaxLines
        {
            get => _maxLines;
            set
            {
                Set(ref _maxLines, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public TimeSpan StartEndTimeTick
        {
            get => _startEndTimeTick;
            set
            {
                Set(ref _startEndTimeTick, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsDirty => ConfigHolder.Current.Subtitle.SingleLineMaxBytes != SingleLineMaxBytes ||
                               ConfigHolder.Current.Subtitle.MaxLines != MaxLines ||
                               ConfigHolder.Current.Subtitle.MaxCharactersPerSecond != MaxCharactersPerSecond ||
                               ConfigHolder.Current.Subtitle.StartEndTimeTick !=
                               StartEndTimeTick ||
                               ConfigHolder.Current.Subtitle.MinDuration != MinDuration ||
                               ConfigHolder.Current.Subtitle.MaxDuration != MaxDuration ||
                               ConfigHolder.Current.Subtitle.MediaBufferingSeconds != MediaBufferingSeconds ||
                               ConfigHolder.Current.Subtitle.MinGap != MinGap;

        public SubtitleJsonData GetJsonData()
        {
            return new SubtitleJsonData
            {
                SingleLineMaxBytes = SingleLineMaxBytes,
                MaxLines = MaxLines,
                MaxCharactersPerSecond = MaxCharactersPerSecond,
                StartEndTimeTickMilliseconds = StartEndTimeTick.TotalMilliseconds,
                MinDurationMilliseconds = MinDuration.TotalMilliseconds,
                MaxDurationMilliseconds = MaxDuration.TotalMilliseconds,
                MinGapMilliseconds = MinGap.TotalMilliseconds,
                MediaBufferingSeconds = MediaBufferingSeconds
            };
        }

        public bool Equal(SubtitleConfigHolder config)
        {
            return SingleLineMaxBytes == config.SingleLineMaxBytes &&
                   MaxCharactersPerSecond == config.MaxCharactersPerSecond &&
                   MaxLines == config.MaxLines &&
                   MinDuration == config.MinDuration &&
                   MaxDuration == config.MaxDuration &&
                   StartEndTimeTick == config.StartEndTimeTick &&
                   MediaBufferingSeconds == config.MediaBufferingSeconds &&
                   MinGap == config.MinGap;
        }
    }
}