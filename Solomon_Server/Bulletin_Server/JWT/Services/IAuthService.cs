using Solomon_Server.JWT.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace Solomon_Server.JWT.Services
{
    public interface IAuthService
    {
        string SecretKey { get; set; }
        bool IsTokenValid(string token);
        string GenerateToken(IAuthContainerModel model);
        IEnumerable<Claim> GetTokenClaims(string token);
    }
}
