﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.View
{
    /// <summary>
    /// ProjectSelectView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProjectSelectView : UserControl
    {
        public ProjectSelectView()
        {
            InitializeComponent();
        }

        private void ProjectSelectView_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var view =  sender as ProjectSelectView;

            int pageWidth = 92 * 2 - 38;

            int stageItemWidth = 340 + 35;

            int stageNumberPerPage =  (int)((view.ActualWidth - (pageWidth)) / stageItemWidth);

            if (DataContext is SignInViewModel viewmodle) viewmodle.StageNumberPerPage = stageNumberPerPage;

            Debug.WriteLine($@"StageItemControl.ActualWidth : {StageItemControl.ActualWidth} stageNumberPerPage : {stageNumberPerPage}");
        }
    }
}
