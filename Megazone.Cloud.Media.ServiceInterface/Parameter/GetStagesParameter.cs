using Megazone.Cloud.Media.ServiceInterface.Model;

namespace Megazone.Cloud.Media.ServiceInterface.Parameter
{
    public class GetStagesParameter
    {
        public GetStagesParameter(Authorization authorization)
        {
            Authorization = authorization;
        }

        public Authorization Authorization { get; }
    }
}