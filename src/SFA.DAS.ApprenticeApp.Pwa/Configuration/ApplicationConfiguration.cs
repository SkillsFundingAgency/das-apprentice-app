using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Http.Configuration;

namespace SFA.DAS.ApprenticeApp.Pwa.Configuration;

[ExcludeFromCodeCoverage]
public class ApplicationConfiguration
{
    public ConnectionStringsConfiguration ConnectionStrings { get; set; } = new();
    public AuthenticationConfiguration Authentication { get; set; } = new();
    public OuterApiConfiguration? ApprenticeAppApimApi { get; set; }
    public string? ClientId { get; set; }
    public string? MetadataAddress { get; set; }
    public string? KeyDir { get; set; }
    public string? TokenUri { get; set; }
    public string? UserInfoUri { get; set; }
    public string? ProductInfoHeaderValue { get; set; }
    public string? CookieName { get; set; }
    public string? StubAuth { get; set; }
}

[ExcludeFromCodeCoverage]
public class ConnectionStringsConfiguration
{
    public string RedisConnectionString { get; set; } = null!;
    public string DataProtectionKeysDatabase { get; set; } = null!;
}

[ExcludeFromCodeCoverage]
public class AuthenticationConfiguration
{
    public string MetadataAddress { get; set; } = null!;
}

[ExcludeFromCodeCoverage]
public class OuterApiConfiguration : IApimClientConfiguration
{
    public string ApiBaseUrl { get; set; } = null!;
    public string SubscriptionKey { get; set; } = null!;
    public string ApiVersion { get; set; } = null!;
}