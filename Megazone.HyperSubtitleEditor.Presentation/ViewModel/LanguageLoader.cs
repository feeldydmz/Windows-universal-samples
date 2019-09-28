using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.IoC;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Source = typeof(LanguageLoader), Scope = LifetimeScope.Singleton)]
    public class LanguageLoader
    {
        private readonly ICloudMediaService _cloudMediaService;
        private readonly SignInViewModel _signInViewModel;

        private List<Language> _languages = new List<Language>();

        public LanguageLoader(ICloudMediaService cloudMediaService, SignInViewModel signInViewModel)
        {
            _cloudMediaService = cloudMediaService;
            _signInViewModel = signInViewModel;
        }

        public IEnumerable<Language> Languages => _languages;

        public async Task LoadAsync()
        {
            _languages = (await GetLanguagesAsync())?.ToList() ?? new List<Language>();
        }

        public async Task<IEnumerable<Language>> GetAsync()
        {
            if (!_languages.Any())
                _languages = (await GetLanguagesAsync())?.ToList() ?? new List<Language>();

            return _languages;
        }

        private async Task<IEnumerable<Language>> GetLanguagesAsync()
        {
            var authorizationAccessToken = _signInViewModel.GetAuthorization().AccessToken;
            var stageId = _signInViewModel.SelectedStage?.Id;
            var projectId = _signInViewModel.SelectedProject?.ProjectId;

            return await _cloudMediaService.GetLanguageAsync(
                new GetLanguageParameter(authorizationAccessToken, stageId, projectId), CancellationToken.None);
        }
    }
}