using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.Api.Helpers;

public static class ProfileMediaLinks
{
    public static ProfileAdminDto Apply(HttpRequest request, ProfileAdminDto profile)
    {
        return profile with
        {
            AvatarUrl = MediaLinks.Absolute(request, profile.AvatarUrl),
            ResumeUrl = MediaLinks.Absolute(request, profile.ResumeUrl)
        };
    }

    public static PortfolioSnapshotDto Apply(HttpRequest request, PortfolioSnapshotDto snapshot)
    {
        if (snapshot.Profile is null)
            return snapshot;

        return snapshot with
        {
            Profile = snapshot.Profile with
            {
                AvatarUrl = MediaLinks.Absolute(request, snapshot.Profile.AvatarUrl),
                ResumeUrl = MediaLinks.Absolute(request, snapshot.Profile.ResumeUrl)
            }
        };
    }

}
