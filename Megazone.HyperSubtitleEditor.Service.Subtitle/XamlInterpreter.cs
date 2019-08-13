using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Megazone.Core.IoC;
using Megazone.HyperSubtitleEditor.Domain.Subtitle;
using Megazone.HyperSubtitleEditor.Domain.Subtitle.Model;

namespace Megazone.HyperSubtitleEditor.Service.Subtitle
{
    [Inject(Scope = LifetimeScope.Transient, Source = typeof(IXamlInterpreter))]
    internal class XamlInterpreter : IXamlInterpreter
    {
        private const string LINE_FEED_TAG = "<br/>";

        public string ConvertITextsToString(IList<IText> texts)
        {
            var xamlText = "<Section><Paragraph>";
            xamlText += ConvertToRunTag(texts, string.Empty);
            xamlText += "</Paragraph></Section>";
            return xamlText;
        }

        public IList<IText> ConvertToStringToITexts(string inputText)
        {
            var xmlRows = ConvertToXmlRow(inputText);

            return ConvertXmlRowToData(xmlRows);
        }

        private IList<IText> ConvertXmlRowToData(IList<XmlRow> xmlRows)
        {
            IList<IText> result = new List<IText>();

            var currentTagType = TagType.Unknown;
            IList<XmlRow> children = new List<XmlRow>();

            foreach (var row in xmlRows)
            {
                if (!row.Types.Any())
                {
                    if (children.Any())
                    {
                        result.Add(GetTagInstance(currentTagType, ConvertXmlRowToData(children)));
                        currentTagType = TagType.Unknown;
                        children = new List<XmlRow>();
                    }
                    result.Add(new Normal {Text = row.Text});
                    continue;
                }

                if (row.Types[0] == TagType.Bold)
                {
                    if (currentTagType == TagType.Unknown || currentTagType == TagType.Bold)
                    {
                        currentTagType = TagType.Bold;
                        row.Types.RemoveAt(0);
                        children.Add(row);
                    }

                    if (children.Any())
                    {
                        result.Add(GetTagInstance(currentTagType, ConvertXmlRowToData(children)));
                        currentTagType = TagType.Unknown;
                        children = new List<XmlRow>();
                    }
                }
                else if (row.Types[0] == TagType.Italic)
                {
                    if (currentTagType == TagType.Unknown || currentTagType == TagType.Italic)
                    {
                        currentTagType = TagType.Italic;
                        row.Types.RemoveAt(0);
                        children.Add(row);
                    }

                    if (children.Any())
                    {
                        result.Add(GetTagInstance(currentTagType, ConvertXmlRowToData(children)));
                        currentTagType = TagType.Unknown;
                        children = new List<XmlRow>();
                    }
                }
                else if (row.Types[0] == TagType.Underline)
                {
                    if (currentTagType == TagType.Unknown || currentTagType == TagType.Underline)
                    {
                        currentTagType = TagType.Underline;
                        row.Types.RemoveAt(0);
                        children.Add(row);
                    }

                    if (children.Any())
                    {
                        result.Add(GetTagInstance(currentTagType, ConvertXmlRowToData(children)));
                        currentTagType = TagType.Unknown;
                        children = new List<XmlRow>();
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///     재귀 메소드.
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        private IList<XmlRow> ConvertToXmlRow(string inputText)
        {
            var xamlReader = new XmlTextReader(new StringReader(inputText));
            IList<XmlRow> xmlRows = new List<XmlRow>();

            var beforeElementName = string.Empty;

            while (xamlReader.Read())
            {
                IList<TagType> tagTypes = new List<TagType>();

                if (beforeElementName.Equals(xamlReader.Name) && xamlReader.Name.Equals("Paragraph"))
                {
                    xmlRows.Add(new XmlRow(LINE_FEED_TAG, tagTypes));
                    continue;
                }

                beforeElementName = xamlReader.Name;

                if (!xamlReader.Name.Equals("Run"))
                    continue;

                while (xamlReader.MoveToNextAttribute())
                    if (xamlReader.Name.Equals("FontStyle") && xamlReader.Value.Equals("Italic"))
                        tagTypes.Add(TagType.Italic);
                    else if (xamlReader.Name.Equals("FontWeight") && xamlReader.Value.Equals("Bold"))
                        tagTypes.Add(TagType.Bold);
                    else if (xamlReader.Name.Equals("TextDecorations") && xamlReader.Value.Equals("Underline"))
                        tagTypes.Add(TagType.Underline);
                xmlRows.Add(new XmlRow(xamlReader.ReadString(), tagTypes));
            }
            return xmlRows;
        }

        /// <summary>
        ///     재귀 메소드.
        /// </summary>
        /// <param name="texts"></param>
        /// <param name="tagStyle"></param>
        /// <returns></returns>
        private string ConvertToRunTag(IList<IText> texts, string tagStyle)
        {
            var convertText = string.Empty;

            foreach (var text in texts)
            {
                if (text is INormal normal)
                {
                    var stringText = normal.Text;
                    stringText = stringText.Replace(LINE_FEED_TAG, "</Run></Paragraph><Paragraph><Run>");
                    convertText += "<Run" + tagStyle + ">" + stringText + "</Run>";
                }

                var tag = text as ITag;

                if (tag == null)
                    continue;

                var localStyle = tagStyle;

                if (tag is Italic && !localStyle.Contains(" FontStyle=\"Italic\""))
                    localStyle += " FontStyle=\"Italic\"";
                if (tag is Bold && !localStyle.Contains(" FontWeight=\"Bold\""))
                    localStyle += " FontWeight=\"Bold\"";
                if (tag is Underline && !localStyle.Contains(" TextDecorations=\"Underline\""))
                    localStyle += " TextDecorations=\"Underline\"";

                convertText += ConvertToRunTag(tag.Children, localStyle);
            }
            return convertText;
        }

        private IText GetTagInstance(TagType currentTagType, IList<IText> children)
        {
            switch (currentTagType)
            {
                case TagType.Bold:
                    return new Bold {Children = children};
                case TagType.Italic:
                    return new Italic {Children = children};
                case TagType.Underline:
                    return new Underline {Children = children};
                default:
                    return null;
            }
        }

        private enum TagType
        {
            Bold,
            Italic,
            Underline,
            Unknown
        }

        private class XmlRow
        {
            public XmlRow(string text, IList<TagType> types)
            {
                Text = text;
                Types = types;
            }

            public string Text { get; }
            public IList<TagType> Types { get; }
        }
    }
}