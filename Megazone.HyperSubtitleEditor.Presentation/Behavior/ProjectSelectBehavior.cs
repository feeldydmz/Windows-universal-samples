using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Megazone.HyperSubtitleEditor.Presentation.View;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    public class ProjectSelectBehavior : Behavior<ProjectSelectView>
    {
        private const int StageWidth = 340;
        private const int stagePaddingWidth = 34;
        private const int SidePageButtonWidth = 92;
        private const int windowFramePadding= 16;


        private int stagePerPage = 1;

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

            var screenWidth = view.ActualWidth + windowFramePadding;

            var pageWidth = (SidePageButtonWidth * 2) - stagePaddingWidth;

            var stageItemWidth = StageWidth + stagePaddingWidth;


            var stageNumberPerPage = Convert.ToInt32((screenWidth - (pageWidth)) / stageItemWidth);

            if (stagePerPage != stageNumberPerPage)
            {
                Command?.Execute(stageNumberPerPage);
            }
        }
    }
}
