using Newtonsoft.Json;
using OpenBots.Core.Server.Models;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace OpenBots.Core.Server.API_Methods
{
    public class AgentMethods
    {
        public static Agent GetAgent(RestClient client, string filter)
        {
            var request = new RestRequest("api/v1/Agents", Method.GET);
            request.AddParameter("$filter", filter);
            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");

            var deserializer = new JsonDeserializer();
            var output = deserializer.Deserialize<Dictionary<string, string>>(response);
            var items = output["items"];
            return JsonConvert.DeserializeObject<List<Agent>>(items).FirstOrDefault();
        }

        public static Agent GetAgentById(RestClient client, Guid agentId)
        {
            var request = new RestRequest("api/v1/Agents/{id}", Method.GET);
            request.AddUrlSegment("id", agentId.ToString());
            request.AddParameter("id", agentId.ToString());
            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");

            return JsonConvert.DeserializeObject<Agent>(response.Content);
        }
    }
}
