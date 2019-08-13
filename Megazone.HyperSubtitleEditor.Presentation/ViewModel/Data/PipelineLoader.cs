using System;
using System.Collections.Generic;
using System.Linq;
using Megazone.Cloud.Common.Domain;
using Megazone.Cloud.Storage.Domain;
using Megazone.Cloud.Storage.ServiceInterface.S3;
using Megazone.Cloud.Transcoder.Domain;
using Megazone.Cloud.Transcoder.Domain.ElasticTranscoder.Model;
using Megazone.Cloud.Transcoder.Repository.ElasticTranscoder;
using Megazone.Core.Extension;
using Megazone.Core.Log.Log4Net.Extension;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal class PipelineLoader
    {
        public static readonly PipelineLoader Instance = new PipelineLoader();
        private readonly IS3Service _s3Service;
        private readonly ITranscodingRepository _transcodingRepository;

        private PipelineLoader()
        {
            _transcodingRepository = Bootstrapper.Container.Resolve<ITranscodingRepository>();
            _s3Service = Bootstrapper.Container.Resolve<IS3Service>();
        }

        public Pipeline SelectedPipeline { get; private set; }
        public ICredentialInfo InputBucketCredentialInfo { get; private set; }
        public ICredentialInfo OutputBucketCredentialInfo { get; private set; }
        public ICredentialInfo ThumbnailBucketCredentialInfo { get; private set; }

        private void SetSelectedPipeline(Pipeline pipeline, ICredentialInfo credentialInfo)
        {
            SelectedPipeline = pipeline;
            InputBucketCredentialInfo = GetS3CredentialInfoWithRegion(pipeline.InputBucket, credentialInfo);
            OutputBucketCredentialInfo = GetS3CredentialInfoWithRegion(pipeline.OutputBucket, credentialInfo);
            if (pipeline.ThumbnailConfig == null)
                return;
            ThumbnailBucketCredentialInfo = pipeline.OutputBucket == pipeline.ThumbnailConfig.Bucket
                ? OutputBucketCredentialInfo
                : GetS3CredentialInfoWithRegion(pipeline.ThumbnailConfig.Bucket, credentialInfo);
        }

        private void LoadPipelines(string parameter, Action<FindAllPipelinesResult> partHandler)
        {
            this.InvokeOnTask(() => { _transcodingRepository.FindAllQueues(parameter, partHandler); });
        }

        private ICredentialInfo GetS3CredentialInfoWithRegion(string driveName, ICredentialInfo credentialInfo)
        {
            var credentialInfoWithRegion = _s3Service.PutRegionTo(new Drive(new DriveName(driveName)),
                credentialInfo);
            return credentialInfoWithRegion;
        }

        public void LoadPipeline(string pipelineId, ICredentialInfo credentialInfo, Action<bool> completionAction)
        {
            var pipelines = new List<Pipeline>();
            var parameter = new ParameterBuilder(credentialInfo).Build();
            LoadPipelines(parameter, partResult =>
            {
                if (partResult != null)
                {
                    partResult.Items?.ForEach(
                        item =>
                            pipelines.Add((Pipeline) item));
                    if (string.IsNullOrEmpty(partResult.NextPageToken))
                        OnLoadPipelinesCompleted(pipelineId, credentialInfo, completionAction, pipelines);
                }
                else
                {
                    OnLoadPipelinesCompleted(pipelineId, credentialInfo, completionAction, pipelines);
                }
            });
        }

        private void OnLoadPipelinesCompleted(string pipelineId, ICredentialInfo credentialInfo,
            Action<bool> completionAction,
            IList<Pipeline> pipelines)
        {
            SelectedPipeline = pipelines.FirstOrDefault(p => p.Id == pipelineId);
            var foundPipeline = SelectedPipeline != null;
            if (foundPipeline)
                SetSelectedPipeline(SelectedPipeline, credentialInfo);
            completionAction.Invoke(foundPipeline);
        }
    }
}