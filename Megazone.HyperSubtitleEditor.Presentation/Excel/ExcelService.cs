using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Megazone.Core.Extension;
using Megazone.Core.IoC;
using Megazone.Core.Log;
using Megazone.Core.VideoTrack.Model;
using Megazone.Core.VideoTrack.WebVtt;

// ReSharper disable PossiblyMistakenUseOfParamsMethod

namespace Megazone.HyperSubtitleEditor.Presentation.Excel
{
    [Inject(Scope = LifetimeScope.Transient)]
    internal class ExcelService
    {
        private const string LINE_FEED_TAG = "<br/>";
        private readonly ILogger _logger;
        private readonly WebVttParser _webVttInterpreter;

        public ExcelService(WebVttParser webVttInterpreter, ILogger logger)
        {
            _webVttInterpreter = webVttInterpreter;
            _logger = logger;
        }

        public IEnumerable<ExcelSheetInfo> GetSheetInfo(string excelPath)
        {
            IList<ExcelSheetInfo> result = new List<ExcelSheetInfo>();
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
                            // TODO: Context Sheet 만들어서 있으면 묶어서 내보내줘서 파싱할 때 사용될 수 있도록 구현하자.
                            if (selectedSheet == null || selectedSheet.Name == "Code")
                                continue;

                            var excelFileInfo = new ExcelSheetInfo
                            {
                                SheetName = selectedSheet.Name.Value
                            };

                            var splitedSheetName = selectedSheet.Name.Value.Split('_');

                            if (!string.IsNullOrEmpty(splitedSheetName[0]))
                            {
                                var splitLanguageAndCountry = splitedSheetName[0].Split('-');

                                if (!string.IsNullOrEmpty(splitLanguageAndCountry[0]))
                                    excelFileInfo.LanguageCode = splitLanguageAndCountry[0];

                                if (!string.IsNullOrEmpty(splitLanguageAndCountry[1]))
                                    excelFileInfo.CountryCode = splitLanguageAndCountry[1];
                            }

                            if (splitedSheetName.Length > 1 && !string.IsNullOrEmpty(splitedSheetName[1]))
                                excelFileInfo.Label = splitedSheetName[1];

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

        public IEnumerable<Subtitle> GetSheetContents(string excelPath, IList<ExcelSheetInfo> sheetInfos)
        {
            IList<Subtitle> subtitles = new List<Subtitle>();

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

                            var subtitle = new Subtitle
                            {
                                Label = info.Label,
                                LanguageCode = info.LanguageCode,
                                CountryCode = info.CountryCode,
                                Format = info.TrackFormat,
                                Kind = info.CaptionKind
                            };

                            var workSheet = ((WorksheetPart) workbookPart.GetPartById(selectedSheet.Id)).Worksheet;
                            var sheetData = workSheet.Elements<SheetData>()
                                .First();
                            var rows = sheetData.Elements<Row>()
                                .ToList();

                            for (var i = 0; i < rows.Count; i++)
                            {
                                var row = rows[i];
                                var cellEnumerator = GetExcelCellEnumerator(row);
                                var columCount = 0;

                                var subtitleItem = new SubtitleItem
                                {
                                    Number = i + 1
                                };
                                while (cellEnumerator.MoveNext())
                                {
                                    var cell = cellEnumerator.Current;
                                    var text = ReadExcelCell(cell, workbookPart);

                                    try
                                    {
                                        switch (columCount)
                                        {
                                            case 0:
                                                subtitleItem.StartTime = TimeSpan.Parse(RefineTime(text));
                                                break;
                                            case 1:
                                                subtitleItem.EndTime = TimeSpan.Parse(RefineTime(text));
                                                break;
                                            case 2:
                                                subtitleItem.Texts = _webVttInterpreter.ParseLine(text)
                                                    .ToList();
                                                break;
                                        }
                                    }
                                    catch (FormatException)
                                    {
                                        // ignored
                                    }
                                    finally
                                    {
                                        columCount++;
                                    }
                                }

                                subtitle.Datasets.Add(subtitleItem);
                            }

                            subtitles.Add(subtitle);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex.Message);
            }

            return subtitles;
        }

        private static string RefineTime(string text)
        {
            // TODO: FramePerSecond 받아서 ; 있는 경우 프레임 값을 총 프레임 값으로 나눠서 밀리세컨즈로 표시필요함
            if (string.IsNullOrEmpty(text))
                return text;

            var semicolonIndex = text.IndexOf(";", StringComparison.InvariantCultureIgnoreCase);
            if (semicolonIndex > -1)
                return text.Substring(0, semicolonIndex);
            return text;
        }

