using System.Reflection;
using Megazone.Core.IoC.Unity;
using Unity;
using S3Repository = Megazone.Cloud.Storage.Repository.S3;
using S3Service = Megazone.Cloud.Storage.Service.S3;

namespace Megazone.HyperSubtitleEditor.Presentation
{
    public class Bootstrapper
    {
        public static IUnityContainer Container { get; private set; }

        public void Initialize()
        {
            new S3Service.Bootstrapper().Initialize();

            var configuration = new Configuration(Assembly.GetAssembly(GetType()));
            configuration.Initialize();
            Container = configuration.Container;

            new S3Repository.Bootstrapper().Initialize();
            new Cloud.Transcoder.Repository.ElasticTranscoder.Bootstrapper().Initialize();
            new Api.Transcoder.Service.Bootstrapper().Initialize();
        }
    }
}