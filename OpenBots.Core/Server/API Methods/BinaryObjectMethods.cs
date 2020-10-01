using OpenBots.Server.Model;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.IO;

namespace OpenBots.Core.Server.API_Methods
{
    public class BinaryObjectMethods
    {
        public static string GetBinaryObject(RestClient client, Guid? binaryObjectID)
        {
            var request = new RestRequest("api/v1/BinaryObjects/{id}/download", Method.GET);
            request.AddUrlSegment("id", binaryObjectID.ToString());
            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);
            var deserializer = new JsonDeserializer();
            var output = deserializer.Deserialize<BinaryObject>(response);

            //TODO - Needs to be finished, but API isn't working properly
            return "";
        }

        public static void UpdateBinaryObject(RestClient client, Guid? binaryObjectID, string filePath)
        {
            var request = new RestRequest("api/v1/BinaryObjects/{id}/upload", Method.PUT);
            request.AddUrlSegment("id", binaryObjectID.ToString());
            request.RequestFormat = DataFormat.Json;

            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddFile("File", filePath);
            request.AddParameter("Name", Path.GetFileName(filePath));

            client.Execute(request); 
            //TODO  - Seems to be functional, but API isn't working properly
        }
    }
}
