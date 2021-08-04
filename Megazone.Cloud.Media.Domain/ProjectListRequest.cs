using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megazone.Cloud.Media.Domain
{
    public class ProjectListRequest
    {
        public ProjectListRequest(string endpoint, string accessToken)
        {
            Endpoint = endpoint;
            AccessToken = accessToken;

        }

        public string Endpoint { get; }
        public string AccessToken { get; }

    }
}
