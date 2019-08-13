using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Megazone.Api.Transcoder.Domain;
using Megazone.Core.IoC;
using Megazone.HyperSubtitleEditor.Domain.Subtitle;
using SubtitleModel = Megazone.HyperSubtitleEditor.Domain.Subtitle.Model;

namespace Megazone.HyperSubtitleEditor.Service.Subtitle
{
    [Inject(Scope = LifetimeScope.Transient, Source = typeof(IExcelService))]
    internal class ExcelService : IExcelService
    {
        private readonly IWebVttInterpreter _webVttInterpreter;

        public ExcelService(IWebVttInterpreter webVttInterpreter)
        {
            _webVttInterpreter = webVttInterpreter;
        }

        public IEnumerable<SubtitleModel.ExcelSheetInfo> GetSheetInfo(string excelPath)
        {
            IList<SubtitleModel.ExcelSheetInfo> result = new List<SubtitleModel.ExcelSheetInfo>();
            try
            {
                using (var fileStream = new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var spreadsheetDocument = SpreadsheetDocument.Open(fileStream, false))
                    {
                        var workbookPart = spreadsheetDocument.WorkbookPart;
                        var sheets = workbookPart.Workbook.Descendants<Sheet>();

                        foreach (var selectedSheet in sheets.ToList())
                        {
                            if (selectedSheet == null)
                                continue;

                            var workSheet = ((WorksheetPart) workbookPart.GetPartById(selectedSheet.Id)).Worksheet;
                            var sheetData = workSheet.Elements<SheetData>().First();
                            var rows = sheetData.Elements<Row>().ToList();

                            if (selectedSheet.Name == "언어코드")
                                continue;

                            var excelFileInfo = new SubtitleModel.ExcelSheetInfo {SheetName = selectedSheet.Name};

                            if (rows.Count < 3)
                                continue;

                            for (var i = 0; i < 3; i++)
                            {
                                var beforeText = string.Empty;
                                var cellEnumerator = GetExcelCellEnumerator(rows[i]);

                                while (cellEnumerator.MoveNext())
                                {
                                    var cell = cellEnumerator.Current;
                                    var text = ReadExcelCell(cell, workbookPart).Trim();

                                    if (string.IsNullOrEmpty(beforeText) && text.Equals("국가 코드") || text.Equals("파일 형식") ||
                                        text.Equals("파일 종류"))
                                    {
                                        beforeText = text;
                                        continue;
                                    }

                                    if (beforeText.Equals("국가 코드"))
                                        excelFileInfo.LanguageCode = text;
                                    if (beforeText.Equals("파일 형식") && text.ToUpper().Equals("WEBVTT"))
                                        excelFileInfo.TrackFormat = TrackFormat.WebVtt;
                                    if (beforeText.Equals("파일 종류"))
                                        switch (text.ToUpper())
                                        {
                                            case "SUBTITLE":
                                                excelFileInfo.TrackKind = TrackKind.Subtitle;
                                                break;
                                            case "CAPTION":
                                                excelFileInfo.TrackKind = TrackKind.Caption;
                                                break;
                                            case "CHAPTER":
                                                excelFileInfo.TrackKind = TrackKind.Chapter;
                                                break;
                                            default:
                                                excelFileInfo.TrackKind = TrackKind.Subtitle;
                                                break;
                                        }
                                }
                            }
                            result.Add(excelFileInfo);
                        }
                        return result;
                    }
                }
            }
            catch
            {
                return result;
            }
        }

