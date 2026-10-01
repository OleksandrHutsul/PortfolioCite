namespace PortfolioCite.Application.Validation;

public class ContentValidationException : Exception
{
    public ContentValidationException(IReadOnlyDictionary<string, string[]> errors) : base("The request is invalid.")
    {
        Errors = new Dictionary<string, string[]>(errors);
    }

    public Dictionary<string, string[]> Errors { get; }
}
