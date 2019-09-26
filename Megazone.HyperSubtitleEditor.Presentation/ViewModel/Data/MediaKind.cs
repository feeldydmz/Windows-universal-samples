using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.Data
{

    public class MediaKind
    {
        public enum TYPE
        {
            None,
            Unknown,
            Hls,
            Mpeg,
            Dash
        }

        public TYPE Type { get; set; }
        public string Url { get; set; }

        public string TypeString
        {
            get
            {
                switch (Type)
                {
                    case TYPE.None:
                        return "None";
                    case TYPE.Unknown:
                        return "Unknown";
                    case TYPE.Hls:
                        return "HLS";
                    case TYPE.Mpeg:
                        return "MP4";
                    case TYPE.Dash:
                        return "DASH";
                    default:
                        return "";
                }
            }
        }


        public MediaKind(TYPE type, string url)
        {
            Type = type;
            Url = url;
        }

        public MediaKind(string type, string url)
        {
            switch (type.ToUpper())
            {
                case "DASH":
                    Type = TYPE.Dash;
                    break;
                case "MP4":
                    Type = TYPE.Mpeg;
                    break;
                case "HLS":
                    Type = TYPE.Hls;
                    break;
                default:
                    Type = TYPE.Unknown;
                    break;
            }

            Url = url;
        }

    }
}
