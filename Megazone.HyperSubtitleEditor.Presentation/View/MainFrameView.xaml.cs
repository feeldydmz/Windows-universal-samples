using System.Windows;
using System.Windows.Controls;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     MainFrameView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainFrameView : UserControl
    {
        private Control _currentSignIn;

        public MainFrameView()
        {
            InitializeComponent();
        }

        private void OnLogoutRequested(SignIn.CreateSignInViewMessage message)
        {
            CreateSingIn();
        }

        public void CreateSingIn()
        {
            var signIn = new SignInView();

            signIn.CloseAction += CloseSingIn;

            _currentSignIn = signIn;
            SignInGrid.Children.Add(_currentSignIn);
        }

        public void CloseSingIn()
        {
            SignInGrid.Children.Remove(_currentSignIn);
        }

        private void MainFrameView_OnLoaded(object sender, RoutedEventArgs e)
        {
            CreateSingIn();

            MessageCenter.Instance.Regist<SignIn.CreateSignInViewMessage>(OnLogoutRequested);
        }


        private void MainFrameView_OnUnloaded(object sender, RoutedEventArgs e)
        {
            MessageCenter.Instance.Unregist<SignIn.CreateSignInViewMessage>(OnLogoutRequested);
        }
    }
}