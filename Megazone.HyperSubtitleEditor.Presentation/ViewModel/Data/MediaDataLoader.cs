using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Megazone.Core.Client.Extensions;
using Megazone.Core.Extension;
using Megazone.Core.Log;
using Megazone.Core.Log.Log4Net.Extension;
using Megazone.Core.Windows.Control.VideoPlayer;
using Megazone.Core.Windows.Extension;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Unity;
using ObjectExtension = Megazone.VideoStudio.Presentation.Common.Infrastructure.Extension.ObjectExtension;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{
    internal class MediaDataLoader
    {
        private const int MAX_THREAD_COUNT = 5;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly ILogger _logger;
        private readonly Queue<IMediaItem> _queue;

        private int _aliveCount;

        public MediaDataLoader(IEnumerable<IMediaItem> items,
            CancellationTokenSource cancellationTokenSource)
        {
            _queue = new Queue<IMediaItem>(items);
            _logger = Bootstrapper.Container.Resolve<ILogger>();
            _cancellationTokenSource = cancellationTokenSource;
        }

        public async void Run()
        {
            await this.CreateTask(() =>
            {
                while (_queue.Any())
                {
                    if (_aliveCount >= MAX_THREAD_COUNT)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    var item = _queue.Dequeue();
                    Load(item);
                }
            });
        }

        private async void Load(IMediaItem item)
        {
            _aliveCount++;
            await this.CreateTask(() =>
            {
                try
                {
                    var fullPath = item.Url;
                    var lastIndexOfSlash = fullPath.LastIndexOf("/", StringComparison.Ordinal);
                    if (lastIndexOfSlash == -1) return;
                    var fileName = fullPath.Substring(lastIndexOfSlash + 1);
                    var indexOfFirstDot = fileName.IndexOf(".", StringComparison.Ordinal);
                    if (indexOfFirstDot == -1) return;
                    fileName = fileName.Substring(0, indexOfFirstDot) + ".jpg";
                    var folderPath = ObjectExtension.TempFolder() + "\\" + Guid.NewGuid() +
                                     DateTime.UtcNow.DateTimeToEpoch();
                    if (!Directory.Exists(folderPath))
                        try
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error.Write(ex);
                            return;
                        }

                    var filePath = folderPath + "\\" + fileName;
                    fullPath = fullPath.EscapeDataStringSharpOnly();
                    var videoData = VideoHeaderHelper.GetVideoHeaderData(new GetVideoDataParameters
                    {
                        Url = fullPath,
                        IsRequestThumbnail = false
                    }, _cancellationTokenSource);
                    if (videoData == null) return;
                    this.InvokeOnUi(() => { item.SetMediaHeaderData(videoData); });
                    if (!videoData.HasVideo) return;
                    var thumbnail = new FFmpegLauncher().GetThumbnail(new FFmpegLauncher.FFmpegLauncherParameter
                    {
                        SaveThumbnailPath = filePath,
                        TimeoutMilliseconds = 7000,
                        LocalFilePath = new Uri(fullPath).AbsoluteUri,
                        ThumbnailAtSeconds = (double) videoData.StartTime
                    });
                    if (thumbnail != null)
                        this.InvokeOnUi(() => { item.SetThumbnail(thumbnail); });
                }
                catch (Exception ex)
                {
                    _logger.Error.Write(ex);
                    //Debug.WriteLine($"{ex.Message}\r\n url:'{item?.Url}'\r\n", "MediaDataLoader Error");
                }
                finally
                {
                    _aliveCount--;
                }
            });
        }
    }
}