using Newtonsoft.Json;
using OpenBots.Core.Server.Models;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace OpenBots.Core.Server.API_Methods
{
    public class QueueItemMethods
    {
        public static QueueItem GetQueueItemById(RestClient client, Guid? queueItemId)
        {
            var request = new RestRequest("api/v1/QueueItems/{id}", Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddUrlSegment("id", queueItemId.ToString());
            request.AddParameter("id", queueItemId.ToString());

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");

            var deserializer = new JsonDeserializer();
            var output = deserializer.Deserialize<Dictionary<string, string>>(response);
            var items = output["items"];
            return JsonConvert.DeserializeObject<List<QueueItem>>(items).FirstOrDefault();
        }

        public static void EnqueueQueueItem(RestClient client, QueueItem queueItem)
        {
            var request = new RestRequest("api/v1/QueueItems/Enqueue", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(queueItem);

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");
        }

        public static QueueItem DequeueQueueItem(RestClient client, Guid? agentId, Guid? queueId)
        {
            var request = new RestRequest("api/v1/QueueItems/Dequeue", Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("agentId", agentId.ToString());
            request.AddParameter("queueId", queueId.ToString());

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                return null;

            return JsonConvert.DeserializeObject<QueueItem>(response.Content);
        }

        public static void CommitQueueItem(RestClient client, Guid transactionKey)
        {
            var request = new RestRequest($"api/v1/QueueItems/Commit", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("transactionKey", transactionKey.ToString(), ParameterType.QueryString);

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");
        }

        public static void RollbackQueueItem(RestClient client, Guid transactionKey, string code, string error, bool isFatal)
        {
            var request = new RestRequest($"api/v1/QueueItems/Rollback", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("transactionKey", transactionKey.ToString(), ParameterType.QueryString);
            request.AddParameter("errorCode", code, ParameterType.QueryString);
            request.AddParameter("errorMessage", error, ParameterType.QueryString);
            request.AddParameter("isFatal", isFatal, ParameterType.QueryString);

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");
        }

        public static void ExtendQueueItem(RestClient client, Guid transactionKey)
        {           
            var request = new RestRequest($"api/v1/QueueItems/Extend", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("transactionKey", transactionKey.ToString(), ParameterType.QueryString);

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"Status Code: {response.StatusCode} - Error Message: {response.ErrorMessage}");
        }
    }
}
