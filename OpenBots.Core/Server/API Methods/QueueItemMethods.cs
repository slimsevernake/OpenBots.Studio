using Newtonsoft.Json;
using OpenBots.Core.Server.Models;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace OpenBots.Core.Server.API_Methods
{
    public class QueueItemMethods
    {
        public static void EnqueQueueItem(RestClient client, QueueItem queueItem)
        {
            var request = new RestRequest("api/v1/QueueItems/Enqueue", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(queueItem);

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"{response.StatusCode} - {response.ErrorMessage}");
        }

        public static QueueItem DequeueQueueItem(RestClient client, Guid? agentId, Guid? queueId)
        {
            var request = new RestRequest("api/v1/QueueItems/Dequeue", Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("agentId", agentId.ToString());
            request.AddParameter("queueId", queueId.ToString());

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"{response.StatusCode} - {response.ErrorMessage}");

            var deserializer = new JsonDeserializer();
            var output = deserializer.Deserialize<Dictionary<string, string>>(response);
            var items = output["items"];
            return JsonConvert.DeserializeObject<List<QueueItem>>(items).FirstOrDefault();
        }

        public static void CommitQueueItem(RestClient client, string transactionKey)
        {
            var request = new RestRequest("api/v1/QueueItems/Commit", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("transactionKey", transactionKey);

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"{response.StatusCode} - {response.ErrorMessage}");
        }

        public static void RollbackQueueItem(RestClient client, string transactionKey, string error)
        {
            var request = new RestRequest("api/v1/QueueItems/Rollback", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("transactionKey", transactionKey);
            request.AddParameter("error", error);

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"{response.StatusCode} - {response.ErrorMessage}");
        }

        public static void ExtendQueueItem(RestClient client, string transactionKey)
        {           
            var request = new RestRequest("api/v1/QueueItems/Extend", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("transactionKey", transactionKey);

            var response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new HttpRequestException($"{response.StatusCode} - {response.ErrorMessage}");
        }
    }
}
