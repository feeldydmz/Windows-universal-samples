using System;
using System.Collections.Generic;
using Megazone.Core.VideoTrack;
using Megazone.HyperSubtitleEditor.Presentation.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Megazone.HyperSubtitleEditor.Test
{
    [TestClass]
    public class UnitTest
    {
        private const string EXCEL_FILE_PATH = @"C:\Users\kimhs\Desktop\Atomy_Subtitle.xlsx";
        private const string SAVE_EXCEL_FILE_PATH = @"C:\Users\kimhs\Desktop\Save_Atomy_Subtitle.xlsx";
        private static readonly string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public IList<Subtitle> subtitles { get; } = new List<Subtitle>();

        [TestMethod]
        public void TestMethod1()
        {
            //var subtitle = GetSubtitle(EXCEL_FILE_PATH, "한국");

            //CreateFile(subtitle, SAVE_EXCEL_FILE_PATH);

            // ConvertTag();

            // CreateJson();
        }

        //[TestMethod]
        //public void CreateFile(Subtitle subtitle, string saveFilePath)
        //{
        //    using (var document = SpreadsheetDocument.Create(SAVE_EXCEL_FILE_PATH, SpreadsheetDocumentType.Workbook))
        //    {
        //        var workbookPart = document.AddWorkbookPart();
        //        workbookPart.Workbook = new Workbook();

        //        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        //        worksheetPart.Worksheet = new Worksheet();

        //        var sheets = workbookPart.Workbook.AppendChild(new Sheets());

        //        var sheet = new Sheet
        //        {
        //            Id = workbookPart.GetIdOfPart(worksheetPart),
        //            SheetId = 1,
        //            Name = subtitle.LanguageCode
        //        };
        //        sheets.Append(sheet);

        //        workbookPart.Workbook.Save();

        //        var subtitleItems = subtitle.Datasets;
        //        var sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

        //        // 한줄씩 추가.
        //        var row = new Row();
        //        row.Append(
        //            ConstructCell("국가 코드", CellValues.String),
        //            ConstructCell(subtitle.LanguageCode, CellValues.String));
        //        sheetData.AppendChild(row);

        //        row = new Row();
        //        row.Append(
        //            ConstructCell("파일 형식", CellValues.String),
        //            ConstructCell(subtitle.Format.ToString(), CellValues.String));
        //        sheetData.AppendChild(row);

        //        row = new Row();
        //        sheetData.AppendChild(row);

        //        row = new Row();
        //        row.Append(
        //            ConstructCell("No", CellValues.String),
        //            ConstructCell("Start Time", CellValues.String),
        //            ConstructCell("End Time", CellValues.String),
        //            ConstructCell("Text", CellValues.String));
        //        sheetData.AppendChild(row);

        //        foreach (var item in subtitleItems)
        //        {
        //            row = new Row();
        //            row.Append(
        //                ConstructCell(item.Number, CellValues.Number),
        //                ConstructCell(item.StartTime.ToString().Substring(0, 12), CellValues.String),
        //                ConstructCell(item.EndTime.ToString().Substring(0, 12), CellValues.String),
        //                ConstructCell(new WebVttInterpreter().ConvertITextsToString(item.Texts), CellValues.String));

        //            sheetData.AppendChild(row);
        //        }
        //        worksheetPart.Worksheet.Save();
        //    }
        //}

        //private Cell ConstructCell(string value, CellValues dataType)
        //{
        //    return new Cell
        //    {
        //        CellValue = new CellValue(value),
        //        DataType = new EnumValue<CellValues>(dataType)
        //    };
        //}

        //private static void InsertValuesInWorksheet(WorksheetPart worksheetPart, IEnumerable<string> values)
        //{
        //    var worksheet = worksheetPart.Worksheet;
        //    var sheetData = worksheet.GetFirstChild<SheetData>();
        //    var row = new Row { RowIndex = 1 }; // add a row at the top of spreadsheet
        //    sheetData.Append(row);

        //    var i = 0;
        //    foreach (var value in values)
        //    {
        //        var cell = new Cell
        //        {
        //            CellValue = new CellValue(value),
        //            DataType = new EnumValue<CellValues>(CellValues.String)
        //        };

        //        row.InsertAt(cell, i);
        //        i++;
        //    }
        //}

        //[TestMethod]
        //public Subtitle GetSubtitle(string excelPath, string sheetName)
        //{
        //    var subtitle = new Subtitle();
        //    try
        //    {
        //        using (
        //            var fileStream = new FileStream(EXCEL_FILE_PATH, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        //        )
        //        {
        //            using (var spreadsheetDocument = SpreadsheetDocument.Open(fileStream, false))
        //            {
        //                var workbookPart = spreadsheetDocument.WorkbookPart;
        //                var sheets = workbookPart.Workbook.Descendants<Sheet>();
        //                Sheet sheet = null;

        //                foreach (var selectedSheet in sheets.ToList())
        //                    if (selectedSheet.Name.Value.Equals(sheetName))
        //                        sheet = selectedSheet;

        //                var workSheet = ((WorksheetPart)workbookPart.GetPartById(sheet?.Id)).Worksheet;
        //                var sheetData = workSheet.Elements<SheetData>().First();
        //                var rows = sheetData.Elements<Row>().ToList();

        //                for (var i = 0; i < rows.Count; i++)
        //                {
        //                    if (i == 0 || i == 1)
        //                    {
        //                        var beforeText = string.Empty;
        //                        var cellEnumerator = GetExcelCellEnumerator(rows[i]);

        //                        while (cellEnumerator.MoveNext())
        //                        {
        //                            var cell = cellEnumerator.Current;
        //                            var text = ReadExcelCell(cell, workbookPart).Trim();

        //                            if (string.IsNullOrEmpty(beforeText) && text.Equals("국가 코드") || text.Equals("파일 형식"))
        //                            {
        //                                beforeText = text;
        //                                continue;
        //                            }

        //                            if (beforeText.Equals("국가 코드"))
        //                                subtitle.LanguageCode = text;
        //                            if (beforeText.Equals("파일 형식") && text.ToUpper().Equals("WEBVTT"))
        //                                subtitle.Format = SubtitleFormat.WebVtt;
        //                        }
        //                    }

        //                    if (i > 2)
        //                    {
        //                        var row = rows[i];
        //                        var cellEnumerator = GetExcelCellEnumerator(row);
        //                        var columCount = 0;

        //                        var subtitleItem = new SubtitleItem();

        //                        while (cellEnumerator.MoveNext())
        //                        {
        //                            var cell = cellEnumerator.Current;
        //                            var text = ReadExcelCell(cell, workbookPart).Trim();

        //                            if (columCount == 0 && string.IsNullOrEmpty(text))
        //                                break;

        //                            try
        //                            {
        //                                switch (columCount)
        //                                {
        //                                    case 0:
        //                                        subtitleItem.Number = text;
        //                                        break;
        //                                    case 1:
        //                                        subtitleItem.StartTime = TimeSpan.Parse(text);
        //                                        break;
        //                                    case 2:
        //                                        subtitleItem.EndTime = TimeSpan.Parse(text);
        //                                        break;
        //                                    case 3:
        //                                        subtitleItem.Texts = new WebVttInterpreter().ConvertToStringToITexts(text);
        //                                        break;
        //                                }
        //                            }
        //                            catch (FormatException)
        //                            {
        //                            }
        //                            finally
        //                            {
        //                                columCount++;
        //                            }
        //                        }

        //                        if (string.IsNullOrEmpty(subtitleItem.Number))
        //                            continue;

        //                        subtitle.Datasets.Add(subtitleItem);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        // ignored
        //    }
        //    return subtitle;
        //}

        //[TestMethod]
        //private string ReadExcelCell(Cell cell, WorkbookPart workbookPart)
        //{
        //    var cellValue = cell.CellValue;
        //    var text = cellValue == null ? cell.InnerText : cellValue.Text;
        //    if (cell.DataType != null && cell.DataType == CellValues.SharedString)
        //        text = workbookPart.SharedStringTablePart.SharedStringTable
        //            .Elements<SharedStringItem>().ElementAt(
        //                Convert.ToInt32(cell.CellValue.Text)).InnerText;

        //    return (text ?? string.Empty).Trim();
        //}

        //[TestMethod]
        //private IEnumerator<Cell> GetExcelCellEnumerator(Row row)
        //{
        //    var currentCount = 0;
        //    foreach (var cell in row.Descendants<Cell>())
        //    {
        //        var columnName = GetColumnName(cell.CellReference);
        //        var currentColumnIndex = ConvertColumnNameToNumber(columnName);

        //        for (; currentCount < currentColumnIndex; currentCount++)
        //        {
        //            var emptycell = new Cell
        //            {
        //                DataType = null,
        //                CellValue = new CellValue(string.Empty)
        //            };
        //            yield return emptycell;
        //        }

        //        yield return cell;
        //        currentCount++;
        //    }
        //}

        //[TestMethod]
        //private int ConvertColumnNameToNumber(string columnName)
        //{
        //    var alpha = new Regex("^[A-Z]+$");
        //    if (!alpha.IsMatch(columnName)) throw new ArgumentException();

        //    var colLetters = columnName.ToCharArray();
        //    Array.Reverse(colLetters);

        //    var convertedValue = 0;
        //    for (var i = 0; i < colLetters.Length; i++)
        //    {
        //        var letter = colLetters[i];
        //        // ASCII 'A' = 65
        //        var current = i == 0 ? letter - 65 : letter - 64;
        //        convertedValue += current * (int)Math.Pow(26, i);
        //    }

        //    return convertedValue;
        //}

        //[TestMethod]
        //private static string GetColumnName(string cellReference)
        //{
        //    return new Regex("[A-Za-z]+").Match(cellReference).Value;
        //}

        //// Service
        //[TestMethod]
        //private void CreateWebVtt(Subtitle subtitle, string saveFullPath)
        //{
        //    if (subtitle == null || subtitle.Format != SubtitleFormat.WebVtt)
        //        return;

        //    IList<string> rows = new List<string>();

        //    rows.Add(subtitle.Format.ToString().ToUpper());

        //    foreach (var item in subtitle.Datasets)
        //    {
        //        //rows.Add("");
        //        //rows.Add(item.StartTime.ToString().Substring(0, 12) + " --> " + item.EndTime.ToString().Substring(0, 12));
        //        //if (item.Texts.Contains("\n"))
        //        //{
        //        //    var splited = item.Texts.Split('\n');

        //        //    foreach (var splitedItem in splited)
        //        //        rows.Add(splitedItem);
        //        //}
        //        //else
        //        //{
        //        //    rows.Add(item.Texts);
        //        //}
        //    }

        //    try
        //    {
        //        var directoryPath = Path.GetDirectoryName(saveFullPath);

        //        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
        //            Directory.CreateDirectory(directoryPath);

        //        File.WriteAllLines(saveFullPath, rows, Encoding.UTF8);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        //[TestMethod]
        //private void ConvertTag()
        //{
        //    var inputText = "<b>가나다</b>라마바사<u>아자차카</u>파하";

        //    var webVttInterpreter = new WebVttInterpreter();
        //    var xamlInterpreter = new XamlInterpreter();

        //   IList<IText> webVttToList = webVttInterpreter.ConvertToStringToITexts(inputText);

        //    var webVttString = webVttInterpreter.ConvertITextsToString(webVttToList);

        //    var xamlString = xamlInterpreter.ConvertITextsToString(webVttToList);

        //    IList<IText> xamlToList = xamlInterpreter.ConvertToStringToITexts(xamlString);
        //}

        // Domain
        //public interface IText
        //{

        //}

        //public interface ITag : IText
        //{
        //    IList<IText> Children { get; set; }
        //}

        //public interface INormal : IText
        //{
        //    string Text { get; set; }
        //}

        //public class Bold : ITag
        //{
        //    public IList<IText> Children { get; set; }
        //}

        //public class Italic : ITag
        //{
        //    public IList<IText> Children { get; set; }
        //}

        //public class Underline : ITag
        //{
        //    public IList<IText> Children { get; set; }
        //}

        //public class Normal : INormal
        //{
        //    public string Text { get; set; }
        //}


        public interface IXamlInterpreter
        {
            string ConvertITextsToString(IList<IText> texts);

            IList<IText> ConvertToStringToITexts(string inputText);
        }

        //        Bold,
        //    {
        //    private enum TagType
        //{
        //internal class XamlInterpreter : IXamlInterpreter

        // Service
        //        Italic,
        //        Underline,
        //        Unknown
        //    }

        //    private class XmlRow
        //    {
        //        public string Text { get; }
        //        public IList<TagType> Types { get; }

        //        public XmlRow(string text, IList<TagType> types)
        //        {
        //            Text = text;
        //            Types = types;
        //        }
        //    }

        //    public string ConvertITextsToString(IList<IText> texts)
        //    {
        //        var xamlText = "<Section><Paragraph>";
        //        xamlText += ConvertToRunTag(texts, string.Empty);
        //        xamlText += "</Paragraph></Section>";
        //        return xamlText;
        //    }

        //    public IList<IText> ConvertToStringToITexts(string inputText)
        //    {
        //        IList<XmlRow> xmlRows = ConvertToXmlRow(inputText);

        //        return ConvertXmlRowToData(xmlRows);
        //    }

        //    private IList<IText> ConvertXmlRowToData(IList<XmlRow> xmlRows)
        //    {
        //        IList<IText> result = new List<IText>();

        //        TagType currentTagType = TagType.Unknown;
        //        IList<XmlRow> children = new List<XmlRow>();

        //        foreach (var row in xmlRows)
        //        {
        //            if (!row.Types.Any())
        //            {
        //                if (children.Any())
        //                {
        //                    result.Add(GetTagInstance(currentTagType, ConvertXmlRowToData(children)));
        //                    currentTagType = TagType.Unknown;
        //                    children = new List<XmlRow>();
        //                }
        //                result.Add(new Normal { Text = row.Text });
        //                continue;
        //            }

        //            if (row.Types[0] == TagType.Bold)
        //            {
        //                if (currentTagType == TagType.Unknown || currentTagType == TagType.Bold)
        //                {
        //                    currentTagType = TagType.Bold;
        //                    row.Types.RemoveAt(0);
        //                    children.Add(row);
        //                    continue;
        //                }

        //                if (children.Any())
        //                {
        //                    result.Add(GetTagInstance(currentTagType, ConvertXmlRowToData(children)));
        //                    currentTagType = TagType.Unknown;
        //                    children = new List<XmlRow>();
        //                }
        //            }
        //            else if (row.Types[0] == TagType.Italic)
        //            {
        //                if (currentTagType == TagType.Unknown || currentTagType == TagType.Italic)
        //                {
        //                    currentTagType = TagType.Italic;
        //                    row.Types.RemoveAt(0);
        //                    children.Add(row);
        //                    continue;
        //                }

        //                if (children.Any())
        //                {
        //                    result.Add(GetTagInstance(currentTagType, ConvertXmlRowToData(children)));
        //                    currentTagType = TagType.Unknown;
        //                    children = new List<XmlRow>();
        //                }
        //            }
        //            else if (row.Types[0] == TagType.Underline)
        //            {
        //                if (currentTagType == TagType.Unknown || currentTagType == TagType.Underline)
        //                {
        //                    currentTagType = TagType.Underline;
        //                    row.Types.RemoveAt(0);
        //                    children.Add(row);
        //                    continue;
        //                }

        //                if (children.Any())
        //                {
        //                    result.Add(GetTagInstance(currentTagType, ConvertXmlRowToData(children)));
        //                    currentTagType = TagType.Unknown;
        //                    children = new List<XmlRow>();
        //                }
        //            }
        //        }
        //        return result;
        //    }

        //    private IList<XmlRow> ConvertToXmlRow(string inputText)
        //    {
        //        var xamlReader = new XmlTextReader(new StringReader(inputText));
        //        IList<XmlRow> xmlRows = new List<XmlRow>();

        //        while (xamlReader.Read())
        //        {
        //            if (!xamlReader.Name.Equals("Run"))
        //                continue;

        //            IList<TagType> tagTypes = new List<TagType>();

        //            while (xamlReader.MoveToNextAttribute())
        //            {
        //                if (xamlReader.Name.Equals("FontStyle") && xamlReader.Value.Equals("Italic"))
        //                {
        //                    tagTypes.Add(TagType.Italic);
        //                }
        //                else if (xamlReader.Name.Equals("FontWeight") && xamlReader.Value.Equals("Bold"))
        //                {
        //                    tagTypes.Add(TagType.Bold);
        //                }
        //                else if (xamlReader.Name.Equals("TextDecorations") && xamlReader.Value.Equals("Underline"))
        //                {
        //                    tagTypes.Add(TagType.Underline);
        //                }
        //            }
        //            xmlRows.Add(new XmlRow(xamlReader.ReadString(), tagTypes));
        //        }
        //        return xmlRows;
        //    }

        //    private string ConvertToRunTag(IList<IText> texts, string tagStyle)
        //    {
        //        var convertText = string.Empty;

        //        foreach (var text in texts)
        //        {
        //            if (text is INormal)
        //            {
        //                convertText += "<Run" + tagStyle + ">" + ((INormal)text).Text + "</Run>";
        //            }

        //            var tag = text as ITag;

        //            if (tag == null)
        //                continue;

        //            var localStyle = tagStyle;

        //            if (tag is Italic && !localStyle.Contains(" FontStyle=\"Italic\""))
        //            {
        //                localStyle += " FontStyle=\"Italic\"";
        //            }
        //            if (tag is Bold && !localStyle.Contains(" FontWeight=\"Bold\""))
        //            {
        //                localStyle += " FontWeight=\"Bold\"";
        //            }
        //            if (tag is Underline && !localStyle.Contains(" TextDecorations=\"Underline\""))
        //            {
        //                localStyle += " TextDecorations=\"Underline\"";
        //            }

        //            convertText += ConvertToRunTag(tag.Children, localStyle);
        //        }
        //        return convertText;
        //    }

        //    private IText GetTagInstance(TagType currentTagType, IList<IText> children)
        //    {
        //        switch (currentTagType)
        //        {
        //            case TagType.Bold:
        //                return new Bold { Children = children };
        //            case TagType.Italic:
        //                return new Italic { Children = children };
        //            case TagType.Underline:
        //                return new Underline { Children = children };
        //            default:
        //                return null;
        //        }
        //    }
        //}

        //public interface IWebVttInterpreter
        //{
        //    string ConvertITextsToString(IList<IText> texts);

        //    IList<IText> ConvertToStringToITexts(string inputText);
        //}

        //public class WebVttInterpreter : IWebVttInterpreter
        //{
        //    private const string START_BOLD_TAG = "<b>";
        //    private const string END_BOLD_TAG = "<b>";
        //    private const string START_ITALIC_TAG = "<i>";
        //    private const string END_ITALIC_TAG = "</i>";
        //    private const string START_UNDERLINE_TAG = "<u>";
        //    private const string END_UNDERLINE_TAG = "</u>";

        //    private class TagSet
        //    {
        //        public string StartTag { get; }

        //        public string EndTag { get; }

        //        public int TagCount { get; set; }

        //        public TagSet(string startTag, string endTag)
        //        {
        //            StartTag = startTag;
        //            EndTag = endTag;
        //        }
        //    }

        //    public IList<IText> ConvertToStringToITexts(string inputText)
        //    {
        //        IList<TagSet> tagSets = new List<TagSet>
        //        {
        //            new TagSet(START_BOLD_TAG, END_BOLD_TAG),
        //            new TagSet(START_ITALIC_TAG, END_ITALIC_TAG),
        //            new TagSet(START_UNDERLINE_TAG, END_UNDERLINE_TAG)
        //        };

        //        IList<IText> result = new List<IText>();

        //        var charInputText = inputText.ToCharArray();

        //        var saveText = string.Empty;
        //        TagSet saveTagSet = null;

        //        for (var i = 0; i < charInputText.Length; i++)
        //        {
        //            if (charInputText[i] == '<')
        //            {
        //                var checkText = inputText.Substring(i);

        //                foreach (var tagSet in tagSets)
        //                {
        //                    if (checkText.StartsWith(tagSet.StartTag))
        //                    {
        //                        if (tagSet.TagCount == 0 && saveTagSet == null)
        //                        {
        //                            saveTagSet = tagSet;
        //                            if (!string.IsNullOrEmpty(saveText))
        //                            {
        //                                result.Add(new Normal { Text = saveText });
        //                                saveText = string.Empty;
        //                            }
        //                        }
        //                        tagSet.TagCount = tagSet.TagCount + 1;
        //                    }
        //                }
        //            }

        //            saveText += charInputText[i];

        //            if (charInputText[i] == '>')
        //            {
        //                var checkText = saveText;

        //                if (saveTagSet != null && checkText.EndsWith(saveTagSet.EndTag))
        //                {
        //                    saveTagSet.TagCount = saveTagSet.TagCount - 1;
        //                    if (saveTagSet.TagCount == 0)
        //                    {
        //                        saveText = saveText.Substring(0, saveText.Length - saveTagSet.EndTag.Length);
        //                        saveText = saveText.Substring(saveTagSet.StartTag.Length);

        //                        result.Add(GetTagInstance(saveTagSet.StartTag, ConvertToStringToITexts(saveText)));
        //                        saveText = string.Empty;
        //                        saveTagSet = null;
        //                    }
        //                }
        //            }

        //        }
        //        if(!string.IsNullOrEmpty(saveText))
        //           result.Add(new Normal { Text = saveText });

        //        return result;
        //    }

        //    public string ConvertITextsToString(IList<IText> texts)
        //    {
        //        var result = string.Empty;

        //        foreach (var text in texts)
        //        {
        //            var normal = text as INormal;
        //            if (normal != null)
        //            {
        //                result += normal.Text;
        //            }

        //            var tag = text as ITag;

        //            if (tag == null)
        //                continue;

        //            var startTag = string.Empty;
        //            var endTag = string.Empty;

        //            if (tag is Italic)
        //            {
        //                startTag = START_ITALIC_TAG;
        //                endTag = END_ITALIC_TAG;
        //            }
        //            if (tag is Bold)
        //            {
        //                startTag = START_BOLD_TAG;
        //                endTag = END_BOLD_TAG;
        //            }
        //            if (tag is Underline)
        //            {
        //                startTag = START_UNDERLINE_TAG;
        //                endTag = END_UNDERLINE_TAG;
        //            }
        //            result += startTag;
        //            result += ConvertITextsToString(tag.Children);
        //            result += endTag;
        //        }

        //        return result;
        //    }

        //    private IText GetTagInstance(string startTag, IList<IText> children)
        //    {
        //        switch (startTag)
        //        {
        //            case START_BOLD_TAG:
        //                return new Bold { Children = children };
        //            case START_ITALIC_TAG:
        //                return new Italic { Children = children };
        //            case START_UNDERLINE_TAG:
        //                return new Underline { Children = children };
        //            default:
        //                return null;
        //        }
        //    }
        //}
    }
}