using Newtonsoft.Json;
using OpenBots.Server.Model;
using RestSharp;
using RestSharp.Serialization.Json;
using System.Collections.Generic;
using System.Linq;

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

            client.Execute(request);
        }
    }
}
