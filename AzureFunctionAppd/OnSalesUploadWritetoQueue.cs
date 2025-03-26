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
using Azure.Storage.Queues.Models;

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
            _queueClient = new QueueClient(connectionString, "azurerequestinbound", new QueueClientOptions
            {
                MessageEncoding= QueueMessageEncoding.Base64
            });//Connection string from portal and queue name

            _queueClient.CreateIfNotExists();

            

           

        }


        [Function("OnSalesUploadWriteToQueue")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] 
        HttpRequestData req)
        {
            try
            {
                _logger.LogInformation("Received request for OnSalesUploadWriteToQueue");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                SalesRequest data = JsonConvert.DeserializeObject<SalesRequest>(requestBody);
            //    await _queueClient.UpdateMessageAsync(data.Name, data.Id, visibilityTimeout: TimeSpan.FromSeconds(10));
                await _queueClient.SendMessageAsync(JsonSerializer.Serialize(data));

              
                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await response.WriteStringAsync("Sales request added to queue successfully");
                return response;
            }
            catch(Exception ex)
            {
                throw;
            }
        
        }
    }
}
