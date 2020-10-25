using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Solomon_Server.JWT.Models
{
    public class JWTContainerModel : IAuthContainerModel
    {
        public string SecretKey { get; set; } = "";
        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha512; // SecurityAlgorithms.Sha512
        public int ExpireMinutes { get; set; } = 10880; // 7days
        public Claim[] Claims { get; set; }
    }
}
