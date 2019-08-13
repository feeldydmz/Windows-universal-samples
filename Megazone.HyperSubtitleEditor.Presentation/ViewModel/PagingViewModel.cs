using System;
using System.Windows.Input;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    internal class PagingViewModel : ViewModelBase
    {
        private readonly Action _onNextAction;
        private readonly Action _onPreviousAction;
        private bool _hasNext;
        private bool _hasPrevious;
        private bool _isPagingEnabled;
        private ICommand _nextCommand;
        private ICommand _previousCommand;

        public PagingViewModel(Action onNextAction, Action onPreviousAction)
        {
            _onNextAction = onNextAction;
            _onPreviousAction = onPreviousAction;
        }

        public bool IsPagingEnabled
        {
            get => _isPagingEnabled;
            set => Set(ref _isPagingEnabled, value);
        }

        public bool HasPrevious
        {
            get => _hasPrevious;
            set => Set(ref _hasPrevious, value);
        }

        public bool HasNext
        {
            get => _hasNext;
            set => Set(ref _hasNext, value);
        }

        public ICommand NextCommand
        {
            get { return _nextCommand = _nextCommand ?? new RelayCommand(OnNext); }
        }

        public ICommand PreviousCommand
        {
            get { return _previousCommand = _previousCommand ?? new RelayCommand(OnPrevious); }
        }

        private void OnNext()
        {
            _onNextAction?.Invoke();
        }

        private void OnPrevious()
        {
            _onPreviousAction?.Invoke();
        }
    }
}