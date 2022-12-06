namespace Collections.Application.Interfaces;

public interface IImageStorageService
{
    Task UploadImageAsync(Guid itemId, string base64Content);
    Task<string> GetImageUrlAsync(Guid itemId);
}
