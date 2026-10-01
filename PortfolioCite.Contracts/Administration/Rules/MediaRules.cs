namespace PortfolioCite.Contracts.Administration.Rules;

public static class MediaRules
{
    public const long ImageMaxBytes = 5 * 1024 * 1024;
    public const long ImageRequestBytes = ImageMaxBytes + 256 * 1024;

    public const string EmptyFileError = "The file is empty.";
    public const string ImageTypeError = "Use a JPG, PNG, or WebP image.";
    public const string ImageSizeError = "The image must be 5 MB or smaller.";

    public static string ExtensionOf(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return string.Empty;

        return Path.GetExtension(fileName).ToLowerInvariant();
    }

    public static bool IsImageExtension(string extension)
    {
        return extension is ".jpg" or ".jpeg" or ".png" or ".webp";
    }

    public static string? ContentTypeFor(string extension)
    {
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            _ => null
        };
    }
}
