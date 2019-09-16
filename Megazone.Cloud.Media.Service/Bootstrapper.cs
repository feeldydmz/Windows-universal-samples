using System.Reflection;
using Megazone.Core.IoC.Unity;
using Unity;

namespace Megazone.Cloud.Media.Service
{
    public class Bootstrapper
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