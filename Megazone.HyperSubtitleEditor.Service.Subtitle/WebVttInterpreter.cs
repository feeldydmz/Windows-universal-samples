using System.Collections.Generic;
using Megazone.Core.IoC;
using Megazone.HyperSubtitleEditor.Domain.Subtitle;
using Megazone.HyperSubtitleEditor.Domain.Subtitle.Model;

namespace Megazone.HyperSubtitleEditor.Service.Subtitle
{
    [Inject(Scope = LifetimeScope.Transient, Source = typeof(IWebVttInterpreter))]
    internal class WebVttInterpreter : IWebVttInterpreter
    {
        private const string START_BOLD_TAG = "<b>";
        private const string END_BOLD_TAG = "</b>";
        private const string START_ITALIC_TAG = "<i>";
        private const string END_ITALIC_TAG = "</i>";
        private const string START_UNDERLINE_TAG = "<u>";
        private const string END_UNDERLINE_TAG = "</u>";
        private const string LINE_FEED_TAG = "<br/>";

        public IList<IText> ConvertToStringToITexts(string inputText)
        {
            inputText = inputText.Replace("\r\n", LINE_FEED_TAG); // window 포맷 통일.
            inputText = inputText.Replace("\n", LINE_FEED_TAG); // window 포맷 통일.
            inputText = inputText.Replace("\r", LINE_FEED_TAG); // 오래된 Mac 포맷 통일.

            IList<TagSet> tagSets = new List<TagSet>
            {
                new TagSet(START_BOLD_TAG, END_BOLD_TAG),
                new TagSet(START_ITALIC_TAG, END_ITALIC_TAG),
                new TagSet(START_UNDERLINE_TAG, END_UNDERLINE_TAG)
            };

            IList<IText> result = new List<IText>();

            var charInputText = inputText.ToCharArray();

            var saveText = string.Empty;
            TagSet saveTagSet = null;

            for (var i = 0; i < charInputText.Length; i++)
            {
                if (charInputText[i] == '<')
                {
                    var checkText = inputText.Substring(i);

                    foreach (var tagSet in tagSets)
                        if (checkText.StartsWith(tagSet.StartTag))
                        {
                            if (tagSet.TagCount == 0 && saveTagSet == null)
                            {
                                saveTagSet = tagSet;
                                if (!string.IsNullOrEmpty(saveText))
                                {
                                    result.Add(new Normal {Text = saveText});
                                    saveText = string.Empty;
                                }
                            }
                            tagSet.TagCount = tagSet.TagCount + 1;
                        }
                }

                saveText += charInputText[i];

                if (charInputText[i] == '>')
                {
                    var checkText = saveText;

                    if (saveTagSet != null && checkText.EndsWith(saveTagSet.EndTag))
                    {
                        saveTagSet.TagCount = saveTagSet.TagCount - 1;
                        if (saveTagSet.TagCount == 0)
                        {
                            saveText = saveText.Substring(0, saveText.Length - saveTagSet.EndTag.Length);
                            saveText = saveText.Substring(saveTagSet.StartTag.Length);

                            result.Add(GetTagInstance(saveTagSet.StartTag, ConvertToStringToITexts(saveText)));
                            saveText = string.Empty;
                            saveTagSet = null;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(saveText))
                result.Add(new Normal {Text = saveText});

            return result;
        }

        public string ConvertITextsToInsideString(IList<IText> texts)
        {
            var result = string.Empty;
            if (texts == null)
                return string.Empty;
            foreach (var text in texts)
            {
	            if (text is INormal normal)
                    result += normal.Text;

                var tag = text as ITag;

                if (tag == null)
                    continue;

                var startTag = string.Empty;
                var endTag = string.Empty;

                if (tag is Italic)
                {
                    startTag = START_ITALIC_TAG;
                    endTag = END_ITALIC_TAG;
                }
                if (tag is Bold)
                {
                    startTag = START_BOLD_TAG;
                    endTag = END_BOLD_TAG;
                }
                if (tag is Underline)
                {
                    startTag = START_UNDERLINE_TAG;
                    endTag = END_UNDERLINE_TAG;
                }
                result += startTag;
                result += ConvertITextsToInsideString(tag.Children);
                result += endTag;
            }

            return result;
        }

        public string ConvertITextsToOutsideString(IList<IText> texts)
        {
            return ConvertITextsToInsideString(texts).Replace(LINE_FEED_TAG, "\n");
        }

        private IText GetTagInstance(string startTag, IList<IText> children)
        {
            switch (startTag)
            {
                case START_BOLD_TAG:
                    return new Bold {Children = children};
                case START_ITALIC_TAG:
                    return new Italic {Children = children};
                case START_UNDERLINE_TAG:
                    return new Underline {Children = children};
                default:
                    return null;
            }
        }

        private class TagSet
        {
            public TagSet(string startTag, string endTag)
            {
                StartTag = startTag;
                EndTag = endTag;
            }

            public string StartTag { get; }

            public string EndTag { get; }

            public int TagCount { get; set; }
        }
    }
}