using Newtonsoft.Json;
using OpenBots.Core.Server.Models;
using RestSharp;
using RestSharp.Serialization.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace OpenBots.Core.Server.API_Methods
{
    public class CredentialMethods
    {
        public static Credential GetCredential(RestClient client, string filter)
        {
            var request = new RestRequest("api/v1/Credentials", Method.GET);
            request.AddParameter("$filter", filter);
            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");

            var deserializer = new JsonDeserializer();
            var output = deserializer.Deserialize<Dictionary<string, string>>(response);
            var items = output["items"];
            return JsonConvert.DeserializeObject<List<Credential>>(items).FirstOrDefault();
        }

        public static void PutCredential(RestClient client, Credential credential)
        {
            var request = new RestRequest("api/v1/Credentials/{id}", Method.PUT);
            request.AddUrlSegment("id", credential.Id.ToString());
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(credential);

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");
        }
    }
}
