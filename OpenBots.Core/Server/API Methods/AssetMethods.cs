using Newtonsoft.Json;
using OpenBots.Core.Server.Models;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

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

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");

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

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");
        }

        public static void DownloadFileAsset(RestClient client, Guid? assetID, string directoryPath, string fileName)
        {
            var request = new RestRequest("api/v1/assets/{id}/export", Method.GET);
            request.AddUrlSegment("id", assetID.ToString());
            request.RequestFormat = DataFormat.Json;

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");

            byte[] file = response.RawBytes;
            File.WriteAllBytes(Path.Combine(directoryPath, fileName), file);
        }

        public static void UpdateFileAsset(RestClient client, Asset asset, string filePath)
        {
            var request = new RestRequest("api/v1/Assets/{id}/upload", Method.PUT);
            request.AddUrlSegment("id", asset.Id.ToString());
            request.AddParameter("id", asset.Id.ToString());
            request.RequestFormat = DataFormat.Json;

            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddFile("File", filePath.Trim());

            request.AddParameter("Name", asset.Name, ParameterType.GetOrPost); //TODO remove after PR gets merged
            request.AddParameter("Type", asset.Type, ParameterType.GetOrPost);
            request.AddParameter("organizationId", "ef8a6670-f522-4fcd-a55e-90aa92d1deb7", ParameterType.GetOrPost);

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");
        }
    }
}
