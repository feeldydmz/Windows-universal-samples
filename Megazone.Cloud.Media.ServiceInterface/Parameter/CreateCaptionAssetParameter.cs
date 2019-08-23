﻿using System.Collections.Generic;
using Megazone.Cloud.Media.Domain.Assets;
using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class CreateCaptionAssetParameter : RequiredParameter
    {
        public CreateCaptionAssetParameter(Authorization authorization, string stageId, string projectId,
            string assetName, IEnumerable<Caption> captions) : base(authorization, stageId, projectId)
        {
            AssetName = assetName;
            Captions = captions;
        }

        public string AssetName { get; }

        public IEnumerable<Caption> Captions { get; }
    }
}