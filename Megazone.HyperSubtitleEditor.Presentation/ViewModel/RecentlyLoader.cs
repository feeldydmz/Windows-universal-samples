using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Extension;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Newtonsoft.Json;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class RecentlyLoader
    {
        private const int MaxSize = 20;
        private readonly ILogger _logger;
        private readonly SignInViewModel _signInViewModel;
        private List<RecentlyItem> _recentlyItems = new List<RecentlyItem>();

        public RecentlyLoader(ILogger logger, SignInViewModel signInViewModel)
        {
            _logger = logger;
            _signInViewModel = signInViewModel;
        }

        public IEnumerable<RecentlyItem> GetRecentlyItems()
        {
            var stageId = _signInViewModel.SelectedStage?.Id;
            var projectId = _signInViewModel.SelectedProject?.ProjectId;

            return _recentlyItems
                .Where(recently => recently.StageId.Equals(stageId) && recently.ProjectId.Equals(projectId))
                .OrderByDescending(recently => recently.CreatedTime).ToList();
        }

        public void Load()
        {
            var filePath = GetSavePath();
            var recentlyDirPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(recentlyDirPath) && !Directory.Exists(recentlyDirPath))
                Directory.CreateDirectory(recentlyDirPath);

            if (!File.Exists(filePath))
                return;

            try
            {
                var jsonString = File.ReadAllText(filePath);
                _recentlyItems = JsonConvert.DeserializeObject<List<RecentlyItem>>(jsonString).ToList();
            }
            catch (Exception e)
            {
                _logger.Error.Write(e);
            }
        }

        public void Save(RecentlyItem item)
        {
            try
            {
                if (_recentlyItems == null)
                    _recentlyItems = new List<RecentlyItem>();

                if (_recentlyItems.Any())
                    _recentlyItems.Insert(0, item);
                else
                    _recentlyItems.Add(item);


                var stageId = _signInViewModel.SelectedStage?.Id;
                var projectId = _signInViewModel.SelectedProject?.ProjectId;
                var list = _recentlyItems
                    .Where(recently => recently.StageId.Equals(stageId) && recently.ProjectId.Equals(projectId))
                    .OrderByDescending(recently => recently.CreatedTime).ToList();

                if (list.Count > MaxSize)
                    _recentlyItems.Remove(list.Last());

                var jsonString = JsonConvert.SerializeObject(_recentlyItems);
                File.WriteAllText(GetSavePath(), jsonString);
            }
            catch (Exception e)
            {
                _logger.Error.Write(e);
            }
        }

        private string GetSavePath()
        {
            return $"{this.AppDataPath()}\\Recently\\Recently.dat";
        }
    }
}