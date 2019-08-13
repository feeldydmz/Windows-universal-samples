using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Megazone.Core.IoC;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    public class LoginViewModel : ViewModelBase
    {
        public ICommand _loginCommand;

        string _loginId;
        string _password;
        bool _isLogin;

        public bool IsLogin
        {
            get => _isLogin;
            set => Set(ref _isLogin, value);
        }

        public string LoginId
        {
            get => _loginId;
            set => Set(ref _loginId, value);
        }

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public ICommand LoginCommand
        {
            get { return _loginCommand = _loginCommand ?? new RelayCommand(Login); }
        }

        private void Login()
        {
            IsLogin = true;
        }
    }
}
