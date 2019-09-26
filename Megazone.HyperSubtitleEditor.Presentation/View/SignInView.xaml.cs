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
    public class WebBrowserHelper
    {
        private const string CODE_PATTEN = "code=";

        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.RegisterAttached("UriSource", typeof(string), typeof(WebBrowserHelper),
                //new PropertyMetadata(OnUriChanged) 
                new PropertyMetadata(OnUriChanged)
            );

        public static readonly DependencyProperty NavigatingCommandProperty =
            DependencyProperty.RegisterAttached("NavigatingCommand", typeof(ICommand), typeof(WebBrowserHelper),
                new PropertyMetadata(null, (d, e) =>
                {
                    var browser = d as WebBrowser;
                    if (browser != null)
                        browser.Navigating += (s, a) =>
                        {
                            var command = (ICommand) e.NewValue;
                            var absoluteUri = a.Uri.AbsoluteUri;

                            var index = absoluteUri.IndexOf(CODE_PATTEN, StringComparison.Ordinal);

                            if (index == -1) return;

                            a.Cancel = true;

                            var viewmodel = browser.DataContext as SignInViewModel;

                            if (viewmodel != null) viewmodel.UriSource = "about:blank";

                            var document2 = browser.Document as IHTMLDocument2;
                            document2?.execCommand("ClearAuthenticationCache");

                            var code = absoluteUri.Substring(index + CODE_PATTEN.Length);

                            if (command.CanExecute(code))
                                command.Execute(code);
                        };
                }));

        public static string GetUriSource(DependencyObject dependencyObject)
        {
            return (string) dependencyObject.GetValue(UriSourceProperty);
        }

        public static void SetUriSource(DependencyObject dependencyObject, string uri)
        {
            dependencyObject.SetValue(UriSourceProperty, uri);
        }

        private static void OnUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var webBrowser = (WebBrowser) d;
            if (webBrowser != null)
            {
                var uri = (string) e.NewValue;

                webBrowser.Source = !string.IsNullOrEmpty(uri) ? new Uri(uri) : new Uri("about:blank");
            }
        }

        public static ICommand GetNavigatingCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(NavigatingCommandProperty);
        }

        public static void SetNavigatingCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(NavigatingCommandProperty, value);
        }
    }

    /// <summary>
    ///     SignInView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SignInView : UserControl
    {
        public SignInView()
        {
            InitializeComponent();
            
        }

        private void WebView_OnNavigationStarting(object sender, WebViewControlNavigationStartingEventArgs e)
        {
            const string CODE_PATTEN = "code=";
            var absoluteUri = e.Uri.AbsoluteUri;
            var index = absoluteUri.IndexOf(CODE_PATTEN, StringComparison.Ordinal);

            if (index == -1)
                return;
            e.Cancel = true;
            
            if (sender is WebView webview)
            {
                webview.Source = new Uri("about: blank");
            }

            //var document2 = browser.Document as IHTMLDocument2;
            //document2?.execCommand("ClearAuthenticationCache");

            var code = absoluteUri.Substring(index + CODE_PATTEN.Length);

            if (DataContext is SignInViewModel vm)
            {

            }

            //if (command.CanExecute(code))
            //    command.Execute(code);
        }
    }
}