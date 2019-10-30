using System;
using System.Collections.Generic;
using System.Linq;
using Megazone.Core.VideoTrack;
using Megazone.Core.VideoTrack.Model;
using Megazone.Core.VideoTrack.WebVtt;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    internal class SubtitleListItemViewModel : ViewModelBase, ISubtitleListItemViewModel
    {
        private readonly bool _isInitializing;
        private readonly Action _onDisplayTextChangedAction;
        private readonly Action _validateAction;

        private string _cueSettingData;
        private string _displayText;
        private TimeSpan _duration;
        private TimeSpan _endTime;
        private bool _isDurationValid;
        private bool _isEndTimeValid = true;
        private bool _isNowPlaying;

        private bool _isOverMaxCharacterPerSecond;
        private bool _isOverMaxLinePerCharacters;
        private bool _isOverMaxLines;
        private bool _isSelected;
        private bool _isStartTimeValid = true;
        private int _number;

        private string _originalDisplayText;
        private TimeSpan _startTime;
        private double _textCharsPerSecond;
        private IList<IText> _texts;

        private string _textSingleLineLength;
        private int _textTotalLength;

        // TODO : SubtitleItem의 WebVttInterpreter 부분이 어떠한 파일 포맷인지에 따라 다르게 보여져야 함.
        public SubtitleListItemViewModel(SubtitleItem subtitleItem,
            Action validateAction,
            Action onDisplayTextChangedAction)
        {
            _isInitializing = true;
            _validateAction = validateAction;
            _onDisplayTextChangedAction = onDisplayTextChangedAction;
            OriginalData = subtitleItem;
            StartTime = subtitleItem.StartTime;
            EndTime = subtitleItem.EndTime;
            Texts = subtitleItem.Texts;
            _originalDisplayText = _displayText;
            CueSettingData = subtitleItem.CueSettingData;
            _isInitializing = false;
            AnalyzeText(Duration, Texts);
        }

        private static WebVttParser WebVttParser { get; } = new WebVttParser();

        public string CueSettingData
        {
            get => _cueSettingData;
            set => Set(ref _cueSettingData, value);
        }

        public string TextSingleLineLength
        {
            get => _textSingleLineLength;
            private set => Set(ref _textSingleLineLength, value);
        }

        public SubtitleItem OriginalData { get; }

        public bool IsOverMaxLines
        {
            get => _isOverMaxLines;
            set => Set(ref _isOverMaxLines, value);
        }

        public bool IsOverMaxLinePerCharacters
        {
            get => _isOverMaxLinePerCharacters;
            set => Set(ref _isOverMaxLinePerCharacters, value);
        }

        public int TextTotalLength
        {
            get => _textTotalLength;
            private set => Set(ref _textTotalLength, value);
        }

        public double TextCharsPerSecond
        {
            get => _textCharsPerSecond;
            private set => Set(ref _textCharsPerSecond, value);
        }

        public bool IsStartTimeValid
        {
            get => _isStartTimeValid;
            set => Set(ref _isStartTimeValid, value);
        }

        public bool IsEndTimeValid
        {
            get => _isEndTimeValid;
            set => Set(ref _isEndTimeValid, value);
        }

        public int Number
        {
            get => _number;
            set => Set(ref _number, value);
        }

        public TimeSpan StartTime
        {
            get => _startTime;
            set
            {
                Set(ref _startTime, value);
                RefreshDuration();
                AnalyzeText(Duration, Texts);
            }
        }

        public TimeSpan EndTime
        {
            get => _endTime;
            set
            {
                Set(ref _endTime, value);
                RefreshDuration();
                AnalyzeText(Duration, Texts);
            }
        }

        public TimeSpan Duration
        {
            get => _duration;
            set
            {
                Set(ref _duration, value);
                ResetEndTime();
            }
        }

        public IList<IText> Texts
        {
            get => _texts;
            set
            {
                Set(ref _texts, value);
                DisplayText = WebVttParser.ToTextLine(value);
                AnalyzeText(Duration, value);
            }
        }

        public string DisplayText
        {
            get => _displayText;
            set
            {
                var isChanged = _displayText != value;
                Set(ref _displayText, value);
                if (isChanged && !_isInitializing)
                    _onDisplayTextChangedAction?.Invoke();
            }
        }

        public bool IsNowPlaying
        {
            get => _isNowPlaying;
            set => Set(ref _isNowPlaying, value);
        }

        public void CheckIsNowPlaying(TimeSpan position)
        {
            IsNowPlaying = _startTime <= position && _endTime >= position;
        }

        public void ResetDirtyCheckFlags()
        {
            OriginalData.StartTime = _startTime;
            OriginalData.EndTime = _endTime;
            _originalDisplayText = _displayText;
        }

        public bool IsDirty()
        {
            return _startTime != OriginalData.StartTime ||
                   _endTime != OriginalData.EndTime ||
                   _displayText != _originalDisplayText;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public bool IsOverMaxCharacterPerSecond
        {
            get => _isOverMaxCharacterPerSecond;
            set => Set(ref _isOverMaxCharacterPerSecond, value);
        }

        public bool IsDurationValid
        {
            get => _isDurationValid;
            set => Set(ref _isDurationValid, value);
        }

        private void AnalyzeText(TimeSpan duration, IList<IText> texts)
        {
            if (_isInitializing)
                return;

            var normalText = WebVttParser.GetNormalTexts(texts);

            if (string.IsNullOrEmpty(normalText))
            {
                TextTotalLength = 0;
                TextSingleLineLength = "0";
                TextCharsPerSecond = 0;
                return;
            }

            TextTotalLength = normalText.Replace("<br/>", "").Length;
            var splitedText = normalText.Split(new[]
            {
                "<br/>"
            }, StringSplitOptions.None);
            TextSingleLineLength = string.Empty;
            foreach (var text in splitedText)
                TextSingleLineLength += text.Length + "/";
            TextSingleLineLength = TextSingleLineLength.Substring(0, TextSingleLineLength.Length - 1);

            var maxCharactersPerSecond = ConfigHolder.Current.Subtitle.MaxCharactersPerSecond;
            var totalSeconds = duration.TotalSeconds;
            TextCharsPerSecond = Math.Round(TextTotalLength / totalSeconds, 3);

            IsOverMaxCharacterPerSecond = TextCharsPerSecond < maxCharactersPerSecond;
        }

        private void RefreshDuration()
        {
            var tempDuration = EndTime - StartTime;
            if (tempDuration.TotalMilliseconds < 0)
                tempDuration = TimeSpan.Zero;
            Duration = tempDuration;
        }

        private void ResetEndTime()
        {
            var tempDuration = _endTime - _startTime;
            if (_duration != tempDuration)
                EndTime = StartTime + _duration;
            if (!_isInitializing)
                _validateAction?.Invoke();
        }

        public override string ToString()
        {
            return
                $"{Number}\r\n{StartTime:hh\\:mm\\:ss\\.fff} --> {EndTime:hh\\:mm\\:ss\\.fff}\r\n{WebVttParser.ToTextLine(Texts)}";
        }

        internal SubtitleListItemViewModel Copy()
        {
            var subtitle = new SubtitleItem(StartTime, EndTime, Texts?.ToList(), CueSettingData);
            return new SubtitleListItemViewModel(subtitle, _validateAction, _onDisplayTextChangedAction);
        }
    }
}