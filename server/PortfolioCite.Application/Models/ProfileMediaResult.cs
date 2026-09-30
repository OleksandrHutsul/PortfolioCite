using PortfolioCite.Application.Enums;
using PortfolioCite.Contracts.Administration.Models;

namespace PortfolioCite.Application.Models;

public record ProfileMediaResult(ProfileAdminDto? Profile, ProfileMediaFailure Failure, string? Message)
{
    public static ProfileMediaResult Success(ProfileAdminDto profile)
    {
        return new ProfileMediaResult(profile, ProfileMediaFailure.None, null);
    }

    public static ProfileMediaResult MissingProfile()
    {
        return new ProfileMediaResult(null, ProfileMediaFailure.ProfileMissing, null);
    }

    public static ProfileMediaResult Invalid(string message)
    {
        return new ProfileMediaResult(null, ProfileMediaFailure.InvalidFile, message);
    }

    public static ProfileMediaResult TooLarge(string message)
    {
        return new ProfileMediaResult(null, ProfileMediaFailure.FileTooLarge, message);
    }
}
