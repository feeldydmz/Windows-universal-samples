using System.Windows;
using Megazone.Core.Log;
using Megazone.Core.Windows;
using Unity;

namespace Megazone.AtomyExcelConverter
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private readonly UnhandledExceptionDelegator _unhandledExceptionDelegator;

        public App()
        {
            new Bootstrapper().Initialize();
            var logger = Bootstrapper.Container.Resolve<ILogger>();
            _unhandledExceptionDelegator = new UnhandledExceptionDelegator(
                this,
                ex => { logger.Error.Write(ex); });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _unhandledExceptionDelegator.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            _unhandledExceptionDelegator.Dispose();
        }

    }
}
