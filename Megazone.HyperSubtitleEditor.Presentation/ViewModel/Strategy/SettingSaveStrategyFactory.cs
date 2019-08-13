namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Strategy
{
    internal static class SettingSaveStrategyFactory
    {
        public static ISettingSaveStrategy GetDefault()
        {
            return new DefaultSettingSaveStrategy();
        }
    }
}