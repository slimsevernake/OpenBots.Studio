using Newtonsoft.Json;
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

            string agentSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"GitHub\OpenBots.Agent\OpenBots.Agent.Client\bin\Debug\OpenBots.Settings");
            //TODO - replace with environment variable

            string agentSettingsText = File.ReadAllText(agentSettingsPath);
            var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(agentSettingsText);
            string agentID = settings["AgentId"];
            string username = "admin@admin.com";
            string password = "Hello321";
            //TODO - get registry key to use for authorization 

            var request = new RestRequest("api/v1/auth/token", Method.POST);
            request.RequestFormat = DataFormat.Json;          
            request.AddJsonBody(new { username, password });

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"{response.StatusCode} - {response.ErrorMessage}");

            var deserializer = new JsonDeserializer();
            var output = deserializer.Deserialize<Dictionary<string, string>>(response);

            string token = output["token"];
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", token));
            return client;
        }
    }
}
