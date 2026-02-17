using Azure.Storage.Blobs;
using Azure.Identity;

namespace Shared.Services
{
    public class AzureBlobStorageService : IImageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly string _baseUrl;

        public AzureBlobStorageService(string containerName, string serviceUri)
        {            var blobServiceClient = new BlobServiceClient(new Uri(serviceUri), new DefaultAzureCredential(
                new DefaultAzureCredentialOptions
                {
                    ExcludeVisualStudioCredential = true,
                }));
            _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            _baseUrl = $"{serviceUri}/{containerName}";
        }

        public async Task<string> UploadImageAsync(string fileName, Stream fileStream)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream);
            return blobClient.Uri.ToString();
        }

        public async Task DeleteImageAsync(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }

        public string? GetImageUrl(string? fileName)
        {
            if(string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(_baseUrl))
            {
                return null;
            }
            return $"{_baseUrl}/{fileName}";
        }
    }
}
