namespace Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View
{
    public abstract class LoadingManagerBase : ILoadingManager
    {
        private int _count;

        public void Show()
        {
            if (_count == 0)
                ShowView();
            _count++;
        }

        public void Hide()
        {
            _count--;
            if (_count == 0)
                HideView();
        }

        protected abstract void HideView();
        protected abstract void ShowView();
    }
}