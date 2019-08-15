using Megazone.Cloud.Common.Domain;
using Megazone.Core.IoC.Unity;
using System.Reflection;
using Unity;

namespace Megazone.Cloud.Media.Service
{
    public class Bootstrapper : IBootstrapper
    {
        public IUnityContainer Container { get; private set; }

        public void Initialize()
        {
            var configuration = new Configuration(Assembly.GetAssembly(GetType()));

            configuration.Initialize();
            Container = configuration.Container;

            new Repository.Bootstrapper().Initialize();
        }
    }
}
