using AzureFunctionAppd.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs;
using Azure.Storage.Queues;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Microsoft.Azure.Functions.Worker.Http;

namespace AzureFunctionAppd
{
    public class OnSalesUploadWritetoQueue
    {
        private readonly ILogger<OnSalesUploadWritetoQueue> _logger;
        private readonly QueueClient _queueClient;

        public OnSalesUploadWritetoQueue(ILogger<OnSalesUploadWritetoQueue> logger)
        {
            _logger = logger;
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            _queueClient = new QueueClient(connectionString, "SalesRequestInBound");//Connection string from portal and queue name
        }

        [Function("OnSalesUploadWriteToQueue")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] 
        HttpRequestData req)
        {
            _logger.LogInformation("Received request for OnSalesUploadWriteToQueue");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SalesRequest data = JsonConvert.DeserializeObject<SalesRequest>(requestBody);
            await _queueClient.SendMessageAsync(JsonSerializer.Serialize(data));
            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteStringAsync("Sales request added to queue successfully");
            return response;
        }
    }
}
