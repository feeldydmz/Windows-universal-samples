//using Megazone.Cloud.Common.Domain;
//using Megazone.Core.IoC;
//using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;

//namespace Megazone.HyperSubtitleEditor.Presentation.Config.Domain
//{
//    [Inject(Source = typeof(IDownloadConfig), Scope = LifetimeScope.Singleton)]
//    public class DownloadConfig : IDownloadConfig
//    {
//        public int ConnectionTimeout => ConfigHolder.Current.General.ConnectionTimeout;
//        public bool IsSecureProtocol => ConfigHolder.Current.Download.IsSecureProtocol;
//        public long ChunkSize => ConfigHolder.Current.Download.ChunkSize;
//    }
//}