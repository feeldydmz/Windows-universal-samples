using System;
using System.Windows;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class SetAutoLoginCommand : DependencyObject, ICommand
    {
        private readonly SignInViewModel _signInViewModel;

        public SetAutoLoginCommand(SignInViewModel signInViewModel)
        {
            _signInViewModel = signInViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _signInViewModel.IsAutoLogin = !_signInViewModel.IsAutoLogin;
            _signInViewModel.Save();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}