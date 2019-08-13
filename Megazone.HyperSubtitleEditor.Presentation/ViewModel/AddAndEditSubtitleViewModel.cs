using System;
using System.Linq;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class AddAndEditSubtitleViewModel : CreateSubtitleViewModelBase
    {
        public AddAndEditSubtitleViewModel(ILogger logger) : base(logger)
        {
        }

        public SubtitleDialogViewMode Mode { get; internal set; }
        public string TabId { get; set; }

        public void SetSelectedTabInfo(ISubtitleTabItemViewModel tabItem)
        {
            TabId = tabItem.Id;
            Label = tabItem.Name;
            SelectedLanguageItemViewModel = Languages.Where(i => i.LanguageCode.Equals(tabItem.LanguageCode))
                .ToList()
                .FirstOrDefault();
            SelectedSubtitleKind = tabItem.Kind;
        }

        protected override bool CanOnConfirm()
        {
            return !string.IsNullOrEmpty(Label);
        }

        protected override void OnConfirm()
        {
            try
            {
                if (Mode == SubtitleDialogViewMode.Add)
                    MessageCenter.Instance.Send(new Subtitle.FileOpenedMessage(this, new FileOpenedMessageParameter
                    {
                        Kind = SelectedSubtitleKind,
                        Label = Label,
                        LanguageCode = SelectedLanguageItemViewModel.LanguageCode
                    }));
                if (Mode == SubtitleDialogViewMode.Edit)
                    MessageCenter.Instance.Send(new Subtitle.EditTabMessage(this, new EditTabMessageParameter
                    {
                        Id = TabId,
                        Kind = SelectedSubtitleKind,
                        Label = Label,
                        LanguageCode = SelectedLanguageItemViewModel.LanguageCode
                    }));
            }
            catch (Exception ex)
            {
                Logger.Error.Write(ex);
            }
            CloseAction?.Invoke();
        }
    }
}