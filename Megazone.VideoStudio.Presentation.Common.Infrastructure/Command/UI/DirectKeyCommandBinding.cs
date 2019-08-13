using System.Windows.Input;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Command.UI
{
    public class DirectKeyCommandBinding : CommandBinding
    {
        private KeyGesture _keyGesture;

        public DirectKeyCommandBinding()
        {
            Command = new RoutedCommand();
            Executed += DirectKeyCommandBinding_Executed;
            CanExecute += DirectKeyCommandBinding_CanExecute;
        }

        public ICommand RealCommand { get; set; }

        private new ICommand Command
        {
            get => base.Command;
            set => base.Command = value;
        }

        public KeyGesture KeyGesture
        {
            get => _keyGesture;
            set
            {
                _keyGesture = value;
                if (_keyGesture != null)
                {
                    ((RoutedCommand) Command).InputGestures.Clear();
                    ((RoutedCommand) Command).InputGestures.Add(value);
                }
            }
        }

        private void DirectKeyCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RealCommand?.Execute(null);
        }

        private void DirectKeyCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = RealCommand?.CanExecute(null) ?? false;
        }
    }
}