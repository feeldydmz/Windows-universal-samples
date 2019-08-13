using System.Windows.Input;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message.View;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class GoToLineViewModel : ViewModelBase
    {
        private ICommand _goToLineCommand;
        private int _lineNumber;
        private int _maximumNumber;
        private int _minimumNumber;

        public int LineNumber
        {
            get => _lineNumber;
            set => Set(ref _lineNumber, value);
        }

        public ICommand GoToLineCommand
        {
            get { return _goToLineCommand = _goToLineCommand ?? new RelayCommand(GoToLine); }
        }

        public int MinimumNumber
        {
            get => _minimumNumber;
            private set => Set(ref _minimumNumber, value);
        }

        public int MaximumNumber
        {
            get => _maximumNumber;
            internal set
            {
                Set(ref _maximumNumber, value);
                if (value > 0)
                    MinimumNumber = 1;
            }
        }

        private void GoToLine()
        {
            MessageCenter.Instance.Send(new SubtitleView.GoToLineNumberMessage(this, LineNumber));
        }
    }
}