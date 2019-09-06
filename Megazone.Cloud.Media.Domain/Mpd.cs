using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Megazone.Cloud.Media.Domain
{
    public class MpdParser
    {
        
    }


    [XmlRoot("MPD", Namespace = "urn:mpeg:dash:schema:mpd:2011")]
    public class Mpd
    {
        public static TimeSpan ToTimeSpan(string iso8601Time)
        {
            return XmlConvert.ToTimeSpan(iso8601Time); ;
        }

        [XmlAttribute("type")]
        public string Type { get; set; }

        //public TimeSpan MinBufferTime { get; set; }

        //private string _minBufferTimeNode;
        [XmlAttribute("minBufferTime")]
        public string MinBufferTime { get; set; }

        [XmlAttribute("profiles")]
        public string profiles { get; set; }

        [XmlAttribute("mediaPresentationDuration")]
        public string MediaPresentationDuration { get; set; }

        //[XmlArray("Period")]
        //[XmlArrayItem("Period")]
        [XmlElement]
        public List<PeriodNode> Period { get; set; }

        public struct PeriodNode
        {

            [XmlAttribute("start")]
            public string Start { get; set; }

            [XmlAttribute("duration")]
            public string Duration { get; set; }

            [XmlAttribute("id")]
            public string Id { get; set; }


            [XmlElement]
            public List<AdaptationSetNode> AdaptationSet { get; set; }


            public struct AdaptationSetNode
            {
                [XmlAttribute("mimeType")]
                public string MimeType { get; set; }

                [XmlAttribute("frameRate")]
                public string FrameRate { get; set; }

                [XmlAttribute("segmentAlignment")]
                public bool SegmentAlignment { get; set; }

                [XmlAttribute("subsegmentAlignment")]
                public bool SubsegmentAlignment { get; set; }

                [XmlAttribute("startWithSAP")]
                public int StartWithSap { get; set; }

                [XmlAttribute("subsegmentStartsWithSAP")]
                public int SubsegmentStartsWithSap { get; set; }

                [XmlAttribute("bitstreamSwitching")]
                public bool BitstreamSwitching { get; set; }




                [XmlElement("ContentProtection")]
                public List<ContentProtectionNode> ContentProtections { get; set; }

                [XmlElement("Representation")]
                public List<RepresentationNode> Representation { get; set; }

                public struct RepresentationNode
                {
                    [XmlAttribute("id")]
                    public string Id { get; set; }

                    [XmlAttribute("width")]
                    public int Width { get; set; }

                    [XmlAttribute("height")]
                    public int Height { get; set; }

                    [XmlAttribute("bandwidth")]
                    public int Bandwidth { get; set; }

                    [XmlAttribute("codecs")]
                    public string Codecs { get; set; }


                    [XmlElement("BaseURL")]
                    public string BaseUrl { get; set; }
                }

                public struct ContentProtectionNode
                {
                    [XmlAttribute("default_KID", Namespace = "urn:mpeg:cenc:2013")]
                    public string DefaultKid { get; set; }

                    [XmlAttribute("schemeIdUri")]
                    public string SchemeIdUri { get; set; }

                    [XmlAttribute("value")]
                    public string Value { get; set; }
                }
            }
        }
    }
}
