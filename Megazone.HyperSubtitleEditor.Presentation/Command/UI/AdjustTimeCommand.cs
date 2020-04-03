using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class AdjustTimeCommand : DependencyObject, ICommand
    {
        private SubtitleViewModel SubtitleViewModel { get; } = Bootstrapper.Container.Resolve<SubtitleViewModel>();
        private IBrowser Browser { get; } = Bootstrapper.Container.Resolve<IBrowser>();

        public bool CanExecute(object parameter)
        {
            return SubtitleViewModel.SelectedTab != null &&
                   (SubtitleViewModel.SelectedTab.Rows?.Any() ?? false);
        }

        public void Execute(object parameter)
        {
            var way = Browser.Main.ShowAdjustTimeWindow();
            MessageCenter.Instance.Send(new Message.SubtitleEditor.AdjustTimeMessage(this, way));
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}