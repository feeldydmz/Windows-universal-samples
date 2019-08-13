using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal class SubtitleListItemViewModelToWebVttStringParaser : ISubtitleListItemViewModelParser
    {
        public string Run(ISubtitleListItemViewModel item)
        {
            // TODO: CueSetting data
            var text = item.DisplayText?.Replace("<br/>", "\r\n");
            return
                $"{item.Number}\r\n{item.StartTime:hh\\:mm\\:ss\\.fff} --> {item.EndTime:hh\\:mm\\:ss\\.fff}\r\n{text}";
        }
    }
}