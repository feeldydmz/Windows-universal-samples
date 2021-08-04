using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class GetProjectsParameter
    {
        public GetProjectsParameter(Authorization authorization, string endpoint)
        {
            Authorization = authorization;
            Endpoint = endpoint;
        }

        public Authorization Authorization { get; }
        public string Endpoint { get; }
        
    }
}