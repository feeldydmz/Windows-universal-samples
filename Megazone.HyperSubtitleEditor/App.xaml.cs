using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using Megazone.Core.Log;
using Megazone.Core.Windows;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;
using Megazone.SubtitleEditor.Resources;
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

            

            if (ConfigHolder.Current != null)
            {
                if (!string.IsNullOrEmpty(ConfigHolder.Current.Subtitle.Language))
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(ConfigHolder.Current.Subtitle.Language);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(ConfigHolder.Current.Subtitle.Language);
                    Resource.Culture = new CultureInfo(ConfigHolder.Current.Subtitle.Language);
                }
            }
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
            //InitializeMcmData(new string[] { "megazone.hypersubtitleeditor.v1://mz-cm-console-dev.s3-website.ap-northeast-2.amazonaws.com/contents?stageId=mz-cm-v1&projectId=mz-cm-v1&videoId=1310&assetId=5910&captionIds='1,2,3,4,5,6'" });
            //InitializeMcmData(new string[] { "stageId=mz-cm-v1&projectId=mz-cm-v1&videoId=&assetId=15879869464u8a&captionIds=490,491" });
            //InitializeMcmData(new string[] { "stageId=mz-cm-v1&projectId=mz-cm-v1&videoId=1588831786qRiN&assetId=1588754547Beq6&captionIds=514" });
            InitializeMcmData(e.Args);
#else
            //InitializeAppContext(e.Args);
            InitializeMcmData(e.Args);
#endif
        }

        private void InitializeMcmData(string[] args)
        {
            if (!(args?.Any() ?? false))
                return;

            var arguments = args[0];
            if (string.IsNullOrEmpty(arguments))
                return;

            var parameterList =
                arguments
                    .Split('&')
                    .Where(part =>
                    {
                        var array = part.Split('=');
                        return array.Length > 1;
                    })
                    .Select(
                    part =>
                    {
                        var array = part.Split('=');

                        return new KeyValuePair<string, string>(array[0], array[1]);
                    }).ToList();

            foreach (var keyValuePair in parameterList)
            {
                Debug.WriteLine($"parameterList-feel key : {keyValuePair.Key}, value : {keyValuePair.Value}");
            }

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