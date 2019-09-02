using System;
using System.Collections.Generic;
using System.Linq;
using Megazone.Cloud.Media.Domain;
using Megazone.Core.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    public class StageItemViewModel : ViewModelBase
    {
        private IEnumerable<ProjectItemViewModel> _projectItems;

        public StageItemViewModel(StageItemViewModel model)
        {
            this.Id = model.Id;
            this.Name = model.Name;
            this.ProjectItems = model.ProjectItems;
        }

        public StageItemViewModel(Stage stage)
        {
            Id = stage.Id;
            Name = stage.Name;
            SymbolTitle = Name.IsNullOrEmpty() ? "E" : Name.Substring(0, 1);
            ProjectItems = stage.Projects?.Select(project => new ProjectItemViewModel(stage.Id, project)).ToList();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string SymbolTitle { get; set; }

        private bool _isSelected;
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
    }
}