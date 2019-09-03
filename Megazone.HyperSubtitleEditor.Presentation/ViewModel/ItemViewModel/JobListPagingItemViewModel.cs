using System.Collections.Generic;
using Megazone.Api.Transcoder.Domain;

namespace Megazone.HyperSubtitleEditor.Presentation.ViewModel.ItemViewModel
{
    internal class JobListPagingItemViewModel
    {
        public enum PageIndexType
        {
            Previous,
            Current,
            Next
        }

        private readonly Dictionary<int, LastEvaluatedKey> _pageIndexAndContinuationParameter =
            new Dictionary<int, LastEvaluatedKey>();

        public int LastViewedPageIndex { get; set; }

        public int ContinuationParameterCount => _pageIndexAndContinuationParameter.Count;

        public LastEvaluatedKey GetContinuationParameter(PageIndexType pageIndexType)
        {
            var targetIndex = 0;
            if (pageIndexType == PageIndexType.Previous)
                targetIndex = LastViewedPageIndex - 2;
            else if (pageIndexType == PageIndexType.Current)
                targetIndex = LastViewedPageIndex - 1;
            else if (pageIndexType == PageIndexType.Next)
                targetIndex = LastViewedPageIndex;
            if (_pageIndexAndContinuationParameter.ContainsKey(targetIndex))
                return _pageIndexAndContinuationParameter[targetIndex];
            return default(LastEvaluatedKey);
        }

        public void SetContinuationParameter(LastEvaluatedKey parameter)
        {
            if (_pageIndexAndContinuationParameter.ContainsKey(LastViewedPageIndex))
                _pageIndexAndContinuationParameter[LastViewedPageIndex] = parameter;
            else
                _pageIndexAndContinuationParameter.Add(LastViewedPageIndex, parameter);
        }
    }
}