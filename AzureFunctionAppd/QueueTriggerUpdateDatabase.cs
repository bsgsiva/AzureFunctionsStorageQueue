using System;
using Azure.Storage.Queues.Models;
using AzureFunctionAppd.Data;
using AzureFunctionAppd.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionAppd
{
    public class QueueTriggerUpdateDatabase
    {
        private readonly ILogger<QueueTriggerUpdateDatabase> _logger;

        private readonly AzureDbContext _dbContext;


        public QueueTriggerUpdateDatabase(ILogger<QueueTriggerUpdateDatabase> logger, AzureDbContext db)
        {
            _logger = logger;
            _dbContext = db ?? throw new ArgumentNullException(nameof(db));
        }


        [Function(nameof(QueueTriggerUpdateDatabase))]
        public void Run([QueueTrigger("azurerequestinbound", Connection = "AzureWebJobsStorage")] SalesRequest myQueueItem)
        {
            try
            {
                _logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
                myQueueItem.Status = "Submitted";
                _dbContext.SalesRequests.Add(myQueueItem);
                _dbContext.SaveChanges();
            }
            catch(Exception ex)
            {
                throw;
            }
         


        }
    }
}
