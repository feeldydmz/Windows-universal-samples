using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Megazone.Core.Extension;
using Megazone.Core.IoC;
using Megazone.Core.Language;
using Megazone.Core.Log;

namespace Megazone.AtomyExcelConverter.Presentation.Excel
{
	[Inject(Scope = LifetimeScope.Transient)]
	internal class ExcelService
	{
		private readonly ILogger _logger;

		public ExcelService(ILogger logger)
		{
			_logger = logger;
		}

		public ExcelInfo LoadExcelInfo(string fileName, string fullPath, double framePerSecond, string converterVersion)
		{
			var excelInfo = new ExcelInfo
			{
				FileName = fileName,
				FileFullPath = fullPath
			};

			try
			{
				using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					using (var spreadsheetDocument = SpreadsheetDocument.Open(fileStream, false))
					{
						var workbookPart = spreadsheetDocument.WorkbookPart;
						var sheets = workbookPart.Workbook.Descendants<Sheet>();

						var sheet = sheets.FirstOrDefault();

						if (sheet == null)
							throw new Exception("첫번째 시트가 없습니다.");

						var sheetName = sheet.Name.Value;

						var workSheet = ((WorksheetPart) workbookPart.GetPartById(sheet.Id)).Worksheet;
						var sheetData = workSheet.Elements<SheetData>().First();

						var createDataSets = CreateDatasets(sheetData.Elements<Row>().ToList(), workbookPart,
							framePerSecond, converterVersion);

						excelInfo.SheetInfos.Add(new ExcelSheetInfo(sheetName, createDataSets));
					}
				}

				return excelInfo;
			}
			catch (Exception ex)
			{
				// ignored
				_logger.Error.Write(ex.Message);
			}

			return null;
		}

		private IList<SubtitleItem> CreateDatasets(IList<Row> rows, WorkbookPart workbookPart, double framePerSecond,
			string converterVersion)
		{
			if ("V1".Equals(converterVersion)) return ConvertToSubtitleItemsV1(rows, workbookPart, framePerSecond);
			if ("V2".Equals(converterVersion)) return ConvertToSubtitleItemsV2(rows, workbookPart, framePerSecond);
			throw new ArgumentException($"Unknown converter version: {converterVersion}");
		}

		private IList<SubtitleItem> ConvertToSubtitleItemsV1(IList<Row> rows, WorkbookPart workbookPart,
			double framePerSecond)
		{
			var datasets = new List<SubtitleItem>();
			SubtitleItem subtitleItem = null;

			for (var i = 0; i < rows.Count;)
			{
				// Column이 'B'로 시작하는 셀만 가져옴.
				var cell = GetCellV1(rows[i]);

				if (cell?.CellReference == null || cell.CellReference.Value.Equals("B1"))
				{
					i++;
					continue;
				}

				var currentCellText = ReadExcelCell(cell, workbookPart);
				string nextCellText;

				// 다음 셀이 있는지 검사.
				if (IsExistNextCell(rows, i, workbookPart, out nextCellText))
				{
					int nowNumber;
					TimeSpan startTime;
					TimeSpan endTime;

					// 현재 셀과 다음 셀이 규격에 맞는지 검사.
					if (int.TryParse(currentCellText, out nowNumber) &&
					    TryParseTimeSpans(nextCellText, out startTime, out endTime, framePerSecond))
					{
						// 이전에 데이터가 존재하면.
						if (subtitleItem != null)
						{
							subtitleItem.Text = subtitleItem.Text.TrimEnd(Environment.NewLine.ToCharArray());
							datasets.Add(subtitleItem);
						}

						subtitleItem = new SubtitleItem(nowNumber, startTime, endTime);
						i += 2;
					}
					else
					{
						if (subtitleItem != null)
							subtitleItem.Text = subtitleItem.Text + currentCellText + Environment.NewLine;
						i++;
					}
				}
				else
				{
					if (subtitleItem != null)
						subtitleItem.Text = subtitleItem.Text + currentCellText + Environment.NewLine;
					i++;
				}
			}

			if (subtitleItem != null)
				datasets.Add(subtitleItem);

			return datasets;
		}

		private IList<SubtitleItem> ConvertToSubtitleItemsV2(IList<Row> rows, WorkbookPart workbookPart,
			double framePerSecond)
		{
			var datasets = new List<SubtitleItem>();
			for (var i = 0; i < rows.Count; i++)
			{
				// Column이 'B'로 시작하는 셀만 가져옴.
				var row = rows[i];
				var columnA = row.Descendants<Cell>().FirstOrDefault(c => GetColumnName(c.CellReference).Equals("A"));
				var columnB = row.Descendants<Cell>().FirstOrDefault(c => GetColumnName(c.CellReference).Equals("B"));
				var columnC = row.Descendants<Cell>().FirstOrDefault(c => GetColumnName(c.CellReference).Equals("C"));

				if (columnA?.CellReference == null ||
				    columnB?.CellReference == null ||
				    columnC?.CellReference == null)
					continue;

				var valueA = ReadExcelCell(columnA, workbookPart);
				var valueB = ReadExcelCell(columnB, workbookPart);
				var valueC = ReadExcelCell(columnC, workbookPart);

				TryParseTimeSpan(valueA, out var startTime, framePerSecond);
				TryParseTimeSpan(valueB, out var endTime, framePerSecond);
				datasets.Add(new SubtitleItem(i + 1, startTime, endTime)
				{
					Text = valueC?.TrimEnd(Environment.NewLine.ToCharArray()) ?? string.Empty
				});
			}

			return datasets;
		}

