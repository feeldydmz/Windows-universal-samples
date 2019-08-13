using PublicModel = Megazone.HyperSubtitleEditor.Domain.Subtitle.Model;
using internalModel = Megazone.HyperSubtitleEditor.Repository.Subtitle.Model;

namespace Megazone.HyperSubtitleEditor.Repository.Subtitle.Extension
{
    internal static class SubtitleExtension
    {
        public static PublicModel.Subtitle ToPublicModel(this internalModel.Subtitle subtitle)
        {
            var publicSubtitle = new PublicModel.Subtitle
            {
                CountryCode = subtitle.CountryCode,
                Format = subtitle.Format,
                Datasets = subtitle.Datasets
            };

            return publicSubtitle;
        }
    }
}
