using PortfolioCite.Application.Enums;

namespace PortfolioCite.Application.Models;

public record MediaBytes(byte[]? Content, MediaBytesStatus Status);