		private bool IsExistNextCell(IList<Row> rows, int position, WorkbookPart workbookPart, out string nextCellText)
		{
			var cell = rows.Count > position + 1 ? GetCellV1(rows[position + 1]) : null;

			nextCellText = cell == null ? string.Empty : ReadExcelCell(cell, workbookPart);

			return nextCellText.IsNotNullOrAny();
		}

		private string TryConvertToStandardExpression(string value, double framePerSecond)
		{
			if (string.IsNullOrEmpty(value) || !value.Contains(";"))
				return value;

			var semicolonIndex = value.LastIndexOf(";", StringComparison.Ordinal);
			var substring = value.Substring(semicolonIndex + 1);
			var milliseconds = int.Parse(substring);
			var convertedMilliseconds = (int) (milliseconds * 1000 / framePerSecond);
			return $"{value.Substring(0, semicolonIndex)}.{convertedMilliseconds}";
		}

		private bool TryParseTimeSpans(string cellText, out TimeSpan startTime, out TimeSpan endTime,
			double framePerSecond)
		{
			if (cellText.Contains("-->"))
			{
				cellText = cellText.Replace(',', '.');
				var splitData = cellText.Split(new[] {"-->"},
					StringSplitOptions.RemoveEmptyEntries);

				if (framePerSecond <= 0)
					framePerSecond = 29.97d; // default

				var tempStartTime = TryConvertToStandardExpression(splitData[0].Trim(), framePerSecond);
				var tempEndTime = TryConvertToStandardExpression(splitData[1].Trim(), framePerSecond);

				var isStartTime = TimeSpan.TryParse(tempStartTime, out startTime);
				var isEndTime = TimeSpan.TryParse(tempEndTime, out endTime);

				return isStartTime && isEndTime;
			}

			startTime = TimeSpan.Zero;
			endTime = TimeSpan.Zero;
			return false;
		}

		private bool TryParseTimeSpan(string cellText, out TimeSpan time, double framePerSecond)
		{
			if (string.IsNullOrEmpty(cellText))
			{
				time = TimeSpan.MinValue;
				return false;
			}

			cellText = cellText.Replace(',', '.');
			if (framePerSecond <= 0)
				framePerSecond = 29.97d; // default

			var tempTime = TryConvertToStandardExpression(cellText.Trim(), framePerSecond);

			var isStartTime = TimeSpan.TryParse(tempTime, out time);

			return isStartTime;
		}

		/// <summary>
		///     주어진 row에서 B column Cell 정보만 가져옴.
		/// </summary>
		/// <param name="row"> 엑셀 시트의 한 줄</param>
		/// <returns></returns>
		private Cell GetCellV1(Row row)
		{
			return row.Descendants<Cell>().FirstOrDefault(i => GetColumnName(i.CellReference).Equals("B"));
		}

		private Cell GetCellV2(Row row)
		{
			return row.Descendants<Cell>().FirstOrDefault(i => GetColumnName(i.CellReference).Equals("B"));
		}

		public IEnumerable<string> CreateExcelFile(IEnumerable<ExcelInfo> excelInfos, string saveFolderPath,
			string saveFileName)
		{
			var errorMessages = new List<string>();

			if (!string.IsNullOrEmpty(saveFolderPath) && !Directory.Exists(saveFolderPath))
				Directory.CreateDirectory(saveFolderPath);

			try
			{
				using (var mySpreadsheet = SpreadsheetDocument.Create(saveFolderPath + "\\" + saveFileName,
					SpreadsheetDocumentType.Workbook))
				{
					var workbookPart = mySpreadsheet.AddWorkbookPart();
					var workbook = new Workbook();
					var sheets = new Sheets();

					workbook.AppendChild(sheets);
					workbookPart.Workbook = workbook;

					var newWorkbookStylesPart = mySpreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>();
					newWorkbookStylesPart.Stylesheet = GenerateStylesheet();
					newWorkbookStylesPart.Stylesheet.Save();

					foreach (var excelInfo in excelInfos.ToList())
						try
						{
							var splitedExcelFileName = excelInfo.FileName.Split('-');

							var excelLanguageCode = splitedExcelFileName[4].ToUpper();

							var languages = LanguageParser.GetLanguages().ToList();

							var sheetName = string.Empty;
							foreach (var language in languages)
								if (language.Alpha2.Equals(SetMatchedLanguage(excelLanguageCode)))
									sheetName = language.Alpha2 + "_" + excelLanguageCode + "_";

							sheetName += splitedExcelFileName[5].TrimEnd(".xlsx".ToCharArray());

							CreateSheet(mySpreadsheet, excelInfo.SheetInfos[0], sheetName);
						}
						catch (Exception ex)
						{
							_logger.Error.Write(ex.Message);
							errorMessages.Add(excelInfo.FileFullPath + " 파일 변환에 실패하였습니다.");
						}
				}
			}
			catch (IOException ex)
			{
				_logger.Error.Write(ex.Message);
				errorMessages.Add("파일 생성에 실패하였습니다, 엑셀 파일이 열려 있는지 확인해주세요.");
			}

			return errorMessages;
		}

