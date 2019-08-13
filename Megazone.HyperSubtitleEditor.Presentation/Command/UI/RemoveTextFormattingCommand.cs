using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Megazone.Core.VideoTrack;
using Megazone.Core.VideoTrack.Model;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class RemoveTextFormattingCommand : DependencyObject, ICommand
    {
        private readonly SubtitleViewModel _subtitleViewModel;

        public RemoveTextFormattingCommand()
        {
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            var model = _subtitleViewModel.SelectedItem;
            return model?.Texts?.Any() ?? false;
        }

        public void Execute(object parameter)
        {
            var model = (SubtitleListItemViewModel) _subtitleViewModel.SelectedItem;
            model.Texts = RemoveTextTag(model.Texts.ToList());
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        private IList<IText> RemoveTextTag(IList<IText> texts)
        {
            IList<IText> result = new List<IText>();

            foreach (var text in texts.ToList())
                if (text is Normal)
                {
                    result.Add(text);
                }
                else
                {
                    var children = RemoveTextTag(((ITag) text).Children);

                    foreach (var item in children.ToList())
                        result.Add(item);
                }
            return result;
        }
    }
}