namespace PortfolioCite.Application.Models;

public record ProfileFileContent(Stream Content, string FileName, string ContentType, long Length);
