﻿using System.Collections.Generic;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Model;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    public class CaptionListViewModel : ViewModelBase
    {
        private readonly ICloudMediaService _cloudMediaService;
        private readonly SignInViewModel _signInViewModel;

        private IEnumerable<AssetItemViewModel<Cloud.Media.Domain.Assets.CaptionAsset>> _assetItems;

        private ICommand _loadCommand;

        public CaptionListViewModel(ICloudMediaService cloudMediaService, SignInViewModel signInViewModel)
        {
            _cloudMediaService = cloudMediaService;
            _signInViewModel = signInViewModel;
        }

        public IEnumerable<AssetItemViewModel<Cloud.Media.Domain.Assets.CaptionAsset>> AssetItems
        {
            get => _assetItems;
            set => Set(ref _assetItems, value);
        }

        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(LoadAsync); }
        }

        private async void LoadAsync()
        {
            var authorization = _signInViewModel.GetAuthorization();
            var stageId = _signInViewModel.SelectedStage.Id;
            var projectId= _signInViewModel.SelectedProject.ProjectId;

            var result =
                await _cloudMediaService.GetCaptionAssetsAsync(new GetAssetsParameter(authorization, stageId, projectId,
                    new Pagination(0, 10)));

            SelectedPageIndex = result.Offset;
        }

        private int _selectedPageIndex;

        public int SelectedPageIndex
        {
            get => _selectedPageIndex;
            set => Set(ref _selectedPageIndex, value);
        }
    }
}