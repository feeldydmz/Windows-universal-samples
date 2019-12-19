using System;
using System.Windows;
using System.Windows.Input;
using Megazone.Core;
using Megazone.Core.Log;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Serializable;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class LoadGroupFileCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;
        private readonly FileManager _fileManager;
        private readonly ILogger _logger;
        private readonly MainViewModel _mainViewModel;
        private readonly SignInViewModel _signInViewModel;
        private readonly SubtitleViewModel _subtitleViewModel;

        public LoadGroupFileCommand()
        {
            _logger = Bootstrapper.Container.Resolve<ILogger>();
            _fileManager = Bootstrapper.Container.Resolve<FileManager>();
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
            _mainViewModel = Bootstrapper.Container.Resolve<MainViewModel>();
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
            _signInViewModel = Bootstrapper.Container.Resolve<SignInViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_subtitleViewModel.HasTab)
            {
                var result = _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                    Resource.MSG_LOAD_GROUP_FILE,
                    MessageBoxButton.OKCancel, 
                    Application.Current.MainWindow));

                if (result == MessageBoxResult.Cancel)
                    return;
            }

            try
            {
                var filePath = _fileManager.OpenFile("HyperSubtitleEditor files (.hsg)|*.hsg");

                if (string.IsNullOrEmpty(filePath))
                    return;

                var group = BinarySerialization.ReadFromBinaryFile<SubtitleGroup>(filePath);
                if (!string.IsNullOrEmpty(group.Stage?.Id) && !string.IsNullOrEmpty(group.Project?.Id))
                    if (!_signInViewModel.SelectedStage.Id.Equals(group.Stage?.Id) ||
                        !_signInViewModel.SelectedProject.ProjectId.Equals(group.Project?.Id))
                    {
                        var message = string.Format(Resource.MSG_CHANGE_PROJECT_WARNING, group.Stage.Name, group.Project.Name);
                            
                        if (_browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_WAITING, message,
                                MessageBoxButton.OK, 
                                Application.Current.MainWindow,
                                TextAlignment.Left)) == MessageBoxResult.OK)
                            return;
                    }

                MessageCenter.Instance.Send(new ReinitializeAppContextMessage(this));

                MessageCenter.Instance.Send(new Subtitle.LoadTabsMessage(this, group.Tabs));
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex.Message);
                
                _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_ERROR, 
                    Resource.MSG_FILE_IMPORT_UNKOWN_ERROR,
                    MessageBoxButton.OK,
                    Application.Current.MainWindow));
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}