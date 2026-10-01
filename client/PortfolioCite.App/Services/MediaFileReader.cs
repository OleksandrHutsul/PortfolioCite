using Microsoft.AspNetCore.Components.Forms;
using PortfolioCite.Contracts.Administration.Rules;
using PortfolioCite.Contracts.Portfolio.Models;

public static class MediaFileReader
{
    public static Task<(PendingUpload? File, string? Error)> ReadImageAsync(IBrowserFile file, CancellationToken cancellationToken)
    {
        return ReadAsync(file, MediaRules.IsImageExtension, MediaRules.ImageMaxBytes, MediaRules.ImageTypeError,
            MediaRules.ImageSizeError, "The image could not be read.", cancellationToken);
    }

    public static Task<(PendingUpload? File, string? Error)> ReadResumeAsync(IBrowserFile file, CancellationToken cancellationToken)
    {
        return ReadAsync(file, ProfileMediaRules.IsResumeExtension, ProfileMediaRules.ResumeMaxBytes, ProfileMediaRules.ResumeTypeError,
            ProfileMediaRules.ResumeSizeError, "The resume could not be read.", cancellationToken);
    }

    private static async Task<(PendingUpload? File, string? Error)> ReadAsync(IBrowserFile file, Func<string, bool> extensionIsValid, long maxBytes,
        string typeError, string sizeError, string readError, CancellationToken cancellationToken)
    {
        var extension = MediaRules.ExtensionOf(file.Name);
        var contentType = MediaRules.ContentTypeFor(extension);

        if (contentType is null || !extensionIsValid(extension))
            return (null, typeError);

        if (file.Size <= 0)
            return (null, MediaRules.EmptyFileError);

        if (file.Size > maxBytes)
            return (null, sizeError);

        try
        {
            await using var stream = file.OpenReadStream(maxBytes, cancellationToken);
            using var memory = new MemoryStream();

            await stream.CopyToAsync(memory, cancellationToken);

            var name = Path.GetFileName(file.Name);
            return (new PendingUpload(memory.ToArray(), string.IsNullOrWhiteSpace(name) ? "upload" : name, contentType), null);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return (null, readError);
        }
    }

    public static string FriendlyError(string? error, string fallback)
    {
        if (string.IsNullOrWhiteSpace(error))
            return fallback;

        if (error.Contains("Exception", StringComparison.OrdinalIgnoreCase)
            || error.Contains("HTTP", StringComparison.OrdinalIgnoreCase)
            || error.Any(char.IsDigit) && error.Contains("status", StringComparison.OrdinalIgnoreCase))
            return fallback;

        return error;
    }

    public static string FormatSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024d:0.#} KB";

        return $"{bytes / (1024d * 1024d):0.#} MB";
    }
}
