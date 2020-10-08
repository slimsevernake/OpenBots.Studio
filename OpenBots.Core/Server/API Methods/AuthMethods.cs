using Newtonsoft.Json;
using OpenBots.Core.Server.UserRegistry;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace OpenBots.Core.Server.API_Methods
{
    public class AuthMethods
    {
        public static RestClient GetAuthToken()
        {
            var client = new RestClient("https://openbotsserver-dev.azurewebsites.net/");

            //string agentSettingsPath = Environment.GetEnvironmentVariable("AgentSettings", EnvironmentVariableTarget.Machine);
            //string agentSettingsText = File.ReadAllText(agentSettingsPath);
            //var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(agentSettingsText);
            //string agentID = settings["AgentId"];

            //if (string.IsNullOrEmpty(agentID))
                //throw new Exception("Agent not found");

            string username = "admin@admin.com";  ///new RegistryManager().AgentUsername;
            string password = "Hello321";  //new RegistryManager().AgentPassword;

            if (username == null || password == null)
                throw new Exception("Agent credentials not found");

            var request = new RestRequest("api/v1/auth/token", Method.POST);
            request.RequestFormat = DataFormat.Json;          
            request.AddJsonBody(new { username, password });

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");

            var deserializer = new JsonDeserializer();
            var output = deserializer.Deserialize<Dictionary<string, string>>(response);

            string token = output["token"];
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", token));
            return client;
        }
    }
}
