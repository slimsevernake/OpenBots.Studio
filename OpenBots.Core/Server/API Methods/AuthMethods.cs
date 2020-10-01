using RestSharp;
using RestSharp.Serialization.Json;
using System.Collections.Generic;

namespace OpenBots.Core.Server.API_Methods
{
    public class AuthMethods
    {
        public static void GetAuthToken(RestClient client, string username, string password)
        {
            var request = new RestRequest("api/v1/auth/token", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { username, password });

            var response = client.Execute(request);
            var deserializer = new JsonDeserializer();
            var output = deserializer.Deserialize<Dictionary<string, string>>(response);

            string token = output["token"];
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", token));
        }
    }
}
