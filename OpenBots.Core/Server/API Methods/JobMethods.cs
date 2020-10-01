using RestSharp;

namespace OpenBots.Core.Server.API_Methods
{
    public class JobMethods
    {
        public static void CreateJob(RestClient client)
        {
            var request = new RestRequest("api/v1/Jobs", Method.POST);
            request.RequestFormat = DataFormat.Json;

            //TODO after endpoint is created

            var response = client.Execute(request);
        }
    }
}
