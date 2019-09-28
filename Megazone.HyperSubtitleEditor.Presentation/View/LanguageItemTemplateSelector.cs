using System.Windows;
using System.Windows.Controls;
using Megazone.Cloud.Media.Domain;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    public class LanguageItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EmptyDataTemplate { get; set; }
        public DataTemplate DataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Language language)
            {
                return string.IsNullOrEmpty(language.LanguageCode) ? EmptyDataTemplate : DataTemplate;
            }
            return item == null ? EmptyDataTemplate : DataTemplate;
            //return base.SelectTemplate(item, container);
        }
    }
}