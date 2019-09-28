using System.Windows;
using System.Windows.Controls;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    public class LanguageItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EmptyDataTemplate { get; set; }
        public DataTemplate DataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is LanguageItem language)
                return string.IsNullOrEmpty(language.Code) ? EmptyDataTemplate : DataTemplate;
            return item == null ? EmptyDataTemplate : DataTemplate;
            //return base.SelectTemplate(item, container);
        }
    }
}