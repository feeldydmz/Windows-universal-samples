using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Megazone.Core.IoC;
using Megazone.HyperSubtitleEditor.Domain.Subtitle;
using Megazone.HyperSubtitleEditor.Domain.Subtitle.Enum;
using Megazone.HyperSubtitleEditor.Repository.Subtitle.Extension;
using Excel = Microsoft.Office.Interop.Excel;
using PublicModel = Megazone.HyperSubtitleEditor.Domain.Subtitle.Model;
using InternalModel = Megazone.HyperSubtitleEditor.Repository.Subtitle.Model;

namespace Megazone.HyperSubtitleEditor.Repository.Subtitle
{
    [Inject(Source = typeof(ISubtitleRepository), Scope = LifetimeScope.Transient)]
    internal class SubtitleRepository : ISubtitleRepository
    {
        public IList<string> FIndWorkSheetNamesFromExcel(string excelFilePath)
        {
            Excel.Sheets workSheets = null;
            Excel.Application app = null;
            Excel.Workbook workBook = null;

            try
            {
                app = new Excel.Application();
                workBook = app.Workbooks.Open(excelFilePath);
                workSheets = workBook.Worksheets;

                if (workSheets == null)
                    return null;

                IList<string> sheetNames = new List<string>();

                foreach (var sheet in workSheets)
                {
                    var name = ((Excel.Worksheet)sheet)?.Name;
                    if (!string.IsNullOrEmpty(name))
                        sheetNames.Add(name);
                }
                return sheetNames;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (Marshal.IsComObject(workSheets) && workSheets != null) Marshal.ReleaseComObject(workSheets);

                if (Marshal.IsComObject(workBook) && workBook != null)
                {
                    workBook.Close(true);
                    Marshal.FinalReleaseComObject(workBook);
                }
                if (Marshal.IsComObject(app) && app != null)
                {
                    app.Quit();
                    Marshal.FinalReleaseComObject(app);
                }
            }
        }

        public PublicModel.Subtitle FindSubtitleFromExcel(string excelFilePath, string workSheetName)
        {
            Excel.Application app = null;
            Excel.Workbook workBook = null;
            Excel.Sheets workSheets = null;
            Excel.Worksheet workSheet = null;

            try
            {
                app = new Excel.Application();
                workBook = app.Workbooks.Open(excelFilePath);
                workSheets = workBook.Worksheets;

                foreach (var ws in workSheets)
                {
                    if (((Excel.Worksheet)ws).Name.Equals(workSheetName))
                    {
                        workSheet = (Excel.Worksheet)ws;
                    }
                }

                if (workSheet == null)
                    return null;

                var subtitle = new InternalModel.Subtitle();
                var range = workSheet.UsedRange;
                var row = range.Rows.Count;
                var columnIndex = 1;

                for (var rowIndex = 1; rowIndex <= row; rowIndex++)
                {
                    var value = (range.Cells[rowIndex, columnIndex] as Excel.Range)?.Value;
                    if (value == null)
                        continue;

                    if (value is string)
                    {
                        if (value.Equals("국가 코드"))
                        {
                            subtitle.CountryCode = (range.Cells[rowIndex, columnIndex + 1] as Excel.Range)?.Value;
                            continue;
                        }

                        if (value.Equals("파일 형식"))
                        {
                            var format = (range.Cells[rowIndex, columnIndex + 1] as Excel.Range)?.Value;
                            if (format != null) subtitle.Format = (SubtitleFormat)System.Enum.Parse(typeof(SubtitleFormat), format);
                            continue;
                        }
                    }

                    if (value is double)
                    {
                        var no = value.ToString(CultureInfo.InvariantCulture);
                        var startTime = (range.Cells[rowIndex, columnIndex + 1] as Excel.Range)?.Value;
                        var endTime = (range.Cells[rowIndex, columnIndex + 2] as Excel.Range)?.Value;
                        var text = (range.Cells[rowIndex, columnIndex + 3] as Excel.Range)?.Value;

                        if (string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime) &&
                            string.IsNullOrEmpty(text))
                            continue;

                        var webVtt = new InternalModel.WebVtt { Number = no };

                        if (!string.IsNullOrEmpty(startTime))
                            webVtt.StartTime = TimeSpan.Parse(startTime);
                        if (!string.IsNullOrEmpty(endTime))
                            webVtt.EndTime = TimeSpan.Parse(endTime);
                        if (!string.IsNullOrEmpty(text))
                            webVtt.Text = text;

                        subtitle.Datasets.Add(webVtt);
                    }
                }
                return subtitle.ToPublicModel();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (Marshal.IsComObject(workSheet) && workSheet != null) Marshal.ReleaseComObject(workSheet);
                if (Marshal.IsComObject(workSheets) && workSheets != null) Marshal.ReleaseComObject(workSheets);

                if (Marshal.IsComObject(workBook) && workBook != null)
                {
                    workBook.Close(true);
                    Marshal.FinalReleaseComObject(workBook);
                }
                if (Marshal.IsComObject(app) && app != null)
                {
                    app.Quit();
                    Marshal.FinalReleaseComObject(app);
                }
            }
        }
    }
}
