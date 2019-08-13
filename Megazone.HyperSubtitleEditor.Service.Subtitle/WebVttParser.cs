using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Megazone.HyperSubtitleEditor.Domain.Subtitle;
using Megazone.HyperSubtitleEditor.Domain.Subtitle.Model;

namespace Megazone.HyperSubtitleEditor.Service.Subtitle
{
    internal class WebVttParser : ISubtitleParser
    {
        private static readonly string[] _newLineArray = {"\n"};
        private static readonly string[] _spaceArray = {" "};
        private readonly IWebVttInterpreter _webVttInterpreter;

        public WebVttParser()
        {
            _webVttInterpreter = new WebVttInterpreter();
        }

        public IList<ISubtitleItem> Parse(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            var subTitleLocal = new List<ISubtitleItem>();
            text = text.Replace("\r\n", "\n"); // window ���� ����.
            text = text.Replace("\r", "\n"); // ������ Mac ���� ����.
            text = text.Trim();
            text = Regex.Replace(text, @"<v(.*? )(.*?)>", "$2: "); // voice tags '�̸� : '���� ����.
            text = Regex.Replace(text, @"<[^>]*>", ""); // anotations ���� ����
            var splited = text.Split(_newLineArray, StringSplitOptions.None).ToList();
            splited.Add(""); // �� ������ ������ Add �ϱ� ���� �� ������ �ϳ� �߰�.
            if (!splited[0].StartsWith("WEBVTT"))
                return null;
            var analysisStatus = SubtitleAnalysisStatus.Initialize; // ���� ����
            var beginTime = new TimeSpan();
            var endTime = new TimeSpan();
            var textContent = string.Empty;
            var cueSettingData = string.Empty;
            var timeRegs = new List<TimeRegInfo>
            {
                new TimeRegInfo(new Regex(@"^((\d\d:\d\d:\d\d.\d\d\d)-->(\d\d:\d\d:\d\d.\d\d\d))"),
                    @"hh\:mm\:ss\.fff"),
                new TimeRegInfo(new Regex(@"^((\d\d:\d\d.\d\d\d)-->(\d\d:\d\d.\d\d\d))"), @"mm\:ss\.fff")
            };
            foreach (var line in splited) // ���پ� �м�
                switch (analysisStatus)
                {
                    case SubtitleAnalysisStatus.Initialize:
                        var linetrim = line.TrimEnd();
                        if (linetrim.Equals("") || linetrim.Equals("WEBVTT"))
                            continue;

                        if (subTitleLocal.Count == 0 && linetrim.Equals("STYLE") || linetrim.StartsWith("STYLE "))
                        {
                            analysisStatus = SubtitleAnalysisStatus.Style; // Style�� ���� �����ϰ� �ѱ�.
                            break;
                        }
                        if (linetrim.Equals("NOTE") || linetrim.StartsWith("NOTE ") || linetrim.Equals("REGION"))
                        {
                            analysisStatus = SubtitleAnalysisStatus.Comment; // Note�� Region�� ���� �����ϰ� �ѱ�.
                            break;
                        }
                        var timeMatchFound = false;
                        var whitespaceRemovedLine = linetrim.Replace(" ", string.Empty);
                        foreach (var timeReg in timeRegs)
                        {
                            var tempMatch = timeReg.Expression.Match(whitespaceRemovedLine);
                            if (tempMatch.Success)
                            {
                                timeMatchFound = true;
                                var value = tempMatch.Value;
                                if (whitespaceRemovedLine.Length > value.Length)
                                {
                                    var endValue = value.Substring(value.IndexOf("-->", StringComparison.Ordinal) + 3);
                                    cueSettingData =
                                        linetrim.Substring(linetrim.IndexOf(endValue, StringComparison.Ordinal) +
                                                           endValue.Length).Trim();
                                    // ex) position:20% line:20% vertical:rl align:0 size:60%
                                }
                                if (
                                    !TimeSpan.TryParseExact(tempMatch.Groups[2].Value, timeReg.Format,
                                        CultureInfo.CurrentCulture, out beginTime))
                                    beginTime = TimeSpan.Zero;
                                var endTimeSplit = tempMatch.Groups[3].Value.Split(_spaceArray, StringSplitOptions.None);
                                if (
                                    !TimeSpan.TryParseExact(endTimeSplit[0], timeReg.Format,
                                        CultureInfo.CurrentCulture, out endTime))
                                    endTime = TimeSpan.Zero;
                                break;
                            }
                        }
                        if (timeMatchFound)
                            analysisStatus = SubtitleAnalysisStatus.Iterating;
                        break;
                    case SubtitleAnalysisStatus.Iterating:
                        if (line.Equals("")) // �� ��Ʈ(�ð�, �ڸ�)�� ������ ������ ���� ��
                        {
                            subTitleLocal.Add(new SubtitleItem(beginTime, endTime,
                                _webVttInterpreter.ConvertToStringToITexts(textContent), cueSettingData));
                            textContent = "";
                            analysisStatus = SubtitleAnalysisStatus.Initialize;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(textContent))
                                textContent += line;
                            else
                                textContent += $"\r\n{line}";
                        }
                        break;
                    case SubtitleAnalysisStatus.Style:
                    case SubtitleAnalysisStatus.Comment:
                        var linetrimc = line.TrimEnd();
                        if (linetrimc.Equals("")) analysisStatus = SubtitleAnalysisStatus.Initialize;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            return subTitleLocal;
        }

        public string ToText(IEnumerable<string> subtitles)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("WEBVTT FILE");
            stringBuilder.AppendLine();
            if (subtitles != null)
            {
                var subtitlesList = subtitles.ToList();
                var itemsCount = subtitlesList.Count;
                for (var i = 0; i < itemsCount; i++)
                {
                    var item = subtitlesList[i].Replace("<br/>", "\n");
                    stringBuilder.AppendLine(item);
                    if (i + 1 < itemsCount)
                        stringBuilder.AppendLine();
                }
            }
            return stringBuilder.ToString();
        }

        private class TimeRegInfo
        {
            public TimeRegInfo(Regex expression, string format)
            {
                Expression = expression;
                Format = format;
            }

            public Regex Expression { get; }

            public string Format { get; }
        }

        private enum SubtitleAnalysisStatus
        {
            Initialize,
            Iterating,
            Comment,
            Style
        }
    }
}