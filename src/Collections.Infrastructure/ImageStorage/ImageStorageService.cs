using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Collections.Application.Interfaces;

namespace Collections.Infrastructure.ImageStorage;

public class ImageStorageService : IImageStorageService
{
    private readonly BlobServiceClient _blobClient;
    private const string ContainerName = "images";

    public ImageStorageService(BlobServiceClient blobClient)
    {
        _blobClient = blobClient;
    }

    public async Task UploadImageAsync(Guid itemId, string base64Content)
    {
        var container = _blobClient.GetBlobContainerClient(ContainerName);

        await container.CreateIfNotExistsAsync();

        byte[] imageBytes = Convert.FromBase64String(base64Content);

        await container.UploadBlobAsync(itemId.ToString(), new MemoryStream(imageBytes));
    }
    public async Task<string> GetImageUrlAsync(Guid itemId)
    {
        var container = _blobClient.GetBlobContainerClient(ContainerName);

        var blobClient = container.GetBlobClient(itemId.ToString());

        if(!await blobClient.ExistsAsync())
        {
            return String.Empty;
        }

        if (!blobClient.CanGenerateSasUri)
        {
            return String.Empty;
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = ContainerName,
            BlobName = itemId.ToString(),
            Resource = "b",
            ExpiresOn = DateTime.UtcNow.AddHours(1),
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);

        return sasUri.ToString();
    }
}
