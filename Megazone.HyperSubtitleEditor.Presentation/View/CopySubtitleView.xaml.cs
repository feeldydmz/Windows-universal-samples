﻿using System.Windows;
using System.Windows.Controls;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    ///     CopySubtitleView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CopySubtitleView : UserControl
    {
        public CopySubtitleView()
        {
            InitializeComponent();
            Loaded += AddSubtitleView_Loaded;
            Unloaded += AddSubtitleView_Unloaded;
        }

        private void AddSubtitleView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is CopySubtitleViewModel vm)
                vm.CloseAction = CloseWindow;
        }

        private void AddSubtitleView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is CopySubtitleViewModel vm)
                vm.CloseAction = null;
        }

        private void CancelButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            var window = Window.GetWindow(this);
            window?.Close();
        }
    }
}