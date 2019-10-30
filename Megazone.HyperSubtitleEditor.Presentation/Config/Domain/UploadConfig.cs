//using Megazone.Cloud.Common.Domain;
//using Megazone.Core.IoC;
//using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;

//namespace Megazone.HyperSubtitleEditor.Presentation.Config.Domain
//{
//    [Inject(Source = typeof(IUploadConfig), Scope = LifetimeScope.Singleton)]
//    public class UploadConfig : IUploadConfig
//    {
//        public int ConnectionTimeout => ConfigHolder.Current.General.ConnectionTimeout;
//        public bool IsSecureProtocol => ConfigHolder.Current.Upload.IsSecureProtocol;
//        public long ChunkSize => ConfigHolder.Current.Upload.ChunkSize;
//    }
//}

