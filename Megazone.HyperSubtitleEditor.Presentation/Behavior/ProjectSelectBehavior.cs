using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Megazone.HyperSubtitleEditor.Presentation.View;

namespace Megazone.HyperSubtitleEditor.Presentation.Behavior
{
    public class ProjectSelectBehavior : Behavior<ProjectSelectView>
    {
        public static readonly DependencyProperty StageMarginWidthProperty =
            DependencyProperty.Register("StargeWidthMargin", typeof(int),
                typeof(ProjectSelectBehavior));

        public static readonly DependencyProperty StageWidthProperty = 
            DependencyProperty.Register("StageWidth", typeof(int),
                typeof(ProjectSelectBehavior));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand),
                typeof(ProjectSelectBehavior));

        private int _stagePerPage = 1;

        public int StageMarginWidth
        {
            get => (int)GetValue(StageMarginWidthProperty);
            set => SetValue(StageMarginWidthProperty, value);
        }

        public int StageWidth
        {
            get => (int) GetValue(StageWidthProperty);
            set => SetValue(StageWidthProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
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

            //var itemsControl = view.FindName("StageItemControl") as ItemsControl;
            var itemsControl = view.FindName("RootStackPanel") as StackPanel;

            var itemsControlWidth = itemsControl?.ActualWidth + StageMarginWidth;

            var stageItemWidth = StageWidth + StageMarginWidth;

            var stageNumberPerPage =
                Convert.ToInt32(Math.Truncate(Convert.ToDecimal(itemsControlWidth / stageItemWidth)));


            if (_stagePerPage != stageNumberPerPage)
            {
                _stagePerPage = stageNumberPerPage;
                Command?.Execute(stageNumberPerPage);
            }
        }
    }
}