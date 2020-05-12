using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class ShowCaptionElementsEditViewCommand : DependencyObject, ICommand
    {
        private readonly CaptionElementsEditViewModel _captionElementsEditViewModel;
        private readonly WorkBarViewModel _workBarViewModel;
        private readonly SubtitleViewModel _subtitleViewModel;

        public ShowCaptionElementsEditViewCommand()
        {
            _captionElementsEditViewModel = Bootstrapper.Container.Resolve<CaptionElementsEditViewModel>();
            _workBarViewModel = Bootstrapper.Container.Resolve<WorkBarViewModel>();
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
        }


        public bool CanExecute(object parameter)
        {
            return _workBarViewModel.HasWorkData;
        }

        public void Execute(object parameter)
        {
            if (_captionElementsEditViewModel.IsShow)
                _captionElementsEditViewModel.Show();
            else
                _captionElementsEditViewModel.Close();
            
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
