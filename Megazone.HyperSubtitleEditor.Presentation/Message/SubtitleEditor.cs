using System.Collections.Generic;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Megazone.HyperSubtitleEditor.Presentation.Serializable;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.Message
{
    internal static class SubtitleEditor
    {
        internal class InsertNewRowMessage : MessageBase
        {
            public InsertNewRowMessage(object sender) : base(sender)
            {
            }
        }

        internal class InsertNewRowAfterSelectedRowMessage : MessageBase
        {
            public InsertNewRowAfterSelectedRowMessage(object sender) : base(sender)
            {
            }
        }

        internal class InsertNewRowBeforeSelectedRowMessage : MessageBase
        {
            public InsertNewRowBeforeSelectedRowMessage(object sender) : base(sender)
            {
            }
        }

        internal class CopySelectedRowsMessage : MessageBase
        {
            public CopySelectedRowsMessage(object sender) : base(sender)
            {
            }
        }

        internal class CopyContentsToClipboardMessage : MessageBase
        {
            public CopyContentsToClipboardMessage(object sender) : base(sender)
            {
            }
        }

        internal class CutSelectedRowsMessage : MessageBase
        {
            public CutSelectedRowsMessage(object sender) : base(sender)
            {
            }
        }

        internal class PasteRowsMessage : MessageBase
        {
            public PasteRowsMessage(object sender) : base(sender)
            {
            }
        }

        internal class CloseTabMessage : MessageBase
        {
            public CloseTabMessage(object sender, SubtitleTabItemViewModel tab) : base(sender)
            {
                Tab = tab;
            }

            public SubtitleTabItemViewModel Tab { get; }
        }

        internal class FileOpenedMessage : MessageBase
        {
            public FileOpenedMessage(object sender, FileOpenedMessageParameter param) : base(sender)
            {
                Param = param;
            }

            public FileOpenedMessageParameter Param { get; }
        }

        internal class CaptionElementCreateNewMessage : MessageBase
        {
            public CaptionElementCreateNewMessage(object sender, FileOpenedMessageParameter param) : base(sender)
            {
                Param = param;
            }

            public FileOpenedMessageParameter Param { get; }
        }

        internal class ImportSubtitleMessage : MessageBase
        {
            public ImportSubtitleMessage(object sender) : base(sender)
            {
            }
        }

        internal class ExportSubtitleMessage : MessageBase
        {
            public ExportSubtitleMessage(object sender) : base(sender)
            {
            }
        }

        internal class SaveMessage : MessageBase
        {
            public SaveMessage(object sender) : base(sender)
            {
            }
        }

        internal class SaveAllMessage : MessageBase
        {
            public SaveAllMessage(object sender) : base(sender)
            {
            }
        }

        internal class SaveAsMessage : MessageBase
        {
            public SaveAsMessage(object sender) : base(sender)
            {
            }
        }

        internal class CopyTabMessage : MessageBase
        {
            public CopyTabMessage(object sender, CopyTabMessageParameter param) : base(sender)
            {
                Param = param;
            }

            public CopyTabMessageParameter Param { get; }
        }

        internal class EditTabMessage : MessageBase
        {
            public EditTabMessage(object sender, EditTabMessageParameter param) : base(sender)
            {
                Param = param;
            }

            public EditTabMessageParameter Param { get; }
        }

        internal class AddNewRowMessage : MessageBase
        {
            public AddNewRowMessage(object sender) : base(sender)
            {
            }
        }

        internal class AdjustTimeMessage : MessageBase
        {
            public AdjustTimeMessage(object sender, AdjustTimeWay way) : base(sender)
            {
                Way = way;
            }

            public AdjustTimeWay Way { get; }
        }

        internal class SyncStartTimeToCurrentMediaPositionMessage : MessageBase
        {
            public SyncStartTimeToCurrentMediaPositionMessage(object sender) : base(sender)
            {
            }
        }

        internal class SyncEndTimeToCurrentMediaPositionMessage : MessageBase
        {
            public SyncEndTimeToCurrentMediaPositionMessage(object sender) : base(sender)
            {
            }
        }

        public class InsertRowAtCurrentMediaPositionMessage : MessageBase
        {
            public InsertRowAtCurrentMediaPositionMessage(object sender) : base(sender)
            {
            }
        }

        internal class LoadTabsMessage : MessageBase
        {
            public LoadTabsMessage(object sender, IEnumerable<SubtitleTabItem> tabs) : base(sender)
            {
                Tabs = tabs;
            }

            public IEnumerable<SubtitleTabItem> Tabs { get; }
        }

        internal class SettingsSavedMessage : MessageBase
        {
            public SettingsSavedMessage(object sender) : base(sender)
            {
            }
        }

        internal class AutoAdjustEndtimesMessage : MessageBase
        {
            public AutoAdjustEndtimesMessage(object sender) : base(sender)
            {
            }
        }

        internal class CleanUpSubtitleMessage : MessageBase
        {
            public CleanUpSubtitleMessage(object sender) : base(sender)
            {
            }
        }

        internal class CreateNewWindowMessage : MessageBase
        {
            public CreateNewWindowMessage(object sender, CreateNewWindowMessageParameter param) : base(sender)
            {
                Param = param;
            }

            public CreateNewWindowMessageParameter Param { get; }
        }

        internal class CaptionOpenRequestedMessage : MessageBase
        {
            public CaptionOpenRequestedMessage(object sender, CaptionOpenMessageParameter param) : base(sender)
            {
                Param = param;
            }

            public CaptionOpenMessageParameter Param { get; }
        }

        internal class CaptionElementUpdateMessage : MessageBase
        {
            public CaptionElementUpdateMessage(object sender, CaptionElementUpdateMessageParameter param) : base(sender)
            {
                Param = param;
                //OpenCaptionElements = openCaptionElements;
                //CloseCaptionElements = closeCaptionElements;
            }

            public CaptionElementUpdateMessageParameter Param { get; }
            //public List<Caption> OpenCaptionElements { get; }
            //public List<SubtitleTabItemViewModel> CloseCaptionElements { get; }
        }
    }
}