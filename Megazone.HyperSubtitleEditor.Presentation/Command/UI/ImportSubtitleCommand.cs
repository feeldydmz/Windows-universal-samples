using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class ImportSubtitleCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;
        private readonly SubtitleViewModel _subtitleViewModel;
        private readonly WorkBarViewModel _workBarViewModel;

        public ImportSubtitleCommand()
        {
            _workBarViewModel = Bootstrapper.Container.Resolve<WorkBarViewModel>();
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
        }

        public bool CanExecute(object parameter)
        {
            return _workBarViewModel.CanImportFile();
        }

        public void Execute(object parameter)
        {
            if (parameter is string content)
            {
                if (content == Resource.CNT_IMPORT_CAPTION)
                    //_subtitleViewModel.OnImportSubtitleFile();
                    _browser.Main.ShowOpenSubtitleDialog();
                else if (content == Resource.CNT_IMPORT_EXCEL) _browser.Main.ShowImportExcelDialog();
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}