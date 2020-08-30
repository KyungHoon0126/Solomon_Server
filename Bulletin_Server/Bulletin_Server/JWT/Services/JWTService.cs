using Bulletin_Server.JWT.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Bulletin_Server.JWT.Services
{
    public class JWTService : IAuthService
    {
        public string SecretKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string GenerateToken(IAuthContainerModel model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Claim> GetTokenClaims(string token)
        {
            throw new NotImplementedException();
        }

        public bool IsTokenValid(string token)
        {
            throw new NotImplementedException();
        }
    }
}
