using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Megazone.Api.Transcoder.Domain;
using Megazone.Cloud.Transcoder.Domain;
using Megazone.Core.IoC;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class JobListItemOutputViewModelBuilder
    {
        private readonly PresetLoader _presetLoader;

        public JobListItemOutputViewModelBuilder()
        {
            _presetLoader = Bootstrapper.Container.Resolve<PresetLoader>();
        }

        public async Task<IList<JobListItemOutputViewModel>> BuildAsync(Job job, bool forceReload = false)
        {
            if (!_presetLoader.IsPresetsAvailable)
                await _presetLoader.LoadAsync(forceReload);
            var presets = _presetLoader.Presets;
            var tempItems = new List<JobListItemOutputViewModel>();

            // Playlist에서 Format과 상대 경로를 저장.
            if (job.Payload.Playlists != null)
                foreach (var playlist in job.Payload.Playlists)
                {
                    var jobOutput = new JobListItemOutputViewModel
                    {
                        DisplayMediaType = MediaType.AdaptiveStreaming,
                        OutputKeys = playlist.OutputKeys?.ToList(),
                        DisplayName = playlist.Format.ToDisplayValue(),
                        RelativePath = playlist.Name,
                        OutputKeyPrefix = job.Payload.OutputKeyPrefix,
                        Extension = playlist.Format.GetExtension(),
                        OutputStatus = "Complete"
                    };
                    jobOutput.Initialize();
                    tempItems.Add(jobOutput);
                }

            // job의 output를 기준으로 playlist의 outputkey와 매칭하여 presetId를 저장.
            if (job.Payload.Outputs != null)
                foreach (var output in job.Payload.Outputs)
                {
                    var existOutputKey = false;

                    foreach (var itemOutput in tempItems)
                        if (itemOutput.OutputKeys.Contains(output.Key))
                        {
                            itemOutput.PresetIds.Add(output.PresetId);
                            existOutputKey = true;
                        }

                    // job의 output에는 있으나, playlist에 없는 output key의 경우 [(ex) video(mp4), audio]
                    if (!existOutputKey)
                    {
                        var extension = Path.GetExtension(output.Key);
                        string name;
                        if (string.IsNullOrEmpty(extension))
                        {
                            var matchedPresetId = presets.FirstOrDefault(p => p.Id == output.PresetId);

                            extension = (matchedPresetId?.Container)
                                        .ToLower() ??
                                        string.Empty;

                            name = extension.ToUpper()
                                .Trim();
                        }
                        else
                        {
                            name = extension.Substring(1)
                                .ToUpper()
                                .Trim();
                        }

                        var jobOutput = new JobListItemOutputViewModel
                        {
                            DisplayName = name,
                            DisplayMediaType = name.ToMediaType(),
                            RelativePath = output.Key,
                            OutputKeyPrefix = job.Payload.OutputKeyPrefix,
                            Extension = extension,
                            OutputStatus = "Complete"
                        };
                        jobOutput.Initialize();
                        jobOutput.OutputKeys.Add(output.Key);
                        jobOutput.PresetIds.Add(output.PresetId);

                        tempItems.Add(jobOutput);
                    }
                }

            return tempItems;
        }
    }
}