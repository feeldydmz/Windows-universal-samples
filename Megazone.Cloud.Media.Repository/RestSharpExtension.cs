using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RestSharp;

namespace Megazone.Cloud.Media.Repository
{
    internal static class RestSharpExtension
    {
        public static RestClient CreateRestClient(string apiEndPoint)
        {
            return new RestClient(apiEndPoint)
            {
                UserAgent = "Hyper series/Megazone.Cloud.Media.SubtitleEditor"
            };
        }

        public static IRestRequest AddJsonString<T>(this IRestRequest restRequest, T body)
        {
            return restRequest.AddParameter("application/json", MakeJsonString(body), ParameterType.RequestBody);
        }

        private static string MakeJsonString<T>(T body)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            settings.Converters.Add(new StringEnumConverter());
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            return JsonConvert.SerializeObject(body,
                Formatting.None,
                settings);
        }

        public static T Convert<T>(this IRestResponse restResponse)
        {
            return JsonConvert.DeserializeObject<T>(restResponse.Content);
        }
    }
}