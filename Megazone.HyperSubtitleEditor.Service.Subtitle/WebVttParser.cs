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
            text = text.Replace("\r\n", "\n"); // window 포맷 통일.
            text = text.Replace("\r", "\n"); // 오래된 Mac 포맷 통일.
            text = text.Trim();
            text = Regex.Replace(text, @"<v(.*? )(.*?)>", "$2: "); // voice tags '이름 : '으로 변경.
            text = Regex.Replace(text, @"<[^>]*>", ""); // anotations 전부 제거
            var splited = text.Split(_newLineArray, StringSplitOptions.None).ToList();
            splited.Add(""); // 맨 마지막 라인을 Add 하기 위해 빈 공간을 하나 추가.
            if (!splited[0].StartsWith("WEBVTT"))
                return null;
            var analysisStatus = SubtitleAnalysisStatus.Initialize; // 현재 상태
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
            foreach (var line in splited) // 한줄씩 분석
                switch (analysisStatus)
                {
                    case SubtitleAnalysisStatus.Initialize:
                        var linetrim = line.TrimEnd();
                        if (linetrim.Equals("") || linetrim.Equals("WEBVTT"))
                            continue;

                        if (subTitleLocal.Count == 0 && linetrim.Equals("STYLE") || linetrim.StartsWith("STYLE "))
                        {
                            analysisStatus = SubtitleAnalysisStatus.Style; // Style은 현재 무시하고 넘김.
                            break;
                        }
                        if (linetrim.Equals("NOTE") || linetrim.StartsWith("NOTE ") || linetrim.Equals("REGION"))
                        {
                            analysisStatus = SubtitleAnalysisStatus.Comment; // Note와 Region은 현재 무시하고 넘김.
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
                        if (line.Equals("")) // 한 세트(시간, 자막)이 끝나고 빈줄이 나올 때
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