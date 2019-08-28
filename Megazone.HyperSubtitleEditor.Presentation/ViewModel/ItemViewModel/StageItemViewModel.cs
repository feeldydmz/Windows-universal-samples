using System;
using System.Collections.Generic;
using System.Linq;
using Megazone.Cloud.Media.Domain;
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
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsSelectedStage { get; set; }

        public IEnumerable<ProjectItemViewModel> ProjectItems
        {
            get => _projectItems;
            set => Set(ref _projectItems, value);
        }
    }
}