using Megazone.Cloud.Media.Domain;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    public class ProjectItemViewModel : ViewModelBase
    {
        private bool _isDefault;
        private bool _isSelected;

        public ProjectItemViewModel(string stageId, Project project)
        {
            StageId = stageId;
            ProjectId = project.Id;
            Name = project.Name;
            DisplayTitle = $"{Name}/{ProjectId}";
            IsActive = project.IsActive;
        }

        public string DisplayTitle { get; }
        public string StageId { get; }
        public string ProjectId { get; }
        public string Name { get; }
        public bool IsActive { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set => Set(ref _isSelected, value);
        }

        public bool IsDefault
        {
            get => _isDefault;
            set => Set(ref _isDefault, value);
        }
    }
}