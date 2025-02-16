using Cashflow.Core;

namespace Cashflow.Domain.Abstractions.RequestPipeline;

public interface IAuthenticatedUser
{
    public long Id { get; }
    public string Email { get; init; }
    public Roles Role { get; init; }
    public Guid TraceIdentifier { get; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }

    public void BindMessageTrace(Guid TraceId, long UserId);
}