using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface;
using Megazone.Core.Extension;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.Core.Log.Log4Net.Extension;
using Megazone.Core.VideoTrack;
using Megazone.Core.VideoTrack.Model;
using Megazone.Core.Windows.Control.VideoPlayer;
using Megazone.Core.Windows.Extension;
using Megazone.Core.Windows.Mvvm;
using Megazone.Core.Windows.Xaml.Behaviors;
using Megazone.HyperSubtitleEditor.Presentation.Excel;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Config;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Enum;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.Message;
using Megazone.HyperSubtitleEditor.Presentation.Message.Parameter;
using Megazone.HyperSubtitleEditor.Presentation.Message.View;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel
{
    [Inject(Scope = LifetimeScope.Singleton)]
    internal class SubtitleViewModel : ViewModelBase
    {
        // ReSharper disable once InconsistentNaming
        private const decimal MIN_INTERVAL = (decimal) 0.25;
        private readonly IBrowser _browser;
        private readonly ICloudMediaService _cloudMediaService;
        private readonly ExcelService _excelService;
        private readonly FileManager _fileManager;
        private readonly ILogger _logger;
        private readonly RecentlyLoader _recentlyLoader;
        private List<RecentlyItem> _recentlyItems;

        private readonly TimeSpan _minimumDuration = TimeSpan.FromMilliseconds(1000);
        private readonly IList<Caption> _removedCaptions = new List<Caption>();

        private readonly SubtitleListItemValidator _subtitleListItemValidator;

        private readonly SubtitleParserProxy _subtitleService;
        private ICommand _addItemCommand;
        private ICommand _closeTabCommand;
        private IList<SubtitleListItemViewModel> _copiedRows;
        private ICommand _deleteSelectedItemsCommand;

        private ICommand _dropToAddSubtitleCommand;
        private IList<EncodingInfo> _encodings;

        private ICommand _goToSelectedRowCommand;

        private bool _hasRegisteredMessageHandlers;
        private bool _isCutRequest;
        private ICommand _loadCommand;
        private decimal _previousPosition;
        private EncodingInfo _selectedEncoding;
        private ISubtitleListItemViewModel _selectedItem;
        private SubtitleTabItemViewModel _selectedTab;
        private ICommand _selectNextRowCommand;
        private ICommand _selectPreviousRowCommand;
        private ICommand _syncMediaPositionCommand;
        private IList<ISubtitleTabItemViewModel> _tabs;

        
        private ICommand _openRecentlyCommand;

        private VideoItemViewModel _videoItem;
        private WorkBarViewModel _workBarViewModel;

        public SubtitleViewModel(SubtitleParserProxy subtitleService,
            ILogger logger,
            FileManager fileManager,
            ExcelService excelService,
            SubtitleListItemValidator subtitleListItemValidator,
            IBrowser browser,
            ICloudMediaService cloudMediaService,
            RecentlyLoader recentlyLoader,
            WorkBarViewModel workBarViewModel)
        {
            _subtitleService = subtitleService;
            _logger = logger;
            _fileManager = fileManager;
            _excelService = excelService;
            _browser = browser;
            _cloudMediaService = cloudMediaService;
            _subtitleListItemValidator = subtitleListItemValidator;
            _recentlyLoader = recentlyLoader;
            _workBarViewModel = workBarViewModel;

            MediaPlayer = new MediaPlayerViewModel(OnMediaPositionChanged, OnMediaPlayStateChanged);
            WorkContext = new McmWorkContext();
        }

        public McmWorkContext WorkContext { get; private set; }
        public MediaPlayerViewModel MediaPlayer { get; }

        public ICommand DeleteSelectedItemsCommand
        {
            get
            {
                return
                    _deleteSelectedItemsCommand =
                        _deleteSelectedItemsCommand ?? new RelayCommand(DeleteSelectedItems, CanDeleteSelectedItems);
            }
        }

        public ICommand AddItemCommand
        {
            get { return _addItemCommand = _addItemCommand ?? new RelayCommand(AddItem, CanAddItem); }
        }

        public ICommand LoadCommand
        {
            get { return _loadCommand = _loadCommand ?? new RelayCommand(Load); }
        }

        public EncodingInfo SelectedEncoding
        {
            get => _selectedEncoding;
            set => Set(ref _selectedEncoding, value);
        }

        public IList<EncodingInfo> Encodings
        {
            get => _encodings;
            private set => Set(ref _encodings, value);
        }

        public ICommand SyncMediaPositionCommand
        {
            get { return _syncMediaPositionCommand = _syncMediaPositionCommand ?? new RelayCommand(SyncMediaPosition); }
        }

        public ISubtitleListItemViewModel SelectedItem
        {
            get => _selectedItem;
            private set => Set(ref _selectedItem, value);
        }

        public ICommand CloseTabCommand
        {
            get { return _closeTabCommand = _closeTabCommand ?? new RelayCommand<SubtitleTabItemViewModel>(CloseTab); }
        }

        public ICommand DropToAddSubtitleCommand
        {
            get
            {
                return
                    _dropToAddSubtitleCommand =
                        _dropToAddSubtitleCommand ?? new RelayCommand<object>(DropToAddSubtitle, CanDropToAddSubtitle);
            }
        }

        /// <summary>
        ///     Binding Mode: One way to source
        /// </summary>
        public IListBoxScroll ListBoxScroll { get; set; }

        public ICommand SelectPreviousRowCommand
        {
            get
            {
                return
                    _selectPreviousRowCommand =
                        _selectPreviousRowCommand ?? new RelayCommand(SelectPreviousRow, CanSelectPreviousRow);
            }
        }

        public ICommand SelectNextRowCommand
        {
            get
            {
                return
                    _selectNextRowCommand =
                        _selectNextRowCommand ?? new RelayCommand(SelectNextRow, CanSelectNextRow);
            }
        }

        public ICommand GoToSelectedRowCommand
        {
            get
            {
                return
                    _goToSelectedRowCommand =
                        _goToSelectedRowCommand ?? new RelayCommand(GoToSelectedRow, CanGoToSelectedRow);
            }
        }

        public ICommand OpenRecentlyCommand
        {
            get
            {
                return _openRecentlyCommand =
                    _openRecentlyCommand ?? new RelayCommand<RecentlyItem>(OpenRecently);
            }
        }
        
        public SubtitleTabItemViewModel SelectedTab
        {
            get => _selectedTab;
            private set => Set(ref _selectedTab, value);
        }

        public IList<ISubtitleTabItemViewModel> Tabs
        {
            get => _tabs;
            set => Set(ref _tabs, value);
        }

        public VideoItemViewModel VideoItem
        {
            get => _videoItem;
            set => Set(ref _videoItem, value);
        }

        public List<RecentlyItem> RecentlyItems
        {
            get => _recentlyItems; 
            set => Set(ref _recentlyItems, value);
        }

        public bool CheckWorkInProgress()
        {
            if (HasTab && Tabs.Any(tab => tab.CheckDirty()))
            {
                return true;
            }

            return false;
        }

        public bool HasTab => Tabs?.Any() ?? false;

        public int SelectedTabRowsCount => SelectedTab?.Rows?.Count ?? 0;
        public bool HasSelectedRows => (SelectedTab?.SelectedRows?.Count ?? 0) > 0;

        public bool IsMediaReady => MediaPlayer.NaturalDuration > 0;
        public bool HasCopiedRows => (_copiedRows?.Count ?? 0) > 0;

        private void GoToSelectedRow()
        {
            var lineNumber = SelectedTab?.SelectedRow?.Number;
            if (lineNumber != null)
                MessageCenter.Instance.Send(new SubtitleView.GoToLineNumberMessage(this, lineNumber.Value));
        }

        private bool CanGoToSelectedRow()
        {
            return SelectedTab?.SelectedRow != null;
        }

        private bool CanDropToAddSubtitle(object parameter)
        {
            if (!(parameter is IDataObject dataObject))
                return true;
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                var filePaths = (dataObject.GetData(DataFormats.FileDrop) as IEnumerable<string>)?.ToList();
                if (filePaths == null || !filePaths.Any())
                    return false;
                // 라이브러리 or 하드 드라이브는 드롭할 수 없다.
                var logicalDrives = Environment.GetLogicalDrives();
                var hasDrive = filePaths.Any(f => logicalDrives.Contains(f.ToUpper()));
                if (hasDrive)
                    return false;
                if (filePaths.Count > 1) return false;
                return !filePaths.Any(f =>
                {
                    if (Directory.Exists(f))
                        return true;
                    var fileExtension = Path.GetExtension(f);
                    return !(FileTypeDistributer.IsSupportedSubtitleFormat(fileExtension) || fileExtension == ".xlsx");
                });
            }

            return false;
        }

        private void DropToAddSubtitle(object parameter)
        {
            if (!(parameter is IDataObject dataObject)) return;
            if (dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                var filePaths = (dataObject.GetData(DataFormats.FileDrop) as IEnumerable<string>)?.ToList();
                if (filePaths == null || !filePaths.Any())
                    return;
                var firstFilePath = filePaths.First();
                if (File.Exists(firstFilePath))
                {
                    ImportSubtitleFile(firstFilePath);
                    //var fileExtension = Path.GetExtension(firstFilePath);
                    //if (fileExtension == ".xlsx")
                    //    _browser.Main.ShowImportExcelDialog(firstFilePath);
                    //else
                    //    _browser.Main.ShowOpenSubtitleDialog(firstFilePath);
                }
            }
        }

        private void OnMediaPlayStateChanged(MediaPlayStates obj)
        {
            if (obj == MediaPlayStates.Play)
                _previousPosition = 0;
        }

        private void OnMediaPositionChanged(decimal position)
        {
            if (SelectedTab == null) return;
            var iTextList = new List<IText>();
            try
            {
                if (Math.Abs(position - _previousPosition) < MIN_INTERVAL) return;
                _previousPosition = position;
                var totalSeconds = Convert.ToDouble(position);
                var timeSpan = TimeSpan.FromSeconds(totalSeconds);
                var rows = SelectedTab.Rows.ToList();
                foreach (var row in rows)
                {
                    row.CheckIsNowPlaying(timeSpan);
                    if (row.IsNowPlaying)
                    {
                        if (iTextList.Any())
                            iTextList.Add(new Normal
                            {
                                Text = "<br/>"
                            });
                        iTextList.AddRange(row.Texts.ToList());
                    }
                }

                MediaPlayer.CurrentPositionText = iTextList;
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex);
            }
        }

        private bool CanDeleteSelectedItems()
        {
            return SelectedTab?.SelectedRow != null;
        }

        private bool CanAddItem()
        {
            return SelectedTab != null;
        }

        private void AddItem()
        {
            var addedRow = SelectedTab?.AddNewRow(_minimumDuration);
            if (addedRow != null)
                MessageCenter.Instance.Send(new SubtitleView.ScrollIntoObjectMessage(this, addedRow));
        }

        private void ClearCurrentPositionText()
        {
            MediaPlayer.CurrentPositionText = new List<IText>();
        }

        private void OnValidateRequested(SubtitleTabItemViewModel tab)
        {
            _subtitleListItemValidator.Validate(SelectedTab.Rows);
        }

        private void DeleteSelectedItems()
        {
            SelectedTab?.DeleteSelectedItems();
        }

        private void InitializeTabs()
        {
            Tabs = new ObservableCollection<ISubtitleTabItemViewModel>();
        }

        private void OnTabSelected(SubtitleTabItemViewModel tab)
        {
            if (SelectedTab != null && !tab.Equals(SelectedTab))
                SelectedTab.IsSelected = false;
            SelectedTab = tab;
            SelectedItem = SelectedTab?.SelectedRow;
        }

        private void OnDoubleClickedItem(ISubtitleListItemViewModel syncItem)
        {
            Debug.WriteLine($"--- OnDoubleClickedItem Start time : {syncItem.StartTime.ToString()}");

            if (!string.IsNullOrEmpty(MediaPlayer?.MediaSource))
                MediaPlayer.SyncPosition(syncItem.StartTime);
            //SelectedItem = row;
        }

        private void OnItemSelected(ISubtitleListItemViewModel row)
        {
            SelectedItem = row;
        }

        private void CloseTab(SubtitleTabItemViewModel tab)
        {
            if (tab == null) return;

            Tabs.Remove(tab);
            if (tab.IsDeployedOnce || tab.Caption != null)
            {
                _removedCaptions.Add(tab.Caption);
                var elementItem = _workBarViewModel.CaptionAssetItem.Elements.FirstOrDefault(e => e.Id == tab.Caption.Id);
                elementItem.IsOpened = false;
            }

            MessageCenter.Instance.Send(new Message.View.CaptionElementsEditView.ChangedTabMessage(this));

            if (SelectedTab == null || !tab.Equals(SelectedTab)) return;
            var lastTab = Tabs.LastOrDefault();
            if (lastTab != null)
                lastTab.IsSelected = true;
            else
                SelectedTab = null;
        }

        private void SyncMediaPosition()
        {
            var selectedItem = SelectedTab?.SelectedRow;
            if (selectedItem != null && !string.IsNullOrEmpty(MediaPlayer?.MediaSource))
                MediaPlayer.SyncPosition(selectedItem.StartTime);
        }

        public void Unload()
        {
            ConfigHolder.Save(ConfigHolder.Current);

            UnregisterMessageHandlers();
        }

        private void RegisterMessageHandlers()
        {
            if (_hasRegisteredMessageHandlers) return;
            _hasRegisteredMessageHandlers = true;
            MessageCenter.Instance.Regist<Message.SubtitleEditor.AutoAdjustEndtimesMessage>(OnAutoAdjustEndtimesRequested);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.SettingsSavedMessage>(OnSettingsSaved);
            //MessageCenter.Instance.Regist<SubtitleEditor.SaveMessage>(OnSave);

            MessageCenter.Instance.Regist<Message.SubtitleEditor.SaveAllMessage>(OnSaveAll);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.FileOpenedMessage>(OnOpenFile);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.CaptionOpenRequestedMessage>(OnOpenCaptionRequest);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.CaptionElementCreateNewMessage>(OnCaptionElementCreateNew);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.CaptionElementUpdateMessage>(OnOpenCaptionElementUpdate);
            MessageCenter.Instance.Regist<CloudMedia.CaptionResetMessage>(OnCaptionReset);
            MessageCenter.Instance.Regist<CloudMedia.VideoOpenRequestedMessage>(OnVideoOpenRequest);
            MessageCenter.Instance.Regist<Message.Excel.FileImportMessage>(OnImportExcelFile);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.ExportSubtitleMessage>(OnExportSubtitleFile);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.CloseTabMessage>(OnCloseTabRequested);
            MessageCenter.Instance.Regist<MediaPlayer.OpenLocalMediaMessage>(OnOpenLocalMediaRequested);
            MessageCenter.Instance.Regist<MediaPlayer.OpenMediaFromUrlMessage>(OnOpenMediaFromFilePathRequested);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.CopyTabMessage>(OnCopySubtitle);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.EditTabMessage>(OnEditSubtitle);

            MessageCenter.Instance.Regist<Message.SubtitleEditor.SyncStartTimeToCurrentMediaPositionMessage>(
                OnSyncStartTimeToCurrentMediaPositionRequested);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.SyncEndTimeToCurrentMediaPositionMessage>(
                OnSyncEndTimeToCurrentMediaPositionRequested);

            MessageCenter.Instance.Regist<SubtitleView.RequestFindCountMessage>(OnFindCountRequested);
            MessageCenter.Instance.Regist<SubtitleView.FindTextMessage>(OnFindTextRequested);
            MessageCenter.Instance.Regist<SubtitleView.AllFindTextMessage>(OnAllFindTextRequested);
            MessageCenter.Instance.Regist<SubtitleView.ReplaceTextMessage>(OnReplaceTextRequested);
            MessageCenter.Instance.Regist<SubtitleView.AllReplaceTextMessage>(OnAllReplaceTextRequested);
            MessageCenter.Instance.Regist<SubtitleView.GoToLineNumberMessage>(OnGoToLineNumberRequested);
            MessageCenter.Instance.Regist<SubtitleView.SelectAllRowsMessage>(OnSelectAllRowsRequested);
            MessageCenter.Instance.Regist<SubtitleView.DeleteSelectRowsMessage>(OnDeleteSelectedRowsRequested);

            MessageCenter.Instance.Regist<Message.SubtitleEditor.AddNewRowMessage>(OnAddNewRowRequested);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.InsertNewRowMessage>(OnInsertNewRowRequested);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.InsertNewRowAfterSelectedRowMessage>(
                OnInsertNewRowAfterSelectedRowRequested);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.InsertNewRowBeforeSelectedRowMessage>(
                OnInsertNewRowBeforeSelectedRowRequested);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.CutSelectedRowsMessage>(OnCutSelectedRowRequested);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.PasteRowsMessage>(OnPasteRowsRequested);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.CopySelectedRowsMessage>(OnCopySelectedRowsRequested);
            //MessageCenter.Instance.Regist<Message.SubtitleEditor.LoadTabsMessage>(OnGroupFileLoaded);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.CopyContentsToClipboardMessage>(OnCopyContentsToClipboardRequested);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.InsertRowAtCurrentMediaPositionMessage>(
                OnInsertRowAtCurrentMediaPositionRequested);
            MessageCenter.Instance.Regist<MediaPlayer.PlayBackByHalfSecondMessage>(OnPlayBackByHalfSecondRequested);
            MessageCenter.Instance.Regist<MediaPlayer.PlayForwardByHalfSecondMessage>(
                OnPlayForwardByHalfSecondRequested);

            MessageCenter.Instance.Regist<Message.SubtitleEditor.AdjustTimeMessage>(AdjustTime);
            MessageCenter.Instance.Regist<ProjectSelect.ProjectChangeMessage>(OnProjectChanged);
            MessageCenter.Instance.Regist<Message.SubtitleEditor.CleanUpSubtitleMessage>(OnCleanUpSubtitle);

            MessageCenter.Instance.Regist<Message.RecentlyLoader.ChangeItemMessage>(OnChangeRecentlyItem);
        }

        

        private void OnProjectChanged(ProjectSelect.ProjectChangeMessage message)
        {
            CleanUpSubtitle();

            RecentlyItems = _recentlyLoader.GetRecentlyItems(false).ToList();
            //var removeTabs = Tabs.ToList();

            //if (removeTabs != null)
            //    foreach (var tab in removeTabs)
            //        CloseTab(tab as SubtitleTabItemViewModel);

            //if (MediaPlayer.MediaSource != null)
            //    MediaPlayer.RemoveMediaItem();


            //var workBarViewModel = Bootstrapper.Container.Resolve<WorkBarViewModel>();
            //workBarViewModel.Initialize();

            //ClearCurrentPositionText();
        }

        private void UnregisterMessageHandlers()
        {
            if (!_hasRegisteredMessageHandlers) return;
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.AutoAdjustEndtimesMessage>(OnAutoAdjustEndtimesRequested);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.SettingsSavedMessage>(OnSettingsSaved);
            //MessageCenter.Instance.Unregist<SubtitleEditor.SaveMessage>(OnSave);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.CaptionOpenRequestedMessage>(OnOpenCaptionRequest);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.CaptionElementCreateNewMessage>(OnCaptionElementCreateNew);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.CaptionElementUpdateMessage>(OnOpenCaptionElementUpdate);
            MessageCenter.Instance.Unregist<CloudMedia.CaptionResetMessage>(OnCaptionReset);
            MessageCenter.Instance.Unregist<CloudMedia.VideoOpenRequestedMessage>(OnVideoOpenRequest);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.SaveAllMessage>(OnSaveAll);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.FileOpenedMessage>(OnOpenFile);
            MessageCenter.Instance.Unregist<Message.Excel.FileImportMessage>(OnImportExcelFile);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.ExportSubtitleMessage>(OnExportSubtitleFile);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.CloseTabMessage>(OnCloseTabRequested);
            MessageCenter.Instance.Unregist<MediaPlayer.OpenLocalMediaMessage>(OnOpenLocalMediaRequested);
            MessageCenter.Instance.Unregist<MediaPlayer.OpenMediaFromUrlMessage>(OnOpenMediaFromFilePathRequested);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.CopyTabMessage>(OnCopySubtitle);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.SyncStartTimeToCurrentMediaPositionMessage>(
                OnSyncStartTimeToCurrentMediaPositionRequested);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.SyncEndTimeToCurrentMediaPositionMessage>(
                OnSyncEndTimeToCurrentMediaPositionRequested);

            MessageCenter.Instance.Unregist<SubtitleView.SelectAllRowsMessage>(OnSelectAllRowsRequested);
            MessageCenter.Instance.Unregist<SubtitleView.DeleteSelectRowsMessage>(OnDeleteSelectedRowsRequested);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.AddNewRowMessage>(OnAddNewRowRequested);
            MessageCenter.Instance
                .Unregist<Message.SubtitleEditor.CopyContentsToClipboardMessage>(OnCopyContentsToClipboardRequested);

            MessageCenter.Instance.Unregist<Message.SubtitleEditor.EditTabMessage>(OnEditSubtitle);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.InsertNewRowMessage>(OnInsertNewRowRequested);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.InsertNewRowAfterSelectedRowMessage>(
                OnInsertNewRowAfterSelectedRowRequested);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.InsertNewRowBeforeSelectedRowMessage>(
                OnInsertNewRowBeforeSelectedRowRequested);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.CutSelectedRowsMessage>(OnCutSelectedRowRequested);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.PasteRowsMessage>(OnPasteRowsRequested);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.CopySelectedRowsMessage>(OnCopySelectedRowsRequested);
            //MessageCenter.Instance.Unregist<Message.SubtitleEditor.LoadTabsMessage>(OnGroupFileLoaded);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.InsertRowAtCurrentMediaPositionMessage>(
                OnInsertRowAtCurrentMediaPositionRequested);
            MessageCenter.Instance.Unregist<MediaPlayer.PlayBackByHalfSecondMessage>(OnPlayBackByHalfSecondRequested);
            MessageCenter.Instance.Unregist<MediaPlayer.PlayForwardByHalfSecondMessage>(
                OnPlayForwardByHalfSecondRequested);

            MessageCenter.Instance.Unregist<Message.SubtitleEditor.AdjustTimeMessage>(AdjustTime);
            MessageCenter.Instance.Unregist<ProjectSelect.ProjectChangeMessage>(OnProjectChanged);
            MessageCenter.Instance.Unregist<Message.SubtitleEditor.CleanUpSubtitleMessage>(OnCleanUpSubtitle);

            MessageCenter.Instance.Unregist<Message.RecentlyLoader.ChangeItemMessage>(OnChangeRecentlyItem);
        }

        public void OnAutoAdjustEndtimesRequested(Message.SubtitleEditor.AutoAdjustEndtimesMessage message)
        {
            var rows = SelectedTab?.Rows;
            var rowsCount = rows?.Count ?? 0;
            if (rowsCount < 1) return;
            for (var i = 0; i < rowsCount; i++)
            {
                if (rows == null) continue;
                var currentRow = rows[i];
                if (i + 1 < rowsCount)
                {
                    var nextRow = rows[i + 1];
                    currentRow.EndTime = nextRow.StartTime.Add(-TimeSpan.FromMilliseconds(100));
                }
            }
        }

        private void OnCopyContentsToClipboardRequested(Message.SubtitleEditor.CopyContentsToClipboardMessage message)
        {
            var contents = SelectedTab?.SelectedRow?.DisplayText;
            if (string.IsNullOrEmpty(contents)) return;
            Clipboard.SetText(contents);
        }

        private void AdjustTime(Message.SubtitleEditor.AdjustTimeMessage message)
        {
            if (SelectedTab == null)
                return;
            IEnumerable<ISubtitleListItemViewModel> rows;
            if (message.Way.Range == AdjustTimeRange.All)
            {
                rows = SelectedTab.Rows;
            }
            else if (message.Way.Range == AdjustTimeRange.SelectedItems)
            {
                rows = SelectedTab.SelectedRows?.Cast<ISubtitleListItemViewModel>()
                    .ToList();
            }
            else
            {
                var selectedRows = SelectedTab.SelectedRows?.Cast<ISubtitleListItemViewModel>()
                    .ToList();
                var lastSelectedItem = selectedRows?.OrderBy(i => i.Number)
                    .LastOrDefault();
                if (lastSelectedItem == null)
                    return;
                rows = selectedRows.Union(SelectedTab.Rows.Where(i => i.Number > lastSelectedItem.Number));
            }

            rows?.ForEach(i =>
            {
                var duration = i.Duration;
                if (message.Way.Behavior == AdjustTimeBehavior.Foward)
                    try
                    {
                        i.StartTime = i.StartTime.Subtract(message.Way.Time);
                    }
                    catch
                    {
                        i.StartTime = TimeSpan.Zero;
                    }
                else
                    try
                    {
                        i.StartTime = i.StartTime.Add(message.Way.Time);
                    }
                    catch
                    {
                        i.StartTime = TimeSpan.Zero;
                    }

                i.EndTime = i.StartTime.Add(duration);
            });
        }

        //private async void OnGroupFileLoaded(Message.SubtitleEditor.LoadTabsMessage message)
        //{
        //    var tabs = message.Tabs?.ToList();
        //    if (tabs == null)
        //        return;

        //    _browser.Main.LoadingManager.Show();

        //    await Task.Factory.StartNew(async () =>
        //    {
        //        var workBar = Bootstrapper.Container.Resolve<WorkBarViewModel>();
        //        var caption = await workBar.GetCaptionAssetAsync(tabs.FirstOrDefault()?.CaptionAssetId);
        //        var video = await workBar.GetVideoAsync(tabs.FirstOrDefault()?.VideoId);

        //        this.InvokeOnUi(() =>
        //        {
        //            WorkContext.Initialize(video, caption);

        //            _subtitleListItemValidator.IsEnabled = false;
        //            Tabs.Clear();

        //            foreach (var tab in tabs)
        //            {
        //                var newTab = new SubtitleTabItemViewModel(tab.Name,
        //                    OnRowCollectionChanged,
        //                    OnValidateRequested,
        //                    OnTabSelected,
        //                    OnItemSelected,
        //                    tab.Kind,
        //                    OnDisplayTextChanged,
        //                    tab.LanguageCode,
        //                    tab.CountryCode,
        //                    tab.Caption)
        //                {
        //                    IsSelected = tab.IsSelected,
        //                    FilePath = tab.FilePath,
        //                    VideoId = tab.VideoId,
        //                    CaptionAssetId = tab.CaptionAssetId
        //                };

        //                if (tab.Rows != null)
        //                    newTab.AddRows(tab.Rows.ToList());
        //                Tabs.Add(newTab);
        //            }

        //            _browser.Main.LoadingManager.Hide();

        //            _subtitleListItemValidator.IsEnabled = true;
        //            _subtitleListItemValidator.Validate(SelectedTab.Rows);
        //            CommandManager.InvalidateRequerySuggested();
        //        });
        //    });
        //}

        private void Load()
        {
            RegisterMessageHandlers();

            InitializeTabs();
            InitializeEncodingItems();

            _recentlyLoader.Load();
        }

        private void OnSettingsSaved(Message.SubtitleEditor.SettingsSavedMessage message)
        {
            if (SelectedTab != null && (SelectedTab.Rows?.Any() ?? false))
                _subtitleListItemValidator.Validate(SelectedTab.Rows);
        }

        private void OnPlayBackByHalfSecondRequested(MediaPlayer.PlayBackByHalfSecondMessage message)
        {
            MediaPlayer.PlayBackBy(TimeSpan.FromSeconds(0.5));
        }

        private void OnPlayForwardByHalfSecondRequested(MediaPlayer.PlayForwardByHalfSecondMessage message)
        {
            MediaPlayer.PlayForwardBy(TimeSpan.FromSeconds(0.5));
        }

        private void OnInsertRowAtCurrentMediaPositionRequested(Message.SubtitleEditor.InsertRowAtCurrentMediaPositionMessage message)
        {
            if (SelectedTab == null) return;
            var position = TimeSpan.FromSeconds(Convert.ToDouble(MediaPlayer.CurrentMediaPosition));
            var subtitleItem = new SubtitleItem(position,
                position + ConfigHolder.Current.Subtitle.MinDuration,
                null,
                null);
            var targetRow = SelectedTab.Rows.FirstOrDefault(row => row.StartTime > position);
            if (targetRow != null)
            {
                var index = SelectedTab.Rows.IndexOf(targetRow);
                SelectedTab.AddRow(subtitleItem, index);
            }
            else
            {
                SelectedTab.AddRow(subtitleItem);
            }

            MessageCenter.Instance.Send(new SubtitleView.ScrollIntoObjectMessage(this, subtitleItem));
        }

        private void OnSyncEndTimeToCurrentMediaPositionRequested(
            Message.SubtitleEditor.SyncEndTimeToCurrentMediaPositionMessage message)
        {
            if (SelectedTab?.SelectedRow == null) return;
            var currentMediaPosition = MediaPlayer.CurrentMediaPosition;
            SelectedTab.SelectedRow.EndTime = TimeSpan.FromSeconds(Convert.ToDouble(currentMediaPosition));
        }

        private void OnSyncStartTimeToCurrentMediaPositionRequested(
            Message.SubtitleEditor.SyncStartTimeToCurrentMediaPositionMessage message)
        {
            if (SelectedTab?.SelectedRow == null) return;
            var currentMediaPosition = MediaPlayer.CurrentMediaPosition;
            SelectedTab.SelectedRow.StartTime = TimeSpan.FromSeconds(Convert.ToDouble(currentMediaPosition));
        }

        private void OnFindCountRequested(SubtitleView.RequestFindCountMessage message)
        {
            var findRows = SelectedTab.Rows.Where(item => item.Texts.HasFindText(message.FindText));
            message.Response?.Invoke(findRows.Count());
        }

        private void OnAddNewRowRequested(Message.SubtitleEditor.AddNewRowMessage message)
        {
            AddItem();
        }

        private void OnCopySelectedRowsRequested(Message.SubtitleEditor.CopySelectedRowsMessage message)
        {
            if (SelectedTab == null) return;
            _copiedRows = SelectedTab.SelectedRows?.Cast<SubtitleListItemViewModel>()
                .Select(r => r.Copy())
                .ToList();
            _isCutRequest = false;
        }

        private void OnCutSelectedRowRequested(Message.SubtitleEditor.CutSelectedRowsMessage message)
        {
            if (SelectedTab == null) return;
            _copiedRows = SelectedTab.SelectedRows?.Cast<SubtitleListItemViewModel>()
                .ToList();
            _isCutRequest = true;
            SelectedTab.DeleteSelectedItems();
            CommandManager.InvalidateRequerySuggested();
        }

        private void OnPasteRowsRequested(Message.SubtitleEditor.PasteRowsMessage message)
        {
            if (_copiedRows == null || SelectedTab == null) return;
            if (_isCutRequest)
            {
                var copiedRows = _copiedRows.Select(r => r.Copy())
                    .ToList();
                if (SelectedTab.SelectedRow == null)
                    SelectedTab.AddRows(copiedRows);
                else
                    SelectedTab.AddRows(copiedRows, SelectedTab.SelectedRow.Number - 1);
                _copiedRows = null;
            }
            else
            {
                SelectedTab.AddRows(_copiedRows.Select(r => r.Copy())
                        .ToList(), SelectedTab.SelectedRow.Number - 1);
            }

            _subtitleListItemValidator.Validate(SelectedTab.Rows);
        }

        private void OnInsertNewRowBeforeSelectedRowRequested(Message.SubtitleEditor.InsertNewRowBeforeSelectedRowMessage message)
        {
            var addedRow = SelectedTab?.AddNewRow(_minimumDuration,
                SubtitleTabItemViewModel.InsertRowDirection.BeforeSelectedItem,
                SelectedTab.SelectedRow.Number - 1);
            if (addedRow != null)
                MessageCenter.Instance.Send(new SubtitleView.ScrollIntoObjectMessage(this, addedRow));
        }

        private void OnInsertNewRowAfterSelectedRowRequested(Message.SubtitleEditor.InsertNewRowAfterSelectedRowMessage message)
        {
            var addedRow = SelectedTab?.AddNewRow(_minimumDuration,
                SubtitleTabItemViewModel.InsertRowDirection.AfterSelectedItem,
                SelectedTab.SelectedRow.Number);
            if (addedRow != null)
                MessageCenter.Instance.Send(new SubtitleView.ScrollIntoObjectMessage(this, addedRow));
        }

        private void OnInsertNewRowRequested(Message.SubtitleEditor.InsertNewRowMessage message)
        {
            AddItem();
        }

        private void OnDeleteSelectedRowsRequested(SubtitleView.DeleteSelectRowsMessage message)
        {
            if (!HasSelectedRows)
                return;

            SelectedTab.DeleteSelectedItems();
        }

        private void OnSelectAllRowsRequested(SubtitleView.SelectAllRowsMessage message)
        {
            if (SelectedTab == null) return;
            SelectedTab.Rows?.ToList()
                .ForEach(row => row.IsSelected = true);
            ListBoxScroll?.Focus();
        }

        private void OnReplaceTextRequested(SubtitleView.ReplaceTextMessage message)
        {
            Replace(message.FindText, message.ReplaceText);
        }

        private void OnAllReplaceTextRequested(SubtitleView.ReplaceTextMessage message)
        {
            ReplaceAll(message.FindText, message.ReplaceText);
        }

        private void OnFindTextRequested(SubtitleView.FindTextMessage message)
        {
            FindText(message.FindText);
        }

        private void OnAllFindTextRequested(SubtitleView.FindTextMessage message)
        {
            FindText(message.FindText, true);
        }

        private void OnGoToLineNumberRequested(SubtitleView.GoToLineNumberMessage message)
        {
            if (SelectedTab == null) return;
            var number = message.LineNumber;
            if (number < 0 || SelectedTab.Rows == null || number > SelectedTab.Rows.Count) return;
            var item = SelectedTab.Rows.FirstOrDefault(row => row.Number == number);
            ListBoxScroll?.ScrollIntoView(item, true);
        }

        private void OnOpenMediaFromFilePathRequested(MediaPlayer.OpenMediaFromUrlMessage message)
        {
            MediaPlayer.InitMedia(message.Url, message.IsLocal);

            //MediaPlayer.OpenMedia(message.Url, false);
        }

        private void OnOpenLocalMediaRequested(MediaPlayer.OpenLocalMediaMessage message)
        {
            var supportedMediaExtensionFilters = "";
            foreach (var extension in FileTypeDistributer.SupportedVideoExtensions)
                supportedMediaExtensionFilters += $"*{extension};";
            foreach (var extension in FileTypeDistributer.SupportedAudioExtensions)
                supportedMediaExtensionFilters += $"*{extension};";
            var mediaFilePath = _fileManager.OpenFile($"Media files |{supportedMediaExtensionFilters}");
            if (string.IsNullOrEmpty(mediaFilePath)) return;
            MediaPlayer.OpenMedia(mediaFilePath, true);
        }

        private void OnCloseTabRequested(Message.SubtitleEditor.CloseTabMessage message)
        {
            CloseTab(message.Tab);
        }

        private void OnDisplayTextChanged(SubtitleTabItemViewModel tab)
        {
            if (MediaPlayer.IsMediaPlaying) return;
            var nowPlayingRows = SelectedTab.Rows.Where(r => r.IsNowPlaying)
                .ToList();
            var iTextList = new List<IText>();
            foreach (var row in nowPlayingRows)
            {
                if (iTextList.Any())
                    iTextList.Add(new Normal
                    {
                        Text = "<br/>"
                    });
                var tempITextList = row.Texts.ToList();
                iTextList.AddRange(tempITextList);
            }

            MediaPlayer.CurrentPositionText = iTextList;
        }

        private void InitializeEncodingItems()
        {
            var encodings = Encoding.GetEncodings();
            Encodings = new List<EncodingInfo>(encodings);
        }

        private async void OnSaveAll(Message.SubtitleEditor.SaveAllMessage message)
        {
            _browser.Main.LoadingManager.Show();
            try
            {
                foreach (var tab in Tabs)
                {
                    tab.IsSelected = true;
                    var saveFilePath = tab.FilePath;
                    if (string.IsNullOrEmpty(saveFilePath))
                    {
                        saveFilePath = _fileManager.OpenSaveFileDialog(null, "WebVtt files (.vtt)|*.vtt", tab.Name);
                        if (string.IsNullOrEmpty(saveFilePath)) return;
                    }

                    await SaveTabAsFileAsync(tab, saveFilePath);
                }

                this.InvokeOnUi(
                    () =>
                    {
                        _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                            Resource.MSG_SAVE_SUCCESS,
                            MessageBoxButton.OK,
                            Application.Current.MainWindow));
                    });
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex);
                this.InvokeOnUi(
                    () =>
                    {
                        _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                            Resource.MSG_SAVE_FAIL,
                            MessageBoxButton.OK,
                            Application.Current.MainWindow));
                    });
            }
            finally
            {
                this.InvokeOnUi(() => { _browser.Main.LoadingManager.Hide(); });
            }
        }

        private async Task SaveTabAsFileAsync(ISubtitleTabItemViewModel tab, string saveFilePath, SubtitleFormatKind subtitleFormat=SubtitleFormatKind.WebVtt)
        {
            var rows = tab.Rows.ToList();
            await this.CreateTask(() => { Save(saveFilePath, rows, subtitleFormat); });
            tab.FilePath = saveFilePath;
        }
       
        private void OnExportSubtitleFile(Message.SubtitleEditor.ExportSubtitleMessage message)
        {
            var saveFilePath = _fileManager.OpenSaveFileDialog(null, "vtt (*.vtt)|*.vtt|srt (*.srt)|*.srt|smi (*.smi)|.smi", SelectedTab.Name);

            if (string.IsNullOrEmpty(saveFilePath))
                return;

            var subtitleFormat = GetSubTitleFormatKindByFileName(saveFilePath);

            switch (subtitleFormat)
            {
                case SubtitleFormatKind.WebVtt:
                case SubtitleFormatKind.Sami:
                case SubtitleFormatKind.Srt:
                    ExportSubtitleAsync(saveFilePath, subtitleFormat);
                    break;
                //case SubtitleFormatKind.Excel:
                //    ExportExcelAsync(saveFilePath);
                    //_browser.Main.ShowImportExcelDialog(saveFilePath);
                    //break;
                default:
                    break;
            }
        }

        private async Task ExportExcelAsync(string savePath)
        {
            try
            {
                _browser.Main.LoadingManager.Show();
                
                var now = DateTime.Now.ToString("yyyyMMddHHmmss");

                if (string.IsNullOrEmpty(savePath))
                    return;

                await this.CreateTask(() => {
                    IList<Subtitle> subtitles = new List<Subtitle>();

                    foreach (var tab in Tabs)
                    {
                        var subtitle = new Subtitle
                        {
                            Label = tab.Name,
                            LanguageCode = tab.LanguageCode,
                            CountryCode = tab.CountryCode,
                            Format = TrackFormat.WebVtt,
                            Kind = tab.Kind
                        };

                        foreach (var item in tab.Rows)
                            subtitle.Datasets.Add(new SubtitleItem
                            {
                                Number = item.Number,
                                StartTime = item.StartTime,
                                EndTime = item.EndTime,
                                Texts = item.Texts
                            });

                        subtitles.Add(subtitle);
                    }

                    var isSuccess = _fileManager.ExportExcel(subtitles, savePath);

                    this.InvokeOnUi(()=>{
                        _browser.Main.LoadingManager.Hide();
                    });
                });

                //await this.CreateTask(() =>
                //{
                //    var isSuccess = _fileManager.ExportExcel(subtitles, savePath);

                //    this.InvokeOnUi(() =>
                //    {
                //        if (isSuccess)
                //            Process.Start("explorer.exe", Path.GetDirectoryName(savePath));
                //        else
                //            _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                //                Resource.MSG_EXPORT_EXCEL_FILE_FAIL,
                //                MessageBoxButton.OK,
                //                Application.Current.MainWindow));

                //        _browser.Main.LoadingManager.Hide();
                //    });
                //});
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex.Message);
                _browser.Main.LoadingManager.Hide();
            }
        }

        private async Task ExportSubtitleAsync(string savePath, SubtitleFormatKind subtitleFormat)
        {
            Debug.WriteLine("-- start ExportSubtitleAsync");
            if (string.IsNullOrEmpty(savePath))
                return;

            _browser.Main.LoadingManager.Show();

            try
            {
                await SaveTabAsFileAsync(SelectedTab, savePath, subtitleFormat);
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex);
                this.InvokeOnUi(
                    () =>
                    {
                        _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                            Resource.MSG_SAVE_FAIL,
                            MessageBoxButton.OK,
                            Application.Current.MainWindow));
                    });
            }
            finally
            {
                _browser.Main.LoadingManager.Hide();
            }

            Debug.WriteLine("-- end ExportSubtitleAsync");

            _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO, Resource.MSG_SAVE_SUCCESS,
                MessageBoxButton.OK,
                Application.Current.MainWindow));
        }

        public void OnImportSubtitleFile()
        {
            string initialPath = ConfigHolder.Current.General.RecentlySubtitleOpenPath;

            var filePath = _fileManager.OpenFile(
                "subtitle files (*.vtt;*.srt;*.smi;*.xlsx)|*.vtt;*.srt;*.smi;*.xlsx",
                initialPath);

            if (!string.IsNullOrEmpty(filePath))
                ConfigHolder.Current.General.RecentlySubtitleOpenPath = Path.GetDirectoryName(filePath);

            ImportSubtitleFile(filePath);
        }

        public void ImportSubtitleFile(string filePath)
        {
            var subtitleFormat = GetSubTitleFormatKindByFileName(filePath);

            switch (subtitleFormat)
            {
                case SubtitleFormatKind.WebVtt:
                case SubtitleFormatKind.Sami:
                case SubtitleFormatKind.Srt:
                    _browser.Main.ShowOpenSubtitleDialog(filePath, subtitleFormat);
                    break;
                case SubtitleFormatKind.Excel:
                    _browser.Main.ShowImportExcelDialog(filePath);
                    break;
                default:
                    break;
            }
        }

        public static SubtitleFormatKind GetSubTitleFormatKindByFileName(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            SubtitleFormatKind subtitleFormat;

            switch (extension)
            {
                case ".vtt":
                    subtitleFormat =  SubtitleFormatKind.WebVtt;
                    break;
                case ".srt":
                    subtitleFormat = SubtitleFormatKind.Srt;
                    break;
                case ".smi":
                    subtitleFormat = SubtitleFormatKind.Sami;
                    break;
                case ".xlsx":
                    subtitleFormat = SubtitleFormatKind.Excel;
                    break;
                default:
                    subtitleFormat = SubtitleFormatKind.Unknown;
                    break;
            }

            return subtitleFormat;
        }

        internal bool Save(string filePath, IList<ISubtitleListItemViewModel> rows, SubtitleFormatKind subtitleFormat)
        {
            //var encoding = SelectedEncoding?.GetEncoding() ?? Encoding.UTF8;
            switch (subtitleFormat)
            {
                case SubtitleFormatKind.WebVtt:
                    var parser = SubtitleListItemParserProvider.Get(subtitleFormat);
                    var subtitles = rows.Select(s => s.ConvertToString(parser))
                        .ToList();
                    return _fileManager.Save(filePath,
                        _subtitleService.ConvertToText(subtitles, SubtitleFormatKind.WebVtt),
                        Encoding.UTF8);

                case SubtitleFormatKind.Sami:
                case SubtitleFormatKind.Srt:
                    var subtitleItems = rows.Select(s => s.ConvertToSubtitleItem())
                        .ToList();

                    var result = _subtitleService.ConvertToText(subtitleItems, subtitleFormat);

                    return _fileManager.Save(filePath, result, Encoding.UTF8);

                default:
                    return false;
            }
        }

        public async void OnOpenCaptionRequest(Message.SubtitleEditor.CaptionOpenRequestedMessage requestedMessage)
        {
            if (requestedMessage.Param == null)
                return;

            // 게시에 필요한 정보.
            var video = requestedMessage.Param.Video;
            var captionAsset = requestedMessage.Param.Asset;
            var captionList = requestedMessage.Param.Captions?.ToList() ?? new List<Caption>();

            ClearCurrentPositionText();

            var metaDataViewmodel = Bootstrapper.Container.Resolve<MetadataViewModel>();
            metaDataViewmodel.Close();

            WorkContext = new McmWorkContext(video, captionAsset);

            if (!string.IsNullOrEmpty(video?.Name))
                _browser.Main.SetWindowTitle($"{Resource.CNT_APPLICATION_NAME} - {video.Name}");

            _browser.Main.LoadingManager.Show();

            if (MediaPlayer.MediaSource != null)
                MediaPlayer.RemoveMediaItem();

            // video영상을 가져온다.
            if (!string.IsNullOrEmpty(WorkContext.VideoMediaUrl))
                MediaPlayer.InitMedia(WorkContext, true);
            //MediaPlayer.OpenMedia(WorkContext.VideoMediaUrl, false);

            var texts = await LoadCaptionTextListAsync(captionList);

            _subtitleListItemValidator.IsEnabled = false;

            Debug.WriteLine("++OnOpenCaptionRequest");
            foreach (var caption in captionList)
            {
                var text = texts.ContainsKey(caption.Id) ? texts[caption.Id] : null;
                Enum.TryParse(caption.Kind, true, out CaptionKind kind);

                var newTab = new SubtitleTabItemViewModel(caption.Label,
                    OnRowCollectionChanged,
                    OnValidateRequested,
                    OnTabSelected,
                    OnItemSelected,
                    OnDoubleClickedItem,
                    kind,
                    OnDisplayTextChanged,
                    SourceLocationKind.Cloud,
                    caption.Language,
                    caption.Country,
                    caption)
                {
                    IsSelected = false,
                    VideoId = video?.Id,
                    CaptionAssetId = captionAsset.Id
                };

                if (!string.IsNullOrEmpty(text))
                {
                    var rows = _subtitleService.Load(text, SubtitleFormatKind.WebVtt)?.ToList();
                    if (rows != null)
                       newTab.AddRowsAsync(rows);
                }

                Tabs.Add(newTab);
            }
            Debug.WriteLine("--OnOpenCaptionRequest");

            if (captionList.Any())
                Tabs.First().IsSelected = true;

            _browser.Main.LoadingManager.Hide();
            _subtitleListItemValidator.IsEnabled = true;
            _subtitleListItemValidator.Validate(SelectedTab?.Rows);

            CommandManager.InvalidateRequerySuggested();

            
        }



        private async void OnOpenCaptionElementUpdate(Message.SubtitleEditor.CaptionElementUpdateMessage message)
        {
            var param = message.Param;
            // 닫힐 탭
            var closeItems = param.CloseCaptionElements;

            foreach (var closeItem in closeItems)
            {
                CloseTab(closeItem);
            }

            // 열릴 탭
            var openItems = param.OpenCaptionElements;

            var captionList = openItems.Select(c => c.Key).ToList();

            var texts = await LoadCaptionTextListAsync(captionList);

            foreach (var pairItem in openItems)
            {
                var caption = pairItem.Key;
                var sourceLocation = pairItem.Value;

                var text = texts.ContainsKey(caption.Id) ? texts[caption.Id] : null;
                var rows = _subtitleService.Load(text, SubtitleFormatKind.WebVtt)?.ToList();
                Enum.TryParse(caption.Kind, true, out CaptionKind kind);
                
                AddTab(caption.Label,
                    kind,
                    caption.Language,
                    caption.Country,
                    caption,
                    rows,
                    _workBarViewModel.VideoItem?.Id,
                    "",
                    caption.Id,
                    sourceLocation
                );
            }
        }

        async Task<Dictionary<string, string>> LoadCaptionTextListAsync(List<Caption> captionList)
        {
            var dic = new Dictionary<string, string>();
            foreach (var caption in captionList)
                try
                {
                    if (!string.IsNullOrEmpty(caption.AccessUrl)) // && Uri.IsWellFormedUriString(caption.Url, UriKind.Absolute)
                    {
                        var text = await _cloudMediaService.ReadAsync(new Uri(caption.AccessUrl), CancellationToken.None);
                        dic.Add(caption.Id, text);
                    }
                }
                catch (WebException e)
                {
                    Console.WriteLine(e);
                }

            return dic;
        }

        private void OnCaptionReset(CloudMedia.CaptionResetMessage message)
        {
            var closeTabList = new List<SubtitleTabItemViewModel>();

            foreach (var tab in Tabs)
            {
                switch (tab.SourceLocation)
                {

                    case SourceLocationKind.Cloud:
                    case SourceLocationKind.LocalFile:
                        tab.Reset();
                        break;
                    case SourceLocationKind.None:
                    case SourceLocationKind.CreatedByEditor:
                        closeTabList.Add(tab as SubtitleTabItemViewModel);
                        //CloseTab(tab as SubtitleTabItemViewModel);
                        break;
                }
            }

            foreach (var tab in closeTabList)
            {
                CloseTab(tab);
            }

            //if (_workBarViewModel.IsOnlineData)
            //{
            //    foreach (var tab in Tabs)
            //    {
            //        tab.Reset();
            //    }
            //}
            //else
            //{
            //    foreach (var tab in Tabs)
            //    {
            //        tab.Dispose();
            //    }

            //    Tabs.Clear();

            //    SelectedTab = null;
            //}
        }


        private void OnVideoOpenRequest(CloudMedia.VideoOpenRequestedMessage requestedMessage)
        {
            var video = requestedMessage.VideoParam;

            WorkContext = new McmWorkContext(video);

            //if (!string.IsNullOrEmpty(video?.Name))
            //    _browser.Main.SetWindowTitle($"{Resource.CNT_APPLICATION_NAME} - {video.Name}");

            _browser.Main.LoadingManager.Show();

            if (MediaPlayer.MediaSource != null)
                MediaPlayer.RemoveMediaItem();

            // video영상을 가져온다.
            if (!string.IsNullOrEmpty(WorkContext.VideoMediaUrl))
                MediaPlayer.InitMedia(WorkContext, false);

            _browser.Main.LoadingManager.Hide();

            CommandManager.InvalidateRequerySuggested();
        }

        private string CheckConflictLabel(string label)
        {
            var result = label;
            var count = 1;

            while (Tabs.Any(x => x.Name == result))
            {
                if (count == 100) break;

                result = $"{label}_{count++}";
            }

            return result;
        }

        private void OnCaptionElementCreateNew(Message.SubtitleEditor.CaptionElementCreateNewMessage message)
        {
            var param = message.Param;

            try
            {
                Debug.WriteLine("Start OnCaptionElementCreateNew");

                _browser.Main.LoadingManager.Show();

                var label = CheckConflictLabel(param.Label);

                var workBar = Bootstrapper.Container.Resolve<WorkBarViewModel>();
                var videoId = workBar.VideoItem?.Id;
                var captionAssetId = workBar.CaptionAssetItem?.Id;

                AddTab(label,
                    param.Kind,
                    param.LanguageCode,
                    param.CountryCode,
                    null,
                    null,
                    videoId,
                    param.FilePath,
                    captionAssetId,
                    SourceLocationKind.CreatedByEditor
                );

                _browser.Main.LoadingManager.Hide();

                Debug.WriteLine("End OnCaptionElementCreateNew");
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex.Message);
                _browser.Main.LoadingManager.Hide();
                throw;
            }
        }

        private void OnOpenFile(Message.SubtitleEditor.FileOpenedMessage message)
        {
            var param = message.Param;
            if (param == null)
                return;

            OpenFileAsync(param);
        }
        
        private async Task OpenFileAsync(FileOpenedMessageParameter param)
        {
            try {
                Debug.WriteLine("Start FileOpenAsync");

                _browser.Main.LoadingManager.Show();

                //_subtitleListItemValidator.IsEnabled = false;

                var label = CheckConflictLabel(param.Label);

                var text = param.FilePath != null ? File.ReadAllText(param.FilePath) : "";
                string videoId = "";
                string captionAssetId = "";

                var rows = _subtitleService.Load(text, param.SubtitleFormat);

                var workBar = Bootstrapper.Container.Resolve<WorkBarViewModel>();
                videoId = workBar.VideoItem?.Id;
                captionAssetId = workBar.CaptionAssetItem?.Id;

                AddTab(label, 
                    param.Kind, 
                    param.LanguageCode,
                    param.CountryCode,
                    null,
                    rows,
                    videoId,
                    param.FilePath,
                    captionAssetId,
                    SourceLocationKind.LocalFile
                    );

                if (param.FilePath.IsNotNullAndAny())
                {
                    SubtitleFormatKind subtitleKind =  GetSubTitleFormatKindByFileName(param.FilePath);
                    _recentlyLoader.Save(new RecentlyItem.OfflineRecentlyCreator().SetLocalFileFullPath(param.FilePath)
                        .SetFormat(subtitleKind).Create());
                }

                this.InvokeOnUi(() =>
                {
                    _browser.Main.LoadingManager.Hide();
                });

                Debug.WriteLine("End OnOpenFile");
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex.Message);
                _browser.Main.LoadingManager.Hide();
                throw;
            }
        }

        private void OnRowCollectionChanged(SubtitleTabItemViewModel tab)
        {
            _subtitleListItemValidator.Validate(tab.Rows);
        }

        private async void OnImportExcelFile(Message.Excel.FileImportMessage message)
        {
            _subtitleListItemValidator.IsEnabled = false;

            var filePath = message.FilePath;
            var sheetInfos = message.SheetInfos;
            var workBar = Bootstrapper.Container.Resolve<WorkBarViewModel>();

            _browser.Main.LoadingManager.Show();

            await this.CreateTask(() =>
            {
                IList<Excel.Subtitle> importedSubtitles =
                    _excelService.GetSheetContents(filePath, sheetInfos)
                        .ToList();

                if (importedSubtitles.All(item => !item.Datasets.Any()))
                    this.InvokeOnUi(() =>
                    {
                        _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_WARNING,
                            Resource.MSG_NO_PRINTABLE_CONTENTS,
                            MessageBoxButton.OK,
                            Application.Current.MainWindow));
                        _browser.Main.LoadingManager.Hide();
                    });

                _subtitleListItemValidator.IsEnabled = false;
                this.InvokeOnUi(() =>
                {
                    SubtitleTabItemViewModel firstTab = null;
                    foreach (var subtitle in importedSubtitles)
                    {
                        var newTab = new SubtitleTabItemViewModel(subtitle.Label,
                            OnRowCollectionChanged,
                            OnValidateRequested,
                            OnTabSelected,
                            OnItemSelected,
                            OnDoubleClickedItem,
                            subtitle.Kind,
                            OnDisplayTextChanged,
                            SourceLocationKind.LocalFile,
                            subtitle.LanguageCode,
                            subtitle.CountryCode)
                        {
                            VideoId = workBar.VideoItem?.Id,
                            CaptionAssetId = workBar?.CaptionAssetItem?.Id
                        };
                        newTab.AddDatasheet(subtitle.Datasets);
                        if (firstTab == null)
                            firstTab = newTab;
                        Tabs.Add(newTab);
                    }

                    if (SelectedTab != null)
                        SelectedTab.IsSelected = false;
                    if (firstTab != null)
                        firstTab.IsSelected = true;
                    if (SelectedTab != null)
                    {
                        _subtitleListItemValidator.IsEnabled = true;
                        _subtitleListItemValidator.Validate(SelectedTab.Rows);
                    }

                    _browser.Main.LoadingManager.Hide();
                });
            });

            _recentlyLoader.Save(new RecentlyItem.OfflineRecentlyCreator().SetLocalFileFullPath(filePath).SetFormat(SubtitleFormatKind.Excel).Create());
        }


        private async void OnCopySubtitle(Message.SubtitleEditor.CopyTabMessage message)
        {
            var param = message.Param;
            if (param == null)
                return;

            _browser.Main.LoadingManager.Show();

            _subtitleListItemValidator.IsEnabled = false;

            IList<SubtitleItem> subtitles = param.Rows.Select(row => new SubtitleItem
                {
                    Number = row.Number,
                    StartTime = row.StartTime,
                    EndTime = row.EndTime,
                    Texts = row.Texts
                })
                .ToList();

            var workBar = Bootstrapper.Container.Resolve<WorkBarViewModel>();

            var newTab = new SubtitleTabItemViewModel(param.Label,
                OnRowCollectionChanged,
                OnValidateRequested,
                OnTabSelected,
                OnItemSelected,
                OnDoubleClickedItem,
                param.Kind,
                OnDisplayTextChanged,
                SourceLocationKind.CreatedByEditor,
                param.LanguageCode,
                param.CountryCode)
            {
                IsSelected = true,
                VideoId = workBar.VideoItem?.Id,
                CaptionAssetId = workBar.CaptionAssetItem?.Id
            };
            if (subtitles.Any())
                newTab.AddRowsAsync(subtitles);

            Tabs.Add(newTab);

            _subtitleListItemValidator.IsEnabled = true;
            _subtitleListItemValidator.Validate(SelectedTab.Rows);
            CommandManager.InvalidateRequerySuggested();

            _browser.Main.LoadingManager.Hide();
        }

        private async void AddTab(string name, 
            CaptionKind kind,
            string languageCode,
            string countryCode,
            Caption caption,
            IEnumerable<SubtitleItem> subtitleContentList,
            string videoId,
            string filePath,
            string captionAssetId,
            SourceLocationKind sourceLocation,
            bool isSelected = true)
        {

            Debug.WriteLine("+ AddTab");
            _browser.Main.LoadingManager.Show();

            _subtitleListItemValidator.IsEnabled = false;

            var newTab = new SubtitleTabItemViewModel(name,
                OnRowCollectionChanged,
                OnValidateRequested,
                OnTabSelected,
                OnItemSelected,
                OnDoubleClickedItem,
                kind,
                OnDisplayTextChanged,
                sourceLocation,
                languageCode,
                countryCode,
                caption)
            {
                VideoId = videoId,
                CaptionAssetId = captionAssetId,
                FilePath = filePath,
                IsSelected = isSelected,
            };
            if (subtitleContentList != null && subtitleContentList.Any())
                await newTab.AddRowsAsync(subtitleContentList);

            this.InvokeOnUi(() =>
            {
                Tabs.Add(newTab);
            });

            _subtitleListItemValidator.IsEnabled = true;
            _subtitleListItemValidator.Validate(newTab.Rows);
            
            CommandManager.InvalidateRequerySuggested();

            CollapseAllPopup();

            _browser.Main.LoadingManager.Hide();

            MessageCenter.Instance.Send(new Message.View.CaptionElementsEditView.ChangedTabMessage(this));

            Debug.WriteLine("- AddTab");
        }

        public void CollapseAllPopup()
        {
            var leftSideViewmodel = Bootstrapper.Container.Resolve<LeftSideMenuViewModel>();
            leftSideViewmodel?.Close();

            var captionElementsEditViewmodel = Bootstrapper.Container.Resolve<CaptionElementsEditViewModel>();
            captionElementsEditViewmodel?.Close();
        }

        private void OnEditSubtitle(Message.SubtitleEditor.EditTabMessage message)
        {
            var param = message.Param;
            if (param == null)
                return;

            foreach (var tab in Tabs)
            {
                if (!tab.Id.Equals(param.Id))
                    continue;

                tab.Name = param.Label;
                tab.Kind = param.Kind;
                tab.LanguageCode = param.LanguageCode;
                tab.CountryCode = param.CountryCode;
            }
        }

        private void OnCleanUpSubtitle(Message.SubtitleEditor.CleanUpSubtitleMessage message)
        {
            CleanUpSubtitle();
            //if (_workBarViewModel.HasWorkData)
            //{
            //    ////TODO 다국어
            //    //if (_browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
            //    //        "작업 중인 내용이 있습니다. 열여진 탭을 모두 닫고, 새 작업을 만드시겠습니까?",
            //    //        MessageBoxButton.OKCancel,
            //    //        Application.Current.MainWindow,
            //    //        TextAlignment.Center)) == MessageBoxResult.Cancel)
            //    //{
            //    //    return;
            //    //}

            //    CleanUpSubtitle();
            //}
        }

        private void OnChangeRecentlyItem(Message.RecentlyLoader.ChangeItemMessage message)
        {
            RecentlyItems = _recentlyLoader.GetRecentlyItems(false).ToList();
        }

        private void CleanUpSubtitle()
        {
            // 초기화 코드
            var removeTabs = Tabs.ToList();

            foreach (var tab in removeTabs)
            {
                Tabs.Remove(tab);
                tab.Dispose();
            }

            SelectedTab = null;

            if (MediaPlayer.MediaSource != null)
                MediaPlayer.RemoveMediaItem();


            var workBarViewModel = Bootstrapper.Container.Resolve<WorkBarViewModel>();
            workBarViewModel.Initialize();

            ClearCurrentPositionText();
        }

        private void SelectPreviousRow()
        {
            var selectedRowNumber = SelectedTab.SelectedRow.Number;
            var targetRowNumber = selectedRowNumber - 1;
            if (targetRowNumber < 0)
                targetRowNumber = 0;
            var nextRow = SelectedTab.Rows.FirstOrDefault(r => r.Number == targetRowNumber);
            SelectedTab.SelectedRow = nextRow;
        }

        private bool CanSelectPreviousRow()
        {
            if (SelectedTab == null)
                return false;
            var rowsCount = SelectedTab.Rows?.Count ?? 0;
            if (rowsCount == 0) return false;
            if (SelectedTab.SelectedRow == null || rowsCount == 1) return false;
            return SelectedTab.SelectedRow.Number > 1;
        }

        private void SelectNextRow()
        {
            var selectedRowNumber = SelectedTab.SelectedRow.Number;
            var targetRowNumber = selectedRowNumber + 1;
            if (targetRowNumber > SelectedTab.Rows.Count)
                targetRowNumber = SelectedTab.Rows.Count;
            var nextRow = SelectedTab.Rows.FirstOrDefault(r => r.Number == targetRowNumber);
            SelectedTab.SelectedRow = nextRow;
        }

        private bool CanSelectNextRow()
        {
            if (SelectedTab == null)
                return false;
            var rowsCount = SelectedTab.Rows?.Count ?? 0;
            if (rowsCount == 0) return false;
            if (SelectedTab.SelectedRow == null || rowsCount == 1) return false;
            return SelectedTab.SelectedRow.Number < rowsCount;
        }

        private void FindText(string findText, bool isAllFind = false)
        {
            if (SelectedTab?.Rows == null) return;

            if (isAllFind)
            {
                var selectedRow = SelectedTab?.SelectedRow;
                if (selectedRow != null)
                    selectedRow.IsSelected = false;

                var selectedRows = SelectedTab.Rows.Where(item => item.Texts.HasFindText(findText))
                    .ToList();
                selectedRows.ForEach(item => item.IsSelected = true);
                SelectedTab.SelectedRows = selectedRows;
            }
            else
            {
                var selectedRow = SelectedTab?.SelectedRow;
                var nextIndex = selectedRow?.Number + 1 ?? 0;
                if (nextIndex >= SelectedTab.Rows.Count)
                    nextIndex = 0;

                var row =
                    SelectedTab.Rows.FirstOrDefault(
                        item => nextIndex <= item.Number && (item.Texts?.HasFindText(findText) ?? false));
                if (row == null) return;
                if (selectedRow != null)
                    selectedRow.IsSelected = false;
                ListBoxScroll?.ScrollIntoView(row, true);
                ListBoxScroll?.Focus();
            }
        }

        private void ReplaceAll(string findText, string replaceText)
        {
            foreach (var row in SelectedTab.Rows)
            {
                row.Texts.ReplaceAll(findText, replaceText);
                row.Texts = row.Texts;
            }
        }

        private void Replace(string findText, string replaceText)
        {
            if (SelectedTab?.Rows == null) return;
            {
                var selectedRow = SelectedTab?.SelectedRow;
                var nextIndex = selectedRow?.Number + 1 ?? 0;
                if (nextIndex >= SelectedTab.Rows.Count)
                    nextIndex = 0;

                var row =
                    SelectedTab.Rows.FirstOrDefault(
                        item => nextIndex <= item.Number && (item.Texts?.HasFindText(findText) ?? false));

                if (row == null) return;
                if (selectedRow != null)
                    selectedRow.IsSelected = false;

                if (row.Texts.ReplaceFirst(findText, replaceText))
                    row.Texts = row.Texts;

                ListBoxScroll?.ScrollIntoView(row, true);
                ListBoxScroll?.Focus();
            }
        }

        private void OpenRecently(RecentlyItem recentlyItem)
        {
            //var subtitleVm = Bootstrapper.Container.Resolve<SubtitleViewModel>();
            if (Tabs?.Any() ?? false)
            {
                if (Tabs.Any(tab => tab.CheckDirty()))
                    if (_browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_WARNING,
                            Resource.MSG_THERE_IS_WORK_IN_PROGRESS,
                            MessageBoxButton.OKCancel,
                            Application.Current.MainWindow)) !=
                        MessageBoxResult.OK)
                        return;

                var removeTabs = Tabs.ToList();
                foreach (var tab in removeTabs)
                {
                    CloseTab(tab as SubtitleTabItemViewModel);
                }
            }

            // 선택된 video 정보를 메인 
            //var video = recentlyItem.Video;
            //var asset = recentlyItem.CaptionAsset;
            var localFileFullPath = recentlyItem.LocalFileFullPath;
            //var selectedCaptionList = recentlyItem.Captions?.ToList() ?? new List<Caption>();

            ImportSubtitleFile(localFileFullPath);
        }

     
    }
}
