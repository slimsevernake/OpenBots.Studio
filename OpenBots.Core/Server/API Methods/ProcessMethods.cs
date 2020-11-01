using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace OpenBots.Core.Server.API_Methods
{
    public class ProcessMethods
    {
        public static Guid CreateProcess(RestClient client, string name)
        {
            var request = new RestRequest("api/v1/Processes", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { name });

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");

            var deserializer = new JsonDeserializer();
            var output = deserializer.Deserialize<Dictionary<string, string>>(response);

            Guid processId = Guid.Parse(output["id"]);

            return processId;
        }

        public static void UploadProcess(RestClient client, string name, string filePath)
        {
            Guid processId = CreateProcess(client, name);
            var request = new RestRequest("api/v1/Processes/{id}/upload", Method.POST);
            request.AddUrlSegment("id", processId.ToString());
            request.RequestFormat = DataFormat.Json;

            request.AddHeader("Content-Type", "multipart/form-data"); 
            request.AddFile("File", filePath);

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");
        }
    }
}
