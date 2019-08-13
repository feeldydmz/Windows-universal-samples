using System.Windows.Input;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message.View;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class FindAndReplaceViewModel : ViewModelBase
    {
        private ICommand _enterCommand;
        private ICommand _findAllCommand;
        private int _findCount;
        private string _findText;
        private bool _isFindComplated;
        private ICommand _loadCommand;
        private ICommand _replaceAllCommand;
        private string _replaceText;
        private ICommand _requestFindCountCommand;

        public bool IsFindComplated
        {
            get => _isFindComplated;
            private set => Set(ref _isFindComplated, value);
        }

        public string FindText
        {
            get => _findText;
            set
            {
                Set(ref _findText, value);
                IsFindComplated = false;
                FindCount = 0;
            }
        }

        public string ReplaceText
        {
            get => _replaceText;
            set => Set(ref _replaceText, value);
        }

        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(Load); }
        }

        public ICommand EnterCommand
        {
            get { return _enterCommand = _enterCommand ?? new RelayCommand(RequestFindCount); }
        }

        public ICommand RequestFindCountCommand
        {
            get
            {
                return _requestFindCountCommand =
                    _requestFindCountCommand ?? new RelayCommand(RequestFindCount, CanFind);
            }
        }

        public ICommand FindAllCommand
        {
            get { return _findAllCommand = _findAllCommand ?? new RelayCommand(FindAll); }
        }

        public ICommand ReplaceAllCommand
        {
            get { return _replaceAllCommand = _replaceAllCommand ?? new RelayCommand(ReplaceAll, CanReplace); }
        }

        public int FindCount
        {
            get => _findCount;
            set => Set(ref _findCount, value);
        }

        private void Load()
        {
            FindText = string.Empty;
            ReplaceText = string.Empty;
            FindCount = 0;
            IsFindComplated = false;
        }

        public void RequestFindCount()
        {
            if (string.IsNullOrWhiteSpace(FindText))
                return;

            MessageCenter.Instance.Send(new SubtitleView.RequestFindCountMessage(this, FindText, count =>
            {
                FindCount = count;
                IsFindComplated = true;
            }));
        }

        public bool CanFind()
        {
            return !string.IsNullOrEmpty(FindText);
        }

        public void Find()
        {
            Find(FindText);
        }

        public void FindAll()
        {
            FindAll(FindText);
        }

        public void Find(string findText)
        {
            MessageCenter.Instance.Send(new SubtitleView.FindTextMessage(this, FindText));
            if (!IsFindComplated)
                RequestFindCount();
        }

        public void FindAll(string findText)
        {
            MessageCenter.Instance.Send(new SubtitleView.AllFindTextMessage(this, FindText));
        }

        public bool CanReplace()
        {
            return !string.IsNullOrEmpty(FindText) && !string.IsNullOrEmpty(ReplaceText);
        }

        public void Replace()
        {
            Replace(FindText, ReplaceText);
        }

        public void ReplaceAll()
        {
            ReplaceAll(FindText, ReplaceText);
        }

        public void Replace(string findText, string replaceText)
        {
            MessageCenter.Instance.Send(new SubtitleView.ReplaceTextMessage(this, FindText, ReplaceText));
        }

        public void ReplaceAll(string findText, string replaceText)
        {
            MessageCenter.Instance.Send(new SubtitleView.AllReplaceTextMessage(this, FindText, ReplaceText));
        }
    }
}