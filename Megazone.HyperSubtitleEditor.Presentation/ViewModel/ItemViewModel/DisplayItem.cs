namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    public class DisplayItem
    {
        public DisplayItem(string display, string key)
        {
            Display = display;
            Key = key;
        }

        public string Display { get; }
        public string Key { get; }
    }
}