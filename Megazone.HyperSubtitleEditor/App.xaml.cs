using System;
using System.Windows;
using Megazone.Core.Log;
using Megazone.Core.Windows;
using Unity;
using AppContext = Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data.AppContext;

namespace Megazone.HyperSubtitleEditor
{
    /// <summary>
    ///     App.xaml에 대한 상호 작용 논리
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
            //InstanceWatcher.Instance.Changed +=
            //    () =>
            //    {
            //        //InstanceWatcher.Instance.Instances.ToList().ForEach(instance => _logger.Debug.Builder.AppendLine(instance.Target?.GetType().ToString()));
            //        //_logger.Debug.Builder.Write();
            //    };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _unhandledExceptionDelegator.Start();

            Initialize(e);
        }

        private void Initialize(StartupEventArgs e)
        {
#if DEBUG
            InitializeAppContext(new[] { "b9a19922-955e-489a-a0dd-66749ed6ea0a", "1499781023404-tl0eyt", "1517818883688-tmur13", "ap-southeast-1" });
#else
            InitializeAppContext(e.Args);
#endif
        }

        private static void InitializeAppContextForDebugMode(StartupEventArgs e)
        {
            if (e.Args.Length != 4)
            {
                //var profileId = "b9a19922-955e-489a-a0dd-66749ed6ea0a"; //"96813c47-ead6-4ab3-befa-e64e1f58d6bd"; // 디버그용
                //var pipelineId =
                // "1499781023404-tl0eyt"; //"1489492252497-6285ew"; // 디버그용 //test: 1499781023404-tl0eyt // kr: 1499779397208-9gjs8s
                //var jobId = "1503650920978-1k2l1x"; //"1497391412204-9cw521";
                //var region = "ap-southeast-1"; // singapore
                var profileId =
                    "b9a19922-955e-489a-a0dd-66749ed6ea0a"; //"96813c47-ead6-4ab3-befa-e64e1f58d6bd"; // 디버그용
                var pipelineId =
                    "1499781023404-tl0eyt"; //"1489492252497-6285ew"; // 디버그용 //test: 1499781023404-tl0eyt // kr: 1499779397208-9gjs8s
                var jobId = "1510898711707-ysecmk"; //"1497391412204-9cw521";
                var region = "ap-southeast-1"; // singapore
                if (string.IsNullOrEmpty(profileId) ||
                    string.IsNullOrEmpty(pipelineId))
                    throw new InvalidOperationException();
                new AppContext().SetConfig(profileId, pipelineId, jobId, region);
            }
            else
            {
                InitializeAppContext(e.Args);
            }
        }

        private static void InitializeAppContext(string[] args)
        {
            if (args.Length >= 2)
            {
                var profileId = args[0];
                var pipelineId = args[1];
                if (args.Length == 4)
                {
                    var jobId = args[2];
                    var region = args[3];
                    new AppContext().SetConfig(profileId, pipelineId, jobId, region);
                }
                else
                {
                    new AppContext().SetConfig(profileId, pipelineId, null, null);
                }
            }
            else
            {
                new AppContext().SetConfig(null, null, null, null);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _unhandledExceptionDelegator.Dispose();
            base.OnExit(e);
        }
    }
}