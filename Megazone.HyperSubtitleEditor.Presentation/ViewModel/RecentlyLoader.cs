using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const int MaxSize = 10;
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

            var reuslt = _recentlyItems
                .Where(recently => recently.StageId.Equals(stageId) && recently.ProjectId.Equals(projectId))
                .OrderByDescending(recently => recently.CreatedTime);

            Debug.WriteLine("---GetRecentlyItems---");

            foreach (var recentlyItem in reuslt)
                Debug.WriteLine($"{recentlyItem.Video?.Name}, Time : {recentlyItem.CreatedTime}");

            return reuslt.ToList();
            //return _recentlyItems
            //    .Where(recently => recently.StageId.Equals(stageId) && recently.ProjectId.Equals(projectId))
            //    .OrderByDescending(recently => recently.CreatedTime).ToList();
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

                var toRemove = new HashSet<RecentlyItem>();

                // 기존 최근 열린 작업내용에 있다면 갱신
                if (item.Video != null && item.CaptionAsset != null)
                    // 비디오 저장시
                    foreach (var recentlyItem in _recentlyItems.Where(recentlyItem =>
                        recentlyItem.Video?.Id == item.Video?.Id &&
                        recentlyItem.CaptionAsset?.Id == item.CaptionAsset?.Id))
                        toRemove.Add(recentlyItem);
                else if (item.Video == null && item.CaptionAsset != null)
                    // 캡션에셋 저장시
                    foreach (var recentlyItem in _recentlyItems.Where(recentlyItem =>
                        recentlyItem.CaptionAsset?.Id == item.CaptionAsset?.Id))
                        toRemove.Add(recentlyItem);
                else if (item.Video != null && item.CaptionAsset == null)
                    // 비디오 저장시
                    foreach (var recentlyItem in _recentlyItems.Where(recentlyItem =>
                        recentlyItem.Video?.Id == item.Video?.Id))
                        toRemove.Add(recentlyItem);


                if (toRemove.Count() != 0) _recentlyItems.RemoveAll(toRemove.Contains);

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