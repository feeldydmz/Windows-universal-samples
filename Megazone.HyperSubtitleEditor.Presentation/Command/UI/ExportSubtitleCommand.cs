using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class ExportSubtitleCommand : DependencyObject, ICommand
    {
        private readonly SubtitleViewModel _subtitleViewModel;

        public ExportSubtitleCommand()
        {
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            return _subtitleViewModel.HasTab;
        }

        public void Execute(object parameter)
        {
            if (parameter is MenuItem menuItem)
            {
                var header = menuItem.Header as string;

                if (header == Resource.CNT_EXPORT_ALL_SUBTITLE_FILE)
                    MessageCenter.Instance.Send(new Message.SubtitleEditor.SaveAllMessage(this));
                else if (header == Resource.CNT_EXPORT_SUBTITLE_FILE)
                    MessageCenter.Instance.Send(new Message.SubtitleEditor.ExportSubtitleMessage(this));
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}