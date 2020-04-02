using System;
using System.Collections.Generic;
using System.Linq;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Megazone.SubtitleEditor.Resources;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class CopySubtitleViewModel : CreateSubtitleViewModelBase
    {
        private IList<ISubtitleListItemViewModel> _rows;
        private ISubtitleTabItemViewModel _selectedTab;
        private IList<ISubtitleTabItemViewModel> _tabs;

        public CopySubtitleViewModel(ILogger logger, LanguageLoader languageLoader) : base(logger, languageLoader)
        {
        }

        public IList<ISubtitleTabItemViewModel> Tabs
        {
            get => _tabs;
            set => Set(ref _tabs, value);
        }

        public IList<ISubtitleListItemViewModel> Rows
        {
            get => _rows;
            set => Set(ref _rows, value);
        }


        public ISubtitleTabItemViewModel SelectedTab
        {
            get => _selectedTab;
            set
            {
                Set(ref _selectedTab, value);
                Label = value.Name + Resource.CNT_COPY;
                SelectedLanguage =
                    Languages.Where(i =>
                            i.LanguageCode.Equals(value.LanguageCode) && i.CountryCode.Equals(value.CountryCode))
                        .ToList()
                        .FirstOrDefault();
                SelectedSubtitleKind = value.Kind;
                Rows = value.Rows;
            }
        }

        protected override bool CanOnConfirm()
        {
            return !string.IsNullOrEmpty(Label);
        }

        protected override void OnConfirm()
        {
            try
            {
                MessageCenter.Instance.Send(new Message.SubtitleEditor.CopyTabMessage(this, new CopyTabMessageParameter
                {
                    Kind = SelectedSubtitleKind,
                    Label = Label,
                    LanguageCode = SelectedLanguage.LanguageCode,
                    CountryCode = SelectedLanguage.CountryCode,
                    Rows = Rows
                }));
            }
            catch (Exception ex)
            {
                Logger.Error.Write(ex);
            }

            CloseAction?.Invoke();
        }

        public void SetSelectedTabInfo(IEnumerable<ISubtitleTabItemViewModel> tabs)
        {
            Tabs = new List<ISubtitleTabItemViewModel>(tabs);
            SelectedTab = Tabs.FirstOrDefault(t => t.IsSelected) ?? Tabs.FirstOrDefault();
        }
    }
}