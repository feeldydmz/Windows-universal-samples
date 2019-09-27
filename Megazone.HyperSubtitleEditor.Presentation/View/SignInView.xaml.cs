using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using Microsoft.Toolkit.Wpf.UI.Controls;
using mshtml;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     SignInView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SignInView : UserControl
    {
        public SignInView()
        {
            InitializeComponent();

            if (DataContext is SignInViewModel vm)
            {
                vm.OnSourceUriChanged = (url) => { WebView.Source = new Uri(url); };
            }

            WebView.IsJavaScriptEnabled = true;
        }

        private void WebView_OnNavigationStarting(object sender, WebViewControlNavigationStartingEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("1");
            const string CODE_PATTEN = "code=";
            var absoluteUri = e.Uri.AbsoluteUri;
            //System.Diagnostics.Debug.WriteLine(absoluteUri);
            var index = absoluteUri.IndexOf(CODE_PATTEN, StringComparison.Ordinal);

            if (index == -1)
                return;
            e.Cancel = true;

            //System.Diagnostics.Debug.WriteLine("2");

            if (sender is WebView webView)
            {
                //webView.Source = new Uri("about: blank");
                //webView.InvokeScript("ClearAuthenticationCache");
            }

            //System.Diagnostics.Debug.WriteLine("3");

            var code = absoluteUri.Substring(index + CODE_PATTEN.Length);

            if (DataContext is SignInViewModel vm)
            {
                vm.NavigateToProject(code);
            }
        }
    }
}