using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megazone.Core.IoC;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class CaptionAssetListViewModel : ViewModelBase
    {

    }
}
