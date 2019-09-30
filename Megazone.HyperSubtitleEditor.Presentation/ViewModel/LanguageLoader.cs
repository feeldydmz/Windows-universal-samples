using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.IoC;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Source = typeof(LanguageLoader), Scope = LifetimeScope.Singleton)]
    public class LanguageLoader
    {
        private readonly ICloudMediaService _cloudMediaService;
        private readonly SignInViewModel _signInViewModel;

        private List<LanguageItem> _languages = new List<LanguageItem>();

        public LanguageLoader(ICloudMediaService cloudMediaService, SignInViewModel signInViewModel)
        {
            _cloudMediaService = cloudMediaService;
            _signInViewModel = signInViewModel;
        }

        public IEnumerable<LanguageItem> Languages => _languages;

        public async Task LoadAsync()
        {
            _languages = (await GetLanguagesAsync())?.ToList() ?? new List<LanguageItem>();
        }

        public async Task<IEnumerable<LanguageItem>> GetAsync()
        {
            if (!_languages.Any())
                _languages = (await GetLanguagesAsync())?.ToList() ?? new List<LanguageItem>();

            return _languages;
        }

        private async Task<IEnumerable<LanguageItem>> GetLanguagesAsync()
        {
            var authorizationAccessToken = _signInViewModel.GetAuthorization()?.AccessToken;
            var stageId = _signInViewModel.SelectedStage?.Id;
            var projectId = _signInViewModel.SelectedProject?.ProjectId;

            if (string.IsNullOrEmpty(authorizationAccessToken) || string.IsNullOrEmpty(stageId) || string.IsNullOrEmpty(projectId))
                return new List<LanguageItem>();

            var languages = await _cloudMediaService.GetLanguageAsync(
                new GetLanguageParameter(authorizationAccessToken, stageId, projectId), CancellationToken.None);

            return languages?.Select(language => new LanguageItem(language)) ?? new List<LanguageItem>();
        }
    }
}