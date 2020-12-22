using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using Megazone.Core.VideoTrack;
using Megazone.Core.VideoTrack.Xaml;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    internal class InterpretRichTextBoxBehavior : Behavior<RichTextBox>
    {
        public static readonly DependencyProperty BoundDocumentProperty =
            DependencyProperty.Register(
                "BoundDocument",
                typeof(IList<IText>),
                typeof(InterpretRichTextBoxBehavior),
                new PropertyMetadata(null,
                    (s, a) =>
                    {
                        ((InterpretRichTextBoxBehavior) s).OnBoundDocumentPropertyChanged((IList<IText>) a.NewValue);
                    }));

        private readonly XamlParser _xamlParser = new XamlParser();

        private bool _isProcessingBoundDocumentChanged;
        private bool _isProcessingTextChanged;

        public IList<IText> BoundDocument
        {
            get => (IList<IText>) GetValue(BoundDocumentProperty);
            set => SetValue(BoundDocumentProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += AssociatedObject_TextChanged;
            AssociatedObject.PreviewKeyDown += AssociatedObject_KeyDown;


            //DataObject.AddPastingHandler(AssociatedObject, OnPaste);
        }

        private void AssociatedObject_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.V || (Keyboard.Modifiers != ModifierKeys.Control)) return;

            RichTextBox richTextBox = sender as RichTextBox;

            if (richTextBox == null) return;

            var textData = (string)Clipboard.GetData("Text");
            
            if (richTextBox.Selection.IsEmpty)
                richTextBox.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Backward)?.InsertTextInRun(textData);
            else
            {
                var range = new TextRange(richTextBox.Selection.Start, richTextBox.Selection.End) {Text = ""};

                richTextBox.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Backward)?.InsertTextInRun(textData);
            }
            
            e.Handled = true;
        }

        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isProcessingBoundDocumentChanged) return;
            _isProcessingTextChanged = true;
            try
            {
                var box = sender as RichTextBox;
                if (box == null) return;
                var document = box.Document;
                var tr = new TextRange(document.ContentStart, document.ContentEnd);
                using (var ms = new MemoryStream())
                {
                    tr.Save(ms, DataFormats.Xaml);
                    var xamlText = Encoding.UTF8.GetString(ms.ToArray());
                    BoundDocument = _xamlParser.ParseLine(xamlText)
                        .ToList();
                }
            }
            finally
            {
                _isProcessingTextChanged = false;
            }
        }

        protected override void OnDetaching()
        {
            //DataObject.RemovePastingHandler(AssociatedObject, OnPaste);
            AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
            base.OnDetaching();
        }

        private void OnBoundDocumentPropertyChanged(IList<IText> eNewValue)
        {
            if (_isProcessingTextChanged) return;
            _isProcessingBoundDocumentChanged = true;
            try
            {
                AssociatedObject.Document.Blocks.Clear();
                if (eNewValue == null) return;
                var translatedXaml = _xamlParser.ToTextLine(eNewValue);
                if (!string.IsNullOrEmpty(translatedXaml))
                {
                    var blocks = new List<Section>();
                    using (var xamlMemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(translatedXaml)))
                    {
                        var parser = new ParserContext();
                        parser.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                        parser.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
                        if (XamlReader.Load(xamlMemoryStream, parser) is Section section)
                            blocks.Add(section);
                    }

                    AssociatedObject.Document.Blocks.AddRange(blocks);
                }
            }
            finally
            {
                _isProcessingBoundDocumentChanged = false;
            }
        }
    }
}