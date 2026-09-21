using System.Security.Claims;

namespace QuizWebApp.Api.Configuration;

public static class ClaimsExtensions
{
    public static int GetParticipantId(this ClaimsPrincipal principal) =>
        Convert.ToInt32(principal.FindFirstValue(ClaimTypes.NameIdentifier));
}