        public IEnumerable<SubtitleModel.Subtitle> GetSheetContents(string excelPath,
            IList<SubtitleModel.ExcelSheetInfo> sheetInfos)
        {
            IList<SubtitleModel.Subtitle> subtitles = new List<SubtitleModel.Subtitle>();

            try
            {
                using (var fileStream = new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var spreadsheetDocument = SpreadsheetDocument.Open(fileStream, false))
                    {
                        var workbookPart = spreadsheetDocument.WorkbookPart;
                        var sheets = workbookPart.Workbook.Descendants<Sheet>();
                        var enumerable = sheets as IList<Sheet> ?? sheets.ToList();

                        foreach (var info in sheetInfos)
                        foreach (var selectedSheet in enumerable.ToList())
                        {
                            if (!selectedSheet.Name.Value.Equals(info.SheetName))
                                continue;

                            var subtitle = new SubtitleModel.Subtitle
                            {
                                Label = info.SheetName,
                                LanguageCode = info.LanguageCode,
                                Format = info.TrackFormat,
                                Kind = info.TrackKind
                            };

                            var workSheet = ((WorksheetPart) workbookPart.GetPartById(selectedSheet.Id)).Worksheet;
                            var sheetData = workSheet.Elements<SheetData>().First();
                            var rows = sheetData.Elements<Row>().ToList();

                            for (var i = 0; i < rows.Count; i++)
                            {
                                if (i < 4) continue;

                                var row = rows[i];
                                var cellEnumerator = GetExcelCellEnumerator(row);
                                var columCount = 0;

                                var subtitleItem = new SubtitleModel.SubtitleItem();

                                while (cellEnumerator.MoveNext())
                                {
                                    var cell = cellEnumerator.Current;
                                    var text = ReadExcelCell(cell, workbookPart).Trim();

                                    if (columCount == 0 && string.IsNullOrEmpty(text) || text.ToUpper().Equals("NO"))
                                        break;

                                    try
                                    {
                                        switch (columCount)
                                        {
                                            case 0:
                                                subtitleItem.Number = text;
                                                break;
                                            case 1:
                                                subtitleItem.StartTime = TimeSpan.Parse(text);
                                                break;
                                            case 2:
                                                subtitleItem.EndTime = TimeSpan.Parse(text);
                                                break;
                                            case 3:
                                                subtitleItem.Texts = _webVttInterpreter.ConvertToStringToITexts(text);
                                                break;
                                        }
                                    }
                                    catch (FormatException)
                                    {
                                    }
                                    finally
                                    {
                                        columCount++;
                                    }
                                }

                                if (string.IsNullOrEmpty(subtitleItem.Number))
                                    continue;

                                subtitle.Datasets.Add(subtitleItem);
                            }
                            subtitles.Add(subtitle);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return subtitles;
        }

        public bool CreateFile(IEnumerable<SubtitleModel.Subtitle> subtitles, string saveFilePath)
        {
            try
            {
                using (var document = SpreadsheetDocument.Create(saveFilePath, SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();
                    var sheets = workbookPart.Workbook.AppendChild(new Sheets());

                    UInt32Value sheetId = 1;

                    foreach (var subtitle in subtitles)
                    {
                        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet();

                        var sheet = new Sheet
                        {
                            Id = workbookPart.GetIdOfPart(worksheetPart),
                            SheetId = sheetId,
                            Name = subtitle.Label + "_" + subtitle.LanguageCode + "_" + sheetId
                        };
                        sheets.Append(sheet);

                        workbookPart.Workbook.Save();

                        var subtitleItems = subtitle.Datasets;

                        var sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                        // 한줄씩 추가.
                        var row = new Row();
                        row.Append(
                            ConstructCell("국가 코드", "A1", CellValues.String),
                            ConstructCell(subtitle.LanguageCode, "B1", CellValues.String));
                        sheetData.AppendChild(row);

                        row = new Row();
                        row.Append(
                            ConstructCell("파일 형식", "A2", CellValues.String),
                            ConstructCell(subtitle.Format.ToString(), "B2", CellValues.String));
                        sheetData.AppendChild(row);

                        row = new Row();
                        row.Append(
                            ConstructCell("파일 종류", "A3", CellValues.String),
                            ConstructCell(subtitle.Kind.ToString(), "B3", CellValues.String));
                        sheetData.AppendChild(row);

                        row = new Row();
                        sheetData.AppendChild(row);

                        row = new Row();
                        row.Append(
                            ConstructCell("No", "A5", CellValues.String),
                            ConstructCell("Start Time", "B5", CellValues.String),
                            ConstructCell("End Time", "C5", CellValues.String),
                            ConstructCell("Text", "D5", CellValues.String));
                        sheetData.AppendChild(row);

                        var cellCount = 6;

                        foreach (var item in subtitleItems)
                        {
                            row = new Row();
                            row.Append(
                                ConstructCell(item.Number, "A" + cellCount, CellValues.Number),
                                ConstructCell(item.StartTime.ToString().Substring(0, 12), "B" + cellCount,
                                    CellValues.String),
                                ConstructCell(item.EndTime.ToString().Substring(0, 12), "C" + cellCount,
                                    CellValues.String),
                                ConstructCell(_webVttInterpreter.ConvertITextsToOutsideString(item.Texts),
                                    "D" + cellCount, CellValues.String));

                            sheetData.AppendChild(row);
                            cellCount++;
                        }
                        worksheetPart.Worksheet.Save();
                        sheetId++;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private string ReadExcelCell(Cell cell, WorkbookPart workbookPart)
        {
            var cellValue = cell.CellValue;
            var text = cellValue == null ? cell.InnerText : cellValue.Text;
            if (cell.DataType != null && cell.DataType == CellValues.SharedString)
                text = workbookPart.SharedStringTablePart.SharedStringTable
                    .Elements<SharedStringItem>().ElementAt(
                        Convert.ToInt32(cell.CellValue.Text)).InnerText;

            return (text ?? string.Empty).Trim();
        }

        private IEnumerator<Cell> GetExcelCellEnumerator(Row row)
        {
            var currentCount = 0;
            foreach (var cell in row.Descendants<Cell>())
            {
                var columnName = GetColumnName(cell.CellReference);
                var currentColumnIndex = ConvertColumnNameToNumber(columnName);

                for (; currentCount < currentColumnIndex; currentCount++)
                {
                    var emptycell = new Cell
                    {
                        DataType = null,
                        CellValue = new CellValue(string.Empty)
                    };
                    yield return emptycell;
                }

                yield return cell;
                currentCount++;
            }
        }

        private int ConvertColumnNameToNumber(string columnName)
        {
            var alpha = new Regex("^[A-Z]+$");
            if (!alpha.IsMatch(columnName)) throw new ArgumentException();

            var colLetters = columnName.ToCharArray();
            Array.Reverse(colLetters);

            var convertedValue = 0;
            for (var i = 0; i < colLetters.Length; i++)
            {
                var letter = colLetters[i];
                // ASCII 'A' = 65
                var current = i == 0 ? letter - 65 : letter - 64;
                convertedValue += current * (int) Math.Pow(26, i);
            }

            return convertedValue;
        }

        private string GetColumnName(string cellReference)
        {
            return new Regex("[A-Za-z]+").Match(cellReference).Value;
        }

        private Cell ConstructCell(string value, string referenceValue, CellValues dataType)
        {
            return new Cell
            {
                CellValue = new CellValue(value),
                CellReference = new StringValue(referenceValue),
                DataType = new EnumValue<CellValues>(dataType)
            };
        }
    }
}