using System.Collections.Generic;
using System.Linq;
using Megazone.Cloud.Media.Domain;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    public class StageItemViewModel : ViewModelBase
    {
        private IEnumerable<ProjectItemViewModel> _projectItems;

        public StageItemViewModel(Stage stage)
        {
            ProjectItems = stage.Projects?.Select(project => new ProjectItemViewModel(project));
            Id = stage.Id;
            Name = stage.Name;
        }

        public string Id { get; }
        public string Name { get; }

        public IEnumerable<ProjectItemViewModel> ProjectItems
        {
            get => _projectItems;
            set => Set(ref _projectItems, value);
        }
    }
}