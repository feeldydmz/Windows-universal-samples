using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Megazone.HyperSubtitleEditor.Presentation.View;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    public class ProjectSelectBehavior : Behavior<ProjectSelectView>
    {
        private const int StageWidth = 340;
        private const int StagePaddingWidth = 30;
        private int _stagePerPage = 1;

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand),
                typeof(ProjectSelectBehavior));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SizeChanged += AssociatedObjectOnSizeChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.SizeChanged -= AssociatedObjectOnSizeChanged;
        }
        private void AssociatedObjectOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!(sender is ProjectSelectView view)) return;

            ItemsControl itemsControl = view.FindName("StageItemControl") as ItemsControl;

            var itemsControlWidth = itemsControl?.ActualWidth + StagePaddingWidth;

            var stageItemWidth = StageWidth + StagePaddingWidth;

            var stageNumberPerPage = Convert.ToInt32(Math.Truncate(Convert.ToDecimal(itemsControlWidth / stageItemWidth)));


            if (_stagePerPage != stageNumberPerPage)
            {
                _stagePerPage = stageNumberPerPage;
                Command?.Execute(stageNumberPerPage);
            }
        }
    }
}
