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

            //Environment.SetEnvironmentVariable("OpenBots_Agent_Config_Path", @"C:\Users\Francesca\Documents\GitHub\OpenBots.Agent\OpenBots.Agent.Client\bin\Debug\OpenBots.settings", EnvironmentVariableTarget.Machine);
            string agentSettingsPath = Environment.GetEnvironmentVariable("OpenBots_Agent_Config_Path", EnvironmentVariableTarget.Machine);

            if (agentSettingsPath == null)
                throw new Exception("Agent settings file not found");

            string agentSettingsText = File.ReadAllText(agentSettingsPath);
            var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(agentSettingsText);
            string agentId = settings["AgentId"];

            if (string.IsNullOrEmpty(agentId))
                throw new Exception("Agent is not connected");

            string username = new RegistryManager().AgentUsername;
            string password = new RegistryManager().AgentPassword;

            if (username == null || password == null)
                throw new Exception("Agent credentials not found in registry");

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
