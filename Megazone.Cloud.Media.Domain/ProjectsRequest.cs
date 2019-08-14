using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megazone.Cloud.Media.Domain
{
    public class ProjectsRequest
    {
        public ProjectsRequest(string stageId)
        {
            StageId = stageId;
        }

        public string StageId { get; }
    }
}
