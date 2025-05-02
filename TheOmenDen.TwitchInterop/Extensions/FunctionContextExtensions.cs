using Microsoft.Azure.Functions.Worker;

namespace TheOmenDen.TwitchInterop.Extensions;

public static class FunctionContextExtensions
{
    public static string GetUserId(this FunctionContext context)
    {
        var principalId = context
            .GetHttpContext()
            ?.User
            .FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?
            .Value;

        if (string.IsNullOrWhiteSpace(principalId))
            throw new UnauthorizedAccessException("User identity not found");

        return principalId;
    }
}
