using PortfolioCite.Contracts.Administration.Models;
using PortfolioCite.Contracts.Portfolio.Models;

namespace PortfolioCite.Api.Helpers;

public static class ProfileMediaLinks
{
    public static ProfileAdminDto Apply(HttpRequest request, ProfileAdminDto profile)
    {
        return profile with
        {
            AvatarUrl = Absolute(request, profile.AvatarUrl),
            ResumeUrl = Absolute(request, profile.ResumeUrl)
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
                AvatarUrl = Absolute(request, snapshot.Profile.AvatarUrl),
                ResumeUrl = Absolute(request, snapshot.Profile.ResumeUrl)
            }
        };
    }

    private static string? Absolute(HttpRequest request, string? url)
    {
        if (string.IsNullOrWhiteSpace(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            return url;

        var scheme = Header(request, "X-Forwarded-Proto") ?? request.Scheme;
        var host = Header(request, "X-Forwarded-Host") ?? request.Host.Value;
        var path = url.StartsWith('/') ? url : $"/{url}";

        return $"{scheme}://{host}{request.PathBase}{path}";
    }

    private static string? Header(HttpRequest request, string name)
    {
        if (!request.Headers.TryGetValue(name, out var values))
            return null;

        var value = values.ToString().Split(',')[0].Trim();

        return value.Length == 0 ? null : value;
    }
}
