using Bulletin_Server.DataBase;
using Bulletin_Server.JWT.Models;
using Bulletin_Server.JWT.Services;
using Bulletin_Server.Model.Member;
using Bulletin_Server.Utils;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;

namespace Bulletin_Server.Service
{
    public partial class BulletinService : IService
    {
        DBManager<UserModel> userDBManager = new DBManager<UserModel>();

        public Response<UserModel> SignUp(string id, string pw, string name, string email)
        {
            if(id != null && pw != null && name != null && email != null)
            {
                try
                {
                    string connectionString = $"SERVER=localhost;DATABASE=Bulletin;UID=root;PASSWORD=#kkh03kkh#";
                    using (IDbConnection db = new MySqlConnection(connectionString))
                    {
                        db.Open();

                        var model = new UserModel();
                        model.id = id;
                        model.pw = pw;
                        model.name = name;
                        model.email = email;

                        string insertSql = @"
INSERT INTO member_tb (
    id,
    pw,
    name,
    email
) 
VALUES(
    @id,
    @pw,
    @name,
    @email
);";
                        userDBManager.Insert(db, insertSql, model);
                        var resp = new Response<UserModel> { data = { }, message = "성공적으로 회원가입이 신청되었습니다.", status = ResponseStatus.OK };
                        return resp;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return new Response<UserModel> { data = { }, message = "서버 오류", status = ResponseStatus.InternalServerError };
                }
            }
            else
            {
                var resp = new Response<UserModel> { data = { }, message = "검증 오류", status = ResponseStatus.BadRequest };
                return resp;
            }
        }

        // TODO : email 빼기
        public Response<UserModel> Login(string id, string password, string email)
        {
            UserModel user = new UserModel();

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
                user.pw = password;
                user.email = email;
                user.token = token;
            }
            
            if (token != null && token.Length > 0)
            {
                Console.WriteLine("로그인 : " + ResponseStatus.OK);
                var resp = new Response<UserModel> { data = user, message = "성공적으로 처리되었습니다.", status = ResponseStatus.OK };
                return resp;
            }
            else
            {
                Console.WriteLine("로그인 : " + ResponseStatus.Unauthorized);
                var resp = new Response<UserModel> { data = user, message = "검증 오류.", status = ResponseStatus.Unauthorized };
                return resp;
            }
        }
    }
}