		private SubtitleItem ParseCellsData(Queue<string> queue)
		{
			var number = -1;
			var startTime = TimeSpan.Zero;
			var endTime = TimeSpan.Zero;
			var text = string.Empty;
			var currentQueueCount = 0;

			while (queue.Count > 0)
			{
				var cellText = queue.Dequeue();
				currentQueueCount++;

				if (currentQueueCount == 1)
				{
					int.TryParse(cellText, out number);
					continue;
				}

				if (currentQueueCount == 2 && cellText.Contains("-->"))
				{
					cellText = cellText.Replace(',', '.');
					var splitData = cellText.Split(new[] {"-->"},
						StringSplitOptions.RemoveEmptyEntries);

					TimeSpan.TryParse(splitData[0], out startTime);
					TimeSpan.TryParse(splitData[1], out endTime);
					continue;
				}

				text += cellText + Environment.NewLine;
			}

			text = text.TrimEnd(Environment.NewLine.ToCharArray());

			return new SubtitleItem(number, startTime, endTime)
			{
				Text = text
			};
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

		private Stylesheet GenerateStylesheet()
		{
			var stylesheet = new Stylesheet();

			// 기본 셋팅
			var fonts = new Fonts(new Font(new FontSize {Val = 11}));
			var fills = new Fills(new Fill(new PatternFill {PatternType = PatternValues.None}));
			var borders = new Borders(new Border());

			// 셀 서식 추가 (텍스트로 지정.)
			var cellFormats = new CellFormats(
				new CellFormat(),
				new CellFormat
				{
					NumberFormatId = 49U, // style 번호 49번은 '@'로 텍스트를 나타냄.
					ApplyNumberFormat = true
				});
			stylesheet = new Stylesheet(fonts, fills, borders, cellFormats);

			return stylesheet;
		}

		private void CreateSheet(SpreadsheetDocument mySpreadsheet, ExcelSheetInfo sheetInfo, string sheetName)
		{
			var sheets = mySpreadsheet.WorkbookPart.Workbook.Sheets;
			var worksheetPart = mySpreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
			worksheetPart.Worksheet = new Worksheet();

			var sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

			// 메인 통합 문서 부분에 새 시트를 추가.
			var newSheet = new Sheet
			{
				Name = sheetName,
				Id = mySpreadsheet.WorkbookPart.GetIdOfPart(worksheetPart),
				SheetId = (uint) sheets.ChildElements.Count + 1
			};
			sheets.Append(newSheet);
			mySpreadsheet.WorkbookPart.Workbook.Save();

			var count = 1;
			foreach (var dataset in sheetInfo.Datasets)
			{
				var row = new Row();

				var startTime = dataset.StartTime.ToString();
				row.Append(CreateCell(SetExportTimeFormat(startTime), "A" + count, CellValues.String, 1));

				var endTime = dataset.EndTime.ToString();
				row.Append(CreateCell(SetExportTimeFormat(endTime), "B" + count, CellValues.String, 1));

				var text = dataset.Text;
				row.Append(CreateCell(text, "C" + count, CellValues.String, 1));

				sheetData.Append(row);
				count++;
			}

			worksheetPart.Worksheet.Save();
		}

		private string GetColumnName(string cellReference)
		{
			return new Regex("[A-Za-z]+").Match(cellReference).Value;
		}

		private static string SetMatchedLanguage(string atomyLanguageCode)
		{
			switch (atomyLanguageCode.ToUpper())
			{
				// 영어
				case "USA":
				case "ENG":
					return "en";
				// 일본어
				case "JPN":
					return "ja";
				// 중국어 번체 (대만)
				case "TWN":
					return "zh";
				// 중국어 간체 (중국)
				case "CHN":
					return "zh";
				// 캄보디아
				case "KHM":
					return "km";
				// 스페인
				case "ESP":
					return "es";
				case "KOR":
					return "ko";
				default:
					return string.Empty;
			}
		}

		private static string ReadExcelCell(Cell cell, WorkbookPart workbookPart)
		{
			var cellValue = cell.CellValue;
			var text = cellValue == null ? cell.InnerText : cellValue.Text;

			if (cell.DataType == null || cell.DataType != CellValues.SharedString || cell.CellValue == null)
				return text ?? string.Empty;

			var sharedStringTable = workbookPart.SharedStringTablePart.SharedStringTable;
			var elements = sharedStringTable.Elements<SharedStringItem>();
			text = elements.ElementAt(Convert.ToInt32(cell.CellValue.Text)).InnerText;

			return text ?? string.Empty;
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
	}
}