using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats;
using System.Reflection.Metadata;
using SixLabors.ImageSharp.Processing;

namespace AzureFunctionAppd
{
    public class ResizeImageOnUploadBlob
    {
        private readonly ILogger<ResizeImageOnUploadBlob> _logger;
        private readonly BlobServiceClient _blobServiceClient;

        public ResizeImageOnUploadBlob(ILogger<ResizeImageOnUploadBlob> logger, BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
        }

        [Function(nameof(ResizeImageOnUploadBlob))]
        public async Task Run([BlobTrigger("hextcontainer/{name}", Connection = "AzureWebJobsStorage")] Stream stream,  string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
            stream.Position = 0;
            _logger.LogInformation(stream.Length.ToString()?? "null");

            using (Image<Rgba32> image = Image.Load<Rgba32>(stream,out  IImageFormat format))
            {
                image.Mutate(x => x.Resize(50, 50));
                var outputContainer = _blobServiceClient.GetBlobContainerClient("hextcontainer-output");
                var outputBlobClient = outputContainer.GetBlobClient(name);
                using (var outputStream = new MemoryStream())
                {
                    //var format = image.Metadata.DecodedImageFormat;
                    image.Save(outputStream, format);
                    //if (format.Name == "JPEG")
                    //    image.SaveAsJpeg(outputStream);
                    //else if (format.Name == "PNG")
                    //    image.SaveAsPng(outputStream);
                    outputStream.Position = 0;
                    await outputBlobClient.UploadAsync(outputStream, true);

                }

            }

            _logger.LogInformation($"Resized image saved to hext container {name}");
        }
    }
}
