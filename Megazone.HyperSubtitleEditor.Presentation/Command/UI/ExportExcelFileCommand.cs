using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Megazone.Core.Log;
using Megazone.Core.Log.Log4Net.Extension;
using Megazone.Core.VideoTrack.Model;
using Megazone.Core.Windows.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Excel;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Browser;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.Extension;
using Megazone.HyperSubtitleEditor.Presentation.Infrastructure.View;
using Megazone.HyperSubtitleEditor.Presentation.ViewModel;
using Megazone.SubtitleEditor.Resources;
using Unity;

namespace Megazone.HyperSubtitleEditor.Presentation.Command.UI
{
    public class ExportExcelFileCommand : DependencyObject, ICommand
    {
        private readonly IBrowser _browser;
        private readonly FileManager _fileManager;
        private readonly ILogger _logger;
        private readonly SubtitleViewModel _subtitleViewModel;

        public ExportExcelFileCommand()
        {
            _fileManager = Bootstrapper.Container.Resolve<FileManager>();
            _logger = Bootstrapper.Container.Resolve<ILogger>();
            _browser = Bootstrapper.Container.Resolve<IBrowser>();
            _subtitleViewModel = Bootstrapper.Container.Resolve<SubtitleViewModel>();
        }

        public bool CanExecute(object parameter)
        {
            return _subtitleViewModel.HasTab;
        }

        public async void Execute(object parameter)
        {
            try
            {
                var now = DateTime.Now.ToString("yyyyMMddHHmmss");

                var savePath = _fileManager.OpenSaveFileDialog(this.MyDocuments(), "Excel files (*.xlsx)|*.xlsx",
                    "CloudPlexMediaSubtitleEditor_" + now + ".xlsx");

                if (string.IsNullOrEmpty(savePath))
                    return;

                IList<Subtitle> subtitles = new List<Subtitle>();

                foreach (var tab in _subtitleViewModel.Tabs)
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

                _browser.Main.LoadingManager.Show();

                await this.CreateTask(() =>
                {
                    var isSuccess = _fileManager.ExportExcel(subtitles, savePath);

                    this.InvokeOnUi(() =>
                    {
                        if (isSuccess)
                        {
                            _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                                Resource.MSG_EXPORT_EXCEL_FILE_SUCCESS,
                                MessageBoxButton.OK,
                                Application.Current.MainWindow));

                            Process.Start("explorer.exe", Path.GetDirectoryName(savePath));
                        }
                        else
                            _browser.ShowConfirmWindow(new ConfirmWindowParameter(Resource.CNT_INFO,
                                Resource.MSG_EXPORT_EXCEL_FILE_FAIL,
                                MessageBoxButton.OK,
                                Application.Current.MainWindow));

                        _browser.Main.LoadingManager.Hide();
                    });
                });
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex.Message);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}