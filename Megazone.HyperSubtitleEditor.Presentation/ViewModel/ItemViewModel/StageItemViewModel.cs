using System.Collections.Generic;
using System.Linq;
using Megazone.Cloud.Media.Domain;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    public class StageItemViewModel : ViewModelBase
    {
        private bool _isSelected;
        private IEnumerable<ProjectItemViewModel> _projectItems;
        private ProjectItemViewModel _selectingProject;

        public StageItemViewModel(Stage stage)
        {
            Source = stage;
            Id = stage.Id;
            Name = stage.Name;
            SymbolTitle = string.IsNullOrEmpty(Name) ? "E" : Name.Substring(0, 1);
            ProjectItems = stage.Projects?.Select(project => new ProjectItemViewModel(stage.Id, project)).ToList();
        }

        public Stage Source { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string SymbolTitle { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public IEnumerable<ProjectItemViewModel> ProjectItems
        {
            get => _projectItems;
            set => Set(ref _projectItems, value);
        }

        public ProjectItemViewModel SelectingProjectInStage
        {
            get => _selectingProject;
            set => Set(ref _selectingProject, value);
        }
    }
}