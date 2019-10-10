using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Navigation;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.AttachedProperty;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     SignInView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SignInView : UserControl
    {
        public Action CloseAction;

        public SignInView()
        {
            InitializeComponent();

            if (this.DataContext is SignInViewModel dataContext) dataContext.CloseAction = CloseControl;

            WebBrowser.Navigated += WebBrowser_Navigated;
        }

        private void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            Error.HideScriptErrors(WebBrowser, true);
        }

        public void CloseControl()
        {
            WebBrowser.ClearValue(WebBrowserAttachedProperty.NavigatingCommandProperty);
            WebBrowser.ClearValue(WebBrowserAttachedProperty.UriSourceProperty);

            WebBrowser.Dispose();

            CloseAction?.Invoke();
        }
        
    }

    public class Error
    {
        public static void HideScriptErrors(WebBrowser wb, bool hide)
        {
            var fiComWebBrowser =
                typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);

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