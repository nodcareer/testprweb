using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace testpr.web.Services;



public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(BlobServiceClient blobServiceClient, ILogger<BlobStorageService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
    }

    /// <summary>
    /// Uploads a file to Azure Blob Storage
    /// </summary>
    public async Task UploadFileAsync(IFormFile file, string containerName)
    {
        try
        {
            // Get or create container
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            // Get blob client
            BlobClient blobClient = containerClient.GetBlobClient(file.FileName);

            // Upload file
            using (Stream stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            _logger.LogInformation($"File {file.FileName} uploaded successfully to container {containerName}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading file: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Lists all blobs in a container
    /// </summary>
    public async Task<List<string>> ListBlobsAsync(string containerName)
    {
        try
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            
            // Check if container exists
            if (!await containerClient.ExistsAsync())
            {
                _logger.LogWarning($"Container {containerName} does not exist");
                return new List<string>();
            }

            var blobs = new List<string>();
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                blobs.Add(blobItem.Name);
            }

            return blobs;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error listing blobs: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Deletes a blob from container
    /// </summary>
    public async Task DeleteBlobAsync(string blobName, string containerName)
    {
        try
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteAsync();

            _logger.LogInformation($"Blob {blobName} deleted successfully from container {containerName}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting blob: {ex.Message}");
            throw;
        }
    }
}
