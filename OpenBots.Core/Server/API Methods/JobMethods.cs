using RestSharp;
using System;
using System.Net.Http;

namespace OpenBots.Core.Server.API_Methods
{
    public class JobMethods
    {
        public static void CreateJob(RestClient client, Guid agentId, Guid processId)
        {
            var request = new RestRequest("api/v1/Jobs", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { agentId, processId});

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");
        }
    }
}
