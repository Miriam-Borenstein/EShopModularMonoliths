namespace Shared.Services;

public interface IImageService
{
    Task<string> UploadImageAsync(string fileName, Stream fileStream);
    Task DeleteImageAsync(string fileName);
    string? GetImageUrl(string? fileName);
}
