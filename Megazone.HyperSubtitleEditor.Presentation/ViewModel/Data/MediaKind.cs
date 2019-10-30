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
            Dash,
            File
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
                case "MPEG":
                case "MP4":
                    Type = TYPE.Mpeg;
                    break;
                case "HLS":
                    Type = TYPE.Hls;
                    break;
                case "FILE":
                    Type = TYPE.File;
                    break;
                default:
                    Type = TYPE.Unknown;
                    break;
            }

            Url = url;
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
                        return "None";
                    case TYPE.Hls:
                        return "HLS";
                    case TYPE.Mpeg:
                        return "MP4";
                    case TYPE.Dash:
                        return "DASH";
                    case TYPE.File:
                        return "FILE";
                    default:
                        return "";
                }
            }
        }
    }
}