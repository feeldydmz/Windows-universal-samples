namespace Megazone.Cloud.Media.Domain.Assets
{
    public class VideoSetting
    {
        public VideoSetting(long bitrate, string codec, int width, int height, string ratingControlMode)
        {
            Bitrate = bitrate;
            Codec = codec;
            Width = width;
            Height = height;
            RatingControlMode = ratingControlMode;
        }

        public long Bitrate { get; }
        public string Codec { get; }
        public int Width { get; }
        public int Height { get; }
        public string RatingControlMode { get; }
    }
}
