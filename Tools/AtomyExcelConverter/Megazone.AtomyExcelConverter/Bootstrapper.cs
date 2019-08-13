using System.Reflection;
using Megazone.AtomyExcelConverter.Presentation.Infrastructure.Extension;
using Megazone.Core.IoC.Unity;
using Megazone.Core.Log;
using Megazone.Core.Log.Log4Net;
using Microsoft.Practices.Unity;

namespace Megazone.AtomyExcelConverter
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
                new InjectionConstructor(this.AtomyExcelConverterAppDataPath() + "logs\\"));
        }
    }
}
