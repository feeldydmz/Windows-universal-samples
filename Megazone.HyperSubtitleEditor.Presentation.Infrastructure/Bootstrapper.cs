using System.Reflection;
using Megazone.Core.IoC.Unity;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure
{
    public class Bootstrapper
    {
        public static IUnityContainer Container { get; private set; }

        public void Initialize()
        {
            var configuration = new Configuration(Assembly.GetAssembly(GetType()));

            configuration.Initialize();
            Container = configuration.Container;
        }
    }
}