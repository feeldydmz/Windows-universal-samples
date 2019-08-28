using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class GetProjectsParameter
    {
        public GetProjectsParameter(Authorization authorization, string stageId, string name)
        {
            Authorization = authorization;
            StageId = stageId;
            Name = name;
        }

        public Authorization Authorization { get; }
        public string StageId { get; }
        public string Name { get; }
    }
}
