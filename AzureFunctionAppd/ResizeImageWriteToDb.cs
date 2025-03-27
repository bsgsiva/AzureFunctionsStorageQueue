using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionAppd
{
    public class ResizeImageWriteToDb
    {
        private readonly ILogger<ResizeImageWriteToDb> _logger;

        public ResizeImageWriteToDb(ILogger<ResizeImageWriteToDb> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ResizeImageWriteToDb))]
        public async Task Run([BlobTrigger("hextcontainer-output/{name}", Connection = "AzureWebJobsStorage")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
        }
    }
}
