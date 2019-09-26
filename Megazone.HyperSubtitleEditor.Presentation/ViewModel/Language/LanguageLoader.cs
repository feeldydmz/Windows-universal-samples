using System.Collections.Generic;
using System.Threading;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Cloud.Media.ServiceInterface.Parameter;
using Megazone.Core.Extension;
using Megazone.Core.IoC;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Language
{
    [Inject(Source = typeof(LanguageLoader), Scope = LifetimeScope.Singleton)]
    public class LanguageLoader
    {
        private readonly ICloudMediaService _cloudMediaService;

        public LanguageLoader(ICloudMediaService cloudMediaService)
        {
            _cloudMediaService = cloudMediaService;

            Languages = new List<Language>();
        }

        public List<Language> Languages { get; }

        public async void UpdateLanguageAsync(string authorizationAccessToken, string stageId, string projectId)
        {
            var languages = await _cloudMediaService.GetLanguageAsync(
                new GetLanguageParameter(authorizationAccessToken, stageId, projectId), CancellationToken.None);

            foreach (var language in languages)
            {
                var newLanguage = new Language();

                if (language.Name.IsNotNullAndAny())
                {
                    var splitStrings = language.Name.Split('-');

                    if (splitStrings.Length >= 2)
                    {
                        newLanguage.Name = splitStrings[0].Trim();
                        newLanguage.CountryInfo.Name = splitStrings[1].Trim();
                    }
                    else
                    {
                        newLanguage.Name = language.Name;
                        newLanguage.CountryInfo.Name = language.Name;
                    }
                }

                if (language.Code.IsNotNullAndAny())
                {
                    var splitStrings = language.Code.Split('-');

                    if (splitStrings.Length >= 2)
                    {
                        newLanguage.Alpha2 = splitStrings[0].Trim();
                        newLanguage.CountryInfo.Alpha2 = splitStrings[1].Trim();
                    }
                }

                Languages.Add(newLanguage);
            }
        }
    }
}