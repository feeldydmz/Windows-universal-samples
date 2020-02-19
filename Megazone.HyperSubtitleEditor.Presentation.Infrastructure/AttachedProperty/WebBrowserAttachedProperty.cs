using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using mshtml;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.AttachedProperty
{
    public class WebBrowserAttachedProperty
    {
        private const string CODE_PATTEN = "code=";
        private const string MEGAONE_OAUTH_PATTEN = "megazone/login?";

        public static readonly DependencyProperty UriSourceProperty =
            DependencyProperty.RegisterAttached("UriSource", typeof(string), typeof(WebBrowserAttachedProperty),
                new PropertyMetadata((d, e) =>
                {
                    var webBrowser = (WebBrowser) d;
                    if (webBrowser != null)
                    {
                        var uri = (string) e.NewValue;

                        webBrowser.Source = !string.IsNullOrEmpty(uri) ? new Uri(uri) : new Uri("about:blank");
                    }
                })
            );

        public static readonly DependencyProperty NavigatingCommandProperty =
            DependencyProperty.RegisterAttached("NavigatingCommand", typeof(ICommand),
                typeof(WebBrowserAttachedProperty),
                new PropertyMetadata(null, (d, e) =>
                {
                    var browser = d as WebBrowser;
                    if (browser != null)
                        browser.Navigating += (s, a) =>
                        {
                            var command = (ICommand) e.NewValue;
                            var absoluteUri = a.Uri.AbsoluteUri;

                            var isMegaoneOAuth  = (absoluteUri.IndexOf(MEGAONE_OAUTH_PATTEN, StringComparison.Ordinal)) > -1? true : false;

                            // MegaOne Oauth 일때만 url에 들어있는 'code=' 체크
                            if (!isMegaoneOAuth) return;

                            var codeIndex = absoluteUri.IndexOf(CODE_PATTEN, StringComparison.Ordinal);

                            if (codeIndex == -1) return;

                            var document2 = browser.Document as IHTMLDocument2;
                            document2?.execCommand("ClearAuthenticationCache");
                                
                            var code = absoluteUri.Substring(codeIndex + CODE_PATTEN.Length);
                                
                            if (command.CanExecute(code))
                                command.Execute(code);

                            a.Cancel = true;
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

        public static ICommand GetNavigatingCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(NavigatingCommandProperty);
        }

        public static void SetNavigatingCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(NavigatingCommandProperty, value);
        }
    }
}