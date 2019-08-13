using Megazone.Core.Debug.Extension;
using Megazone.Core.Windows.Mvvm;

namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure
{
    public abstract class ViewModelBase : BindableBase
    {
        protected ViewModelBase()
        {
            this.Watch();
        }
    }
}