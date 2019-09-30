using System.Reflection;
using System.Windows.Controls;
using System.Windows.Navigation;

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
            WebBrowser.Navigated += WebBrowser_Navigated;
        }

        private void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            Error.HideScriptErrors(WebBrowser, true);
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