using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Megazone.Core.Windows.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     Interaction logic for SubtitleView.xaml
    /// </summary>
    public partial class SubtitleView : UserControl
    {
        public SubtitleView()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            MessageCenter.Instance.Regist<Message.View.SubtitleView.SetFocusToTextBoxMessage>(SetFocusToRichTextBox);
            MessageCenter.Instance.Regist<Message.View.SubtitleView.ApplyBoldToAllTextMessage>(ApplyBoldToAllText);
            MessageCenter.Instance.Regist<Message.View.SubtitleView.ApplyItalicToAllTextMessage>(ApplyItalicToAllText);
            MessageCenter.Instance.Regist<Message.View.SubtitleView.ApplyUnderlineToAllTextMessage>(
                ApplyUnderlineToAllText);
            MessageCenter.Instance.Regist<Message.View.SubtitleView.ScrollIntoObjectMessage>(ScrollIntoObject);
            SubtitleRichTextBox.GotFocus += SubtitleRichTextBoxOnGotFocus;
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            MessageCenter.Instance.Unregist<Message.View.SubtitleView.SetFocusToTextBoxMessage>(SetFocusToRichTextBox);
            MessageCenter.Instance.Unregist<Message.View.SubtitleView.ApplyBoldToAllTextMessage>(ApplyBoldToAllText);
            MessageCenter.Instance
                .Unregist<Message.View.SubtitleView.ApplyItalicToAllTextMessage>(ApplyItalicToAllText);
            MessageCenter.Instance.Unregist<Message.View.SubtitleView.ApplyUnderlineToAllTextMessage>(
                ApplyUnderlineToAllText);
            MessageCenter.Instance.Unregist<Message.View.SubtitleView.ScrollIntoObjectMessage>(ScrollIntoObject);
            SubtitleRichTextBox.GotFocus -= SubtitleRichTextBoxOnGotFocus;
        }

        private void ScrollIntoObject(Message.View.SubtitleView.ScrollIntoObjectMessage message)
        {
            if (message.TargetObject == null) return;
            SubtitleListView.ScrollIntoView(message.TargetObject);
        }

        private void ApplyBoldToAllText(Message.View.SubtitleView.ApplyBoldToAllTextMessage message)
        {
            SubtitleRichTextBox.SelectAll();
            EditingCommands.ToggleBold.Execute(null, SubtitleRichTextBox);
        }

        private void ApplyUnderlineToAllText(Message.View.SubtitleView.ApplyUnderlineToAllTextMessage message)
        {
            SubtitleRichTextBox.SelectAll();
            EditingCommands.ToggleUnderline.Execute(null, SubtitleRichTextBox);
        }

        private void ApplyItalicToAllText(Message.View.SubtitleView.ApplyItalicToAllTextMessage message)
        {
            SubtitleRichTextBox.SelectAll();
            EditingCommands.ToggleItalic.Execute(null, SubtitleRichTextBox);
        }

        private void SetFocusToRichTextBox(Message.View.SubtitleView.SetFocusToTextBoxMessage message)
        {
            this.InvokeOnUi(() => { SubtitleRichTextBox.Focus(); }, true);
        }

        private void SubtitleRichTextBoxOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            SubtitleRichTextBox.CaretPosition = SubtitleRichTextBox.Document.ContentEnd;
        }
    }
}