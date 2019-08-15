﻿using Megazone.Cloud.Media.Domain;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    public class ProjectItemViewModel : ViewModelBase
    {
        private bool _isDefault;
        private bool _isSelected;

        public ProjectItemViewModel(Project project)
        {
            Id = project.Id;
            Name = project.Name;
        }

        public string Id { get; }
        public string Name { get; }

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