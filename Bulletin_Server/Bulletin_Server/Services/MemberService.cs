using Bulletin_Server.JWT.Models;
using Bulletin_Server.JWT.Services;
using Bulletin_Server.Model.Member;
using Bulletin_Server.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Bulletin_Server.Service
{
    public partial class BulletinService : IService
    {
        public Response<User> Login(string id, string password, string email)
        {
            User user = new User();

            IAuthContainerModel model = JWTService.GetJWTContainerModel(email);
            IAuthService authService = new JWTService(model.SecretKey);

            string token = authService.GenerateToken(model);

            if(!authService.IsTokenValid(token))
            {
                throw new UnauthorizedAccessException();
            }
            else
            {
                List<Claim> claims = authService.GetTokenClaims(token).ToList();
                Console.WriteLine("Login Eamil : " + claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email)).Value);
                user.id = id;
                user.password = password;
                user.email = email;
                user.token = token;
            }
            
            if (token != null && token.Length > 0)
            {
                Console.WriteLine("로그인 : " + ResponseStatus.OK);
                var resp = new Response<User> { data = user, message = "성공적으로 처리되었습니다.", status = ResponseStatus.OK };
                return resp;
            }
            else
            {
                Console.WriteLine("로그인 : " + ResponseStatus.Unauthorized);
                var resp = new Response<User> { data = user, message = "검증 오류.", status = ResponseStatus.Unauthorized };
                return resp;
            }
        }
    }
}
