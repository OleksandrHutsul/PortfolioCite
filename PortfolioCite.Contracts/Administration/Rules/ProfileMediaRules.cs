namespace PortfolioCite.Contracts.Administration.Rules;

public static class ProfileMediaRules
{
    public const long ResumeMaxBytes = 10 * 1024 * 1024;
    public const long ResumeRequestBytes = ResumeMaxBytes + 256 * 1024;

    public const string ResumeTypeError = "Use a PDF file.";
    public const string ResumeSizeError = "The resume must be 10 MB or smaller.";

    public static bool IsResumeExtension(string extension)
    {
        return extension is ".pdf";
    }
}
