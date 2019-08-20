namespace Megazone.Cloud.Media.Domain.Assets
{
    public class AudioSetting
    {
        public AudioSetting(long bitrate, string codec, long sampleRate, string ratingControlMode)
        {
            Bitrate = bitrate;
            Codec = codec;
            SampleRate = sampleRate;
            RatingControlMode = ratingControlMode;
        }

        public long Bitrate { get; }
        public string Codec { get; }
        public long SampleRate { get; }
        public string RatingControlMode { get; }
    }
}
