public interface IBlobStorageService
{
    Task UploadFileAsync(IFormFile file, string containerName);
    Task<List<string>> ListBlobsAsync(string containerName);
    Task DeleteBlobAsync(string blobName, string containerName);
}