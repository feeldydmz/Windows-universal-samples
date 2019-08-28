using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megazone.Cloud.Media.Domain
{
    public class ProjectListRequest
    {
        public ProjectListRequest(string endpoint, string accessToken, string stageId, string name)
        {
            Endpoint = endpoint;
            AccessToken = accessToken;
            StageId = stageId;
            Name = name;
        }

        public string Endpoint { get; }
        public string AccessToken { get; }
        public string StageId { get; }
        public string Name { get; }
    }
}
