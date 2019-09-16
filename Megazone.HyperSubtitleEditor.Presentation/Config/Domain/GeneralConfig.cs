//using Megazone.Cloud.Common.Domain;
//using Megazone.Core.IoC;
//using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;

//namespace Megazone.HyperSubtitleEditor.Presentation.Config.Domain
//{
//    [Inject(Source = typeof(IGeneralConfig), Scope = LifetimeScope.Singleton)]
//    public class GeneralConfig : IGeneralConfig
//    {
//        public int ConnectionTimeout => ConfigHolder.Current.General.ConnectionTimeout;
//        public int RetryCount => ConfigHolder.Current.General.IsRetry ? ConfigHolder.Current.General.RetryCount : 1;

//        public int RetryInterval =>
//            ConfigHolder.Current.General.IsRetry ? ConfigHolder.Current.General.RetryInterval : 0;
//    }
//}