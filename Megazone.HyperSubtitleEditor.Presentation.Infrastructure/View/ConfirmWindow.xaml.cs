using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Megazone.SubtitleEditor.Resources;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View
{
    /// <summary>
    ///     Interaction logic for ConfirmWindow.xaml
    /// </summary>
    public partial class ConfirmWindow : Window
    {
        public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register("Buttons",
            typeof(IList<ButtonInfo>),
            typeof(ConfirmWindow));

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked",
            typeof(bool),
            typeof(ConfirmWindow));

        public static readonly DependencyProperty IsCheckBoxVisibleProperty =
            DependencyProperty.Register("IsCheckBoxVisible",
                typeof(bool),
                typeof(ConfirmWindow));

        public static readonly DependencyProperty CheckBoxContentProperty =
            DependencyProperty.Register("CheckBoxContent",
                typeof(string),
                typeof(ConfirmWindow));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",
            typeof(string),
            typeof(ConfirmWindow));

        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment",
            typeof(TextAlignment),
            typeof(ConfirmWindow));

        private readonly IList<CustomButtonSetting> _buttonContents = new List
            <CustomButtonSetting>
            {
                new CustomButtonSetting(MessageBoxResult.OK, Resource.CNT_CONFIRM, ButtonRunTypes.Default),
                new CustomButtonSetting(MessageBoxResult.Cancel, Resource.CNT_CANCEL, ButtonRunTypes.Cancel),
                new CustomButtonSetting(MessageBoxResult.Yes, "예", ButtonRunTypes.Default),
                new CustomButtonSetting(MessageBoxResult.No, "아니오")
            };

        public ConfirmWindow()
        {
            InitializeComponent();
        }

        public IList<ButtonInfo> Buttons
        {
            get => (IList<ButtonInfo>) GetValue(ButtonsProperty);
            set => SetValue(ButtonsProperty, value);
        }

        public bool IsChecked
        {
            get => (bool) GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public bool IsCheckBoxVisible
        {
            get => (bool) GetValue(IsCheckBoxVisibleProperty);
            set => SetValue(IsCheckBoxVisibleProperty, value);
        }

        public string CheckBoxContent
        {
            get => (string) GetValue(CheckBoxContentProperty);
            set => SetValue(CheckBoxContentProperty, value);
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public TextAlignment TextAlignment
        {
            get => (TextAlignment) GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }

        public MessageBoxResult Result { get; private set; }

        public void Initialize(string text,
            MessageBoxButton buttonKind,
            IList<CustomButtonSetting> buttonContents = null,
            string checkBoxContent = null,
            bool defaultCheckedValue = false)
        {
            Text = text;
            Buttons = new ObservableCollection<ButtonInfo>();

            if (!string.IsNullOrEmpty(checkBoxContent))
            {
                IsCheckBoxVisible = true;
                CheckBoxContent = checkBoxContent;
                IsChecked = defaultCheckedValue;
            }

            switch (buttonKind)
            {
                case MessageBoxButton.OK:
                {
                    var customButtonSetting = GetCustomButtonSetting(MessageBoxResult.OK, buttonContents);
                    Buttons.Add(new ButtonInfo(MessageBoxResult.OK,
                        customButtonSetting.Content,
                        customButtonSetting.RunType));
                }
                    break;
                case MessageBoxButton.OKCancel:
                {
                    var okCustomButtonSetting = GetCustomButtonSetting(MessageBoxResult.OK, buttonContents);
                    Buttons.Add(new ButtonInfo(MessageBoxResult.OK,
                        okCustomButtonSetting.Content,
                        okCustomButtonSetting.RunType));
                    var cancelCustomButtonSetting = GetCustomButtonSetting(MessageBoxResult.Cancel, buttonContents);
                    Buttons.Add(new ButtonInfo(MessageBoxResult.Cancel,
                        cancelCustomButtonSetting.Content,
                        cancelCustomButtonSetting.RunType));
                }
                    break;
                case MessageBoxButton.YesNoCancel:
                {
                    var yesCustomButtonSetting = GetCustomButtonSetting(MessageBoxResult.Yes, buttonContents);
                    Buttons.Add(new ButtonInfo(MessageBoxResult.Yes,
                        yesCustomButtonSetting.Content,
                        yesCustomButtonSetting.RunType));

                    var noCustomButtonSetting = GetCustomButtonSetting(MessageBoxResult.No, buttonContents);
                    Buttons.Add(new ButtonInfo(MessageBoxResult.No,
                        noCustomButtonSetting.Content,
                        noCustomButtonSetting.RunType));

                    var cancelCustomButtonSetting = GetCustomButtonSetting(MessageBoxResult.Cancel, buttonContents);
                    Buttons.Add(new ButtonInfo(MessageBoxResult.Cancel,
                        cancelCustomButtonSetting.Content,
                        cancelCustomButtonSetting.RunType));
                }
                    break;
                case MessageBoxButton.YesNo:
                {
                    var yesCustomButtonSetting = GetCustomButtonSetting(MessageBoxResult.Yes, buttonContents);
                    Buttons.Add(new ButtonInfo(MessageBoxResult.Yes,
                        yesCustomButtonSetting.Content,
                        yesCustomButtonSetting.RunType));

                    var noCustomButtonSetting = GetCustomButtonSetting(MessageBoxResult.No, buttonContents);
                    Buttons.Add(new ButtonInfo(MessageBoxResult.No,
                        noCustomButtonSetting.Content,
                        noCustomButtonSetting.RunType));
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buttonKind), buttonKind, null);
            }
        }

        private CustomButtonSetting GetCustomButtonSetting(MessageBoxResult resultType,
            IList<CustomButtonSetting> buttonContents = null)
        {
            if (buttonContents == null || buttonContents.All(b => b.Result != resultType))
                return _buttonContents.First(b => b.Result == resultType);
            return buttonContents.First(b => b.Result == resultType);
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var buttonInfo = (ButtonInfo) ((Button) sender).DataContext;
            Result = buttonInfo.ResultType;
            Close();
        }

        public static MessageBoxResult ShowDialog(ConfirmWindowParameter parameter)
        {
            if (parameter == null)
                throw new InvalidOperationException();
            var confirmWindow = new ConfirmWindow
            {
                Title = string.IsNullOrEmpty(parameter.WindowTitle) ? string.Empty : parameter.WindowTitle
            };
            if (parameter.WindowOwner != null && parameter.WindowOwner.IsLoaded)
            {
                confirmWindow.Owner = parameter.WindowOwner;
                confirmWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                confirmWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            confirmWindow.Text = parameter.Message;
            confirmWindow.TextAlignment = parameter.TextAlignment;
            confirmWindow.Initialize(parameter.Message, parameter.ButtonType, parameter.ButtonContents,
                parameter.ConfirmWindowCheckBoxParameter?.CheckBoxContent,
                parameter.ConfirmWindowCheckBoxParameter?.DefaultCheckedValue ?? false);
            confirmWindow.ShowDialog();
            return confirmWindow.Result;
        }
    }
}