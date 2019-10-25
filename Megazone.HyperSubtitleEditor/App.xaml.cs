using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
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

            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

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
            //InitializeAppContext(new[] { "b9a19922-955e-489a-a0dd-66749ed6ea0a", "1503635312970-mqi68t"});
            //InitializeAppContext(new[] { "b9a19922-955e-489a-a0dd-66749ed6ea0a", "1503635312970-mqi68t", "1503991012824-nhzcae", "ap-southeast-1" });
            //InitializeAppContext(new[] { "b9a19922-955e-489a-a0dd-66749ed6ea0a", "1499781023404-tl0eyt", "1543807711137-znn0wp", "ap-southeast-1" });
            //InitializeAppContext(new[] { "b9a19922-955e-489a-a0dd-66749ed6ea0a", "1499781023404-tl0eyt", "1518573724969-ngy3k1", "ap-southeast-1" });

            //InitializeMcmData(new string[] { "megazone.hypersubtitleeditor.v1://mz-cm-console-dev.s3-website.ap-northeast-2.amazonaws.com/contents?stageId=mz-cm-v1&projectId=mz-cm-v1&videoId=1310&assetId=5910&captionIds='1,2,3,4,5,6'" });
#else
            //InitializeAppContext(e.Args);
            InitializeMcmData(e.Args);
#endif
        }

        private void InitializeMcmData(string[] args)
        {
            if (!(args?.Any() ?? false))
                return;

            var uri = new Uri(args[0]);
            if (!uri.IsWellFormedOriginalString())
                return;

            var parameterList =
                (uri.Query.StartsWith("?") ? uri.Query.Substring(1, uri.Query.Length - 1) : uri.Query)
                .Split('&')
                .Select(
                    part =>
                    {
                        var array = part.Split('=');
                        return new KeyValuePair<string, string>(array[0], array[1]);
                    }).ToList();

            AppContext.SetMcmOpenData(parameterList);
        }

        //private static void InitializeAppContext(string[] args)
        //{
        //    if (args.Length >= 2)
        //    {
        //        var profileId = args[0];
        //        var pipelineId = args[1];
        //        if (args.Length == 4)
        //        {
        //            var jobId = args[2];
        //            var region = args[3];
        //            new AppContext().SetConfig(profileId, pipelineId, jobId, region);
        //        }
        //        else
        //        {
        //            new AppContext().SetConfig(profileId, pipelineId, null, null);
        //        }
        //    }
        //    else
        //    {
        //        new AppContext().SetConfig(null, null, null, null);
        //    }
        //}

        protected override void OnExit(ExitEventArgs e)
        {
            _unhandledExceptionDelegator.Dispose();
            base.OnExit(e);
        }
    }
}