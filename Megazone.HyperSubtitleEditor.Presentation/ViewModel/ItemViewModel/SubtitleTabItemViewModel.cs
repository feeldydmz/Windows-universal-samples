using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Megazone.Cloud.Media.Domain;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Core.Extension;
using Megazone.Core.VideoTrack.Model;
using Megazone.Core.Windows.Mvvm;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Messagenger;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Model;
using Megazone.HyperSubtitleEditor.Presentation.Message.View;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    internal class SubtitleTabItemViewModel : ViewModelBase, ISubtitleTabItemViewModel
    {
        private readonly Action<SubtitleTabItemViewModel> _onDisplayTextChangedAction;
        private readonly Action<SubtitleTabItemViewModel> _onSelectedAction;
        private readonly Action<ISubtitleListItemViewModel> _onSelectedRowAction;
        private readonly OriginalData _originalData;
        private readonly Action<SubtitleTabItemViewModel> _rowCollectionChangedAction;
        private readonly IList<ISubtitleListItemViewModel> _rows;
        private readonly Action<SubtitleTabItemViewModel> _validateAction;
        private IList<SubtitleItem> _datasheet;

        private bool _ignoreCollectionChanged;

        private bool _isAddedFromLocal;
        private bool _isDirty;
        private bool _isLoadedFromDataSheet;
        private bool _isRowCollectionChanged;
        private bool _isSelected;
        private CaptionKind _kind;
        private string _languageCode;
        private string _name;
        private ISubtitleListItemViewModel _selectedRow;

        private IList _selectedRows;
        private ICommand _setFocusToSubtitleTextBox;

        public SubtitleTabItemViewModel(string name, Action<SubtitleTabItemViewModel> rowCollectionChangedAction,
            Action<SubtitleTabItemViewModel> validateAction, Action<SubtitleTabItemViewModel> onSelectedAction,
            Action<ISubtitleListItemViewModel> onSelectedRowAction,
            CaptionKind kind,
            Action<SubtitleTabItemViewModel> onDisplayTextChangedAction,
            string languageCode = null,
            Caption caption = null)
        {
            _originalData = new OriginalData(name, kind, languageCode);

            _rowCollectionChangedAction = rowCollectionChangedAction;
            _validateAction = validateAction;
            _onSelectedAction = onSelectedAction;
            _onSelectedRowAction = onSelectedRowAction;
            _onDisplayTextChangedAction = onDisplayTextChangedAction;
            _languageCode = languageCode;
            _kind = kind;
            _name = name;
            var rows = new ObservableCollection<ISubtitleListItemViewModel>();
            rows.CollectionChanged += Items_CollectionChanged;
            _rows = rows;
            Caption = caption;
            _isAddedFromLocal = caption == null;
        }

        public IList SelectedRows
        {
            get => _selectedRows;
            set => Set(ref _selectedRows, value);
        }

        public ISubtitleListItemViewModel SelectedRow
        {
            get => _selectedRow;
            set
            {
                Set(ref _selectedRow, value);
                _onSelectedRowAction.Invoke(value);
            }
        }

        public ICommand SetFocusToSubtitleTextBoxCommand
        {
            get
            {
                return
                    _setFocusToSubtitleTextBox =
                        _setFocusToSubtitleTextBox ?? new RelayCommand(SetFocusToSubtitleTextBox);
            }
        }

        public bool IsDeployedOnce { get; private set; }

        public Caption Caption { get; }
        public string VideoId { get; set; }
        public string CaptionAssetId { get; set; }
        public string FilePath { get; set; }

        public string Id { get; } = Guid.NewGuid()
            .ToString();

        public string LanguageCode
        {
            get => _languageCode;
            set
            {
                Set(ref _languageCode, value);
                CheckDirty();
            }
        }

        public CaptionKind Kind
        {
            get => _kind;
            set
            {
                Set(ref _kind, value);
                CheckDirty();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                Set(ref _name, value);
                CheckDirty();
            }
        }

        public IList<ISubtitleListItemViewModel> Rows
        {
            get
            {
                if (!_isLoadedFromDataSheet && !_rows.Any() && _datasheet != null)
                    AddRowsFromDataSheet();
                return _rows;
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                Set(ref _isSelected, value);
                if (value)
                    _onSelectedAction?.Invoke(this);
            }
        }

        public bool IsDirty
        {
            get => _isDirty;
            set => Set(ref _isDirty, value);
        }


        public void SetAsDeployed()
        {
            IsDeployedOnce = true;
            ResetDirtyCheckFlags();
        }

        public bool CheckDirty()
        {
            if (_isDirty) return true;
            var isBaseInfoChanged = _originalData.Name != Name ||
                                    _originalData.Kind != Kind ||
                                    _originalData.LanguageCode != LanguageCode;
            var hasDirtyRow = Rows != null && Rows.Any(r => r.IsDirty());
            IsDirty = isBaseInfoChanged || _isAddedFromLocal || _isRowCollectionChanged || hasDirtyRow;
            return IsDirty;
        }

        private void SetFocusToSubtitleTextBox()
        {
            MessageCenter.Instance.Send(new SubtitleView.SetFocusToTextBoxMessage(this));
        }

        private void AddRowsFromDataSheet()
        {
            _isLoadedFromDataSheet = true;
            AddRows(_datasheet);
            _datasheet = null;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_ignoreCollectionChanged) return;
            _rowCollectionChangedAction?.Invoke(this);
            _isRowCollectionChanged = true;
            CheckDirty();
        }

        internal void DeleteSelectedItems()
        {
            var selectedItems = SelectedRows?.Cast<SubtitleListItemViewModel>()
                .ToList();
            if (selectedItems == null || !selectedItems.Any()) return;
            foreach (var selectedItem in selectedItems)
                Rows.Remove(selectedItem);
            ResetNumbers();
            SelectedRow = null;
        }

        private void ResetNumbers()
        {
            var i = 1;
            Rows.ForEach(item => item.Number = i++);
        }

        public void AddRow(SubtitleItem subtitle, int insertAt = -1)
        {
            if (insertAt == -1)
            {
                var index = Rows.Count + 1;
                var item = new SubtitleListItemViewModel(subtitle, OnValidateRequested, OnDisplayTextChanged)
                {
                    Number = index
                };
                Rows.Add(item);
                SelectedRow = item;
            }
            else
            {
                var item = new SubtitleListItemViewModel(subtitle, OnValidateRequested, OnDisplayTextChanged);
                Rows.Insert(insertAt, item);
                ResetNumbers();
                SelectedRow = item;
            }
        }

        private void OnDisplayTextChanged()
        {
            _onDisplayTextChangedAction?.Invoke(this);
            CheckDirty();
        }

        public void AddRows(IList<SubtitleItem> subtitles)
        {
            _ignoreCollectionChanged = true;
            var index = _rows.Count + 1;
            foreach (var subtitleItem in subtitles)
            {
                var item = new SubtitleListItemViewModel(subtitleItem, OnValidateRequested, OnDisplayTextChanged)
                {
                    Number = index++
                };
                Rows.Add(item);
            }

            _ignoreCollectionChanged = false;
        }

        public void AddRows(IList<SubtitleListItemViewModel> rows, int insertAt = -1)
        {
            _ignoreCollectionChanged = true;
            if (insertAt != -1)
            {
                var insertIndex = insertAt;
                foreach (var row in rows)
                    if (insertIndex < _rows.Count)
                        Rows.Insert(insertIndex++, row);
                    else
                        Rows.Add(row);
            }
            else
            {
                foreach (var row in rows)
                    Rows.Add(row);
            }

            ResetNumbers();
            _ignoreCollectionChanged = false;
        }

        public void AddDatasheet(IList<SubtitleItem> datasheet)
        {
            _datasheet = datasheet;
        }

        private SubtitleItem PrepareSubtitleItemForNewRow(TimeSpan minDuration,
            InsertRowDirection direction = InsertRowDirection.AtTheEnd, int insertAt = -1)
        {
            var subtitleItem = new SubtitleItem();
            if (insertAt == -1 || direction == InsertRowDirection.AtTheEnd)
            {
                var lastItem = Rows.LastOrDefault();
                if (lastItem != null)
                {
                    subtitleItem.StartTime = lastItem.EndTime;
                    subtitleItem.EndTime = subtitleItem.StartTime + minDuration;
                }
                else
                {
                    subtitleItem.EndTime = minDuration;
                }
            }
            else
            {
                if (Rows.Count == 0)
                {
                    subtitleItem.EndTime = minDuration;
                }
                else
                {
                    int targetIndex;
                    switch (direction)
                    {
                        case InsertRowDirection.BeforeSelectedItem:
                            targetIndex = insertAt - 1;
                            break;
                        case InsertRowDirection.AfterSelectedItem:
                            targetIndex = insertAt;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (targetIndex < 0)
                        targetIndex = 0;
                    else if (targetIndex >= Rows.Count)
                        targetIndex = Rows.Count - 1;
                    var targetItem = Rows[targetIndex];
                    subtitleItem.StartTime = targetItem.EndTime;
                    subtitleItem.EndTime = subtitleItem.StartTime + minDuration;
                }
            }

            return subtitleItem;
        }

        public SubtitleListItemViewModel AddNewRow(TimeSpan minDuration,
            InsertRowDirection direction = InsertRowDirection.AtTheEnd,
            int insertAt = -1)
        {
            var subtitleItem = PrepareSubtitleItemForNewRow(minDuration, direction, insertAt);
            if (insertAt != -1)
            {
                var insertIndex = insertAt;
                if (insertIndex < _rows.Count)
                {
                    var row = new SubtitleListItemViewModel(subtitleItem, OnValidateRequested, OnDisplayTextChanged);
                    Rows.Insert(insertAt,
                        row);
                    ResetNumbers();
                    SelectedRow = row;
                    return row;
                }
                else
                {
                    var row = new SubtitleListItemViewModel(subtitleItem, OnValidateRequested, OnDisplayTextChanged)
                    {
                        Number = _rows.Count + 1
                    };
                    Rows.Add(row);
                    SelectedRow = row;
                    return row;
                }
            }

            {
                var row = new SubtitleListItemViewModel(subtitleItem, OnValidateRequested, OnDisplayTextChanged)
                {
                    Number = _rows.Count + 1
                };
                Rows.Add(row);
                SelectedRow = row;
                return row;
            }
        }

        private void OnValidateRequested()
        {
            _validateAction?.Invoke(this);
            CheckDirty();
        }

        private void ResetDirtyCheckFlags()
        {
            _isRowCollectionChanged = false;
            _isAddedFromLocal = false;
            var rows = Rows.ToList();
            foreach (var row in rows)
                row.ResetDirtyCheckFlags();
            IsDirty = false;
        }

        private class OriginalData
        {
            public OriginalData(string name, CaptionKind kind, string languageCode)
            {
                Name = name;
                Kind = kind;
                LanguageCode = languageCode;
            }

            public string Name { get; }
            public CaptionKind Kind { get; }
            public string LanguageCode { get; }
        }

        internal enum InsertRowDirection
        {
            BeforeSelectedItem,
            AfterSelectedItem,
            AtTheEnd
        }
    }
}