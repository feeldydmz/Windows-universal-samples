using System;
using System.Windows;
using System.Windows.Input;
using Megazone.Core.Debug.Extension;
using Megazone.Core.Windows.Wpf.Debug;

namespace Megazone.HyperSubtitleEditor.View
{
    /// <summary>
    ///     MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Application.Current.MainWindow = this;

            InitializeComponent();

#if DEBUG
            ConsoleBuilder.Create()
                .View(
                    RootPanel,
                    new ConsoleView(),
                    "Usage\r\nCtrl+D : Toggle this console\r\nCtrl+R : Refresh watching instances\r\nCtrl+G : GC.Collect")
                ?
                .Command(
                    this,
                    new OpenConsoleRoutedCommand()
                        .AddInputGesture(new KeyGesture(Key.D, ModifierKeys.Control))
                        .AddInputGesture(new KeyGesture(Key.T, ModifierKeys.Control))
                        .AddInputGesture(new KeyGesture(Key.D, ModifierKeys.Alt)))
                .Command(
                    this,
                    new RefreshInstanceRoutedCommand()
                        .AddInputGesture(new KeyGesture(Key.R, ModifierKeys.Control)))
                .Command(
                    this,
                    new CollectFromGCRoutedCommand()
                        .AddInputGesture(new KeyGesture(Key.G, ModifierKeys.Control)));


            //var obj = this.New(() => new Object());
            //this.Watch();
#endif
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            // ToDO : 자연 종료 유도
            Application.Current.Shutdown();
        }
    }
}