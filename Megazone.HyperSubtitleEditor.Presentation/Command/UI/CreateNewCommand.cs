using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class CreateNewCommand : DependencyObject, ICommand
    {
        private readonly WorkBarViewModel _workBar;
        private readonly IBrowser _browser;

        public CreateNewCommand()
        {
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
            _workBar = Bootstrapper.Container.Resolve<WorkBarViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();

            if (subtitleViewModel.Tabs?.Any() ?? false)
            {
                //if (subtitleViewModel.Tabs.Any(tab => tab.CheckDirty()))
                //{
                var result = _browser.Main.ShowCreateWorkspaceConfirmWindow();

                if (!result.HasValue) return;

                if (result.Value)
                {
                    MessageCenter.Instance.Send(new Message.SubtitleEditor.CreateNewWindowMessage(this,
                        new CreateNewWindowMessageParameter(null, null, null)));

                    return;
                }

                MessageCenter.Instance.Send(new Message.SubtitleEditor.CleanUpSubtitleMessage(this));

                return;
                //}
            }

            MessageCenter.Instance.Send(new Message.SubtitleEditor.CreateNewWindowMessage(this,
                new CreateNewWindowMessageParameter(null, null, null)));
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}