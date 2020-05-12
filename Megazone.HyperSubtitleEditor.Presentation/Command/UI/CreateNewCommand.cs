using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Megazone.Core.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
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
            return true;
        }

        public void Execute(object parameter)
        {
            var subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();

            if (subtitleViewModel.Tabs?.Any() ?? false)
            {
                if (subtitleViewModel.Tabs.Any(tab => tab.CheckDirty()))
                {
                    var result = _browser.Main.ShowCreateWorkspaceConfirmWindow();

                    if (!result.HasValue) return;

                    if (result.Value)
                    {
                        MessageCenter.Instance.Send(new Message.SubtitleEditor.CreateNewWindowMessage(this, 
                            new CreateNewWindowMessageParameter(null, null, null)));

                        return;
                    }
                    else
                    {
                        MessageCenter.Instance.Send(new Message.SubtitleEditor.CleanUpSubtitleMessage(this));

                        return;
                    }
                }
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