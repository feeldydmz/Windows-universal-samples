using System.Reflection;
using Megazone.Core.IoC.Unity;
using Megazone.Core.Log;
using Megazone.Core.Log.Log4Net;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Extension;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Megazone.HyperSubtitleEditor
{
    public class Bootstrapper
    {
        public static IUnityContainer Container { get; private set; }

        public void Initialize()
        {
            var configuration = new Configuration(Assembly.GetAssembly(GetType()));
            configuration.Initialize();
            Container = configuration.Container;

            configuration.Container.RegisterType<ILogger, Logger>(
                new ContainerControlledLifetimeManager(),
                new InjectionConstructor(this.AppDataPath() + "logs\\"));
            new VideoStudio.Presentation.Common.Infrastructure.Bootstrapper().Initialize();
            new Presentation.Infrastructure.Bootstrapper().Initialize();
        }
    }
}