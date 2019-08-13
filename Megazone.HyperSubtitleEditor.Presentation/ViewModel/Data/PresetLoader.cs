using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Megazone.Cloud.Common.Domain;
using Megazone.Cloud.Transcoder.Domain;
using Megazone.Cloud.Transcoder.Domain.ElasticTranscoder.Model;
using Megazone.Cloud.Transcoder.Repository.ElasticTranscoder;
using Megazone.Core.Extension;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.Core.Log.Log4Net.Extension;
using Megazone.Core.Reference;
using Megazone.Core.Windows.Extension;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class PresetLoader
    {
        private readonly ILogger _logger;
        private readonly ITranscodingRepository _repository;

        private bool _isLoading;

        public PresetLoader(ITranscodingRepository repository,
            ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public bool IsPresetsAvailable => Presets != null && Presets.Any() && !_isLoading;

        public IList<Preset> Presets { get; private set; }

        public void FindById(string id, WeakAction<Preset> callbackAction)
        {
            this.InvokeOnTask(() =>
            {
                if (Presets == null || !Presets.Any())
                    Load(null);
                while (_isLoading)
                    Thread.Sleep(500);
                if (callbackAction?.IsAlive ?? false)
                    callbackAction.Execute(Presets?.FirstOrDefault(preset => preset.Id == id));
            });
        }

        public async Task<Preset> FindByIdAsync(string id)
        {
            return await this.CreateTask(() =>
            {
                if (Presets == null || !Presets.Any())
                    Load(null);
                while (_isLoading)
                    Thread.Sleep(500);
                return Presets?.FirstOrDefault(preset => preset.Id == id);
            });
        }

        public Task LoadAsync(bool forceReload = false)
        {
            return this.CreateTask(() =>
            {
                if (_isLoading)
                {
                    while (_isLoading)
                        Thread.Sleep(500);
                    if (forceReload == false)
                        return;
                }
                _isLoading = true;
                try
                {
                    if (forceReload == false && Presets != null && Presets.Any())
                        return;
                    var parameter =
                        new ParameterBuilder(AppContext.CredentialInfo).Build();
                    Presets = new List<Preset>();
                    _repository.FindAllOutputPresets(parameter,
                        partResult =>
                        {
                            if (partResult != null)
                            {
                                partResult.Items.ForEach(item => Presets.Add((Preset) item));
                                if (string.IsNullOrEmpty(partResult.NextPageToken))
                                    _isLoading = false;
                            }
                            else
                            {
                                _isLoading = false;
                            }
                        });
                }
                catch (ServiceException ex)
                {
                    _logger.Error.Write(ex);
                }
                finally
                {
                    _isLoading = false;
                }
            });
        }

        public void Load(WeakAction<IList<Preset>> callbackAction, bool forceReload = false)
        {
            this.InvokeOnTask(() =>
            {
                if (_isLoading)
                {
                    while (_isLoading)
                        Thread.Sleep(500);
                    if (forceReload == false)
                    {
                        if (callbackAction?.IsAlive ?? false)
                            callbackAction.Execute(Presets);
                        return;
                    }
                }
                _isLoading = true;
                try
                {
                    if (forceReload == false && Presets != null && Presets.Any())
                    {
                        if (callbackAction?.IsAlive ?? false)
                            callbackAction.Execute(Presets);
                        return;
                    }
                    var parameter =
                        new ParameterBuilder(AppContext.CredentialInfo).Build();
                    Presets = new List<Preset>();
                    _repository.FindAllOutputPresets(parameter, partResult =>
                    {
                        if (partResult != null)
                        {
                            partResult.Items.ForEach(item => Presets.Add((Preset) item));
                            if (string.IsNullOrEmpty(partResult.NextPageToken))
                            {
                                _isLoading = false;
                                this.InvokeOnUi(
                                    () =>
                                    {
                                        if (callbackAction?.IsAlive ?? false)
                                            callbackAction.Execute(Presets);
                                    });
                            }
                        }
                        else
                        {
                            _isLoading = false;
                            this.InvokeOnUi(() =>
                            {
                                if (callbackAction?.IsAlive ?? false)
                                    callbackAction.Execute(Presets);
                            });
                        }
                    });
                }
                catch (ServiceException ex)
                {
                    _logger.Error.Write(ex);
                    this.InvokeOnUi(() =>
                    {
                        if (callbackAction?.IsAlive ?? false)
                            callbackAction.Execute(Presets);
                    });
                }
                finally
                {
                    _isLoading = false;
                }
            });
        }
    }
}