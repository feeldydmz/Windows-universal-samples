using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Megazone.Cloud.Aws.Domain;
using Megazone.Cloud.Transcoder.Repository.ElasticTranscoder;
using Megazone.Core.Windows.Mvvm;
using Megazone.VideoStudio.Presentation.Common.Infrastructure.Data.Profile;

namespace Megazone.VideoStudio.Presentation.Common.Infrastructure.Data
{
    public class RegionManager : BindableBase
    {
        private RegionInformation _current;

        private IEnumerable<RegionInformation> _regions =
            new ObservableCollection<RegionInformation>();

        private RegionManager()
        {
        }

        public static RegionManager Instance { get; } = new RegionManager();

        public RegionInformation Current
        {
            get => _current;
            set
            {
                Set(ref _current, value);
                ClientProfileManager.Instance.GetSelectedProfile()
                        .ClientProfileCredentialInfo.CredentialJson =
                    new ParameterProvider().MakeCredentialJson(CreateCredentialInfo());
                ClientProfileManager.Instance.Save();
            }
        }

        public IEnumerable<RegionInformation> Regions
        {
            get => _regions;
            private set => Set(ref _regions, value);
        }

        public bool IsReady { get; private set; }

        public CredentialInfo CreateCredentialInfo()
        {
            var credentialInfo = ClientProfileManager.Instance.GetSelectedProfile()
                ?.ClientProfileCredentialInfo.Credential;
            if (credentialInfo == null)
                throw new InvalidOperationException();

            return new CredentialInfo(credentialInfo.ServiceType, credentialInfo.AccessKeyId,
                credentialInfo.SecretAccessKey, Current.Code);
        }

        private IEnumerable<AwsRegion> GetAvailableRegions()
        {
            // TODO: use API
            return new List<AwsRegion>
            {
                new AwsRegion("us-east-1", "US East (N. Virginia)"),
                new AwsRegion("us-west-1", "US West (N. California)"),
                new AwsRegion("us-west-2", "US West (Oregon)"),
                new AwsRegion("ap-south-1", "Asia Pacific (Mumbai)"),
                new AwsRegion("ap-southeast-1", "Asia Pacific (Singapore)"),
                new AwsRegion("ap-southeast-2", "Asia Pacific (Sydney)"),
                new AwsRegion("ap-northeast-1", "Asia Pacific (Tokyo)"),
                new AwsRegion("eu-west-1", "EU (Ireland)")
            };
        }

        public void Initialize(IEnumerable<RegionInformation> savedRegionInformations)
        {
            var availableRegions = GetAvailableRegions();
            var mergedRegionInformations = Merge(availableRegions, savedRegionInformations);
            Regions = new ObservableCollection<RegionInformation>(mergedRegionInformations);
            InitializeCurrent();
            IsReady = true;
        }

        private void InitializeCurrent()
        {
            var defaultRegion = Regions.FirstOrDefault(r => r.IsDefault);
            if (defaultRegion != null)
            {
                Current = defaultRegion;
            }
            else if (!string.IsNullOrEmpty(ClientProfileManager.Instance.GetSelectedProfile()
                         .ClientProfileCredentialInfo.Credential.Region) &&
                     Regions.Any(i => i.Code ==
                                      ClientProfileManager.Instance.GetSelectedProfile()
                                          .ClientProfileCredentialInfo.Credential.Region))
            {
                Current = Regions.FirstOrDefault(i => i.Code ==
                                                      ClientProfileManager.Instance.GetSelectedProfile()
                                                          .ClientProfileCredentialInfo.Credential.Region);
            }
            else
            {
                var apiConnectedRegion = Regions.FirstOrDefault(r => !string.IsNullOrEmpty(r.API));
                Current = apiConnectedRegion ?? Regions.FirstOrDefault();
            }
        }

        private IEnumerable<RegionInformation> Merge(IEnumerable<AwsRegion> availableRegions,
            IEnumerable<RegionInformation> savedRegionInformations)
        {
            var mergedList = new List<RegionInformation>();
            var profileRegionList = savedRegionInformations?.ToList();
            foreach (var region in availableRegions)
            {
                var existingInfo = profileRegionList?.FirstOrDefault(s => s.Code == region.Code);
                mergedList.Add(new RegionInformation
                {
                    Code = region.Code,
                    Name = region.Name,
                    API = existingInfo?.API,
                    IsDefault = existingInfo?.IsDefault ?? false
                });
            }
            return mergedList;
        }

        private class AwsRegion
        {
            public AwsRegion(string code, string name)
            {
                Code = code;
                Name = name;
            }

            public string Code { get; }
            public string Name { get; }
        }
    }
}