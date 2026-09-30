namespace PortfolioCite.Contracts.Administration.Rules;

public static class ProfileMediaRules
{
    public const long AvatarMaxBytes = 5 * 1024 * 1024;
    public const long ResumeMaxBytes = 10 * 1024 * 1024;
    public const long AvatarRequestBytes = AvatarMaxBytes + 256 * 1024;
    public const long ResumeRequestBytes = ResumeMaxBytes + 256 * 1024;

    public const string EmptyFileError = "The file is empty.";
    public const string AvatarTypeError = "Use a JPG, PNG, or WebP image.";
    public const string ResumeTypeError = "Use a PDF file.";
    public const string AvatarSizeError = "The image must be 5 MB or smaller.";
    public const string ResumeSizeError = "The résumé must be 10 MB or smaller.";

    public static string ExtensionOf(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return string.Empty;

        return Path.GetExtension(fileName).ToLowerInvariant();
    }

    public static bool IsAvatarExtension(string extension)
    {
        return extension is ".jpg" or ".jpeg" or ".png" or ".webp";
    }

    public static bool IsResumeExtension(string extension)
    {
        return extension is ".pdf";
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
