﻿using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
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
            WebBrowser.Navigated += WebBrowser_Navigated;
        }

        private void WebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Error.HideScriptErrors(WebBrowser, true);
        }
    }

    public class Error
    {
        public static void HideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);

            if (fiComWebBrowser == null)
                return;

            var objComWebBrowser = fiComWebBrowser.GetValue(wb);
            if (objComWebBrowser == null)
                return;

            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser,
                new object[] {hide});
        }
    }
}