        public bool CreateFile(IEnumerable<Subtitle> subtitles, string saveFilePath)
        {
            const string resourceName =
                "Megazone.HyperSubtitleEditor.Presentation.Excel.HyperSubtitleEditor_Caption_Format.xlsx";
            const string exampleSheetName = "Example";

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fileStream = File.Create(saveFilePath);

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                        throw new FileNotFoundException();

                    stream.CopyTo(fileStream);
                    fileStream.Close();

                    using (var mySpreadsheet = SpreadsheetDocument.Open(saveFilePath, true))
                    {
                        foreach (var subtitle in subtitles)
                            CreateSheet(mySpreadsheet, subtitle);

                        DeleteSheet(mySpreadsheet, exampleSheetName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error.Write(ex.Message);
                return false;
            }

            return true;
        }

        private void DeleteSheet(SpreadsheetDocument mySpreadsheet, string sheetNameToDelete)
        {
            var workbookPart = mySpreadsheet.WorkbookPart;

            var sheetToDelete = workbookPart.Workbook.Descendants<Sheet>()
                .Where(s => s.Name == sheetNameToDelete)
                .FirstOrDefault();
            var sheetIdToDelete = sheetToDelete.SheetId;

            if (sheetIdToDelete == null)
                return;

            // Remove the sheet reference from the workbook.
            var worksheetPart = (WorksheetPart) workbookPart.GetPartById(sheetToDelete.Id);
            sheetToDelete.Remove();

            workbookPart.DeletePart(worksheetPart);

            //Get the DefinedNames
            var definedNames = workbookPart.Workbook.Descendants<DefinedNames>()
                .FirstOrDefault();
            if (definedNames != null)
            {
                var defNamesToDelete = new List<DefinedName>();

                foreach (DefinedName Item in definedNames)
                {
                    var itemName = Item.Text.Replace("'\' ", "");
                    // This condition checks to delete only those names which are part of Sheet in question
                    if (itemName.Contains(sheetToDelete + "!"))
                        defNamesToDelete.Add(Item);
                }

                foreach (var Item in defNamesToDelete)
                    Item.Remove();
            }

            // Get the CalculationChainPart 
            //Note: An instance of this part type contains an ordered set of references to all cells in all worksheets in the 
            //workbook whose value is calculated from any formula
            CalculationChainPart calChainPart;
            calChainPart = workbookPart.CalculationChainPart;
            if (calChainPart != null)
            {
                var calChainEntries = calChainPart.CalculationChain.Descendants<CalculationCell>()
                    .Where(c => c.SheetId.Value == sheetIdToDelete);

                var calcsToDelete = new List<CalculationCell>();
                foreach (var Item in calChainEntries)
                    calcsToDelete.Add(Item);

                foreach (var Item in calcsToDelete)
                    Item.Remove();

                if (calChainPart.CalculationChain.Count() == 0)
                    workbookPart.DeletePart(calChainPart);
            }

            workbookPart.Workbook.Save();
        }

        private void CreateSheet(SpreadsheetDocument mySpreadsheet, Subtitle subtitle)
        {
            var workbookPart = mySpreadsheet.WorkbookPart;
            var newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();

            // 셀 서식 추가 (텍스트로 지정.)
            var cellFormats = workbookPart.WorkbookStylesPart.Stylesheet.Elements<CellFormats>()
                .First();
            var cellFormat = new CellFormat
            {
                NumberFormatId = 49U, // style 번호 49번은 '@'로 텍스트를 나타냄.
                FontId = 0U,
                FillId = 0U,
                BorderId = 0U,
                FormatId = 0U,
                ApplyNumberFormat = true
            };
            cellFormats.Append(cellFormat);
            var styleIndex = cellFormats.Count++; // 1부터 시작.

            for (var i = 0; i < subtitle.Datasets.Count; i++)
            {
                var row = new Row();

                var startTime = subtitle.Datasets[i]
                    .StartTime.ToString();
                row.Append(CreateCell(SetExportTimeFormat(startTime), "A" + (i + 1), CellValues.String, styleIndex));

                var endTime = subtitle.Datasets[i]
                    .EndTime.ToString();
                row.Append(CreateCell(SetExportTimeFormat(endTime), "B" + (i + 1), CellValues.String, styleIndex));

                var text = _webVttInterpreter.ToTextLine(subtitle.Datasets[i]
                    .Texts);
                row.Append(CreateCell(text.Replace(LINE_FEED_TAG, "\n"), "C" + (i + 1), CellValues.String, styleIndex));

                sheetData.Append(row);
            }

            newWorksheetPart.Worksheet = new Worksheet(sheetData);

            // 메인 통합 문서 부분에 새 시트를 추가.
            var sheets = mySpreadsheet.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            var newSheet = new Sheet
            {
                Name = subtitle.LanguageCode +
                       "-" +
                       subtitle.CountryCode +
                       "_" +
                       subtitle.Label +
                       "(" +
                       ((uint) sheets.ChildElements.Count - 1) +
                       ")",
                Id = mySpreadsheet.WorkbookPart.GetIdOfPart(newWorksheetPart),
                SheetId = (uint) sheets.ChildElements.Count + 10
            };
            sheets.Append(newSheet);

            newWorksheetPart.Worksheet.Save();
            workbookPart.Workbook.Save();
        }

        private static string SetExportTimeFormat(string time)
        {
            if (time.Length > 12)
                time = time.Substring(0, 12);

            while (time.Length < 12)
                if (!time.Contains("."))
                    time += ".";
                else
                    time += 0;
            return time;
        }

        private Cell CreateCell(string value, string referenceValue, CellValues dataType, uint styleIndex)
        {
            return new Cell
            {
                CellValue = new CellValue(value),
                CellReference = new StringValue(referenceValue),
                DataType = new EnumValue<CellValues>(dataType),
                StyleIndex = styleIndex
            };
        }

        private static string ReadExcelCell(Cell cell, WorkbookPart workbookPart)
        {
            var cellValue = cell.CellValue;
            var text = cellValue == null ? cell.InnerText : cellValue.Text;

            if (cell.DataType == null || cell.DataType != CellValues.SharedString || cell.CellValue == null)
                return text ?? string.Empty;

            var sharedStringTable = workbookPart.SharedStringTablePart.SharedStringTable;
            var elements = sharedStringTable.Elements<SharedStringItem>();
            text = elements.ElementAt(Convert.ToInt32(cell.CellValue.Text))
                .InnerText;

            return text ?? string.Empty;
        }

        private static IEnumerator<Cell> GetExcelCellEnumerator(Row row)
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
                    yield return emptycell; // 하나씩 리턴.
                }

                yield return cell;
                currentCount++;
            }
        }

        private static int ConvertColumnNameToNumber(string columnName)
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

        private static string GetColumnName(string cellReference)
        {
            return new Regex("[A-Za-z]+").Match(cellReference)
                .Value;
        }
    }
}