using Cashflow.Core;
using Cashflow.Domain.Abstractions.RequestPipeline;

namespace Cashflow.Infrastructure.RequestPipeline;

public class AuthenticatedUser : IAuthenticatedUser
{
    public long Id { get; private set; }
    public string Email { get; init; }
    public Roles Role { get; init; }
    public Guid TraceIdentifier { get; private set; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    
    public void BindMessageTrace(Guid traceIdentifier, long userId)
    {
        TraceIdentifier = traceIdentifier;
        Id = userId;
    }
}