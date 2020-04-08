using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Megazone.Core.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class CreateNewCommand : DependencyObject, ICommand
    {
        private IBrowser _browser;
        private readonly WorkBarViewModel _workBar;

        public CreateNewCommand()
        {
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
            _workBar = Bootstrapper.Container.Resolve<WorkBarViewModel>();
        }
        public bool CanExecute(object parameter)
        {
            return _workBar.HasWorkData;
        }

        public void Execute(object parameter)
        {
            var result = _browser.Main.ShowCreateWorkspaceConfirmWindow();

            if (!result.HasValue) return;

            if (result.Value)
            {
                var fileFullPath = $"{AppDomain.CurrentDomain.BaseDirectory}{this.GetApplicationName()}";
                if (File.Exists(fileFullPath))
                    Process.Start(fileFullPath);
            }
            else
            {
                MessageCenter.Instance.Send(new Message.SubtitleEditor.CleanUpSubtitleMessage(this));
            }

            //var result = _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
            //    Resource.MSG_CREATE_NEW,
            //    MessageBoxButton.YesNoCancel,
            //    Application.Current.MainWindow,
            //    TextAlignment.Center));

            //switch (result)
            //{
            //    case MessageBoxResult.Yes:
            //        var fileFullPath = $"{AppDomain.CurrentDomain.BaseDirectory}{this.GetApplicationName()}";
            //        if (File.Exists(fileFullPath))
            //            Process.Start(fileFullPath);
            //        break;
            //    case MessageBoxResult.No:
            //        MessageCenter.Instance.Send(new Message.SubtitleEditor.CleanUpSubtitleMessage(this));
            //        break;
            //    case MessageBoxResult.Cancel:
            //    default:
            //        break;
            //}
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}