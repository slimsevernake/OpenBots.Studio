using Newtonsoft.Json;
using OpenBots.Server.Model;
using RestSharp;
using RestSharp.Serialization.Json;
using System.Collections.Generic;
using System.Linq;

namespace OpenBots.Core.Server.API_Methods
{
    public class AssetMethods
    {
        public static Asset GetAsset(RestClient client, string filter)
        {           
            var request = new RestRequest("api/v1/Assets", Method.GET);
            request.AddParameter("$filter", filter);
            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);
            var deserializer = new JsonDeserializer();
            var output = deserializer.Deserialize<Dictionary<string, string>>(response);
            var items = output["items"];
            return JsonConvert.DeserializeObject<List<Asset>>(items).FirstOrDefault();
        }       

        public static void PutAsset(RestClient client, Asset asset)
        {
            var request = new RestRequest("api/v1/Assets/{id}", Method.PUT);
            request.AddUrlSegment("id", asset.Id.ToString());           
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(asset);

            client.Execute(request);
        }
    }
}
