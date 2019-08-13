using System;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;

namespace Megazone.HyperSubtitleEditor.Presentation.Message.View
{
    internal static class SubtitleView
    {
        internal class GoToLineNumberMessage : MessageBase
        {
            public GoToLineNumberMessage(object sender, int lineNumber) : base(sender)
            {
                LineNumber = lineNumber;
            }

            public int LineNumber { get; }
        }

        internal class SelectAllRowsMessage : MessageBase
        {
            public SelectAllRowsMessage(object sender) : base(sender)
            {
            }
        }

        internal class DeleteSelectRowsMessage : MessageBase
        {
            public DeleteSelectRowsMessage(object sender) : base(sender)
            {
            }
        }

        internal class ApplyBoldToAllTextMessage : MessageBase
        {
            public ApplyBoldToAllTextMessage(object sender) : base(sender)
            {
            }
        }

        internal class ApplyItalicToAllTextMessage : MessageBase
        {
            public ApplyItalicToAllTextMessage(object sender) : base(sender)
            {
            }
        }

        internal class ApplyUnderlineToAllTextMessage : MessageBase
        {
            public ApplyUnderlineToAllTextMessage(object sender) : base(sender)
            {
            }
        }

        internal class ScrollIntoObjectMessage : MessageBase
        {
            public ScrollIntoObjectMessage(object sender, object obj) : base(sender)
            {
                TargetObject = obj;
            }

            public object TargetObject { get; }
        }

        internal class RequestFindCountMessage : MessageBase
        {
            public RequestFindCountMessage(object sender, string findText, Action<int> response) : base(sender)
            {
                FindText = findText;
                Response = response;
            }

            public string FindText { get; }
            public Action<int> Response { get; }
        }

        internal class FindTextMessage : MessageBase
        {
            public FindTextMessage(object sender, string findText) : base(sender)
            {
                FindText = findText;
            }

            public string FindText { get; }
        }

        internal class ReplaceTextMessage : FindTextMessage
        {
            public ReplaceTextMessage(object sender, string findText, string replaceText) : base(sender, findText)
            {
                ReplaceText = replaceText;
            }

            public string ReplaceText { get; }
        }

        internal class AllFindTextMessage : FindTextMessage
        {
            public AllFindTextMessage(object sender, string findText) : base(sender, findText)
            {
            }
        }

        internal class AllReplaceTextMessage : ReplaceTextMessage
        {
            public AllReplaceTextMessage(object sender, string findText, string replaceText) : base(sender, findText,
                replaceText)
            {
            }
        }

        internal class SetFocusToTextBoxMessage : MessageBase
        {
            public SetFocusToTextBoxMessage(object sender) : base(sender)
            {
            }
        }
    }
}