using System;
using System.Windows;
using System.Windows.Controls;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.SubtitleEditor.Resources;
using Microsoft.Win32;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     OpenVideoAddressWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OpenVideoAddressWindow : Window, IClosable
    {
        public string OpenTypeString { get; set; }

        public OpenVideoAddressWindow()
        {
            InitializeComponent();

            Loaded+=OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            WindowStartupLocation = WindowStartupLocation.Manual;

            this.Top -= 150;


            if (OpenTypeString == Resource.CNT_OPEN_URL)
            {
                UrlOpenButton.IsChecked = true;
            }
            else if (OpenTypeString == Resource.CNT_OPEN_VIDEO)
            {
                FileOpenButton.IsChecked = true;
            }
            else if (OpenTypeString == Resource.CNT_OPEN_FROM_MCM)
            {
                McmOpenButton.IsChecked = true;
            }


        }

        private void UrlTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            OpenButton.IsEnabled = !string.IsNullOrEmpty(UrlTextBox.Text.Trim());
        }

        private void OpenButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = $"{Resource.CNT_ALL_FILES} (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() ?? false)
                FilePathTextBox.Text = openFileDialog.FileName;
        }
    }
}