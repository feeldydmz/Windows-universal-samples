using System.Windows;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     CreateWorksapceConfirmView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CreateWorksapceConfirmView : Window
    {
        public CreateWorksapceConfirmView()
        {
            InitializeComponent();
        }

        public bool IsCreateNewWindow { get; set; }

        private void ConfirmButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            IsCreateNewWindow = YesButton.IsChecked == true;

            CloseWindow();
        }

        private void CloseWindow()
        {
            var window = GetWindow(this);
            window?.Close();
        }
    }
}